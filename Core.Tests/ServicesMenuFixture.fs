namespace TourAssistant.Client.Core.Tests

open TourAssistant.Client.Core.ViewModels
open TourAssistant.Client.Core
open Microsoft.VisualStudio.TestTools.UnitTesting
open Foq
open Mvvm
open SipPhone.Core.ViewModels

[<TestClass>]
type ServicesMenuFixture() = 
    
    let mutable _c = Unchecked.defaultof<CoreComposition>
    
    [<TestInitialize>]
    member __.TestInitialize() = 
         _c <- asmTestComposition()


    [<TestMethod>]
    member __.ShouldGoToPhonePage() =
        
        let mockNavigator = 
            Mock<INavigator>()
                .Setup(fun x -> <@ x.PushAsync(It.IsAny<SipPhoneViewModel>()) @>)
                .Returns(async {()})
                .Create()
        _c.ReRegister<INavigator>(mockNavigator)

        let sut = _c.Resolve<ServicesMenuViewModel>()
        //when
        sut.ShowPhonePage.Execute()
        //then
        let targetVm = _c.Resolve<SipPhoneViewModel>()
        Mock.Verify(<@ mockNavigator.PushAsync(targetVm) @>, once)
