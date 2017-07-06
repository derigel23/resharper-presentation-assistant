using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Tools.NuGet;
using Nuke.Core;
using Nuke.Core.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

class DefaultBuild : GitHubBuild
{
    public static void Main () => Execute<DefaultBuild>(x => x.Compile);

    Target Clean => _ => _
            .Executes(
                () => DeleteDirectories(GlobDirectories(SolutionDirectory, "**/bin", "**/obj")),
                () => PrepareCleanDirectory(OutputDirectory));

    Target Restore => _ => _
            .DependsOn(Clean)
            .Executes(() => MSBuild(s => DefaultSettings.MSBuildRestore));

    Target Compile => _ => _
            .DependsOn(Restore)
            .Executes(() => MSBuild(s => DefaultSettings.MSBuildCompile));

    Target Pack => _ => _
            .DependsOn(Compile)
            .Executes(() => GlobFiles(RootDirectory, "install/*.nuspec")
                    .ForEach(x => NuGetPack(x, s => DefaultSettings.NuGetPack
                                .SetBasePath(Path.GetDirectoryName(x)))));

}
