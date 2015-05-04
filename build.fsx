#r @"packages/FAKE.3.30.2/tools/FakeLib.dll"
#load "build-helpers.fsx"
open Fake
open System
open System.IO
open System.Linq
open BuildHelpers
open Fake.XamarinHelper

let coreSolution = "OpenTK.core.sln";
let androidSolution = "OpenTK.android.sln";
Target "core-build" (fun () ->
    //RestorePackages coreSolution

    MSBuild "Binaries\OpenTK\Debug" "Build" [ ("Configuration", "Debug"); ("Platform", "Any CPU") ] [ coreSolution ] 
    |> ignore
)

Target "core-tests" (fun () -> 
    //RunNUnitTests "src/TipCalc/bin/Debug/TipCalc.Tests.dll" "src/TipCalc/bin/Debug/testresults.xml"
    Console.WriteLine "No tests"
)

Target "android-build" (fun () ->
    RestorePackages androidSolution

    MSBuild "Binaries\OpenTK\Release" "Build" [ ("Configuration", "Release") ] [ androidSolution ] |> ignore
)

Target "android-package" (fun () ->
    AndroidPackage (fun defaults ->
        {defaults with
            ProjectPath = "Source/OpenTK/OpenTK.Android.csproj"
            Configuration = "Release"
            OutputPath = "Binaries\OpenTK\Release"
        }) 
//    |> AndroidSignAndAlign (fun defaults ->
//        {defaults with
//            KeystorePath = "tipcalc.keystore"
//            KeystorePassword = "tipcalc" // TODO: don't store this in the build script for a real app!
//            KeystoreAlias = "tipcalc"
//        })
    |> fun file -> TeamCityHelper.PublishArtifact file.FullName
)

Target "android-uitests" (fun () ->
    AndroidPackage (fun defaults ->
        {defaults with
            ProjectPath = "Source/Tests/Android/OpenTK.Android.Test/OpenTK.Android.Test.csproj"
            Configuration = "Release"
            OutputPath = "Source/Tests/Android/OpenTK.Android.Test/bin/Release"
        }) |> ignore

    let appPath = Directory.EnumerateFiles(Path.Combine("Source","Tests", "Android", "OpenTK.Android.Test", "bin", "Release"), "*.apk", SearchOption.AllDirectories).First()

    RunUITests appPath
)

//Target "android-testcloud" (fun () ->
//    AndroidPackage (fun defaults ->
//        {defaults with
//            ProjectPath = "src/TipCalc.Android/TipCalc.Android.csproj"
//            Configuration = "Release"
//            OutputPath = "src/TipCalc.Android/bin/Release"
//        }) |> ignore
//
//    let appPath = Directory.EnumerateFiles(Path.Combine("src", "TipCalc.Android", "bin", "Release"), "*.apk", SearchOption.AllDirectories).First()
//
//    getBuildParam "devices" |> RunTestCloudTests appPath
//)

"core-build"
  ==> "core-tests"

"android-build"
  ==> "android-uitests"

//"android-build"
//  ==> "android-testcloud"

"android-build"
  ==> "android-package"

RunTargetOrDefault "core-build"