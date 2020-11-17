namespace TourAssistant.Client.Core.Platform

open System
open SipPhone.Core.Models

type ILogger = 
    interface
        abstract Error : e:Exception -> unit
        abstract Warn : msg:string -> unit
    end
type ITextFileService = interface 
    abstract Save: name:string * text:string -> unit
    abstract Load: name:string -> string
end

type IAppPlatform = 
    interface
        abstract GetLogger : unit -> ILogger
        abstract GetTextFileService : unit -> ITextFileService
    end



