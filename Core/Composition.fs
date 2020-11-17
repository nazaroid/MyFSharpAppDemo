namespace TourAssistant.Client.Core

open Ninject
open Ninject.Modules
open TourAssistant.Client.Core.Models
open TourAssistant.Client.Core.ViewModels
open TourAssistant.Client.Core.Platform
open TourAssistant.Domain
open TourAssistant.Client.Core.Data
open TourAssistant.Client.Core.DataAccess
open SipPhone.Core.Models
open SipPhone.Core

type internal CoreModule(settings:ServerSettings, platform:IAppPlatform) =
    inherit NinjectModule()
    
    override this.Load() =

            let kernel = this.Kernel

            ignore <| kernel.Bind<ServerSettings>().ToConstant(settings)

            ignore <| kernel.Bind<ILogger>().ToConstant(platform.GetLogger())
            ignore <| kernel.Bind<ITextFileService>().ToConstant(platform.GetTextFileService())


            ignore <| kernel.Bind<HttpService>().ToSelf()
                             .InSingletonScope()
            
            ignore <| kernel.Bind<ServicesMenuViewModel>().ToSelf()
                             .InSingletonScope()
            ignore <| kernel.Bind<YourTourMenuViewModel>().ToSelf()
                             .InSingletonScope()
            ignore <| kernel.Bind<MainMenuViewModel>().ToSelf()
                             .InSingletonScope() 
            
            //#region tour info

            ignore <| kernel.Bind<BreezeLocalCache<_>>().ToSelf()
                             .InSingletonScope()

            ignore <| kernel.Bind<IEntityProvider<Tour>>().To<TourProvider>()
                             .InSingletonScope()
            ignore <| kernel.Bind<IDataService<Tour>>().To<TourDataService>()
                             .InSingletonScope()

            ignore <| kernel.Bind<TourInfoViewModel>().ToSelf()
                             .InSingletonScope() 
            //#endregion

            //#region area info
            ignore <| kernel.Bind<AreaInfoMenuViewModel>().ToSelf()
                             .InSingletonScope()

            ignore <| kernel.Bind<JsonLocalCache<_>>().ToSelf()
                             .InSingletonScope()

            ignore <| kernel.Bind<IEntityProvider<CountryInfo>>().To<AreaInfoProvider<CountryInfo>>()
                             .InSingletonScope()
            ignore <| kernel.Bind<IEntityProvider<KurortInfo>>().To<AreaInfoProvider<KurortInfo>>()
                             .InSingletonScope()
            ignore <| kernel.Bind<IEntityProvider<HotelInfo>>().To<AreaInfoProvider<HotelInfo>>()
                             .InSingletonScope()

            ignore <| kernel.Bind<IDataService<CountryInfo>>().To<AreaInfoDataService<CountryInfo>>()
                             .InSingletonScope()
            ignore <| kernel.Bind<IDataService<KurortInfo>>().To<AreaInfoDataService<KurortInfo>>()
                             .InSingletonScope()
            ignore <| kernel.Bind<IDataService<HotelInfo>>().To<AreaInfoDataService<HotelInfo>>()
                             .InSingletonScope() 

            ignore <| kernel.Bind<CountryInfoMenuViewModel>().ToSelf()
                             .InSingletonScope()
            ignore <| kernel.Bind<KurortInfoMenuViewModel>().ToSelf()
                             .InSingletonScope()
            ignore <| kernel.Bind<HotelInfoMenuViewModel>().ToSelf()
                             .InSingletonScope()

            //#endregion

type CoreComposition(kernel:IKernel) =
    
    new() = CoreComposition(new StandardKernel())

    member __.Assemble(settings, platform:IAppPlatform) =
        kernel.Load(new CoreModule(settings, platform))
        
    member __.ReRegister<'TInterface>(instance) = 
        kernel.Rebind<'TInterface>().ToConstant(instance) |> ignore
    
    member __.Resolve() = 
        kernel.Get<'TInstance>()

    