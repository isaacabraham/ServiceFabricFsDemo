namespace ServiceFabricDemo

open System
open System.Threading.Tasks
open Microsoft.ServiceFabric.Actors

type ICat =
    inherit IActor
    abstract member Jump : destination : string -> Task
    abstract member Purr : unit -> Task
    abstract member Eat : food : string -> Task
    abstract member Sleep : duration : TimeSpan -> Task
    abstract member Pester : duration : TimeSpan -> Task
    abstract member Meow : volume:int -> timeOfDay:TimeSpan -> Task

    abstract member FavouriteFood : unit -> string Task
    abstract member Colour : unit -> string Task
    abstract member GetState : unit -> CatState Task