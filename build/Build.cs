using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.CompressionTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using FileMode = System.IO.FileMode;
using Project = Nuke.Common.ProjectModel.Project;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    // Parameters

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Path to scripts folder")]
    readonly string ScriptsPath = null;

    [Parameter()]
    readonly string DeveloperName = "Amadare";


    // Paths

    Project MainProject => Solution.Projects.First();

    AbsolutePath ManifestPath => MainProject.Directory / "manifest.json";

    AbsolutePath OutputPath => MainProject.Directory / MainProject.GetMSBuildProject(Configuration).GetPropertyValue("OutputPath");

    AbsolutePath ArchiveName => BuildProjectDirectory / "dist" / $"{DeveloperName}-RailCharges.zip";


    [Solution(SuppressBuildProjectCheck = true)]
    readonly Solution Solution;

    Target CopyManifest => _ => _
        .Executes(() =>
        {
            void Update(JObject target, Project project, string targetProp, string srcProp)
            {
                var prev = target[targetProp];
                target[targetProp] = project.GetRequiredProperty(srcProp);
                if (!target[targetProp].Equals(prev))
                {
                    Log.Information($"Updated '{targetProp}'");
                }
            }

            var manifest = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(ManifestPath));
            Update(manifest, MainProject, "version_number", "Version");
            Update(manifest, MainProject, "description", "Description");
            Update(manifest, MainProject, "name", "AssemblyName");

            var resultPath = OutputPath / "manifest.json";
            Log.Information($"Copying manifest.json to {resultPath}");
            File.WriteAllText(resultPath, JsonConvert.SerializeObject(manifest, Formatting.Indented));
        });

    Target Clean => _ => _
        .Executes(() =>
        {
            DeleteDirectory(OutputPath);
            EnsureExistingDirectory(OutputPath);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(MainProject));
        });

    Target Compile => _ => _
        .DependsOn(Clean, Restore, CopyManifest)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetConfiguration(Configuration)
                .SetProjectFile(MainProject)
            );
        });

    Target CopyToScripts => _ => _
        .DependsOn(Compile)
        .Requires(() => ScriptsPath)
        .Executes(() =>
        {
            var dll = OutputPath.GlobFiles("*.dll").First();
            CopyFileToDirectory(
                dll,
                ScriptsPath,
                FileExistsPolicy.Overwrite,
                createDirectories: true
            );
            ReportSummary(_ =>
            {
                _.Add("CopiedTo", ScriptsPath);
                return _;
            });
        });

    Target Zip => _ => _
        .DependsOn(Compile)
        .Produces(ArchiveName)
        .Requires(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            Log.Information($"Zipping to {ArchiveName}");
            CompressZip(OutputPath, ArchiveName, fileMode: FileMode.Create);
            ReportSummary(_ =>
            {
                _.Add("Created", ArchiveName);
                return _;
            });
        });
}