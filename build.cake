
var target = Argument("target", "Default");

Setup<MyBuildData>(setupContext =>
{
	return new MyBuildData(
		configuration: Argument("configuration", "Release"),
        artifactsDirectory: Directory("./artifacts/"),
        buildDirectories: new List<ConvertableDirectoryPath> {
            Directory("./src/Cake.Coverlet/bin/"),
        });
});

Task("Clean")
    .Does<MyBuildData>((data) =>
{
    foreach (var dir in data.BuildDirs)
    {
        CleanDirectory(dir + Directory(data.Configuration));
    }
    CleanDirectory(data.ArtifactsDirectory);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore("./Cake.Coverlet.sln");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does<MyBuildData>((data) =>
{
    DotNetCoreBuild("./Cake.Coverlet.sln", data.BuildSettings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does<MyBuildData>((data) => 
{
    DotNetCoreTest("./test/Cake.Coverlet.Tests", data.TestSettings);
});

Task("Pack")
    .IsDependentOn("Test")
    .Does<MyBuildData>((data) =>
{
    DotNetCorePack("./src/Cake.Coverlet/Cake.Coverlet.csproj", data.PackSettings);
});

Task("AppVeyor")
    .IsDependentOn("Pack");

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);

public class MyBuildData
{
	public string Configuration { get; }
    public ConvertableDirectoryPath ArtifactsDirectory { get; }
    public DotNetCoreBuildSettings BuildSettings { get; }
    public DotNetCorePackSettings PackSettings { get; }
    public DotNetCoreTestSettings TestSettings { get; }
    public IReadOnlyList<ConvertableDirectoryPath> BuildDirs { get; }

	public MyBuildData(
		string configuration,
        ConvertableDirectoryPath artifactsDirectory,
        IReadOnlyList<ConvertableDirectoryPath> buildDirectories)
	{
		Configuration = configuration;
        ArtifactsDirectory = artifactsDirectory;
        BuildDirs = buildDirectories;

        BuildSettings = new DotNetCoreBuildSettings {
            Configuration = configuration,
            NoRestore = true,
            ArgumentCustomization = args => args.Append("/property:WarningLevel=0") // Until Warnings are fixed in StyleCop
        };

        PackSettings = new DotNetCorePackSettings
        {
            OutputDirectory = ArtifactsDirectory,
            NoBuild = true,
            Configuration = Configuration,
        };

        TestSettings = new DotNetCoreTestSettings
        {
            NoBuild = true,
            Configuration = Configuration
        };
	}
}