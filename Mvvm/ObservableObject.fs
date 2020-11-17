﻿namespace Mvvm

open System.ComponentModel
open Microsoft.FSharp.Quotations.Patterns

type ObservableObject () =
    let propertyChanged = 
        Event<PropertyChangedEventHandler,PropertyChangedEventArgs>()
    let getPropertyName = function 
        | PropertyGet(_,pi,_) -> pi.Name
        | _ -> invalidOp "Expecting property getter expression"
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member __.PropertyChanged = propertyChanged.Publish
    member this.NotifyPropertyChanged propertyName = 
        propertyChanged.Trigger(this,PropertyChangedEventArgs(propertyName))
    member this.NotifyPropertyChanged quotation = 
        quotation |> getPropertyName |> this.NotifyPropertyChanged
//
//type MessageViewModel () =
//    inherit ObservableObject()
//    let mutable text = ""
//    member this.Message
//        with get () = text
//        and set value = 
//            text <- value
//            this.NotifyPropertyChanged <@ this.Message @>
//
//let xaml = 
//    @"<UserControl xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
//              xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
//       <TextBlock FontSize='48' Text='{Binding Message}'/>
//      </UserControl>"
//
//#if INTERACTIVE
//open Microsoft.TryFSharp
//App.Dispatch (fun() -> 
//  App.Console.ClearCanvas()
//  let view = XamlReader.Load(xaml) :?> UserControl
//  let viewModel = MessageViewModel(Message="Countdown")
//  view.DataContext <- viewModel
//  view |> App.Console.Canvas.Children.Add
//  async {
//    do! Async.Sleep(2000)
//    for i = 10 downto 1 do
//       viewModel.Message <- i.ToString()
//       do! Async.Sleep(1000)    
//    viewModel.Message <- "Blast off!"
//  } |> Async.StartImmediate
//  App.Console.CanvasPosition <- CanvasPosition.Right
//)
//#endif 


