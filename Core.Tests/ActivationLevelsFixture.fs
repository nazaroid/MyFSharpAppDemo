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
open Mvvm.Tools

[<TestClass>]
type NonActivatedLevelFixture() = 
    
    let mutable _c = Unchecked.defaultof<CoreComposition>
    
    [<TestInitialize>]
    member  __.TestInitialize() = 
        _c <- asmTestComposition()

        let mockNavigator = Mock<INavigator>().Create()
        _c.ReRegister<INavigator>(mockNavigator)

        let e = Exception()
        let mockDataService = Mock<IDataService<Tour>>()
                                .Setup(fun (x) -> <@ x.GetAsync() @>)
                                .Returns(async { return fail e })
                                .Create()

        _c.ReRegister<IDataService<Tour>>(mockDataService)


    [<TestMethod>]
    member  __.MostMenuItemsShouldBeBlocked() =
        //when
        let sut = _c.Resolve<MainMenuViewModel>()
        sut.RefreshAsync() |> Async.RunSynchronously
        //then
        Assert.IsTrue(sut.ShowCommonPartPage.CanExecute())
        Assert.IsFalse(sut.ShowYourTourPage.CanExecute())
        Assert.IsFalse(sut.ShowNewsAndNotificationsPage.CanExecute())

[<TestClass>]
type PreActivatedLevelFixture() = 
    
    let mutable _c = Unchecked.defaultof<CoreComposition>
    let mutable _mockDataService = Unchecked.defaultof<IDataService<Tour>>
    
    [<TestInitialize>]
    member  __.TestInitialize() = 
        _c <- asmTestComposition()

        let entity = Tour(FlightThere = Flight(Code = "there", 
                                               DepartureAeroport = Aeroport(), 
                                               ArrivalAeroport = Aeroport()), 
                          FlightBack = Flight(Code = "back", 
                                               DepartureAeroport = Aeroport(), 
                                               ArrivalAeroport = Aeroport()),
                          Client = Client(ShortName = "client name"),
                          MobApp = MobApp(ActivationLevel = MobAppActivationLevel.PreActivated))

        let mockDataService = Mock<IDataService<Tour>>()
                                .Setup(fun (x) -> <@ x.GetAsync() @>)
                                .Returns(async { return succeed entity })
                                .Create()

        _c.ReRegister<IDataService<Tour>>(mockDataService)


    [<TestMethod>]
    member  __.SomeMenuItemsShouldBeUnblocked() =

        let mockNavigator = Mock<INavigator>().Create()
        _c.ReRegister<INavigator>(mockNavigator)
        //when
        let sut = _c.Resolve<MainMenuViewModel>()
        sut.RefreshAsync() |> Async.RunSynchronously
        //then
        Assert.IsTrue(sut.ShowCommonPartPage.CanExecute())
        Assert.IsTrue(sut.ShowYourTourPage.CanExecute())
        Assert.IsFalse(sut.ShowNewsAndNotificationsPage.CanExecute())

        //when
        let sut = _c.Resolve<YourTourMenuViewModel>()
        //then
        Assert.IsTrue(sut.ShowPersonalInfoPage.CanExecute())
        Assert.IsTrue(sut.ShowFlightInfoPage.CanExecute())
        Assert.IsFalse(sut.ShowAreaInfoPage.CanExecute())
        Assert.IsFalse(sut.ShowServicesPage.CanExecute())

[<TestClass>]
type FullActivatedLevelFixture() = 
    
    let mutable _c = Unchecked.defaultof<CoreComposition>
    let mutable _mockDataService = Unchecked.defaultof<IDataService<Tour>>
    
    [<TestInitialize>]
    member  __.TestInitialize() = 
        _c <- asmTestComposition()

        let entity = Tour(FlightThere = Flight(Code = "there", 
                                               DepartureAeroport = Aeroport(), 
                                               ArrivalAeroport = Aeroport()), 
                          FlightBack = Flight(Code = "back", 
                                               DepartureAeroport = Aeroport(), 
                                               ArrivalAeroport = Aeroport()),
                          Client = Client(ShortName = "client name"),
                          MobApp = MobApp(ActivationLevel = MobAppActivationLevel.FullActivated))

        let mockDataService = Mock<IDataService<Tour>>()
                                .Setup(fun (x) -> <@ x.GetAsync() @>)
                                .Returns(async { return succeed entity })
                                .Create()

        _c.ReRegister<IDataService<Tour>>(mockDataService)


    [<TestMethod>]
    member  __.AllMenuItemsShouldBeUnblocked() =
        let mockNavigator = Mock<INavigator>().Create()
        _c.ReRegister<INavigator>(mockNavigator)

        //when
        let sut = _c.Resolve<MainMenuViewModel>()
        sut.RefreshAsync() |> Async.RunSynchronously
        //then
        Assert.IsTrue(sut.ShowCommonPartPage.CanExecute())
        Assert.IsTrue(sut.ShowYourTourPage.CanExecute())
        Assert.IsTrue(sut.ShowNewsAndNotificationsPage.CanExecute())

        //when
        let sut = _c.Resolve<YourTourMenuViewModel>()
        //then
        Assert.IsTrue(sut.ShowPersonalInfoPage.CanExecute())
        Assert.IsTrue(sut.ShowFlightInfoPage.CanExecute())
        Assert.IsTrue(sut.ShowAreaInfoPage.CanExecute())
        Assert.IsTrue(sut.ShowServicesPage.CanExecute())