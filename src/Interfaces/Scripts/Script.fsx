#load "load-references.fsx"
#r @"..\bin\debug\Interfaces.dll"
open Microsoft.ServiceFabric.Actors
open ServiceFabricDemo
open System

// Download Service Fabric SDK and VS2015
// Convert from nuget
// Create console app
// Create class library
// Copy across package root folder
// Create actor
// Create interface
// Target 64 bit (not AnyCPU) via Configuration Manager, .NET 4.5.1
// Interfaces must have NAMED arguments!
// Reference interface via assembly, NOT code.

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

async { return! cat.GetState() |> Async.AwaitTask } |> Async.RunSynchronously