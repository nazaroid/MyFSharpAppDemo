namespace TourAssistant.Client.Core.ViewModels

open System
open TourAssistant.Domain
open TourAssistant.Client.Core.Platform
open Rop
open Microsoft.FSharp.Control
open Mvvm
open Mvvm.Tools
open SipPhone.Core.ViewModels
open System.Windows.Input


type ServicesMenuViewModel(navigator : INavigator, sipPhone : SipPhoneViewModel) =
    class
        inherit NavigatedViewModel(navigator)

        member this.ShowPhonePage = 
            Command((fun _ -> this.GoTo sipPhone), 
                    (fun _ -> true)) :> ICommand
    end

type AreaInfoMenuViewModel(navigator : INavigator, 
                               countryInfoMenu : CountryInfoMenuViewModel,
                               kurortInfoMenu : KurortInfoMenuViewModel,
                               hotelInfoMenu : HotelInfoMenuViewModel) =
    class
        inherit NavigatedViewModel(navigator)

        member this.ShowCounrtyInfoPage = 
            Command((fun _ -> this.GoTo countryInfoMenu), 
                    (fun _ -> true)) :> ICommand
        member this.ShowKurortInfoPage = 
            Command((fun _ -> this.GoTo kurortInfoMenu),
                    (fun _ -> true)) :> ICommand
        member this.ShowHotelInfoPage = 
            Command((fun _ -> this.GoTo hotelInfoMenu),
                    (fun _ -> true)) :> ICommand
    end

type YourTourMenuViewModel(navigator : INavigator, 
                           tourInfo : TourInfoViewModel, 
                           areaInfoMenu : AreaInfoMenuViewModel,
                           servicesMenu: ServicesMenuViewModel) =
    class
        inherit NavigatedViewModel(navigator)

        member this.ShowPersonalInfoPage = 
            Command((fun _ -> this.GoTo tourInfo.PersonalInfo), 
                    (fun _ -> true)) :> ICommand
        member this.ShowFlightInfoPage = 
            Command((fun _ -> this.GoTo tourInfo.FlightInfo), 
                    (fun _ -> true)) :> ICommand
        member this.ShowAreaInfoPage = 
            Command((fun _ -> this.GoTo areaInfoMenu), 
                    (fun _ -> tourInfo.MobAppActivationLevel = MobAppActivationLevel.FullActivated)) :> ICommand
        member this.ShowServicesPage = 
            Command((fun _ -> this.GoTo servicesMenu), 
                    (fun _ -> tourInfo.MobAppActivationLevel = MobAppActivationLevel.FullActivated)) :> ICommand
    end

type MainMenuViewModel(navigator : INavigator, 
                       vm : TourInfoViewModel, yourTourMenu : YourTourMenuViewModel,
                       logger:ILogger) = 
    inherit NavigatedViewModel(navigator)
    


    member this.Initialize () = 
        this.RefreshAsync() |> Async.StartImmediate

    member this.RefreshAsync () =

        let logFail (ex : Exception) =
                 logger.Warn <| ViewModelTasks.formatTaskError this "refresh" ex

        let notifyDataChanged() =
            this.NotifyPropertyChanged(<@ this.ShowCommonPartPage @>)
            this.NotifyPropertyChanged(<@ this.ShowYourTourPage @>)
            this.NotifyPropertyChanged(<@ this.ShowNewsAndNotificationsPage @>)

        ViewModelTasks.waitAsyncTask this vm.RefreshAsync 
                                    notifyDataChanged logFail  
       

    member this.ShowCommonPartPage = 
        Command(ignore, 
                (fun _ -> not this.IsBusy 
                          && true)) :> ICommand
    member this.ShowYourTourPage = 
        Command((fun _ -> this.GoTo yourTourMenu), 
                (fun _ -> not this.IsBusy 
                          && vm.MobAppActivationLevel > MobAppActivationLevel.NonActivated)) :> ICommand
    member this.ShowNewsAndNotificationsPage = 
        Command(ignore, 
                (fun _ -> not this.IsBusy 
                          && vm.MobAppActivationLevel = MobAppActivationLevel.FullActivated)) :> ICommand
