namespace TourAssistant.Client.Core.Tests

[<AutoOpen>]
module TestComposition =
    open TourAssistant.Client.Core
    open TourAssistant.Client.Core.Stubs
    open TourAssistant.Client.Core.Data
    open SipPhone.Core.Stubs
    open Ninject
    open SipPhone.Core

    let asmTestComposition () =
        let kernel = new StandardKernel()
        let core = CoreComposition(kernel)
        core.Assemble(ServerSettings(), StubAppPlatform())
        let sipPhone = SipPhoneCoreComposition(kernel)
        sipPhone.Assemble(StubSipPhonePlatform())
        core

