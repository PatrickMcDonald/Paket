﻿namespace Paket

open System.IO
open System

/// The Framework profile.
type FrameworkProfile = 
    | Client
    | Full

[<RequireQualifiedAccess>]
type FrameworkVersionNo = 
    | V1
    | V1_1
    | V2
    | V3_5
    | V4
    | V4_5
    | V4_5_1
    | V4_5_2
    | V4_5_3
    override this.ToString() = 
        match this with
        | V1 -> "v1.0"
        | V1_1 -> "v1.1"
        | V2 -> "v2.0"
        | V3_5 -> "v3.5"
        | V4 -> "v4.0"
        | V4_5 -> "v4.5"
        | V4_5_1 -> "v4.5.1"
        | V4_5_2 -> "v4.5.2"
        | V4_5_3 -> "v4.5.3"

/// The Framework version.
type FrameworkVersion = 
    | All
    | Framework of FrameworkVersionNo

    static member KnownDotNetFrameworks = 
        [ FrameworkVersionNo.V1, Full
          FrameworkVersionNo.V1_1, Full
          FrameworkVersionNo.V2, Full
          FrameworkVersionNo.V3_5, Full
          FrameworkVersionNo.V4, Client
          FrameworkVersionNo.V4, Full
          FrameworkVersionNo.V4_5, Full
          FrameworkVersionNo.V4_5_1, Full
          FrameworkVersionNo.V4_5_2, Full
          FrameworkVersionNo.V4_5_3, Full ]

type PlatformVersion = string

type PortableFrameworkProfile = string

