#r @"packages/FAKE.3.30.2/tools/FakeLib.dll"
#load "build-helpers.fsx"
open Fake
open System
open System.IO
open System.Linq
open BuildHelpers
open Fake.XamarinHelper

Target "core-build" (fun () ->
    RestorePackages "OpenTK.core.sln"

    MSBuild "src/TipCalc/bin/Debug" "Build" [ ("Configuration", "Debug"); ("Platform", "Any CPU") ] [ "OpenTK.core.sln" ] |> ignore
)

Target "core-tests" (fun () -> 
    RunNUnitTests "src/TipCalc/bin/Debug/TipCalc.Tests.dll" "src/TipCalc/bin/Debug/testresults.xml"
)

Target "android-build" (fun () ->
    RestorePackages "TipCalc.Android.sln"

    MSBuild "src/TipCalc.Android/bin/Release" "Build" [ ("Configuration", "Release") ] [ "TipCalc.Android.sln" ] |> ignore
)

Target "android-package" (fun () ->
    AndroidPackage (fun defaults ->
        {defaults with
            ProjectPath = "src/TipCalc.Android/TipCalc.Android.csproj"
            Configuration = "Release"
            OutputPath = "src/TipCalc.Android/bin/Release"
        }) 
    |> AndroidSignAndAlign (fun defaults ->
        {defaults with
            KeystorePath = "tipcalc.keystore"
            KeystorePassword = "tipcalc" // TODO: don't store this in the build script for a real app!
            KeystoreAlias = "tipcalc"
        })
    |> fun file -> TeamCityHelper.PublishArtifact file.FullName
)

Target "android-uitests" (fun () ->
    AndroidPackage (fun defaults ->
        {defaults with
            ProjectPath = "src/TipCalc.Android/TipCalc.Android.csproj"
            Configuration = "Release"
            OutputPath = "src/TipCalc.Android/bin/Release"
        }) |> ignore

    let appPath = Directory.EnumerateFiles(Path.Combine("src", "TipCalc.Android", "bin", "Release"), "*.apk", SearchOption.AllDirectories).First()

    RunUITests appPath
)

Target "android-testcloud" (fun () ->
    AndroidPackage (fun defaults ->
        {defaults with
            ProjectPath = "src/TipCalc.Android/TipCalc.Android.csproj"
            Configuration = "Release"
            OutputPath = "src/TipCalc.Android/bin/Release"
        }) |> ignore

    let appPath = Directory.EnumerateFiles(Path.Combine("src", "TipCalc.Android", "bin", "Release"), "*.apk", SearchOption.AllDirectories).First()

    getBuildParam "devices" |> RunTestCloudTests appPath
)

"core-build"
  ==> "core-tests"

"android-build"
  ==> "android-uitests"

//"android-build"
//  ==> "android-testcloud"

"android-build"
  ==> "android-package"

RunTarget() 