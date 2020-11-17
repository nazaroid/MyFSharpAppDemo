namespace TourAssistant.Client.Core.Stubs

open TourAssistant.Client.Core.Platform
open System.Collections.Generic
open SipPhone.Core.Stubs
open SipPhone.Core.Models

type InMemoryLogger() =

    let out = List<string>()
    
    interface ILogger with
        member __.Warn(msg) = out.Add("Warn: " + msg)
        member __.Error(e) = out.Add("Error: " + e.Message)

type InMemoryTextFileService() =

    let storage = Dictionary<string, string>()
     
    interface ITextFileService with
        member __.Load(name: string) = 
            storage.Item(name)
        member __.Save(name, text) = 
            storage.Item(name) <- text

type StubAppPlatform() = 

    member val public Logger = (InMemoryLogger() :> ILogger) with get, set
    member val public TextFileService = (InMemoryTextFileService() :> ITextFileService) with get, set

    interface IAppPlatform with
        member x.GetTextFileService() =  x.TextFileService 
        member x.GetLogger() = x.Logger