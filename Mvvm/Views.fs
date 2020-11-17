namespace Mvvm

open System
open Xamarin.Forms
open Microsoft.FSharp.Control

    type INavigator = interface 
        
        abstract member PopAsync: unit -> Async<IViewModel>

        abstract member PopModalAsync: unit -> Async<IViewModel>

        abstract member PopToRootAsync: unit -> Async<unit>


        abstract member PushAsync<'TViewModel when 'TViewModel :> IViewModel and 'TViewModel: not struct> 
                 : 'TViewModel -> Async<unit>

        abstract member PushAsync<'TViewModel when 'TViewModel :> IViewModel and 'TViewModel: not struct> 
                 : setState:('TViewModel -> unit ) -> Async<'TViewModel>
    
        abstract member PushAsync<'TViewModel when 'TViewModel :> IViewModel and 'TViewModel: not struct> 
                 : unit -> Async<'TViewModel>
        
        
        abstract member PushModalAsync<'TViewModel when 'TViewModel :> IViewModel and 'TViewModel: not struct> 
                 : 'TViewModel -> Async<unit>

        abstract member PushModalAsync<'TViewModel when 'TViewModel :> IViewModel and 'TViewModel: not struct> 
                 : setState:('TViewModel -> unit ) -> Async<'TViewModel>
    
        abstract member PushModalAsync<'TViewModel when 'TViewModel :> IViewModel and 'TViewModel: not struct> 
                 : unit -> Async<'TViewModel>
    end
    
    type IDialogProvider = interface
        abstract member DisplayAlert:
                title:string * message:string * cancel:string -> Async<unit>
        abstract member DisplayAlert:
                title:string * message:string * accept:string * cancel:string -> Async<bool>
        abstract member DisplayActionSheet:
                title:string * cancel:string * destruction:string * [<ParamArray>]buttons:string[] -> Async<string>
    end

    type IPage = interface
        inherit IDialogProvider
        abstract member Navigation : INavigation
    end

    type IViewFactory = interface
        abstract member Register<'TViewModel, 'TView 
                        when 'TViewModel :> IViewModel and 'TViewModel: not struct 
                        and 'TView :> Page> 
                        : unit -> unit
        abstract member Resolve<'TViewModel when 'TViewModel :> IViewModel and 'TViewModel: not struct> 
              : viewModel:'TViewModel ->  Page*'TViewModel

        abstract member Resolve<'TViewModel when 'TViewModel :> IViewModel and 'TViewModel: not struct> 
              : setState:('TViewModel -> unit) ->  Page*'TViewModel
        
        abstract member Resolve<'TViewModel when 'TViewModel :> IViewModel and 'TViewModel: not struct> 
              : unit -> Page*'TViewModel
    end

    type PageProxy(resolvePage:unit -> Page) = class
        interface IPage with

            member __.DisplayActionSheet(title: string, cancel: string, destruction: string, buttons: string []): Async<string> = 
                resolvePage().DisplayActionSheet(title, cancel, destruction, buttons) |> Async.AwaitTask
            
            member __.DisplayAlert(title: string, message: string, accept: string, cancel: string): Async<bool> = 
                resolvePage().DisplayAlert(title, message, accept, cancel) |> Async.AwaitTask
            
            member __.DisplayAlert(title: string, message: string, cancel: string): Async<unit> = 
                resolvePage().DisplayAlert(title, message, cancel) |> Async.AwaitTask
            
            member __.Navigation: INavigation = 
                resolvePage().Navigation
    end

    type Navigator(page:IPage, viewFactory: IViewFactory) = 

        let navigation () = page.Navigation

        interface INavigator with
            
            //Main 
            member __.PopToRootAsync() =
                page.Navigation.PopToRootAsync() |> Async.AwaitTask

            member __.PopAsync() =
                async {
                    let! view = navigation().PopAsync() |> Async.AwaitTask
                    let viewModel = view.BindingContext :?> IViewModel;
                    viewModel.NavigatedFrom();
                    return viewModel;
                }

            member __.PushAsync(vm:'TViewModel) =
                async {
                    let view, _ = viewFactory.Resolve(vm);
                    do! navigation().PushAsync(view) |> Async.AwaitTask
                    vm.NavigatedTo()
                    ()
                }
    
            member __.PushAsync(setState:'TViewModel -> unit) =
                async {
                    let view, vm = viewFactory.Resolve(setState)
                    do! navigation().PushAsync(view) |> Async.AwaitTask
                    vm.NavigatedTo();
                    return vm
                } 
            member __.PushAsync() =
                async {
                    let view, vm = viewFactory.Resolve<'TViewModel>()
                    do! navigation().PushAsync(view) |> Async.AwaitTask
                    vm.NavigatedTo();
                    return vm
                } 

            //Modal
            member __.PopModalAsync() =
                async {
                    let! view = navigation().PopModalAsync() |> Async.AwaitTask
                    let viewModel = view.BindingContext :?> IViewModel;
                    viewModel.NavigatedFrom();
                    return viewModel;
                } 

            member __.PushModalAsync(vm:'TViewModel) =
                async {
                    let view, _ = viewFactory.Resolve(vm);
                    do! navigation().PushModalAsync(view) |> Async.AwaitTask
                    vm.NavigatedTo()
                }
            member __.PushModalAsync(setState:'TViewModel -> unit) =
                async {
                    let view, vm = viewFactory.Resolve(setState)
                    do! navigation().PushModalAsync(view) |> Async.AwaitTask
                    vm.NavigatedTo();
                    return vm
                } 
            member __.PushModalAsync() =
                async {
                    let view, vm = viewFactory.Resolve<'TViewModel>()
                    do! navigation().PushModalAsync(view) |> Async.AwaitTask
                    vm.NavigatedTo();
                    return vm
                } 
    
 