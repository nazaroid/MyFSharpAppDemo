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

[<TestClass>]
type TourInfoViewModelFixture() = 
    
    let mutable _c = Unchecked.defaultof<CoreComposition>
    
    [<TestInitialize>]
    member __.TestInitialize() = 
        _c <- asmTestComposition()
    
    [<TestMethod>]
    member __.RealCompositionTest() =
        //when
        let sut = _c.Resolve<TourInfoViewModel>()
        //then
        Assert.IsNotNull(sut)
    
    [<TestMethod>]
    member __.ShouldRefreshData() =

        let entity = Tour(FlightThere = Flight(Code = "there", 
                                               DepartureAeroport = Aeroport(), 
                                               ArrivalAeroport = Aeroport()), 
                          FlightBack = Flight(Code = "back", 
                                               DepartureAeroport = Aeroport(), 
                                               ArrivalAeroport = Aeroport()),
                          Client = Client(ShortName = "client name"), 
                          MobApp = MobApp(ActivationLevel = MobAppActivationLevel.PreActivated))

        let mockDataService = 
                Mock<IDataService<Tour>>()
                    .Setup(fun x -> <@ x.GetAsync() @>)
                    .Returns(async { return Success entity})
                    .Create()
        //when
        _c.ReRegister<IDataService<Tour>>(mockDataService)
        let sut = _c.Resolve<TourInfoViewModel>()
        sut.RefreshAsync() |> Async.RunSynchronously
        //then
        Assert.AreEqual("there", sut.FlightInfo.There.Code)
        Assert.AreEqual("back", sut.FlightInfo.Back.Code)
        Assert.AreEqual("client name", sut.PersonalInfo.ShortName)

    [<TestMethod>]
    member __.WhenFailToObtainData_ThenShouldLeavePreviouslyDisplayedData() =

        let mockDataService = 
                Mock<IDataService<Tour>>()
                    .Setup(fun x -> <@ x.GetAsync() @>)
                    .Returns(async { return Failure (Exception())})
                    .Create()
        _c.ReRegister<IDataService<Tour>>(mockDataService)

        let sut = _c.Resolve<TourInfoViewModel>()
        let expected = sut.FlightInfo, sut.PersonalInfo
        Assert.IsNotNull(expected)
        //when
        sut.RefreshAsync() |> Async.RunSynchronously
        //then
        Assert.AreSame(fst expected, sut.FlightInfo)
        Assert.AreSame(snd expected, sut.PersonalInfo)
