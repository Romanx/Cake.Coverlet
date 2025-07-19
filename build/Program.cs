using System.Collections.Generic;
using System.Threading.Tasks;
using Cake.Common;
using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Common.Tools.DotNet.Test;
using Cake.Common.Tools.NuGet;
using Cake.Common.Tools.NuGet.Push;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.GitHub;
using Cake.GitVersioning;
using Nerdbank.GitVersioning;

return new CakeHost()
    .UseContext<BuildContext>()
    .Run(args);

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.CleanDirectories($"{context.SourceDirectory}/**/bin");
        context.CleanDirectory(context.ArtifactsDirectory);
    }
}

[TaskName("Restore")]
[IsDependentOn(typeof(CleanTask))]
public sealed class RestoreTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetRestore($"{context.BaseDirectory}/Cake.Coverlet.slnx");
    }
}

[TaskName("Build")]
[IsDependentOn(typeof(RestoreTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetBuild($"{context.BaseDirectory}/Cake.Coverlet.slnx", context.BuildSettings);
    }
}

[TaskName("Test")]
[IsDependentOn(typeof(BuildTask))]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetTest($"{context.TestDirectory}/Cake.Coverlet.Tests", context.TestSettings);
    }
}

[TaskName("Pack")]
[IsDependentOn(typeof(TestTask))]
public sealed class PackTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetPack($"{context.SourceDirectory}/Cake.Coverlet/Cake.Coverlet.csproj", context.PackSettings);
    }
}

[TaskName("Publish")]
[IsDependentOn(typeof(PackTask))]
public sealed class PublishTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var apiKey = context.Environment.GetEnvironmentVariable("NUGET_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey)) throw new CakeException("No NuGet API key specified.");

        foreach (var file in context.GetFiles($"{context.ArtifactsDirectory}/*.nupkg"))
        {
            context.Log.Information("Publishing {0}...", file.GetFilename().FullPath);
            context.NuGetPush(file,
                new NuGetPushSettings { ApiKey = apiKey, Source = "https://api.nuget.org/v3/index.json" });
        }
    }
}

[TaskName("Github")]
[IsDependentOn(typeof(PublishTask))]
public sealed class GithubTask : AsyncFrostingTask<BuildContext>
{
    public override async Task RunAsync(BuildContext context)
    {
        var githubToken = context.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrWhiteSpace(githubToken))
        {
            context.Log.Warning("No GitHub token specified. Skipping GitHub release creation.");
            return;
        }

        var gitHubRef = context.Environment.GetEnvironmentVariable("GITHUB_REF");
        if (string.IsNullOrWhiteSpace(gitHubRef) || !gitHubRef.StartsWith("refs/tags/"))
        {
            context.Log.Warning("Not running on a tag. Skipping GitHub release creation.");
            return;
        }

        var versionInfo = context.GitVersioningGetVersion();

        var tag = gitHubRef.Replace("refs/tags/", "");
        var version = versionInfo.SimpleVersion;
        var repository = context.Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
        
        if (string.IsNullOrWhiteSpace(repository))
        {
            throw new CakeException("GITHUB_REPOSITORY environment variable not found.");
        }

        var parts = repository.Split('/');
        var owner = parts[0];
        var repoName = parts[1];

        context.Log.Information($"Creating GitHub release for tag: {tag}");
        context.Log.Information($"Repository: {owner}/{repoName}");
        context.Log.Information($"Version: {version}");

        // Get all .nupkg files to attach to the release
        var packageFiles = context.GetFiles($"{context.ArtifactsDirectory}/*.nupkg");
        
        var releaseSettings = new GitHubCreateReleaseSettings
        {
            Name = $"v{version}",
            Body = "",
            Draft = true,
            Prerelease = !versionInfo.PublicRelease,
            Assets = [..packageFiles]
        };
        
        var release = await context.GitHubCreateReleaseAsync(
            userName: null,
            apiToken: githubToken,
            owner: owner,
            repository: repository,
            tagName: tag,
            settings: releaseSettings);

        context.Log.Information($"Created GitHub release: {release.HtmlUrl}");
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(PackTask))]
public sealed class DefaultTask : FrostingTask<BuildContext>
{
}

public sealed class BuildContext : FrostingContext
{
    public BuildContext(ICakeContext context)
        : base(context)
    {
        BuildConfiguration = context.Argument("configuration", "Release");
        BaseDirectory = new DirectoryPath("../").MakeAbsolute(context.Environment.WorkingDirectory);
        TestDirectory = BaseDirectory + "/test";
        SourceDirectory = BaseDirectory + "/src";
        ArtifactsDirectory = BaseDirectory + context.Directory("./artifacts");
        BuildDirs = (List<DirectoryPath>) [context.Directory($"{SourceDirectory}/Cake.Coverlet/bin")];

        BuildSettings = new DotNetBuildSettings
        {
            Configuration = BuildConfiguration,
            NoRestore = true,
            ArgumentCustomization = args => args.Append("/property:WarningLevel=0")
        };

        PackSettings = new DotNetPackSettings
        {
            OutputDirectory = ArtifactsDirectory, NoBuild = true, Configuration = BuildConfiguration
        };

        TestSettings = new DotNetTestSettings { NoBuild = true, Configuration = BuildConfiguration };
    }

    public string BuildConfiguration { get; }
    public DirectoryPath ArtifactsDirectory { get; }
    public DirectoryPath BaseDirectory { get; }
    public DirectoryPath TestDirectory { get; }
    public DirectoryPath SourceDirectory { get; }
    public IReadOnlyList<DirectoryPath> BuildDirs { get; }
    public DotNetBuildSettings BuildSettings { get; }
    public DotNetPackSettings PackSettings { get; }
    public DotNetTestSettings TestSettings { get; }
}
