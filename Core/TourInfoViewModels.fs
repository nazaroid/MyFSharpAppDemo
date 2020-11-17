namespace TourAssistant.Client.Core.ViewModels

open System
open TourAssistant.Domain
open TourAssistant.Client.Core.Models
open TourAssistant.Client.Core.Platform
open Microsoft.FSharp.Control
open System.Threading
open Mvvm
open Mvvm.Tools
open Rop
open System.Windows.Input

type PersonalInfoViewModel() = 
    inherit ViewModel()

    member val ShortName = "" with get, set
    member val Passport = "" with get, set
    member val Insurance = "" with get, set

type FlightViewModel() = 
    inherit ViewModel()

    member val Code = "" with get, set
    member val DepartureAeroport = "" with get, set
    member val DepartureTime = DateTime() with get, set
    member val ArrivalAeroport = "" with get, set

type FlightInfoViewModel() = 
    inherit ViewModel()

    member val There = FlightViewModel() with get, set
    member val Back = FlightViewModel() with get, set
    
type TourInfoViewModel(dataService : IDataService<Tour>, logger : ILogger) = 
    class
        inherit ViewModel()

        let mutable _flightsInfo = FlightInfoViewModel()
        let mutable _personalInfo = PersonalInfoViewModel()
        let mutable _mobAppActivationLevel = MobAppActivationLevel.NonActivated
        
        member this.FlightInfo 
            with get () = _flightsInfo
            and set (v) = 
                _flightsInfo <- v
                this.NotifyPropertyChanged(<@ this.FlightInfo @>)

        member this.PersonalInfo 
            with get () = _personalInfo
            and set (v) = 
                _personalInfo <- v
                this.NotifyPropertyChanged(<@ this.PersonalInfo @>)
        
        member this.MobAppActivationLevel 
            with get () = _mobAppActivationLevel
            and set (v) = 
                _mobAppActivationLevel <- v
                this.NotifyPropertyChanged(<@ this.MobAppActivationLevel @>)

        member this.RefreshAsync() = 

            let mapFlightInfo (entity : Tour) = 
                let map (entity : Flight) = FlightViewModel(Code = entity.Code, 
                                                            DepartureTime = entity.DepartureTime, 
                                                            DepartureAeroport = entity.DepartureAeroport.Name, 
                                                            ArrivalAeroport = entity.ArrivalAeroport.Name)
                FlightInfoViewModel(There = map (entity.FlightThere), Back = map (entity.FlightBack))

            let mapPersonalInfo (entity : Tour) = 
                PersonalInfoViewModel(ShortName = entity.Client.ShortName, 
                                      Passport = entity.Client.Passport, 
                                      Insurance = entity.Insurance)

            let logFail (ex : Exception) =
                 logger.Warn <| ViewModelTasks.formatTaskError this "refresh" ex
            
            let onSuccess (entity : Tour) = 
                this.FlightInfo <- entity |> mapFlightInfo
                this.PersonalInfo <- entity |> mapPersonalInfo
                this.MobAppActivationLevel <- entity.MobApp.ActivationLevel

            let onFail ex = logFail ex
                            this.MobAppActivationLevel <- MobAppActivationLevel.NonActivated

            ViewModelTasks.loadDataAsync dataService.GetAsync onSuccess onFail
    end
