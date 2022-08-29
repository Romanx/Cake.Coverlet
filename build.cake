
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
    DotNetRestore("./Cake.Coverlet.sln");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does<MyBuildData>((data) =>
{
    DotNetBuild("./Cake.Coverlet.sln", data.BuildSettings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does<MyBuildData>((data) => 
{
    DotNetTest("./test/Cake.Coverlet.Tests", data.TestSettings);
});

Task("Pack")
    .IsDependentOn("Test")
    .Does<MyBuildData>((data) =>
{
    DotNetPack("./src/Cake.Coverlet/Cake.Coverlet.csproj", data.PackSettings);
});

Task("Github")
    .IsDependentOn("Pack");

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);

public class MyBuildData
{
	public string Configuration { get; }
    public ConvertableDirectoryPath ArtifactsDirectory { get; }
    public DotNetBuildSettings BuildSettings { get; }
    public DotNetPackSettings PackSettings { get; }
    public DotNetTestSettings TestSettings { get; }
    public IReadOnlyList<ConvertableDirectoryPath> BuildDirs { get; }

	public MyBuildData(
		string configuration,
        ConvertableDirectoryPath artifactsDirectory,
        IReadOnlyList<ConvertableDirectoryPath> buildDirectories)
	{
		Configuration = configuration;
        ArtifactsDirectory = artifactsDirectory;
        BuildDirs = buildDirectories;

        BuildSettings = new DotNetBuildSettings {
            Configuration = configuration,
            NoRestore = true,
            ArgumentCustomization = args => args.Append("/property:WarningLevel=0") // Until Warnings are fixed in StyleCop
        };

        PackSettings = new DotNetPackSettings
        {
            OutputDirectory = ArtifactsDirectory,
            NoBuild = true,
            Configuration = Configuration,
        };

        TestSettings = new DotNetTestSettings
        {
            NoBuild = true,
            Configuration = Configuration
        };
	}
}