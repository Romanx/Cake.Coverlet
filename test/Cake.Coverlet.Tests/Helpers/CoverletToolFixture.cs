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

    public new CoverletToolResult Run()
    {
        var innerResult = base.Run();
        return CoverletToolResult.Convert(innerResult);
    }
}

[UsedImplicitly]
public class CoverletToolTargetFixture() : ToolFixture<CoverletSettings>("coverlet")
{
    public Path Path { get; set; }
    public FilePath Target { get; set; }

    protected override void RunTool()
    {
        new CoverletTool(FileSystem, Environment, ProcessRunner, Tools)
            .RunTarget(Path, Target, Settings);
    }

    public new CoverletToolResult Run()
    {
        var innerResult = base.Run();
        return CoverletToolResult.Convert(innerResult);
    }
}
