namespace TourAssistant.Client.Core.Models

open System
open Rop
open TourAssistant.Client.Core.Platform
open TourAssistant.Domain
open TourAssistant.Client.Core.Data

type IDataService<'T> = interface
   abstract GetAsync : unit -> Async<Result<'T, Exception>>
end

type DataService<'T>(entityProvider : IEntityProvider<'T>, 
                     logger : ILogger, 
                     toString : 'T -> string) = 
    class
        interface IDataService<'T> with
            member __.GetAsync() = 

                let bindFail f twoTrackInput =
                    either succeed (fun (e) -> f()) twoTrackInput


                let logInputEntity source (entity : 'T) = 

                    let msg = sprintf "'%s' enitity from '%s' with %s" 
                                        typeof<'T>.Name
                                        source
                                        (toString entity)

                    logger.Warn(msg)
                    entity

                let logEx msg (ex) = logger.Error(Exception(msg, ex)); ex


                let getEntityRemote () = async { return! entityProvider.GetRemoteAsync()} 
                                               |> Async.RunSynchronously 
                                               |> (logInputEntity "remote service")

                let getEntityLocal () = entityProvider.GetLocal() |> (logInputEntity "local file cache")

                let getEntityCache () = entityProvider.GetCached() |> (logInputEntity "in memory cache")



                let tryGetEntityRemote() = tryCatch getEntityRemote
                                                    (logEx "fail to fetch entity remotely")
                                                    ()

                let tryGetEntityLocal() = tryCatch getEntityLocal
                                                   (logEx "fail to fetch entity from local file cache")
                                                   ()

                let tryGetEntityCache () = tryCatch getEntityCache
                                                   (logEx "fail to fetch entity from memory cache")
                                                   ()

                
                async { 
                    return tryGetEntityRemote() |> bindFail tryGetEntityCache |> bindFail tryGetEntityLocal
                }
    end 
        
type TourDataService(entityProvider : IEntityProvider<Tour>, logger : ILogger) = 
    class
        inherit DataService<Tour>(entityProvider, 
                                          logger, 
                                          Format.tour)
    end 
                
type AreaInfoDataService<'T when 'T :> AreaInfo>(entityProvider : IEntityProvider<'T>, logger : ILogger) =
    class
        inherit DataService<'T>(entityProvider, logger, Format.stayArea)
    end



