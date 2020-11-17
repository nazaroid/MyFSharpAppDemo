namespace Xaml.Converters
open Xamarin.Forms
open System
open System.IO
open System.Globalization

type ByteArrayToImageSourceConverter() =
        class
            interface IValueConverter with
                member __.Convert(value: obj, targetType: Type, parameter: obj, culture: CultureInfo): obj = 
                            if(isNull(box value)) then null
                            else
                               let bytes = value :?> byte[]
                               ImageSource.FromStream(fun () -> new MemoryStream(bytes) :> Stream) :> obj
                member __.ConvertBack(value: obj, targetType: Type, parameter: obj, culture: CultureInfo): obj = 
                    failwith "Not implemented yet"
        end

type NotBoolConverter() =
        class
            interface IValueConverter with
                member __.Convert(value: obj, targetType: Type, parameter: obj, culture: CultureInfo): obj = 
                            not (value :?> bool) |> box
                         
                member __.ConvertBack(value: obj, targetType: Type, parameter: obj, culture: CultureInfo): obj = 
                    not (value :?> bool) |> box
        end