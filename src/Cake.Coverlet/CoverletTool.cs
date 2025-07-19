using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;
using Path = Cake.Core.IO.Path;

namespace Cake.Coverlet;

/// <summary>
/// A class for the coverlet tool
/// </summary>
[PublicAPI]
public sealed class CoverletTool : Tool<CoverletSettings>
{
    private readonly ICakeEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoverletTool"/> class.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="environment">The environment.</param>
    /// <param name="processRunner">The process runner.</param>
    /// <param name="tools">The tool locator.</param>
    public CoverletTool(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IToolLocator tools)
        : base(fileSystem, environment, processRunner, tools)
    {
        _environment = environment;
    }

    /// <summary>
    /// Gets the name of the tool.
    /// </summary>
    /// <returns>The name of the tool</returns>
    protected override string GetToolName() => "coverlet";

    /// <summary>
    /// Runs the tool with the given test file, test project and settings
    /// </summary>
    /// <param name="testFile"></param>
    /// <param name="testProject"></param>
    /// <param name="settings"></param>
    public void Run(FilePath testFile, FilePath testProject, CoverletSettings settings)
    {
        ArgumentNullException.ThrowIfNull(testFile);
        ArgumentNullException.ThrowIfNull(testProject);
        ArgumentNullException.ThrowIfNull(settings);

        Run(settings, GetArguments(testFile, testProject, settings));
    }

    /// <summary>
    /// Executes the specified target using the coverlet tool for a given app directory.
    /// </summary>
    /// <param name="path">The Path to the test assembly or application directory.</param>
    /// <param name="target">Path to the test runner application.</param>
    /// <param name="settings">The settings used to configure the coverlet tool.</param>
    public void RunTarget(Path path, FilePath target, CoverletSettings settings)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(settings);
        
        var argumentBuilder = new ProcessArgumentBuilder();

        var absolutePath = path switch
        {
            FilePath fp => fp.MakeAbsolute(_environment).FullPath,
            DirectoryPath dp => dp.MakeAbsolute(_environment).FullPath,
            _ => throw new ArgumentException("Unhandled type of path")
        };
        
        argumentBuilder.AppendQuoted(absolutePath);
        argumentBuilder.AppendSwitchQuoted("--target", target.MakeAbsolute(_environment).FullPath);
        ArgumentsProcessor.ProcessToolArguments(settings, _environment, argumentBuilder, path);
        
        Run(settings, argumentBuilder);
    }

    private ProcessArgumentBuilder GetArguments(
        FilePath coverageFile,
        FilePath testProject,
        CoverletSettings settings)
    {
        var argumentBuilder = new ProcessArgumentBuilder();

        argumentBuilder.AppendQuoted(coverageFile.MakeAbsolute(_environment).FullPath);

        argumentBuilder.AppendSwitchQuoted("--target", "dotnet");
        argumentBuilder.AppendSwitchQuoted($"--targetargs", $"test {testProject.MakeAbsolute(_environment)} --no-build");

        return ArgumentsProcessor.ProcessToolArguments(settings, _environment, argumentBuilder, testProject);
    }

    /// <summary>
    /// Gets the possible executable names
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<string> GetToolExecutableNames() => ["coverlet", "coverlet.exe"];

    /// <summary>
    /// Gets the alternative tool paths for the tool
    /// </summary>
    /// <param name="settings"></param>
    /// <returns>The alternate tool paths</returns>
    protected override IEnumerable<FilePath> GetAlternativeToolPaths(CoverletSettings settings)
    {
        var globalToolLocation = _environment.Platform.IsUnix()
            ? new DirectoryPath(_environment.GetEnvironmentVariable("HOME"))
            : new DirectoryPath(_environment.GetEnvironmentVariable("USERPROFILE"));

        return GetToolExecutableNames()
            .Select(name => globalToolLocation.Combine(".dotnet/tools").GetFilePath(name));
    }
}
