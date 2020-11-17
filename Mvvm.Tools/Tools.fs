namespace Mvvm.Tools

open System
open Microsoft.FSharp.Control
open System.Threading
open Mvvm
open Rop
open System.Windows.Input

type NavigatedViewModel(navigator : INavigator) =
    class
        inherit ViewModel()

        member __.GoTo(targetVm : 'TViewModel when 'TViewModel :> IViewModel) 
            = navigator.PushAsync(targetVm) |> Async.StartImmediate

    end


module ViewModelTasks =
    let loadDataAsync loadTask onSuccess onFail = 
                let run = 
                    async { 
                        let context = SynchronizationContext.Current
                        do! Async.SwitchToThreadPool()
                        let! result = loadTask()
                        do! Async.SwitchToContext(context)
                        match result with
                        | Success entity -> entity |> onSuccess
                        | Failure ex ->  ex |> onFail
                    }
                run
    let waitAsyncTask (this:ViewModel) task onCompleted onFail = 
                        async { 

                            try
                                this.IsBusy <- true

                                try 
                                    do! task()
                                with ex -> onFail ex

                            finally
                                this.IsBusy <- false
                                onCompleted()
                        }
    let formatTaskError (vm:ViewModel) (taskName:string) (ex:Exception) = 
                sprintf "'%s' %s task completed with error: %s" 
                            (vm.GetType().Name) taskName ex.Message
