namespace ServiceFabricDemo

open System.Runtime.Serialization
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

[<DataContract>]
type MutableCatState() =
    [<DataMember>] member val CatHappiness = 0 with get, set
    [<DataMember>] member val OwnerRating = 0 with get, set
    [<DataMember>] member val OwnerHappiness = 0 with get, set
    [<DataMember>] member val HungerLevel = 0 with get, set
    [<DataMember>] member val Weight = 0. with get, set

[<DataContract>]
[<CLIMutable>]
type ImmutableCatState = {
    [<DataMember>] CatHappiness : int
    [<DataMember>] OwnerRating : int
    [<DataMember>] OwnerHappiness : int
    [<DataMember>] HungerLevel : int
    [<DataMember>] Weight : float<kg> }