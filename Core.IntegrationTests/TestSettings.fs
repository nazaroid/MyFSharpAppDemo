
module TestSettingsFor

open TourAssistant.Client.Core.Models
open TourAssistant.Client.Core.Stubs
open TourAssistant.Client.Core.Data
    
let server() = ServerSettings(//Host = "http://localhost.fiddler:11358", 
                                  Host = "http://localhost.fiddler:81", 
                                  AuthPath = "/api/MobileAccount/Authenticate", 
                                  TourPath = "/breeze/MobileDb",
                                  AreaInfoPath = "/api/MobileAreaInfo",
                                  ClientCode = "Code1",
                                  Pwd = "Code1Pwd")

let appPlatform() = StubAppPlatform()

let all() = server(), appPlatform()