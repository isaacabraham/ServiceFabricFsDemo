open Microsoft.ServiceFabric.Services
open Suave
open Suave.Http.Successful
open Suave.Web
open System.Fabric
open System.Threading
open System.Threading.Tasks

type SuaveService() =
    inherit StatelessService()
    override __.CreateCommunicationListener() =
        { new ICommunicationListener with
            member __.Abort() = ()
            member __.CloseAsync _ = Task.FromResult() :> Task
            member __.Initialize _ = ()
            member __.OpenAsync cancellationToken =
                async {
                    let starting, server = startWebServerAsync defaultConfig (OK "Hello from Service Fabric!")
                    Async.Start(server, cancellationToken)
                    do! starting |> Async.Ignore
                    return (defaultConfig.bindings.Head.ToString())
                } |> Async.StartAsTask
        }
[<EntryPoint>]
let main argv = 
    use fabricRuntime = FabricRuntime.Create()
    fabricRuntime.RegisterServiceType("SuaveApiType", typeof<SuaveService>)
    Thread.Sleep Timeout.Infinite
    0