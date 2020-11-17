[<AutoOpen>]
module Mocks

    open SipPhone.Core.Models

    type MockOutcomingCall() =

        member val IsActive = false with get, set
    
        interface IOutcomingCall with
            member this.Begin(address: string): unit = this.IsActive <- true 
            member this.End(): unit = this.IsActive <- false             

    type MockTraceNotifier() =
        
        let mutable _listeners = []
        
        member __.Notify(msg: string): unit = 
                _listeners |> List.iter (fun (x:ITraceListener) -> x.OnTrace(msg))

        interface ITraceNotifier with
            member __.RegisterListener(listener: ITraceListener): unit = 
                _listeners <- listener :: _listeners

    type MockSipPhonePlatform() = 

        interface ISipPhonePlatform with
            member __.OutcomingCall(): IOutcomingCall = 
                MockOutcomingCall() :> IOutcomingCall
            member __.TraceNotifier(): ITraceNotifier = 
                MockTraceNotifier() :> ITraceNotifier
