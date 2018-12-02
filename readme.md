# Cake.Coverlet

[![Build status](https://ci.appveyor.com/api/projects/status/pr3lyh3baynax8gx/branch/master?svg=true)](https://ci.appveyor.com/project/Romanx/cake-coverlet/branch/master)
[![NuGet version](https://img.shields.io/nuget/v/Cake.Coverlet.svg)](https://www.nuget.org/packages/Cake.Coverlet/)

## Usage
In order to use the addin please make sure you've included Coverlet in the project you wish to cover and add the following to your cake build file
```csharp
#addin nuget:?package=Cake.Coverlet
```

You can also install coverlet as a global tool on your machine or with the [Cake.DotNetTool.Module]() and run the command separately from MSBuild.

**Note:** Works with Coverlet 2.1.1 and up

Then use one of the following snippets

```csharp
Task("Test")
    .IsDependentOn("Build")
    .Does<MyBuildData>((data) =>
{
    var testSettings = new DotNetCoreTestSettings {
    };

    var coveletSettings = new CoverletSettings {
        CollectCoverage = true,
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        CoverletOutputDirectory = Directory(@".\coverage-results\"),
        CoverletOutputName = $"results-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}"
    };

    DotNetCoreTest("./test/My.Project.Tests/My.Project.Tests.csproj", testSetting, coveletSettings);
}
```

Or for when installed as a tool:

```csharp
Task("Test")
    .IsDependentOn("Build")
    .Does<MyBuildData>((data) =>
{
    var coveletSettings = new CoverletSettings {
        CollectCoverage = true,
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        CoverletOutputDirectory = Directory(@".\coverage-results\"),
        CoverletOutputName = $"results-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}"
    };

    // I want to specify the specific dll file and the project exactly.
    Coverlet(
        "./test/My.Project.Tests/bin/Debug/net46/My.Project.Tests.dll", "./test/My.Project.Tests/My.Project.Tests.csproj", 
        coveletSettings);

    // I want to specify just the project file and the dll can be
    // inferred from the name of the project file.
    Coverlet("./test/My.Project.Tests/My.Project.Tests.csproj", 
        coveletSettings);

    // I want to specify just the project directory, we will discover
    // any proj file in the directory (take the first) and infer the 
    // name from the found project.
    Coverlet("./test/My.Project.Tests",
        coveletSettings);
}
```

There is an additional api exposed for transforming the output name of the coverage file at the time of calling `dotnet test`.
This transform function follows the form `Func<string, string>` being passed the `CoverletOutputName` and the return is used for the filename.

```csharp
Task("Test")
    .IsDependentOn("Build")
    .Does<MyBuildData>((data) =>
{
    var testSettings = new DotNetCoreTestSettings {
    };

    var coveletSettings = new CoverletSettings {
        CollectCoverage = true,
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        CoverletOutputDirectory = Directory(@".\coverage-results\"),
        CoverletOutputName = $"results"
        OutputNameTransformer = (fileName, directory) => $@"{directory}\{fileName}-HelloWorld"
    };

    DotNetCoreTest("./test/Stubble.Core.Tests/Stubble.Core.Tests.csproj", testSetting, coveletSettings);
}
```

We expose a default transformer for the standard practice of appending the current datetime to the file as `WithDateTimeTransformer()`

If you wish to only change the directory that the output is written to then set the `CoverletOutputDirectory` and the filename handling will be done by coverlet as usual.