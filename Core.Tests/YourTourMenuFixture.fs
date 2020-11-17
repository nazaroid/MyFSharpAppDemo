namespace TourAssistant.Client.Core.Tests

open TourAssistant.Client.Core.ViewModels
open TourAssistant.Client.Core.Data
open TourAssistant.Client.Core
open TourAssistant.Client.Core.Stubs
open Microsoft.VisualStudio.TestTools.UnitTesting
open Foq
open Mvvm

[<TestClass>]
type YourTourMenuFixture() = 
    
    let mutable _c = Unchecked.defaultof<CoreComposition>
    
    [<TestInitialize>]
    member __.TestInitialize() = 
        _c <- asmTestComposition()


    [<TestMethod>]
    member __.ShouldGoToPersonalInfoPage() =

        let targetVm = PersonalInfoViewModel()
        let tourInfo = _c.Resolve<TourInfoViewModel>()
        tourInfo.PersonalInfo <- targetVm

        let mockNavigator = 
            Mock<INavigator>()
                .Setup(fun x -> <@ x.PushAsync(targetVm) @>)
                .Returns(async {()})
                .Create()
        _c.ReRegister<INavigator>(mockNavigator)

        let sut = _c.Resolve<YourTourMenuViewModel>()
        //when
        sut.ShowPersonalInfoPage.Execute()
        //then
        Mock.Verify(<@ mockNavigator.PushAsync(targetVm) @>, once)

    [<TestMethod>]
    member __.ShouldGoToFlightInfoPage() =

        let targetVm = FlightInfoViewModel()
        let tourInfo = _c.Resolve<TourInfoViewModel>()
        tourInfo.FlightInfo <- targetVm

        let mockNavigator = 
            Mock<INavigator>()
                .Setup(fun x -> <@ x.PushAsync(targetVm) @>)
                .Returns(async {()})
                .Create()
        _c.ReRegister<INavigator>(mockNavigator)

        let sut = _c.Resolve<YourTourMenuViewModel>()
        //when
        sut.ShowFlightInfoPage.Execute()
        //then
        Mock.Verify(<@ mockNavigator.PushAsync(targetVm) @>, once) 

    [<TestMethod>]
    member __.ShouldGoToStayAreaInfoMenu() =

        let mockNavigator = 
            Mock<INavigator>()
                .Setup(fun x -> <@ x.PushAsync(It.IsAny<AreaInfoMenuViewModel>()) @>)
                .Returns(async {()})
                .Create()
        _c.ReRegister<INavigator>(mockNavigator)

        let sut = _c.Resolve<YourTourMenuViewModel>()
        //when
        sut.ShowAreaInfoPage.Execute()
        //then
        Mock.Verify(<@ mockNavigator.PushAsync(It.IsAny<AreaInfoMenuViewModel>()) @>, once)

    [<TestMethod>]
    member __.ShouldGoToServicesMenu() =

        let mockNavigator = 
            Mock<INavigator>()
                .Setup(fun x -> <@ x.PushAsync(It.IsAny<ServicesMenuViewModel>()) @>)
                .Returns(async {()})
                .Create()
        _c.ReRegister<INavigator>(mockNavigator)

        let sut = _c.Resolve<YourTourMenuViewModel>()
        //when
        sut.ShowServicesPage.Execute()
        //then
        Mock.Verify(<@ mockNavigator.PushAsync(It.IsAny<ServicesMenuViewModel>()) @>, once)