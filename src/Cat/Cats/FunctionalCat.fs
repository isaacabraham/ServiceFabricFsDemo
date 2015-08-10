namespace ServiceFabricDemo

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Microsoft.ServiceFabric
open Microsoft.ServiceFabric.Actors
open FSharp.Data.UnitSystems.SI.UnitSymbols

module CatActions =
    let jump destination state =
        { state with
            HungerLevel = state.HungerLevel + 1
            CatHappiness = state.CatHappiness + 1
            Weight = state.Weight + 0.1<kg>
            OwnerHappiness =
                state.OwnerHappiness -
                    match destination with 
                    | "Table" -> 2
                    | "Bed" -> 1
                    | _ -> 0 }
    let getColour state = "Black"
    let purr state =
        { state with
            CatHappiness = state.CatHappiness + 1
            OwnerHappiness = state.OwnerHappiness + 2 }
    let eat food favouriteFood state =
        { state with
            CatHappiness = state.CatHappiness + (if food = favouriteFood then 2 else 1)
            HungerLevel = 0
            Weight = state.Weight + 0.5<kg>
            OwnerRating = state.OwnerRating + 1 }
    let sleep duration state =
        { state with
            CatHappiness = state.CatHappiness + 1
            OwnerHappiness = if state.OwnerHappiness < 0 then 0 else state.OwnerHappiness }
    let pester duration state =
        { state with
            CatHappiness = state.CatHappiness + 1
            OwnerHappiness = state.OwnerHappiness - 1 }
    let meow volume (timeOfDay:TimeSpan) state =
        let timeMultiplier = if timeOfDay.Hours < 8 then 3 elif timeOfDay.Hours > 22 then 2 else 1            
        { state with
            OwnerHappiness = state.OwnerHappiness - (volume * timeMultiplier)
            CatHappiness = state.CatHappiness - 1 }

open Adapters
open CatActions

type Cat() =
    inherit Actor<ImmutableCatState>()
    let favouriteFood = "Sheba"
    interface ICat with
        [<Readonly>] member actor.Colour() = actor |> doReadOnly getColour
        [<Readonly>] member __.FavouriteFood() = Task.FromResult favouriteFood
        [<Readonly>] member actor.GetState() = Task.FromResult actor.State

        member actor.Eat food = actor |> doUpdate (eat food "Sheba")
        member actor.Jump destination = actor |> doUpdate (jump destination) // using the doUpdate function explicitly
        member actor.Purr() = actor |+> purr // alteratively we can use a custom operator
        member actor.Sleep duration = actor |+> sleep duration
        member actor.Pester duration = actor |+> pester duration
        member actor.Meow volume timeOfDay = actor |+> meow volume timeOfDay