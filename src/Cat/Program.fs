module ServiceFabricDemo.Program

open System
open System.Fabric
open System.Threading
open Microsoft.ServiceFabric.Actors

[<EntryPoint>]
let main argv = 
    use fabricRuntime = FabricRuntime.Create()
    fabricRuntime.RegisterActor<Cat>()
    Thread.Sleep(Timeout.Infinite)
    0