namespace TourAssistant.Client.Core.DataAccess
open System.Net.Http
open System.Net.Http.Headers
open System.Data.Services.Client
open Breeze.Sharp
open System
open System.Net

module Json =
    open Newtonsoft.Json

    let deserialize<'T> jsonString = JsonConvert.DeserializeObject<'T>(jsonString)

module AuthN =

    exception FailureHttpStatusCodeException of HttpStatusCode * content:string

    type Credentials() = 
        class
            [<DefaultValue>] val mutable clientCode : string
            [<DefaultValue>] val mutable password : string
        end

    type Request = { endPoint: string; credentials: Credentials }

    let exec (request:Request) = 

        let sendAuthRequest (request: Request) = 

            let body = request.credentials
                                    
            async {

                use httpClient = new HttpClient()

                let! response = httpClient.PostAsJsonAsync(request.endPoint, body) |> Async.AwaitTask
                let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask

                return response, content
            }

        let validateStatusCode (response : HttpResponseMessage, content : string) = 
            if (response.IsSuccessStatusCode) then response, content
            else raise (FailureHttpStatusCodeException(response.StatusCode, content))

        async {
            let! response = sendAuthRequest request
            return response |> validateStatusCode |> snd
        }

module AuthNHttpProxy = 

    let mutable actualAuthToken = "" 
    
    let exec (authNRequest : AuthN.Request) (client : HttpClient) queryFn = 

        let authN = AuthN.exec authNRequest
        let setCachedToken token = actualAuthToken <- token
        let getCachedToken() = actualAuthToken
        let getNewToken = async { return! authN }
        
        let getToken = 
            async { 
                let cachedToken = getCachedToken()
                if String.IsNullOrEmpty(cachedToken) then return! authN
                else return cachedToken
            }
        
        let updAuthToken = 
            async { 
                let! token = getNewToken
                setCachedToken (token)
                return token
            }
        
        let setTokenTo (client : HttpClient) (token : string) = 
            client.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", token)
            ()

        let exec queryFn = 
            let (|Unathorized|Other|) (ex : DataServiceRequestException) = 
                if (ex.HttpResponse.StatusCode = HttpStatusCode.Unauthorized) then Unathorized
                else Other
            try 
                queryFn()
                |> Async.RunSynchronously
            with :? AggregateException as ex -> 
                match ex.InnerException.InnerException with
                | :? DataServiceRequestException as ex -> 
                    match ex with
                    | Unathorized -> 
                        updAuthToken
                        |> Async.RunSynchronously
                        |> setTokenTo client
                        queryFn()
                        |> Async.RunSynchronously
                    | Other -> reraise()
                | _ -> reraise()
        async { 
            let! token = getToken
            token |> setTokenTo client
            setCachedToken (token)
            let queryResult = exec queryFn
            return queryResult
        }

module AuthNBreezeProxy = 

    let mutable actualAuthToken = "" 
    
    let exec authNRequest (manager : EntityManager) query = 

        let queryFn() = manager.ExecuteQuery(query) |> Async.AwaitTask
        let client = manager.DataService.HttpClient
        AuthNHttpProxy.exec authNRequest client queryFn

type HttpService() =
        member __.GetAsync(authNRequest: AuthN.Request,  url:string) = 

            let httpClient = new HttpClient()

            let getStringAsync (url:string) =
                
                async {
                    let! response = httpClient.GetAsync(url) |> Async.AwaitTask
                    response.EnsureSuccessStatusCode () |> ignore
                    let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                    return content
                }

            let query() = getStringAsync url

            AuthNHttpProxy.exec authNRequest httpClient query