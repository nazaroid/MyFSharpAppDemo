namespace Mvvm

open Ninject
open Ninject.Modules
open Xamarin.Forms
open System
open System.Collections.Generic

    type ViewFactory(kernel:IKernel) = class
        
        let map = Dictionary<Type, Type>()

        let resolve setState (vm:'TViewModel) =
                let viewType = map.[typeof<'TViewModel>]
                let view = kernel.Get(viewType) :?> Page;
                setState(vm)
                view.BindingContext <- vm;
                (view, vm)

        interface IViewFactory with

            member __.Register<'TViewModel, 'TView 
                        when 'TViewModel :> IViewModel and 'TViewModel: not struct 
                        and 'TView :> Page>() = 
                map.[typeof<'TViewModel>] <- typeof<'TView>
            
            member __.Resolve<'TViewModel when 'TViewModel :> IViewModel and 'TViewModel: not struct>
                (vm: 'TViewModel) : Page*'TViewModel = 
                resolve ignore vm
            
            member __.Resolve<'TViewModel when 'TViewModel :> IViewModel and 'TViewModel: not struct>
                (setState: 'TViewModel-> unit) : Page*'TViewModel = 
                let vm = kernel.Get<'TViewModel>()
                resolve setState vm
            
            member __.Resolve(): Page * 'TViewModel = 
                let vm = kernel.Get<'TViewModel>()
                resolve ignore vm
    end
    
    type MvvmModule() = class
        inherit NinjectModule()

        let resolvePageByDefault () = 
                          let mainPage = Application.Current.MainPage

                          let (|IsMasterDetail|_|) (page:Page) = 
                                    match page with 
                                        | :? MasterDetailPage as page -> Some page.Detail
                                        | _ -> None

                          let (|IsNavigationPage|_|) (page:Page) = 
                                    match page :> obj with
                                        | :? IPageContainer<Page> as navigationPage -> Some navigationPage.CurrentPage 
                                        | _ -> None

                          let detailOrMain = match mainPage with 
                                                | IsMasterDetail page -> page
                                                | _ -> mainPage 

                          let navOrPage = match detailOrMain with 
                                            | IsNavigationPage page -> page
                                            | _ -> detailOrMain 
                            
                          navOrPage

        override this.Load() =
            ignore <| this.Kernel.Bind(typeof<unit -> Page>)
                                 .ToConstant(resolvePageByDefault)
            ignore <| this.Kernel.Bind<IViewFactory>().To<ViewFactory>()
                                 .InSingletonScope()
            ignore <| this.Kernel.Bind<INavigator>().To<Navigator>()
                                 .InSingletonScope()
            ignore <| this.Kernel.Bind<IPage>().To<PageProxy>()
                                 .InSingletonScope()
    end
    
    type IBootstrapper = interface
        abstract member Run : unit -> Page
    end

    [<AbstractClass>]
    type NinjectBootstrapper(kernel : IKernel) = class

            abstract member RegisterModels : IKernel -> unit
            default __.RegisterModels(kernel:IKernel) = kernel.Load<MvvmModule>()

            abstract member RegisterViews : IViewFactory -> unit
            default __.RegisterViews(factory:IViewFactory) = ()
 
            abstract member ResolveMainPage : IKernel -> Page
            default __.ResolveMainPage(kernel) = null

            abstract member Run : unit -> Page
            default this.Run() =
                        this.RegisterModels(kernel)
                        let viewFactory = kernel.Get<IViewFactory>();
                        this.RegisterViews(viewFactory)
                        this.ResolveMainPage(kernel)      
                                     
            interface IBootstrapper with

                member this.Run() = this.Run()
    end
           
            
        
