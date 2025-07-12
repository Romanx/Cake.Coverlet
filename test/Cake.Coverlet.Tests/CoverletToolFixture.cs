using Cake.Core.IO;
using Cake.Testing.Fixtures;

namespace Cake.Coverlet.Tests;

[UsedImplicitly]
public class CoverletToolFixture() : ToolFixture<CoverletSettings>("coverlet")
{
    public FilePath TestFile { get; set; }
    public FilePath TestProject { get; set; }

    protected override void RunTool()
    {
        var tool = new CoverletTool(FileSystem, Environment, ProcessRunner, Tools);
        tool.Run(TestFile, TestProject, Settings);
    }
}
