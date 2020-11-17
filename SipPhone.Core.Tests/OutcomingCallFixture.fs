namespace SipPhone.Core.Tests

open Microsoft.VisualStudio.TestTools.UnitTesting
open Foq
open SipPhone.Core.Models
open SipPhone.Core.ViewModels
open SipPhone.Core
open Mvvm

[<TestClass>]
type OutcomingCallFixture() = class

        let mutable _c = Unchecked.defaultof<SipPhoneCoreComposition>
        let mutable _mockCall = Unchecked.defaultof<MockOutcomingCall>
        let mutable _mockTraceNotifier = Unchecked.defaultof<MockTraceNotifier>
    
        [<TestInitialize>]
        member __.TestInitialize() = 
            _c <- SipPhoneCoreComposition()
            _c.Assemble(MockSipPhonePlatform() :> ISipPhonePlatform)
            _mockCall <- _c.Resolve<IOutcomingCall>() :?> MockOutcomingCall
            _mockTraceNotifier <- _c.Resolve<ITraceNotifier>() :?> MockTraceNotifier


        [<TestMethod>]
        member ___.ShouldMakeCallToContact() = 
            
            let mockNavigator = 
                Mock<INavigator>()
                    .Setup(fun x -> <@ x.PushAsync(It.IsAny<OutcomingCallViewModel>()) @>)
                    .Calls<OutcomingCallViewModel>(fun (x) -> async { x.NavigatedTo() } )
                    .Create()
            _c.ReRegister<INavigator>(mockNavigator)

            let target = _c.Resolve<SipPhoneViewModel>()
            let callVm = _c.Resolve<OutcomingCallViewModel>()
            //when
            let callingContact = target.Contacts.Item(0)
            callingContact.Call.Execute();
            //then
            Mock.Verify(<@ mockNavigator.PushAsync(callVm) @>, once)
            Assert.IsTrue(_mockCall.IsActive)

        [<TestMethod>]
        member ___.CanEndCall() = 
            
            let mockNavigator = 
                Mock<INavigator>()
                    .Setup(fun x -> <@ x.PushAsync(It.IsAny<OutcomingCallViewModel>()) @>)
                    .Calls<OutcomingCallViewModel>(fun (x) -> async { x.NavigatedTo() } )
                    .Create()
            _c.ReRegister<INavigator>(mockNavigator)

            let callVm = _c.Resolve<OutcomingCallViewModel>()
            //when
            callVm.End.Execute()
            //then
            Assert.IsFalse(_mockCall.IsActive)


        [<TestMethod>]
        member ___.CanDisplayCallingStatusMessages() = 
             let callVm = _c.Resolve<OutcomingCallViewModel>()
             //when
             _mockTraceNotifier.Notify("Call to test1 is in progress...")
             //then
             CollectionAssert.Contains(callVm.CallingLog, "Call to test1 is in progress...")
end

        
