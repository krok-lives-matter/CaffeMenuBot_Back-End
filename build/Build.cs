using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.TeamCity;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[TeamCity(
    TeamCityAgentPlatform.Unix,
    Version = "2020.2",
    VcsTriggeredTargets = new[] {nameof(Up), nameof(Test)},
    NonEntryTargets = new[] {nameof(Restore)},
    ExcludedTargets = new[] {nameof(Clean)})]
[GitHubActions(
    "nuke",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = new[] {"main"},
    InvokedTargets = new[] {nameof(Test)},
    ImportGitHubTokenAs = nameof(GitHubToken))]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Up);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Check both checkboxes to wipe the database data")]
    readonly bool WipeDatabaseData;

    [Parameter]
    readonly bool WipeDatabaseDataProtection;

    [Parameter]
    readonly string GitHubToken;

    [Solution] readonly Solution Solution;
    [PathExecutable("docker-compose")] readonly Tool DockerCompose;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";

    Target Down => _ => _
        .Executes(() =>
        {
            if (WipeDatabaseData && WipeDatabaseDataProtection)
                DockerCompose("down --volumes", SourceDirectory);
            else
                DockerCompose("down", SourceDirectory);
        });

    Target Up => _ => _
        .DependsOn(Down)
        .Executes(() =>
        {
            DockerCompose("up --build -d", SourceDirectory);
        });
    
    Target Clean => _ => _
        .Executes(() =>
        {
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });
    
    [Partition(2)] readonly Partition TestPartition;
    IEnumerable<Project> TestProjects => TestPartition.GetCurrent(Solution.GetProjects("*Tests"));
    
    Target Test => _ => _
        .Partition(() => TestPartition)
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetConfiguration(Configuration)
                .SetVerbosity(DotNetVerbosity.Minimal)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .CombineWith(TestProjects, (_, v) => _
                    .SetProjectFile(v)), completeOnFailure: false);
        });
}
