using System;
using FluentAssertions;
using Xunit;

namespace Cake.Coverlet.Tests
{
    public class CoverletToolTest : IClassFixture<CoverletToolFixture>
    {
        private readonly CoverletToolFixture _fixture;

        public CoverletToolTest(CoverletToolFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void AbsolutePath_Should_Be_Passed()
        {
            _fixture.Settings = new CoverletSettings();
            _fixture.TestFile = "./test/Cake.Coverlet.Tests/bin/Debug/netcoreapp2.1/Cake.Coverlet.Tests.dll";
            _fixture.TestProject = "./test/Cake.Coverlet.Tests/";

            var result = _fixture.Run();

            result.Args.Should().StartWith("\"/Working/test/Cake.Coverlet.Tests/bin/Debug/netcoreapp2.1/Cake.Coverlet.Tests.dll\"");
            result.Args.Should().Contain("--targetargs \"test /Working/test/Cake.Coverlet.Tests --no-build\"");
            result.Args.Should().Contain("--format json");
        }

        [Fact]
        public void OutputFormat_Supports_TeamCity()
        {
            _fixture.Settings = new CoverletSettings 
            {
                CoverletOutputFormat = CoverletOutputFormat.teamcity
            };
            _fixture.TestFile = "./test/Cake.Coverlet.Tests/bin/Debug/netcoreapp2.1/Cake.Coverlet.Tests.dll";
            _fixture.TestProject = "./test/Cake.Coverlet.Tests/";

            var result = _fixture.Run();

            result.Args.Should().Contain("--format teamcity");
        }
    }
}
