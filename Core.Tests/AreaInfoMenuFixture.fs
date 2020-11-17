namespace TourAssistant.Client.Core.Tests

open TourAssistant.Client.Core.ViewModels
open TourAssistant.Client.Core.Data
open TourAssistant.Client.Core
open TourAssistant.Client.Core.Stubs
open Microsoft.VisualStudio.TestTools.UnitTesting
open Foq
open Mvvm

[<TestClass>]
type AreaInfoMenuFixture() = 
    
    let mutable _c = Unchecked.defaultof<CoreComposition>
    
    [<TestInitialize>]
    member __.TestInitialize() = 
        _c <- asmTestComposition()


    [<TestMethod>]
    member __.ShouldGoToCountryInfoMenu() =
            
        let mockNavigator = 
            Mock<INavigator>()
                .Setup(fun x -> <@ x.PushAsync(It.IsAny<CountryInfoMenuViewModel>()) @>)
                .Returns(async {()})
                .Create()
        _c.ReRegister<INavigator>(mockNavigator)

        let sut = _c.Resolve<AreaInfoMenuViewModel>()
        //when
        sut.ShowCounrtyInfoPage.Execute()
        //then
        Mock.Verify(<@ mockNavigator.PushAsync(It.IsAny<CountryInfoMenuViewModel>()) @>, once)    

    [<TestMethod>]
    member __.ShouldGoToKurortInfoMenu() =

        let mockNavigator = 
            Mock<INavigator>()
                .Setup(fun x -> <@ x.PushAsync(It.IsAny<KurortInfoMenuViewModel>()) @>)
                .Returns(async {()})
                .Create()
        _c.ReRegister<INavigator>(mockNavigator)

        let sut = _c.Resolve<AreaInfoMenuViewModel>()
        //when
        sut.ShowKurortInfoPage.Execute()
        //then
        Mock.Verify(<@ mockNavigator.PushAsync(It.IsAny<KurortInfoMenuViewModel>()) @>, once)
    
    [<TestMethod>]
    member __.ShouldGoToHotelInfoMenu() =

        let mockNavigator = 
            Mock<INavigator>()
                .Setup(fun x -> <@ x.PushAsync(It.IsAny<HotelInfoMenuViewModel>()) @>)
                .Returns(async {()})
                .Create()
        _c.ReRegister<INavigator>(mockNavigator)

        let sut = _c.Resolve<AreaInfoMenuViewModel>()
        //when
        sut.ShowHotelInfoPage.Execute()
        //then
        Mock.Verify(<@ mockNavigator.PushAsync(It.IsAny<HotelInfoMenuViewModel>()) @>, once)

    [<TestMethod>]
    member __.ShouldGoToCountryMap() =

        let vm = _c.Resolve<CountryInfoViewModel>()
        
        let mockNavigator = 
            Mock<INavigator>()
                .Setup(fun x -> <@ x.PushAsync(vm.Map) @>)
                .Returns(async {()})
                .Create()
        _c.ReRegister<INavigator>(mockNavigator)
        _c.ReRegister<CountryInfoViewModel>(vm)

        let sut = _c.Resolve<CountryInfoMenuViewModel>()
        //when
        sut.ShowMapPage.Execute()
        //then
        Mock.Verify(<@ mockNavigator.PushAsync(vm.Map) @>, once)

    [<TestMethod>]
    member __.ShouldGoToCountryDescription() =

        let vm = _c.Resolve<CountryInfoViewModel>()
        
        let mockNavigator = 
            Mock<INavigator>()
                .Setup(fun x -> <@ x.PushAsync(vm.Description) @>)
                .Returns(async {()})
                .Create()
        _c.ReRegister<INavigator>(mockNavigator)
        _c.ReRegister<CountryInfoViewModel>(vm)

        let sut = _c.Resolve<CountryInfoMenuViewModel>()
        //when
        sut.ShowDescriptionPage.Execute()
        //then
        Mock.Verify(<@ mockNavigator.PushAsync(vm.Description) @>, once)