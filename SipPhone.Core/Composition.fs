namespace SipPhone.Core

open Ninject.Modules
open SipPhone.Core.Models
open SipPhone.Core.ViewModels
open Ninject

type internal SipPhoneCoreModule(sipPhonePlatform: ISipPhonePlatform) =
    class
        inherit NinjectModule()

        override this.Load() =
            let kernel = this.Kernel

            ignore <| kernel.Bind<IOutcomingCall>().ToConstant(sipPhonePlatform.OutcomingCall())
            ignore <| kernel.Bind<ITraceNotifier>().ToConstant(sipPhonePlatform.TraceNotifier())
            ignore <| kernel.Bind<OutcomingCallViewModel>().ToSelf()
                                 .InSingletonScope()
            ignore <| kernel.Bind<SipPhoneViewModel>().ToSelf()
                                 .InSingletonScope()
            ignore <| kernel.Bind<BackgroundServiceBroker>().ToSelf()
                                 .InSingletonScope()
                                 
    end

type SipPhoneCoreComposition(kernel: IKernel) =
    
    new() = SipPhoneCoreComposition(new StandardKernel())

    member __.Assemble(sipPhonePlatform: ISipPhonePlatform) =       
        kernel.Load(new SipPhoneCoreModule(sipPhonePlatform))
        
    member __.ReRegister<'TInterface>(instance) = 
        kernel.Rebind<'TInterface>().ToConstant(instance) |> ignore
    
    member __.Resolve() = 
        kernel.Get<'TInstance>()

