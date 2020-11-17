namespace TourAssistant.Client.Core.Tests

open Microsoft.VisualStudio.TestTools.UnitTesting
open TourAssistant.Client.Core
open TourAssistant.Client.Core.Models
open TourAssistant.Client.Core.Stubs
open Rop
open Foq
open System
open TourAssistant.Domain
open TourAssistant.Client.Core.Data

[<TestClass>]
type AreaInfoDataServiceFixture() = 
    
    let mutable _c = Unchecked.defaultof<CoreComposition>
    
    [<TestInitialize>]
    member __.TestInitialize() = 
        _c <- asmTestComposition()
    
    [<TestMethod>]
    member __.WhenOffline_ThenShouldLoadEntityFromCache() =
        let expected = CountryInfo(Map = Array.empty<byte>, Description = "")
        let mockEntityProvider = 
                Mock<IEntityProvider<CountryInfo>>()
                    .Setup(fun x -> <@ x.GetRemoteAsync() @>)
                    .Raises(Exception())
                    .Setup(fun x -> <@ x.GetCached() @>)
                    .Returns(expected)
                    .Create()
        _c.ReRegister<IEntityProvider<CountryInfo>>(mockEntityProvider)
        let sut = _c.Resolve<IDataService<CountryInfo>>() 
        //when
        let result = sut.GetAsync() |> Async.RunSynchronously
        //then
        match result with
            | Success actual -> Assert.AreEqual(expected, actual)
            | _ -> Assert.Fail()

    [<TestMethod>]
    member __.WhenOffline_AndCacheIsEmpty_ThenShouldLoadEntityFromLocalStorage() =
        let expected = CountryInfo(Map = Array.empty<byte>, Description = "")
        let mockEntityProvider = 
                Mock<IEntityProvider<CountryInfo>>()
                    .Setup(fun x -> <@ x.GetRemoteAsync() @>)
                    .Raises(Exception())
                    .Setup(fun x -> <@ x.GetCached() @>)
                    .Raises(Exception())
                    .Setup(fun x -> <@ x.GetLocal() @>)
                    .Returns(expected)
                    .Create()
        _c.ReRegister<IEntityProvider<CountryInfo>>(mockEntityProvider)
        let sut = _c.Resolve<IDataService<CountryInfo>>() 
        //when
        let result = sut.GetAsync() |> Async.RunSynchronously
        //then
        match result with
            | Success actual -> Assert.AreEqual(expected, actual)
            | _ -> Assert.Fail()