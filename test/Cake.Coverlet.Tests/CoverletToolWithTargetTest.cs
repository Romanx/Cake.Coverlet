using System.Collections.Generic;
using AwesomeAssertions;
using Cake.Core.IO;
using Xunit;

namespace Cake.Coverlet.Tests;

public class CoverletToolWithTargetTest(CoverletToolTargetFixture fixture) : IClassFixture<CoverletToolTargetFixture>
{
    [Fact]
    public void ItShouldHandlePassingFileAsPath()
    {
        fixture.Path = new FilePath("/path/to/test-assembly.dll");
        fixture.Target = new FilePath("/application/runner.exe");
        fixture.Settings = new CoverletSettings() { Threshold = 80 };

        var result = fixture.Run();
        result.Path.Should().BeEquivalentTo(FilePath.FromString("/path/to/test-assembly.dll"));
        result.Arguments.Should().BeEquivalentTo(new Dictionary<string, IReadOnlyList<string>>
        {
            ["--target"] = ["/application/runner.exe"],
            ["--format"] = ["json"],
            ["--threshold"] = ["80"],
        });
    }
    
    [Fact]
    public void ItShouldHandlePassingDirectoryAsPath()
    {
        fixture.Path = new DirectoryPath("/integrationtests");
        fixture.Target = new FilePath("/application/runner.exe");
        fixture.Settings = new CoverletSettings() { Threshold = 80 };

        var result = fixture.Run();
        result.Path.Should().BeEquivalentTo(DirectoryPath.FromString("/integrationtests"));
        result.Arguments.Should().BeEquivalentTo(new Dictionary<string, IReadOnlyList<string>>
        {
            ["--target"] = ["/application/runner.exe"],
            ["--format"] = ["json"],
            ["--threshold"] = ["80"],
        });
    }
}
