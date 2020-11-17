namespace TourAssistant.Client.Core.Data

open System
open Breeze.Sharp
open TourAssistant.Client.Core.Platform
open TourAssistant.Domain
open System.Reflection
open System.Collections.Generic
open TourAssistant.Client.Core.DataAccess

type ServerSettings() =
    class

    member val Host = "" with get, set
    member val AuthPath = "" with get, set
    member val TourPath = "" with get, set
    member val AreaInfoPath = "" with get, set
    member val ClientCode = "" with get, set
    member val Pwd = "" with get, set

    end

type IEntityProvider<'T> = interface
   abstract GetRemoteAsync : unit -> Async<'T>
   abstract GetCached : unit -> 'T
   abstract GetLocal : unit -> 'T
end

//#region extensions

module Format =
    let stayArea (entity : AreaInfo) = 
                        let fmtMap (x:Image) = 
                            if(isNull(box x)) then "<null>"
                            else
                               sprintf "{%i chars}" x.Length

                        let fmtDesc (x:Text) = 
                            if(isNull(box x)) then "<null>"
                            else
                               sprintf "{%i chars}" x.Length

                           
                        sprintf "{Map = %s; Description = %s;}" 
                                            (fmtMap entity.Map)
                                            (fmtDesc entity.Description)
    let tour (entity : Tour) = 

                    let fmtA (x:Aeroport) = 
                        if(isNull(box x)) then "<null>"
                        else
                            sprintf "{Name=%A}" x.Name

                    let fmtMA (x:MobApp) = 
                        if(isNull(box x)) then "<null>"
                        else
                            sprintf "{ActivationLevel=%A}" x.ActivationLevel

                    let fmtF (x:Flight) = 
                        if(isNull(box x)) then "<null>"
                        else
                            let msg = sprintf "{Code=%A; Dp.Aeroport=%s; Ar.Aeroport=%s;}"
                                            x.Code 
                                            (fmtA x.DepartureAeroport) 
                                            (fmtA x.ArrivalAeroport)
                            msg

                    let fmtC (x:Client) = 
                        if(isNull(box x)) then "<null>"
                        else
                            sprintf "{ShortName=%A}" x.ShortName
                           
                    sprintf "{Id = %i; MobApp = %s; Client = %s; FlightThere = %s; FlightBack = %s;}" 
                                        entity.Id 
                                        (fmtMA entity.MobApp)
                                        (fmtC entity.Client)
                                        (fmtF entity.FlightThere)
                                        (fmtF entity.FlightBack)

type ServerSettings with

    member this.GetAuthNRequest() = 
        let authNRequest = { AuthN.endPoint = this.Host + this.AuthPath; 
                             AuthN.credentials = AuthN.Credentials(clientCode = this.ClientCode, 
                                                                   password = this.Pwd)}
        authNRequest
        
    member this.GetAreaInfoEndPoint<'T when 'T :> AreaInfo>() = 
                    let action = match typeof<'T> with 
                                    | x when x = typeof<CountryInfo> -> "Country"
                                    | x when x = typeof<KurortInfo> -> "Kurort"
                                    | x when x = typeof<HotelInfo> -> "Hotel"
                                    | _ -> let msg = sprintf "endpoint for type '%s' not supported" typeof<'T>.Name
                                           raise (NotSupportedException(msg))

                    this.Host + this.AreaInfoPath + "/" + action

//#endregion extensions

type BreezeLocalCache<'T>(textFile : ITextFileService) =
    class
        let fileName = sprintf "TourAssistant__%s.brz" typeof<'T>.Name

        member __.Save(text) = textFile.Save(fileName, text)
        member __.Load() = textFile.Load(fileName)
    end

type JsonLocalCache<'T>(textFile : ITextFileService) =
    class
        let fileName = sprintf "TourAssistant__%s.json" typeof<'T>.Name

        member __.Save(text) = textFile.Save(fileName, text); Json.deserialize<'T> text 
        member __.Load() = textFile.Load(fileName) |> Json.deserialize<'T>
    end


type TourProvider(settings : ServerSettings, localCache : BreezeLocalCache<Tour>) = 
    class
        let authNRequest = settings.GetAuthNRequest()

        let manager = 
            lazy (let mng = EntityManager(settings.Host + settings.TourPath)
                  let assembly = typeof<Tour>.GetTypeInfo().Assembly
                  Configuration.Instance.ProbeAssemblies(assembly) |> ignore
                  mng)

        let entitiesToExpand = "Client,
                                FlightThere, FlightThere.ArrivalAeroport, FlightThere.ArrivalAeroport.Country, FlightThere.DepartureAeroport, 
                                FlightBack, FlightBack.ArrivalAeroport, 
                                Hotel, Hotel.Kurort, 
                                MobApp"
        let query = EntityQuery<Tour>().Expand(entitiesToExpand)
        let getRemotely () = 

            let executeQuery query = AuthNBreezeProxy.exec authNRequest manager.Value query 
            
            async { 
                let! result = executeQuery query
                let entity = Seq.head result
                return entity
            }

        let getCached () = Seq.head (manager.Value.ExecuteQueryLocally(query))
                   
        let export () = 
            let text = manager.Value.ExportEntities()
            localCache.Save(text)

        let import () = 
            let text = localCache.Load()
            manager.Value.ImportEntities(text) |> ignore

        interface IEntityProvider<Tour> with
            member __.GetRemoteAsync() : Async<Tour> = 
                async {
                    let! entity = getRemotely()
                    export()
                    return entity
                }

            member __.GetCached() = getCached()
                
            member __.GetLocal(): Tour = 
                import()
                getCached()
    end

type AreaInfoProvider<'T when 'T :> AreaInfo>(settings : ServerSettings, 
                                              http : HttpService, 
                                              localCache : JsonLocalCache<'T>) =
    class
        let mutable inMemoryCache : 'T option = None

        let getCached () = match inMemoryCache with
                           | Some entity -> entity 
                           | _ -> raise (KeyNotFoundException("Area cache is empty"))
        let setCached (v) = inMemoryCache <- Some v
         
        let authNRequest = settings.GetAuthNRequest()

        let execRemoteRequest () = 
            let url = settings.GetAreaInfoEndPoint<'T>();
            let executeQuery = http.GetAsync(authNRequest, url)
            executeQuery 
            
        let export json = localCache.Save(json)

        let import () = 
            let entity = localCache.Load()
            setCached entity

        interface IEntityProvider<'T> with
            member __.GetRemoteAsync() = 
                async {
                    let! json = execRemoteRequest()
                    let entity = export(json)
                    setCached entity
                    return entity
                }

            member __.GetCached() = getCached()
                
            member __.GetLocal() = import(); getCached()
    end
 