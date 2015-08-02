namespace ServiceFabricDemo

open System.Runtime.Serialization

[<DataContract>]
type CatState() =
    [<DataMember>] member val CatHappiness = 0 with get, set
    [<DataMember>] member val OwnerRating = 0 with get, set
    [<DataMember>] member val OwnerHappiness = 0 with get, set
    [<DataMember>] member val HungerLevel = 0 with get, set
    [<DataMember>] member val Weight = 0. with get, set