using System.Collections.Generic;
using AwesomeAssertions;
using Cake.Core.IO;
using Xunit;

namespace Cake.Coverlet.Tests;

public class CoverletToolTest(CoverletToolFixture fixture) : IClassFixture<CoverletToolFixture>
{
    [Fact]
    public void AbsolutePath_Should_Be_Passed()
    {
        fixture.Settings = new CoverletSettings();
        fixture.TestFile = "./test/Cake.Coverlet.Tests/bin/Debug/netcoreapp2.1/Cake.Coverlet.Tests.dll";
        fixture.TestProject = "./test/Cake.Coverlet.Tests/";

        var result = fixture.Run();
        
        result.Path.Should().BeEquivalentTo(FilePath.FromString("/Working/test/Cake.Coverlet.Tests/bin/Debug/netcoreapp2.1/Cake.Coverlet.Tests.dll"));
        result.Arguments.Should().BeEquivalentTo(new Dictionary<string, IReadOnlyList<string>>
        {
            ["--target"] = ["dotnet"],
            ["--targetargs"] = ["test /Working/test/Cake.Coverlet.Tests --no-build"],
            ["--format"] = ["json"],
        });
    }

    [Fact]
    public void ExcludeByAttribute_Should_Be_Passed()
    {
        fixture.Settings = new CoverletSettings();
        fixture.TestFile = "./test/Cake.Coverlet.Tests/bin/Debug/netcoreapp2.1/Cake.Coverlet.Tests.dll";
        fixture.TestProject = "./test/Cake.Coverlet.Tests/";
        fixture.Settings.WithAttributeExclusion("abc.def");
        fixture.Settings.WithAttributeExclusion("abc2.def");

        var result = fixture.Run();
        
        result.Path.Should().BeEquivalentTo(FilePath.FromString("/Working/test/Cake.Coverlet.Tests/bin/Debug/netcoreapp2.1/Cake.Coverlet.Tests.dll"));
        result.Arguments.Should().BeEquivalentTo(new Dictionary<string, IReadOnlyList<string>>
        {
            ["--target"] = ["dotnet"],
            ["--targetargs"] = ["test /Working/test/Cake.Coverlet.Tests --no-build"],
            ["--format"] = ["json"],
            ["--exclude-by-attribute"] = ["abc.def", "abc2.def"],
        });
    }
    
    [Fact]
    public void OutputDirectoryWithoutFileName_ShouldEndInTrailingSeparator()
    {
        fixture.Settings = new CoverletSettings 
        {
            CoverletOutputDirectory = DirectoryPath.FromString("./Output/TestResults"),
            CoverletOutputFormat = CoverletOutputFormat.opencover | CoverletOutputFormat.cobertura,
            CollectCoverage = true
        };
        fixture.TestFile = "./test/Cake.Coverlet.Tests/bin/Debug/netcoreapp2.1/Cake.Coverlet.Tests.dll";
        fixture.TestProject = "./test/Cake.Coverlet.Tests/";

        var result = fixture.Run();
        result.Arguments.Should().BeEquivalentTo(new Dictionary<string, IReadOnlyList<string>>
        {
            ["--target"] = ["dotnet"],
            ["--targetargs"] = ["test /Working/test/Cake.Coverlet.Tests --no-build"],
            ["--format"] = ["opencover", "cobertura"],
            ["--output"] = ["/Working/Output/TestResults/"]
        });
    }

    [Fact]
    public void OutputFormat_Supports_TeamCity()
    {
        fixture.Settings = new CoverletSettings 
        {
            CoverletOutputFormat = CoverletOutputFormat.teamcity
        };
        fixture.TestFile = "./test/Cake.Coverlet.Tests/bin/Debug/netcoreapp2.1/Cake.Coverlet.Tests.dll";
        fixture.TestProject = "./test/Cake.Coverlet.Tests/";

        var result = fixture.Run();
        result.Arguments.Should().BeEquivalentTo(new Dictionary<string, IReadOnlyList<string>>
        {
            ["--target"] = ["dotnet"],
            ["--targetargs"] = ["test /Working/test/Cake.Coverlet.Tests --no-build"],
            ["--format"] = ["teamcity"]
        });
    }
}
