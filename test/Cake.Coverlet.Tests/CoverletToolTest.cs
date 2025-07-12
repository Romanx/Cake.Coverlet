using System;
using FluentAssertions;
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

        result.Args.Should().StartWith("\"/Working/test/Cake.Coverlet.Tests/bin/Debug/netcoreapp2.1/Cake.Coverlet.Tests.dll\"");
        result.Args.Should().Contain("--targetargs \"test /Working/test/Cake.Coverlet.Tests --no-build\"");
        result.Args.Should().Contain("--format json");
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

        Console.WriteLine(result.Args);

        result.Args.Should().StartWith("\"/Working/test/Cake.Coverlet.Tests/bin/Debug/netcoreapp2.1/Cake.Coverlet.Tests.dll\"");
        result.Args.Should().Contain("--targetargs \"test /Working/test/Cake.Coverlet.Tests --no-build\"");
        result.Args.Should().Contain("--format json");
        result.Args.Should().Contain("--exclude-by-attribute \"abc.def\"");
        result.Args.Should().Contain("--exclude-by-attribute \"abc2.def\"");
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

        result.Args.Should().Contain("--format teamcity");
    }
}
