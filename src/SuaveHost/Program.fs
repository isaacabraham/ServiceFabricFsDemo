open Microsoft.ServiceFabric.Services
open Suave
open Suave.Http.Successful
open Suave.Types
open Suave.Utils
open Suave.Web
open System
open System.Fabric
open System.Threading
open System.Threading.Tasks
open Suave.Http
open Suave.Http.Applicatives

type SuaveService() =
    inherit StatelessService()
    let mutable port = 8083 // default port
    let buildConfig portToUse =
        { defaultConfig with
            bindings =
            [ { defaultConfig.bindings.Head with
                    socketBinding =
                        { defaultConfig.bindings.Head.socketBinding with
                            port = uint16 portToUse } } ] }
    override __.CreateCommunicationListener() =
        { new ICommunicationListener with
            member __.Abort() = ()
            member __.CloseAsync _ = Task.FromResult() :> Task
            member __.Initialize parameters =
                // We get the port to use for Suave from configuration.
                port <- parameters.CodePackageActivationContext.GetEndpoint("SuaveEndpoint").Port
            member __.OpenAsync cancellationToken =
                async {
                    let config = buildConfig port
                    let starting, server = startWebServerAsync config (context (fun _ -> OK <| sprintf "Hello from Service Fabric: %O" System.DateTime.UtcNow))
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