namespace SipPhone.Core.Tests

open Microsoft.VisualStudio.TestTools.UnitTesting
open Foq
open SipPhone.Core.Models
open SipPhone.Core.ViewModels
open SipPhone.Core
open Mvvm

[<TestClass>]
type BackgroudServiceInteractionFixture() = class

    let mutable _c = Unchecked.defaultof<SipPhoneCoreComposition>
    let mutable _mockNavigator = Unchecked.defaultof<INavigator>

    [<TestInitialize>]
    member __.TestInitialize() = 
            _c <- SipPhoneCoreComposition()
            _c.Assemble(MockSipPhonePlatform() :> ISipPhonePlatform)
            _mockNavigator <- 
                Mock<INavigator>()
                    .Setup(fun x -> <@ x.PushAsync(It.IsAny<IncomingCallViewModel>()) @>)
                    .Calls<IncomingCallViewModel>(fun (x) -> async { x.NavigatedTo() } )
                    .Create()
            _c.ReRegister<INavigator>(_mockNavigator)


    [<TestMethod>]
    member ___.ShouldStartService() = 

        let broker = _c.Resolve<BackgroundServiceBroker>()
        let mutable serviceStarted = false
        let service = 
            { new IBackgroundServiceCommandListener with
                  member __.OnStartCmd() = serviceStarted <- true
                  member __.OnRaiseRegistrationStatusCmd() = failwith "not implemented" }
        //when
        BackgroundServiceInputCommands.Listen(service)
        broker.SendStartServiceCmd()
        //then
        Assert.IsTrue(serviceStarted)
        ()


    [<TestMethod>]
    member ___.ShouldDisplayIncomingCall() =

        let broker = _c.Resolve<BackgroundServiceBroker>()
        //when
        broker.BeginListenIncomingCalls()
        let call =   { new IIncomingCall with
                           member __.End(): unit = 
                               failwith "Not implemented yet"
                            
                  }
        BackgroundServiceNotifications.NotifyIncomingCall(call)
        //then
        Mock.Verify(<@ _mockNavigator.PushAsync(It.IsAny<IncomingCallViewModel>()) @>, once)
        () 
end

