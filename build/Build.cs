using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.TeamCity;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.Tooling;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[TeamCity(
    TeamCityAgentPlatform.Unix,
    Version = "2020.1",
    VcsTriggeredTargets = new[] {nameof(Up)},
    ManuallyTriggeredTargets = new[] {nameof(Up), nameof(Down)})]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Up);

    /*[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;*/

    [Parameter("Check both checkboxes to wipe the database data")]
    readonly bool WipeDatabaseData;

    [Parameter(Name = "DATABASE_WIPE_SHIELD_CHECKBOX")]
    readonly bool WipeDatabaseDataShield;

    //[Solution] readonly Solution Solution;
    [PathExecutable("docker-compose")] readonly Tool DockerCompose;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    //AbsolutePath TestsDirectory => RootDirectory / "tests";
    //AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Down => _ => _
        .Executes(() =>
        {
            if (WipeDatabaseData && WipeDatabaseDataShield)
                DockerCompose("down --volumes", SourceDirectory);
            else
                DockerCompose("down", SourceDirectory);
        });

    Target Up => _ => _
        .After(Down)
        .Executes(() =>
        {
            DockerCompose("up --build -d", SourceDirectory);
        });
}
