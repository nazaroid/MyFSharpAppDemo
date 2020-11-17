namespace Core.IntegrationTests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open TourAssistant.Client.Core
open TourAssistant.Client.Core.Platform
open TourAssistant.Client.Core.Models
open Rop
open TourAssistant.Domain


[<TestClass>]
type AreaInfoGettingScenarioFixture() = 
    class
        
        [<TestMethod>]
        member x.OnlineAndOfflineGettingScenarioShouldBeSupported() = 

            //before
            let logger = 
                { new ILogger with
                      member x.Error(e : Exception) : unit = Console.WriteLine(e)
                      member x.Warn(msg : string) : unit = Console.WriteLine(msg) }

            let srvSettings, appPlatformSettings =  TestSettingsFor.all()
            appPlatformSettings.Logger <- logger
            let settings = srvSettings, appPlatformSettings

            //#region ONLINE REQUEST

            //given
            let _cOnline = CoreComposition()
            _cOnline.Assemble(settings)
            let sutOnline = _cOnline.Resolve<IDataService<CountryInfo>>()

            //when
            let actualOnline = sutOnline.GetAsync() |> Async.RunSynchronously
            
            //then
            match actualOnline with
                | Success entity -> Assert.IsNotNull(entity.Map, "online")
                                    Assert.IsNotNull(entity.Description, "online")
                | Failure ex -> Assert.Fail(ex.Message, "online") 

            //#endregion ONLINE REQUEST

            //#region OFFLINE REQUEST

            //given
            let _cOffline = CoreComposition()
            srvSettings.AuthPath <- "http://not.existing.host:81"
            srvSettings.Host <- "http://not.existing.host:81"
            _cOffline.Assemble(settings)
            let sutOffline = _cOffline.Resolve<IDataService<CountryInfo>>()

            //when
            let actualOffline = sutOffline.GetAsync() |> Async.RunSynchronously
            
            //then
            match actualOffline with
                | Success entity -> Assert.IsNotNull(entity.Map, "offline")
                                    Assert.IsNotNull(entity.Description, "offline")
                | Failure ex -> Assert.Fail(ex.Message, "offline") 
            
            //#endreegion OFFLINE REQUEST
    end

