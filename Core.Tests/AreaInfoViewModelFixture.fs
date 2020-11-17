namespace TourAssistant.Client.Core.Tests

open System
open TourAssistant.Client.Core.ViewModels
open TourAssistant.Domain
open TourAssistant.Client.Core.Models
open TourAssistant.Client.Core.Data
open TourAssistant.Client.Core
open TourAssistant.Client.Core.Stubs
open Microsoft.VisualStudio.TestTools.UnitTesting
open Foq
open Rop
open Mvvm

[<TestClass>]
type AreaInfoViewModelFixture() = 
    
    let mutable _c = Unchecked.defaultof<CoreComposition>
    
    [<TestInitialize>]
    member __.TestInitialize() = 
         _c <- asmTestComposition()


    [<TestMethod>]
    member __.ShouldDisplayStayAreaInfo() =
        let entity = CountryInfo(Map = [||], Description = "")    
        let mockDataService = 
                Mock<IDataService<CountryInfo>>()
                    .Setup(fun x -> <@ x.GetAsync() @>)
                    .Returns(async { return Success entity})
                    .Create()
        //when
        _c.ReRegister<IDataService<CountryInfo>>(mockDataService)
        let sut = _c.Resolve<CountryInfoViewModel>()
        sut.RefreshAsync() |> Async.RunSynchronously
        //then
        Assert.AreEqual(entity.Map, sut.Map.Image)
        Assert.AreEqual(entity.Description, sut.Description.Text)
        //and
        Assert.IsFalse(sut.Map.FailToObtainData)
        Assert.IsFalse(sut.Description.FailToObtainData)

    [<TestMethod>]
    member __.WhenFailToObtainData_ThenShouldDisplayError() =

        let mockDataService = 
                Mock<IDataService<AreaInfo>>()
                    .Setup(fun x -> <@ x.GetAsync() @>)
                    .Returns(async { return Failure (Exception())})
                    .Create()
        _c.ReRegister<IDataService<AreaInfo>>(mockDataService)

        let sut = _c.Resolve<CountryInfoViewModel>()
        let expected = sut.Map.Image, sut.Description.Text
        Assert.IsNotNull(expected)
        //when
        sut.RefreshAsync() |> Async.RunSynchronously
        //then should leave previously displayed data
        Assert.AreSame(fst expected, sut.Map.Image)
        Assert.AreSame(snd expected, sut.Description.Text)
        //and display error
        Assert.IsTrue(sut.Map.FailToObtainData)
        Assert.IsTrue(sut.Description.FailToObtainData)

        