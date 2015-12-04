namespace ServiceFabricDemo

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Microsoft.ServiceFabric
open Microsoft.ServiceFabric.Actors
open FSharp.Data.UnitSystems.SI.UnitSymbols

type DisfunctionalCat() =
    inherit StatefulActor<ImmutableCatState>()
    let emptyTask() = Task.FromResult() :> Task
    let favouriteFood = "Sheba"

    interface ICat with
        // Side-effect-free actions
        [<Readonly>] member __.Colour() = Task.FromResult "Black"
        [<Readonly>] member __.FavouriteFood() = Task.FromResult favouriteFood
        [<Readonly>] member this.GetState() = Task.FromResult this.State
        
        // Side-effect-full actions
        member this.Jump(destination) =
            this.State <-
                { this.State with
                    HungerLevel = this.State.HungerLevel + 1
                    CatHappiness = this.State.CatHappiness + 1
                    Weight = this.State.Weight + 0.1<kg> }
            
            this.State <-
                { this.State with
                    OwnerHappiness =
                        match destination with 
                        | "Table" -> this.State.OwnerHappiness - 2
                        | "Bed" -> this.State.OwnerHappiness - 1
                        | _ -> this.State.OwnerHappiness }
            emptyTask()

        member this.Purr() =
            this.State <-
                { this.State with
                    CatHappiness = this.State.CatHappiness + 1
                    OwnerHappiness = this.State.OwnerHappiness + 2 }
            emptyTask()
        member this.Eat food =
            this.State <-
                { this.State with
                    CatHappiness = this.State.CatHappiness + (if food = favouriteFood then 2 else 1)
                    HungerLevel = 0
                    Weight = this.State.Weight + 0.5<kg>
                    OwnerRating = this.State.OwnerRating + 1 }
            emptyTask()
        member this.Sleep duration =
            this.State <- { this.State with CatHappiness = this.State.CatHappiness + 1 }
            if (this.State.OwnerHappiness < 0) then
                this.State <- { this.State with OwnerHappiness = 0 }
            emptyTask()
        member this.Pester duration =
            this.State <-
                { this.State with 
                    CatHappiness = this.State.CatHappiness + 1
                    OwnerHappiness = this.State.OwnerHappiness - 1 }
            emptyTask()
        member this.Meow volume timeOfDay =
            let timeMultiplier = if timeOfDay.Hours < 8 then 3 elif timeOfDay.Hours > 22 then 2 else 1
            this.State <-
                { this.State with
                    OwnerHappiness = this.State.OwnerHappiness - (volume * timeMultiplier)
                    CatHappiness = this.State.CatHappiness - 1 }
            emptyTask()

