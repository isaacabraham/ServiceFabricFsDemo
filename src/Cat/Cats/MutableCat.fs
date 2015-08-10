namespace ServiceFabricDemo

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Microsoft.ServiceFabric
open Microsoft.ServiceFabric.Actors
open FSharp.Data.UnitSystems.SI.UnitSymbols

type MutableCat() =
    inherit Actor<MutableCatState>()
    let emptyTask() = Task.FromResult() :> Task
    let favouriteFood = "Sheba"

    interface ICat with
        // Side-effect-free actions
        [<Readonly>] member __.Colour() = Task.FromResult "Black"
        [<Readonly>] member __.FavouriteFood() = Task.FromResult favouriteFood
        [<Readonly>] member this.GetState() = Task.FromResult { CatHappiness = this.State.CatHappiness; OwnerRating = this.State.OwnerRating; OwnerHappiness = this.State.OwnerHappiness; HungerLevel = this.State.HungerLevel; Weight = this.State.Weight * 1.<kg> }
        
        // Side-effect-full actions
        member this.Jump(destination) =
            this.State.HungerLevel <- this.State.HungerLevel + 1
            this.State.CatHappiness <- this.State.CatHappiness + 1
            this.State.Weight <- this.State.Weight + 0.1

            match destination with 
            | "Table" -> this.State.OwnerHappiness <- this.State.OwnerHappiness - 2
            | "Bed" -> this.State.OwnerHappiness <- this.State.OwnerHappiness - 1
            | _ -> ()
            Task.FromResult() :> Task

        member this.Purr() =
            this.State.CatHappiness <- this.State.CatHappiness + 1
            this.State.OwnerHappiness <- this.State.OwnerHappiness + 2
            emptyTask()
        member this.Eat food =
            this.State.CatHappiness <- this.State.CatHappiness + (if food = favouriteFood then 2 else 1)
            this.State.HungerLevel <- 0
            this.State.Weight <- this.State.Weight + 0.5
            this.State.OwnerRating <- this.State.OwnerRating + 1
            emptyTask()
        member this.Sleep duration =
            this.State.CatHappiness <- this.State.CatHappiness + 1
            if (this.State.OwnerHappiness < 0) then this.State.OwnerHappiness <- 0
            emptyTask()
        member this.Pester duration =
            this.State.CatHappiness <- this.State.CatHappiness + 1
            this.State.OwnerHappiness <- this.State.OwnerHappiness - 1
            emptyTask()
        member this.Meow volume timeOfDay =
            let timeMultiplier = if timeOfDay.Hours < 8 then 3 elif timeOfDay.Hours > 22 then 2 else 1            
            this.State.OwnerHappiness <- this.State.OwnerHappiness - (volume * timeMultiplier)
            this.State.CatHappiness <- this.State.CatHappiness - 1
            emptyTask()