/// Framework Identifier type.
type FrameworkIdentifier = 
    | DotNetFramework of FrameworkVersion * FrameworkProfile
    | PortableFramework of PlatformVersion * PortableFrameworkProfile
    | MonoAndroid
    | MonoTouch
    | Windows of string
    | WindowsPhoneApp of string
    | Silverlight of string

    static member KnownAliases =
        ["NET", "net"
         ".NET", "net"
         "NETFramework", "net"
         ".NETFramework", "net"
         "NETCore", "netcore"
         ".NETCore", "netcore"
         "WinRT", "netcore"
         ".NETMicroFramework", "netmf"
         "SL", "sl"
         "Silverlight", "sl"
         ".NETPortable", "portable-"
         "NETPortable", "portable-"
         "WindowsPhone", "wp"
         "Windows", "win"
         "WindowsPhoneApp", "wpa"
         "Xamarin.iOS", "Xamarin.iOS"
         "XamariniOS", "Xamarin.iOS"
         "Xamarin.Mac", "Xamarin.Mac"
         "XamarinMac", "Xamarin.Mac"
         "Xamarin.PlayStationThree", "Xamarin.PlayStation3"
         "XamarinPlayStationThree", "Xamarin.PlayStation3"
         "XamarinPSThree", "Xamarin.PlayStation3"
         "Xamarin.PlayStationFour", "Xamarin.PlayStation4"
         "XamarinPlayStationFour", "Xamarin.PlayStation4"
         "XamarinPSFour", "Xamarin.PlayStation4"
         "Xamarin.PlayStationVita", "Xamarin.PlayStationVita"
         "XamarinPlayStationVita", "Xamarin.PlayStationVita"
         "XamarinPSVita", "Xamarin.PlayStationVita"
         "Xamarin.XboxThreeSixty", "Xamarin.Xbox360"
         "XamarinXboxThreeSixty", "Xamarin.Xbox360"
         "Xamarin.XboxOne", "Xamarin.XboxOne"
         "XamarinXboxOne", "Xamarin.XboxOne"
         
         "3.5", "35" 
         "4.0", "40" 
         "4.5", "45" 
         "0.0", "" ]

    static member Extract(path:string) = 
        let path = 
            FrameworkIdentifier.KnownAliases
            |> List.fold (fun (path:string) (pattern,replacement) -> path.Replace(pattern.ToLower(),replacement.ToLower())) (path.ToLower())

        match path with
        | "net" -> Some(DotNetFramework(All, Full))
        | "1.0" -> Some(DotNetFramework(All, Full))
        | "1.1" -> Some(DotNetFramework(All, Full))
        | "2.0" -> Some(DotNetFramework(All, Full))
        | "net20" -> Some(DotNetFramework(Framework FrameworkVersionNo.V2, Full))
        | "net20-full" -> Some(DotNetFramework(Framework FrameworkVersionNo.V2, Full))
        | "net35" -> Some(DotNetFramework(Framework FrameworkVersionNo.V3_5, Full))
        | "net35-full" -> Some(DotNetFramework(Framework FrameworkVersionNo.V3_5, Full))
        | "net4" -> Some(DotNetFramework(Framework FrameworkVersionNo.V4, Full))
        | "net40" -> Some(DotNetFramework(Framework FrameworkVersionNo.V4, Full))
        | "net40-full" -> Some(DotNetFramework(Framework FrameworkVersionNo.V4, Full))
        | "net40-client" -> Some(DotNetFramework(Framework FrameworkVersionNo.V4, Client))
        | "net45" -> Some(DotNetFramework(Framework FrameworkVersionNo.V4_5, Full))
        | "net45-full" -> Some(DotNetFramework(Framework FrameworkVersionNo.V4_5, Full))
        | "net451" -> Some(DotNetFramework(Framework FrameworkVersionNo.V4_5_1, Full))
        | "35" -> Some(DotNetFramework(Framework FrameworkVersionNo.V3_5, Full))
        | "40" -> Some(DotNetFramework(Framework FrameworkVersionNo.V4, Full))
        | "45" -> Some(DotNetFramework(Framework FrameworkVersionNo.V4_5, Full))
        | "sl3" -> Some(Silverlight "v3.0")
        | "sl4" -> Some(Silverlight "v4.0")
        | "sl5" -> Some(Silverlight "v5.0")
        | "sl50" -> Some(Silverlight "v5.0")
        | "sl4-wp" -> Some(WindowsPhoneApp "7.1")
        | "sl4-wp71" -> Some(WindowsPhoneApp "7.1")
        | "sl4-windowsphone71" -> Some(WindowsPhoneApp "7.1")
        | "win8" -> Some(Windows "v8.0")
        | "wp8" -> Some(WindowsPhoneApp "v8.0")
        | "wpa81" -> Some(WindowsPhoneApp "v8.1")
        | "monoandroid" -> Some(MonoAndroid)
        | "monotouch" -> Some(MonoTouch)
        | _ ->                         
            if path.ToLower().StartsWith("portable-") then
                Some(PortableFramework("7.0", path.ToLower().Replace("portable-","")))
            else None
    
    member x.GetFrameworkIdentifier() =
        match x with
        | DotNetFramework _ -> "$(TargetFrameworkIdentifier) == '.NETFramework'"
        | PortableFramework _ -> "$(TargetFrameworkIdentifier) == '.NETPortable'"
        | WindowsPhoneApp _ -> "$(TargetFrameworkIdentifier) == 'WindowsPhoneApp'"
        | Windows _ -> "$(TargetFrameworkIdentifier) == 'Windows'"
        | Silverlight _ -> "$(TargetFrameworkIdentifier) == 'Silverlight'"
        | MonoAndroid -> "$(TargetFrameworkIdentifier) == 'MonoAndroid'"
        | MonoTouch -> "$(TargetFrameworkIdentifier) == 'MonoTouch'"

    member x.GetFrameworkProfile() =        
        match x with 
        | DotNetFramework(_,Client) -> " And $(TargetFrameworkProfile) == 'Client'" 
        | PortableFramework(_,profile) -> 
            let profileMapping = 
                [ "Profile2", "net4+sl4+netcore45+wp7"
                  "Profile3", "net4+sl4"
                  "Profile4", "net45+sl4+netcore45+wp7"
                  "Profile5", "net4+netcore45+MonoAndroid1+MonoTouch1"
                  "Profile6", "net403+netcore45+MonoAndroid1+MonoTouch1"
                  "Profile7", "net45+netcore45+MonoAndroid1+MonoTouch1"
                  "Profile14", "net4+sl5+MonoAndroid1+MonoTouch1"
                  "Profile18", "net403+sl4"
                  "Profile19", "net403+sl5+MonoAndroid1+MonoTouch1"
                  "Profile23", "net45+sl4"
                  "Profile24", "net45+sl5+MonoAndroid1+MonoTouch1"
                  "Profile31", "netcore451+wp81"
                  "Profile32", "netcore451+wpa81"
                  "Profile36", "net4+sl4+netcore45+wp8"
                  "Profile37", "net4+sl5+netcore45+MonoAndroid1+MonoTouch1"
                  "Profile41", "net403+sl4+netcore45"
                  "Profile42", "net403+sl5+netcore45+MonoAndroid1+MonoTouch1"
                  "Profile44", "net451+netcore451"
                  "Profile46", "net45+sl4+netcore45"
                  "Profile47", "net45+sl5+netcore45+MonoAndroid1+MonoTouch1"
                  "Profile49", "net45+wp8+MonoAndroid1+MonoTouch1"
                  "Profile78", "net45+netcore45+wp8+MonoAndroid1+MonoTouch1"
                  "Profile84", "wpa81+wp81"
                  "Profile88", "net4+sl4+netcore45+wp71"
                  "Profile92", "net4+netcore45+wpa81+MonoAndroid1+MonoTouch1"
                  "Profile95", "net403+sl4+netcore45+wp7"
                  "Profile96", "net403+sl4+netcore45+wp71"
                  "Profile102", "net403+netcore45+wpa81+MonoAndroid1+MonoTouch1"
                  "Profile104", "net45+sl4+netcore45+wp71"
                  "Profile111", "net45+netcore45+wpa81+MonoAndroid1+MonoTouch1"
                  "Profile136", "net4+sl5+netcore45+wp8+MonoAndroid1+MonoTouch1"
                  "Profile136", "net4+sl5+wp8+win8+wpa81+MonoTouch+MonoAndroid"
                  "Profile143", "net403+sl4+netcore45+wp8"
                  "Profile147", "net403+sl5+netcore45+wp8+MonoAndroid1+MonoTouch1"
                  "Profile151", "net451+netcore451+wpa81"
                  "Profile154", "net45+sl4+netcore45+wp8"
                  "Profile157", "netcore451+wpa81+wp81"
                  "Profile158", "net45+sl5+netcore45+wp8+MonoAndroid1+MonoTouch1"
                  "Profile225", "net4+sl5+netcore45+wpa81+MonoAndroid1+MonoTouch1"
                  "Profile240", "net403+sl5+netcore45+wpa81"
                  "Profile255", "net45+sl5+netcore45+wpa81+MonoAndroid1+MonoTouch1"
                  "Profile259", "net45+netcore45+wpa81+wp8+MonoAndroid1+MonoTouch1"
                  "Profile328", "net4+sl5+netcore45+wpa81+wp8+MonoAndroid1+MonoTouch1"
                  "Profile328", "net4+sl5+wp8+win8+wpa81+monoandroid16+monotouch40"
                  "Profile336", "net403+sl5+netcore45+wpa81+wp8+MonoAndroid1+MonoTouch1"
                  "Profile344", "net45+sl5+netcore45+wpa81+wp8+MonoAndroid1+MonoTouch1"

                  // unsure
                  "Profile88", "net40+sl4+win8+wp71+wpa81"
                  "Profile84", "net45+wp80+win8+wpa81" ]
            
            let profile =
                match profileMapping |> Seq.tryFind (fun (_, p) -> profile.ToLower() = p.ToLower()) with
                | None -> profile
                | Some(mappedProfile, _) -> mappedProfile

            sprintf " And $(TargetFrameworkProfile) == '%s'"  profile
        | _ -> ""

    member x.GetPlatformIdentifier() =        
        match x with 
        | PortableFramework(_,_) -> sprintf " And $(TargetPlatformIdentifier) == 'Portable'"
        | _ -> ""

    member x.GetPlatformVersion() =        
        match x with 
        | PortableFramework(v,_) -> sprintf " And $(TargetPlatformVersion) == '%s'"  v
        | WindowsPhoneApp v -> sprintf " And $(TargetPlatformVersion) == '%s'"  v
        | Windows v -> sprintf " And $(TargetPlatformVersion) == '%s'"  v
        | _ -> ""

    member x.GetCondition() =
        match x with
        | DotNetFramework(v,_) ->
            match v with
            | Framework fw -> sprintf "%s And $(TargetFrameworkVersion) == '%s'%s" (x.GetFrameworkIdentifier()) (fw.ToString()) (x.GetFrameworkProfile())
            | All -> "true"
        | PortableFramework _ -> sprintf "%s%s%s%s" (x.GetFrameworkIdentifier()) (x.GetFrameworkProfile()) (x.GetPlatformIdentifier()) (x.GetPlatformVersion())
        | WindowsPhoneApp _ -> sprintf "%s%s" (x.GetFrameworkIdentifier()) (x.GetPlatformVersion())
        | Windows _ -> sprintf "%s%s" (x.GetFrameworkIdentifier()) (x.GetPlatformVersion())
        | Silverlight v -> sprintf "%s And $(SilverlightVersion) == '%s'" (x.GetFrameworkIdentifier()) v
        | MonoAndroid -> x.GetFrameworkIdentifier()
        | MonoTouch -> x.GetFrameworkIdentifier()

    override x.ToString() = x.GetCondition()

    static member DetectFromPath(path : string) : FrameworkIdentifier option = 
        
        let path = path.Replace("\\", "/").ToLower()
        let fi = new FileInfo(path)
        
        if path.Contains("lib/" + fi.Name.ToLower()) then Some(DotNetFramework(All, Full))
        else 
            let startPos = path.IndexOf("lib/")
            let endPos = path.IndexOf(fi.Name.ToLower())
            if startPos < 0 || endPos < 0 then None
            else path.Substring(startPos + 4, endPos - startPos - 5) |> FrameworkIdentifier.Extract