namespace TourAssistant.Client.Core.Tests

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
type MainMenuViewModelFixture() = 
    
    let mutable _c = Unchecked.defaultof<CoreComposition>
    let mutable _mockDataService = Unchecked.defaultof<IDataService<Tour>>
    
    [<TestInitialize>]
    member __.TestInitialize() = 
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
    member __.ShouldGoToYourTourMenu() =

        let mockNavigator = 
            Mock<INavigator>()
                .Setup(fun x -> <@ x.PushAsync(It.IsAny<YourTourMenuViewModel>()) @>)
                .Returns(async {()})
                .Create()
        _c.ReRegister<INavigator>(mockNavigator)

        //when
        let sut = _c.Resolve<MainMenuViewModel>()
        sut.RefreshAsync() |> Async.RunSynchronously
        sut.ShowYourTourPage.Execute()
        //then
        Mock.Verify(<@ mockNavigator.PushAsync(It.IsAny<YourTourMenuViewModel>()) @>, once)
