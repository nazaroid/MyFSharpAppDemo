namespace Mvvm

open System.Windows.Input

type Command(action, canExecute) =

        let canExecuteChanged = Event<_, _>()

        new(action) = Command(action, fun _ -> true)

        interface ICommand with 
            member __.CanExecute(obj) = canExecute (obj)
            member __.Execute(obj) = action (obj)
            member __.add_CanExecuteChanged (handler) = 
                            canExecuteChanged.Publish.AddHandler(handler)
            member __.remove_CanExecuteChanged (handler) = 
                            canExecuteChanged.Publish.AddHandler(handler)    
