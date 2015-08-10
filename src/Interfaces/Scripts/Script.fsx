#load "load-references.fsx"
#r @"..\bin\debug\Interfaces.dll"
open Microsoft.ServiceFabric.Actors
open ServiceFabricDemo
open System

let cat = ActorProxy.Create<ICat>(ActorId "Blackie", "fabric:/CatActors")

async {
    let! color = cat.Colour() |> Async.AwaitTask
    let! food = cat.FavouriteFood() |> Async.AwaitTask
    return sprintf "%s is %s and likes %s." (cat.GetActorId().GetStringId()) color food
} |> Async.RunSynchronously

cat.Eat "Whiskers"
cat.Eat "Sheba"
cat.Meow 3 (TimeSpan.FromHours 6.)
cat.Purr()

let state = async { return! cat.GetState() |> Async.AwaitTask } |> Async.RunSynchronously
