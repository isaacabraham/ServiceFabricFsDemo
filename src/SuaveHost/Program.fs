open Suave
open Suave.Http.Successful
open Suave.Types
open Suave.Web
open System.Fabric
open System.Threading
open System.Threading.Tasks
open Suave.Http
open Microsoft.ServiceFabric.Services.Runtime
open Microsoft.ServiceFabric.Services.Communication.Runtime

type SuaveService() =
    inherit StatelessService()

    let buildConfig portToUse =
        { defaultConfig with
            bindings =
            [ { defaultConfig.bindings.Head with
                    socketBinding =
                        { defaultConfig.bindings.Head.socketBinding with
                            port = uint16 portToUse } } ] }
    
    override __.CreateServiceInstanceListeners() =
        seq {
            yield ServiceInstanceListener(fun parameters ->
                { new ICommunicationListener with
                    member __.Abort() = ()
                    member __.CloseAsync _ = Task.FromResult() :> Task
                    member __.OpenAsync cancellationToken =
                        async {
                            let config =
                                parameters.CodePackageActivationContext.GetEndpoint("SuaveEndpoint").Port
                                |> buildConfig
                            let starting, server = startWebServerAsync config (OK "Hello from Service Fabric!")
                            Async.Start(server, cancellationToken)
                            do! starting |> Async.Ignore
                            return (defaultConfig.bindings.Head.ToString())
                        } |> Async.StartAsTask
                }) }

[<EntryPoint>]
let main argv = 
    use fabricRuntime = FabricRuntime.Create()
    fabricRuntime.RegisterServiceType("SuaveApiType", typeof<SuaveService>)
    Thread.Sleep Timeout.Infinite
    0