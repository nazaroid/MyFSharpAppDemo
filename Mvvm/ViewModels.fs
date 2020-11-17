namespace Mvvm

open System.ComponentModel

[<AutoOpen>]
module ViewModels =

    type INavigationAware = interface 
   
        abstract member NavigatedTo : unit -> unit

        abstract member NavigatedFrom : unit -> unit
    end

    type IViewModel = interface
        inherit INotifyPropertyChanged
        inherit INavigationAware
    end

    type ViewModel() = class
        inherit ObservableObject()

        let mutable _isBusy = false

        member this.IsBusy 
            with get () = _isBusy
            and set (v) = 
                _isBusy <- v
                this.NotifyPropertyChanged(<@ this.IsBusy @>)

        abstract member NavigatedFrom : unit->unit
              default __.NavigatedFrom() = ()

        abstract member NavigatedTo : unit->unit
              default __.NavigatedTo() = ()

        interface IViewModel with

            member x.NavigatedFrom(): unit = 
                x.NavigatedFrom()
            
            member x.NavigatedTo(): unit = 
                x.NavigatedTo()
    end