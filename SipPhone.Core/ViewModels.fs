namespace SipPhone.Core.ViewModels

open Mvvm
open Mvvm.Tools
open SipPhone.Core.Models
open System.Windows.Input
open System.Collections.ObjectModel
open System.Threading

type OutcomingCallViewModel(call : IOutcomingCall, trace: ITraceNotifier) as this =
    class
        inherit ViewModel()

        let mutable _to = Unchecked.defaultof<ContactViewModel>
        let mutable _callingLog = ObservableCollection<string>()
        let mutable _guiCtx = SynchronizationContext.Current

        do
            trace.RegisterListener(this)

        member __.To
            with get () = _to
            and set (v) = _to <- v

        member this.Make = 
            Command((fun _ -> call.Begin(this.To.Address)), 
                    (fun _ -> true)) :> ICommand

        member __.End = 
            Command((fun _ -> call.End()), 
                    (fun _ -> true)) :> ICommand

        override this.NavigatedTo() = 
            this.Make.Execute()
               
                           
        member this.CallingLog 
                    with get() = _callingLog 
                    and set(v) = _callingLog <- v 
                                 this.NotifyPropertyChanged(<@ this.CallingLog @>)
        interface ITraceListener with
            member this.OnTrace(msg: string): unit = 
                try
                    let ctx = SynchronizationContext.Current
                    Async.SwitchToContext _guiCtx |> ignore
                    this.CallingLog.Add(msg)
                    Async.SwitchToContext ctx |> ignore     
                with
                | ex -> this.CallingLog.Add(sprintf "error '%s' catched while writing a msg '%s'" ex.Message msg)
    end
and ContactViewModel(navigator : INavigator, call : OutcomingCallViewModel) =
    class
        inherit NavigatedViewModel(navigator)

        let mutable _address = ""

        member __.Address
            with get () = _address
            and set (v) = _address <- v

        member this.Call = 
            Command((fun _ -> call.To <- this; this.GoTo call), 
                    (fun _ -> true)) :> ICommand
    end

type SipPhoneViewModel(navigator : INavigator, call : OutcomingCallViewModel) = 
    class
        inherit ViewModel()

        let mutable _registrationStatus = ""
        let mutable _contacts = 
            ObservableCollection<ContactViewModel>([ ContactViewModel(navigator, call, Address = "test1")
                                                     ContactViewModel(navigator, call, Address = "test2") ])
        interface IRegistraionStatusSubscriber with
           member this.OnRegistrationStatus(status: RegistrationStatus) : unit = this.RegistrationStatus <- (status.ToString())
                                        
        member __.Contacts with get () = _contacts
        member this.RegistrationStatus with get () = _registrationStatus 
                                       and set (v) = _registrationStatus <- v
                                                     this.NotifyPropertyChanged(<@ this.RegistrationStatus @>)

        override __.NavigatedTo() = 
            BackgroundServiceInputCommands.RaiseRegistrationStatusCmd()
    end

type IncomingCallViewModel(call : IIncomingCall) =
    class
        inherit ViewModel()
        
        let mutable _from = Unchecked.defaultof<ContactViewModel>

        member __.From
            with get () = _from
            and set (v) = _from <- v

        member __.End = 
            Command((fun _ -> call.End()), 
                    (fun _ -> true)) :> ICommand
    end

type BackgroundServiceBroker(navigator : INavigator, sipPhone: SipPhoneViewModel) = class
    inherit NavigatedViewModel(navigator)

    member __.SendStartServiceCmd() = 
            BackgroundServiceInputCommands.SendStartCmd();
    member this.BeginListenIncomingCalls() = 
        
        let s = 
            { new IIncomingCallSubscriber with
                  member __.OnIncomingCall(call:IIncomingCall) = 
                                            let incomingCallVm = IncomingCallViewModel(call)
                                            this.GoTo incomingCallVm 
                  }
        BackgroundServiceNotifications.SubscribeForIncomingCalls(s)
        
    member __.BeginListenRegistrationStatus() = 
        
        BackgroundServiceNotifications.SubscribeForRegistrationStatus(sipPhone)
end