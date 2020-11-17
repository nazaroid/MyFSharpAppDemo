namespace Core.IntegrationTests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open TourAssistant.Client.Core
open TourAssistant.Client.Core.Platform
open TourAssistant.Client.Core.Models
open TourAssistant.Client.Core.Stubs
open Foq
open TourAssistant.Domain
open TourAssistant.Client.Core.Data


[<TestClass>]
type AuthenticationFixture() = 
    class
        
        [<TestMethod>]
        member x.ShouldGetRemoteData() = 

            let logger = 
                { new ILogger with
                      member x.Error(e : Exception) : unit = Console.WriteLine(e)
                      member x.Warn(msg : string) : unit = Console.WriteLine(msg) }
            let _c = CoreComposition()
            _c.Assemble(TestSettingsFor.all())
            _c.ReRegister<ILogger>(logger)
            let sut = _c.Resolve<IEntityProvider<Tour>>()

            //when
            let actual = sut.GetRemoteAsync() |> Async.RunSynchronously
            Assert.IsNotNull(actual)

            //should skip ~/Authenticate for this request
            let actual = sut.GetRemoteAsync() |> Async.RunSynchronously

            //then
            Assert.IsNotNull(actual)
            Assert.IsTrue(actual.MobApp.Installed)
    end

