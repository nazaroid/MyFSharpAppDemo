namespace TourAssistant.Client.Core.ViewModels

open System
open TourAssistant.Domain
open TourAssistant.Client.Core.Models
open TourAssistant.Client.Core.Platform
open Rop
open Microsoft.FSharp.Control
open System.Threading
open Mvvm
open Mvvm.Tools
open System.Windows.Input

[<AbstractClass>]
type AreaItemViewModel(areaType: string) =
    class 
        inherit ViewModel()

        let mutable _failToObtainData = false

        member this.FailToObtainData  
                    with get() = _failToObtainData 
                    and set(v) = 
                        _failToObtainData <- v 
                        this.NotifyPropertyChanged(<@ this.FailToObtainData @>)

        member __.AreaType 
            with get () = areaType
    end

type AreaMapViewModel(areaType: string) =
    class
        inherit AreaItemViewModel(areaType)

        let mutable _image = Unchecked.defaultof<Image>

        member this.Image 
            with get () = _image
            and set (v) = 
                _image <- v
                this.NotifyPropertyChanged(<@ this.Image @>)
    end

type AreaDescriptionViewModel(areaType: string) =
    class
        inherit AreaItemViewModel(areaType)

        let mutable _text = Unchecked.defaultof<Text>

        member this.Text 
            with get () = _text
            and set (v) = 
                _text <- v
                this.NotifyPropertyChanged(<@ this.Text @>)
    end

[<AbstractClass>]
type AreaInfoViewModel<'T when 'T :> AreaInfo>(dataService : IDataService<'T>, 
                                               logger : ILogger, 
                                               areaType: string) =
    class
        inherit ViewModel()

        let mutable _map = AreaMapViewModel(areaType)
        let mutable _description = AreaDescriptionViewModel(areaType)

        member __.AreaType with get () = areaType

        member this.Map 
            with get () = _map
            and set (v) = 
                _map <- v
                this.NotifyPropertyChanged(<@ this.Map @>)

        member this.Description 
            with get () = _description
            and set (v) = 
                _description <- v
                this.NotifyPropertyChanged(<@ this.Description @>) 
                                                              
        member this.RefreshAsync () = 
                let logFail (e : Exception) =
                     logger.Warn <| ViewModelTasks.formatTaskError this "refresh" e

                let onSuccess (entity : 'T) = 
                    this.Map.Image <- entity.Map
                    this.Description.Text <- entity.Description

                let onFail ex = 
                        logFail ex
                        this.Map.FailToObtainData <- true
                        this.Description.FailToObtainData <- true

                ViewModelTasks.loadDataAsync dataService.GetAsync onSuccess onFail 
    end
type CountryInfoViewModel(dataService : IDataService<CountryInfo>, logger : ILogger) =
    class
        inherit AreaInfoViewModel<CountryInfo>(dataService, logger, "country")
    end
type KurortInfoViewModel(dataService : IDataService<KurortInfo>, logger : ILogger) =
    class
        inherit AreaInfoViewModel<KurortInfo>(dataService, logger, "kurort")
    end
type HotelInfoViewModel(dataService : IDataService<HotelInfo>, logger : ILogger) =
    class
        inherit AreaInfoViewModel<HotelInfo>(dataService, logger, "hotel")
    end

[<AbstractClass>]
type AreaItemInfoMenuViewModel<'TM, 'TVm when 'TM :> AreaInfo and 'TVm :> AreaInfoViewModel<'TM>>
        (navigator : INavigator, vm: 'TVm, logger: ILogger) =
    class
        inherit NavigatedViewModel(navigator)

        abstract AreaType : string with get
        
        member this.RefreshAsync () =

            let logFail (ex : Exception) =
                 logger.Warn <| ViewModelTasks.formatTaskError this "refresh" ex

            let notifyDataChanged() =
                this.NotifyPropertyChanged(<@ this.ShowDescriptionPage @>)
                this.NotifyPropertyChanged(<@ this.ShowMapPage @>)

            ViewModelTasks.waitAsyncTask this vm.RefreshAsync 
                                        notifyDataChanged logFail  

        override this.NavigatedTo() = this.RefreshAsync() |> Async.StartImmediate


        member this.ShowDescriptionPage = 
            Command((fun _ -> this.GoTo vm.Description), 
                    (fun _ -> not this.IsBusy)) :> ICommand

        member this.ShowMapPage = 
            Command((fun _ -> this.GoTo vm.Map), 
                    (fun _ -> not this.IsBusy)) :> ICommand

    end
type CountryInfoMenuViewModel(navigator : INavigator, 
                              vm: CountryInfoViewModel, 
                              logger: ILogger) =
    class
        inherit AreaItemInfoMenuViewModel<CountryInfo, CountryInfoViewModel>(navigator, vm, logger)

        override __.AreaType with get () = "country"
    end
type KurortInfoMenuViewModel(navigator : INavigator, 
                             vm: KurortInfoViewModel, 
                             logger: ILogger) =
    class
        inherit AreaItemInfoMenuViewModel<KurortInfo, KurortInfoViewModel>(navigator, vm, logger)

        override __.AreaType with get () = "kurort"
    end
type HotelInfoMenuViewModel(navigator : INavigator,
                            vm: HotelInfoViewModel, 
                            logger: ILogger) =
    class
        inherit AreaItemInfoMenuViewModel<HotelInfo, HotelInfoViewModel>(navigator, vm, logger)

        override __.AreaType with get () = "hotel"
    end