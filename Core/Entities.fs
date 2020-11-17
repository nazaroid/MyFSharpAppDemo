namespace TourAssistant.Domain

open Breeze.Sharp
open System

[<AbstractClass>]
type Entity() = 
    class
        inherit BaseEntity()
        
        member this.Id 
                with get () = this.GetValue<int>("Id")
                and set (v : int) = this.SetValue(v, "Id")
    end

type Client() = 
    class
        inherit Entity()

        member this.Code with get () = this.GetValue<string>("Code") and set (v : string) = this.SetValue(v, "Code")
        member this.PasswordHash with get () = this.GetValue<string>("PasswordHash") and set (v : string) = this.SetValue(v, "PasswordHash")

        member this.FirstName with get () = this.GetValue<string>("FirstName") and set (v : string) = this.SetValue(v, "FirstName")
        member this.MiddleName with get () = this.GetValue<string>("MiddleName") and set (v : string) = this.SetValue(v, "MiddleName")
        member this.LastName with get () = this.GetValue<string>("LastName") and set (v : string) = this.SetValue(v, "LastName")
        member this.ShortName with get () = this.GetValue<string>("ShortName") and set (v : string) = this.SetValue(v, "ShortName")
        member this.Passport with get () = this.GetValue<string>("Passport") and set (v : string) = this.SetValue(v, "Passport")
    end

type Country() =
    class
        inherit Entity()

        member this.Code with get () = this.GetValue<string>("Code") and set (v : string) = this.SetValue(v, "Code")
        member this.Name with get () = this.GetValue<string>("Name") and set (v : string) = this.SetValue(v, "Name")
    end

type Aeroport() = 
    class
        inherit Entity()

        member this.Code with get () = this.GetValue<string>("Code") and set (v : string) = this.SetValue(v, "Code")
        member this.Name with get () = this.GetValue<string>("Name") and set (v : string) = this.SetValue(v, "Name")
    
        member this.CountryId with get () = this.GetValue<int>("CountryId") and set (v : int) = this.SetValue(v, "CountryId")
        member this.Country with get () = this.GetValue<Country>("Country") and set (v : Country) = this.SetValue(v, "Country")
    end

type Flight() = 
    class
        inherit Entity()

        member this.Code with get () = this.GetValue<string>("Code") and set (v : string) = this.SetValue(v, "Code")

        member this.DepartureTime with get () = this.GetValue<DateTime>("DepartureTime") and set (v : DateTime) = this.SetValue(v, "DepartureTime")


        member this.DepartureAeroportId with get () = this.GetValue<int>("DepartureAeroportId") and set (v : int) = this.SetValue(v, "DepartureAeroportId")
        member this.DepartureAeroport with get () = this.GetValue<Aeroport>("DepartureAeroport") and set (v : Aeroport) = this.SetValue(v, "DepartureAeroport")


        member this.ArrivalAeroportId with get () = this.GetValue<int>("ArrivalAeroportId") and set (v : int) = this.SetValue(v, "ArrivalAeroportId")
        member this.ArrivalAeroport with get () = this.GetValue<Aeroport>("ArrivalAeroport") and set (v : Aeroport) = this.SetValue(v, "ArrivalAeroport")
    end

type Kurort() =
    class
        inherit Entity()

        member this.Code with get () = this.GetValue<string>("Code") and set (v : string) = this.SetValue(v, "Code")
        member this.Name with get () = this.GetValue<string>("Name") and set (v : string) = this.SetValue(v, "Name")

        member this.CountryId with get () = this.GetValue<int>("CountryId") and set (v : int) = this.SetValue(v, "CountryId")
        member this.Country with get () = this.GetValue<Country>("Country") and set (v : Country) = this.SetValue(v, "Country")
    end

type Hotel() = 
    class
        inherit Entity()

        member this.Code with get () = this.GetValue<string>("Code") and set (v : string) = this.SetValue(v, "Code")
        member this.Name with get () = this.GetValue<string>("Name") and set (v : string) = this.SetValue(v, "Name")

        member this.KurortId with get () = this.GetValue<int>("KurortId") and set (v : int) = this.SetValue(v, "KurortId")
        member this.Kurort with get () = this.GetValue<Kurort>("Kurort") and set (v : Kurort) = this.SetValue(v, "Kurort")
    end

type MobAppActivationLevel = 
 | NonActivated = 0
 | PreActivated = 1
 | FullActivated = 2

type Tour() =
    class
        inherit Entity()

        member this.ClientId with get () = this.GetValue<int>("ClientId") and set (v : int) = this.SetValue(v, "ClientId")
        member this.Client with get () = this.GetValue<Client>("Client") and set (v : Client) = this.SetValue(v, "Client")

        member this.Insurance with get () = this.GetValue<string>("Insurance") and set (v : string) = this.SetValue(v, "Insurance")
        
        member this.FlightThereId with get () = this.GetValue<int>("FlightThereId") and set (v : int) = this.SetValue(v, "FlightThereId")
        member this.FlightThere with get () = this.GetValue<Flight>("FlightThere") and set (v : Flight) = this.SetValue(v, "FlightThere")

        member this.FlightBackId with get () = this.GetValue<int>("FlightBackId") and set (v : int) = this.SetValue(v, "FlightBackId")
        member this.FlightBack with get () = this.GetValue<Flight>("FlightBack") and set (v : Flight) = this.SetValue(v, "FlightBack")

        member this.HotelId with get () = this.GetValue<int>("HotelId") and set (v : int) = this.SetValue(v, "HotelId")
        member this.Hotel with get () = this.GetValue<Hotel>("Hotel") and set (v : Hotel) = this.SetValue(v, "Hotel")

        member this.IsActual with get () = this.GetValue<bool>("IsActual") and set (v : bool) = this.SetValue(v, "IsActual")

        member this.MobApp with get () = this.GetValue<MobApp>("MobApp") and set (v : MobApp) = this.SetValue(v, "MobApp")

        override this.Initialize() =
                    (*
                        By unknown reason 'MobApp.TourId' is '0' after serialization, 
                        so after import entity from local cache 'MobApp' prop is 'null'.
                    *)
                    this.MobApp.TourId <- this.Id
    end

and MobApp() = 
    class
        inherit BaseEntity()

        member this.TourId with get () = this.GetValue<int>("TourId") 
                            and set (v : int) = this.SetValue(v, "TourId")
        member this.Tour with get () = this.GetValue<Tour>("Tour") 
                          and set (v : Tour) = this.SetValue(v, "Tour"); 

        member this.Installed with get () = this.GetValue<bool>("Installed") and set (v : bool) = this.SetValue(v, "Installed")
        member this.ActivationLevel with get () = this.GetValue<MobAppActivationLevel>("ActivationLevel") and set (v : MobAppActivationLevel) = this.SetValue(v, "ActivationLevel")

    end

type Image = byte[]
type Text = string

type AreaInfo () =
    class
        let mutable _map = Array.empty<byte>
        let mutable _description = ""

        member this.Description with get() : Text = _description and set(v : Text) = _description <- v 
        member this.Map  with get() : Image = _map and set(v : Image) = _map <- v 
    end

type CountryInfo () = 
    class
        inherit AreaInfo()
    end

type KurortInfo () = 
    class
        inherit AreaInfo()
    end

type HotelInfo () = 
    class
        inherit AreaInfo()
    end