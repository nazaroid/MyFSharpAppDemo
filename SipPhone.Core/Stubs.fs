namespace SipPhone.Core.Stubs

open SipPhone.Core.Models

type StubOutcomingCall() =
    interface IOutcomingCall with
        member __.Begin(address: string): unit = ()
        member __.End(): unit = ()

type StubTraceNotifier() =
    interface ITraceNotifier with
        member __.RegisterListener(listener: ITraceListener): unit = 
            ()

type StubSipPhonePlatform() = 

    interface ISipPhonePlatform with
        member __.OutcomingCall() = StubOutcomingCall() :> IOutcomingCall
        member __.TraceNotifier() = StubTraceNotifier() :> ITraceNotifier
        
         