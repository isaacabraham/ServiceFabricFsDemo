namespace ServiceFabricDemo

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Microsoft.ServiceFabric
open Microsoft.ServiceFabric.Actors

type Cat() =
    inherit Actor<CatState>()
    let emptyTask() = Task.FromResult() :> Task
    let favouriteFood = "Sheba"

    interface ICat with
        // Side-effect-free actions
        member __.Colour() = Task.FromResult "Black"
        member __.FavouriteFood() = Task.FromResult favouriteFood
        member this.GetState() = Task.FromResult this.State
        
        // Side-effect-full actions
        member this.Jump(destination) =
            this.State.HungerLevel <- this.State.HungerLevel + 1
            this.State.CatHappiness <- this.State.CatHappiness + 1
            this.State.Weight <- this.State.Weight + 0.1

            match destination with 
            | "Table" -> this.State.OwnerHappiness <- this.State.OwnerHappiness - 2
            | "Bed" -> this.State.OwnerHappiness <- this.State.OwnerHappiness - 1
            | _ -> ()
            emptyTask()

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

