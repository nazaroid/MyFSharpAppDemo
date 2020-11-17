namespace SipPhone.Core.Models
open System.Collections.Generic

type ITraceListener = interface
        abstract OnTrace : msg:string -> unit
end
type ITraceNotifier = interface
        abstract RegisterListener : listener:ITraceListener -> unit
end
type IOutcomingCall = interface
        abstract Begin : address:string -> unit
        abstract End : unit -> unit
end    

type IIncomingCall = interface
        abstract End : unit -> unit
end  

type ISipPhonePlatform = interface
    abstract member OutcomingCall : unit -> IOutcomingCall
    abstract member TraceNotifier : unit -> ITraceNotifier
end


type IIncomingCallSubscriber = interface
    abstract OnIncomingCall : IIncomingCall -> unit
end

type RegistrationStatus = 
    | Offline = 0
    | Registering = 1
    | Online = 2
    | Error = 3
type IRegistraionStatusSubscriber = interface
    abstract OnRegistrationStatus : RegistrationStatus -> unit
end

type IBackgroundServiceCommandListener = interface
    abstract OnStartCmd : unit -> unit
    abstract OnRaiseRegistrationStatusCmd : unit -> unit
end

module BackgroundServiceNotifications =
    let callSubscribers = List<IIncomingCallSubscriber>()
    let statusSubscribers = List<IRegistraionStatusSubscriber>()
    let SubscribeForIncomingCalls(s) = callSubscribers.Add(s)
    let SubscribeForRegistrationStatus(s) = statusSubscribers.Add(s)
    let NotifyIncomingCall(call:IIncomingCall) 
        = callSubscribers |> Seq.iter (fun(el) -> el.OnIncomingCall(call))
    let NotifyRegistrationStatus(status:RegistrationStatus) 
        = statusSubscribers |> Seq.iter (fun(el) -> el.OnRegistrationStatus(status))

module BackgroundServiceInputCommands =
    let listeners = List<IBackgroundServiceCommandListener>()
    let Listen(l) = listeners.Add(l)
    let SendStartCmd() = listeners |> Seq.iter (fun(el) -> el.OnStartCmd())
    let RaiseRegistrationStatusCmd() = listeners |> Seq.iter (fun(el) -> el.OnRaiseRegistrationStatusCmd())