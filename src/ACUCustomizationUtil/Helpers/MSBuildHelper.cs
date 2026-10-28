using System.Diagnostics;

using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;
using ACUCustomizationUtils.Helpers.CommonTypes;

using Microsoft.Extensions.Logging;

using Spectre.Console;

namespace ACUCustomizationUtils.Helpers;

public class MsBuildHelper
{
    private readonly IAcuConfiguration _config;
    private readonly StatusContext _ctx;
    private string? _msbuildPath;
    private string? _msbuildArgs;
    private readonly string? _packageSourceBin;
    private readonly string? _msBuildTargetDirectory;
    private readonly string? _msBuildAssemblyFileName;
    private readonly MetaDataHelper _metaDataHelper;
    private readonly ILogger? _logger;

    public MsBuildHelper(IAcuConfiguration config, StatusContext ctx, ILogger? logger = null)
    {
        _config = config;
        _ctx = ctx;
        _logger = logger;
        _packageSourceBin = _config.Src.PkgSourceBinDirectory!;
        _msBuildTargetDirectory = _config.Src.MsBuildTargetDirectory;
        _msBuildAssemblyFileName = _config.Src.MsBuildAssemblyName;
        _metaDataHelper = new MetaDataHelper(_config);
    }

    public async Task Execute(PackageVersion packageVersion)
    {
        _metaDataHelper.SetBuildVersion(packageVersion);
        _metaDataHelper.SetBuildMetadata();
        //Build solution
        _msbuildPath = GetMsbuildPath();
        _msbuildArgs = GetMsBuildArgs();
        ProcessHelper process = new ProcessHelper(_msbuildPath, _msbuildArgs, _ctx);
        await process.Execute();
    }

    public async Task CopyAssemblyToPackageBinAsync()
    {
        await Task.Run(() =>
        {
            string assemblyDllFile = Path.Combine(_msBuildTargetDirectory!, _msBuildAssemblyFileName!);
            if (File.Exists(assemblyDllFile))
            {
                string packageDllFile = Path.Combine(_packageSourceBin!, _msBuildAssemblyFileName!);
                packageDllFile.TryCheckFileDirectory();
                File.Copy(assemblyDllFile, packageDllFile, true);
                if (!File.Exists(packageDllFile))
                    throw new InvalidOperationException(
                        $"Source file {assemblyDllFile} not copied to {packageDllFile}!"
                    );
            }
            else
            {
                throw new FileNotFoundException($"Assembly file {assemblyDllFile} not found");
            }
        });
    }

    private string GetMsBuildArgs()
    {
        const string buildConfiguration = "/property:Configuration=Release";
        const string buildTarget = "/target:Rebuild";
        string? solutionFilePath = _config.Src.MsBuildSolutionFile;

        return $"{buildConfiguration} {buildTarget} \"{solutionFilePath}\"";
    }

    /// <summary>
    /// Resolves MSBuild.exe, from the most explicit to the most generic source:
    /// <list type="number">
    /// <item>src.msBuildPath from acu.json / acu.json.user;</item>
    /// <item>vswhere: latest stable Visual Studio / Build Tools;</item>
    /// <item>.NET SDK MSBuild, highest stable SDK (what the former full-disk search picked on most machines);</item>
    /// <item>MSBuild.exe from PATH;</item>
    /// <item>full scan of drive C: (former behavior, slow) with a warning.</item>
    /// </list>
    /// </summary>
    private string GetMsbuildPath()
    {
        if (_config.Src.MsBuildPath != null && File.Exists(_config.Src.MsBuildPath))
            return LogResolved("config", _config.Src.MsBuildPath);

        string vswhere = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            "Microsoft Visual Studio",
            "Installer",
            "vswhere.exe"
        );
        if (File.Exists(vswhere))
        {
            const string vswhereArgs =
                @"-latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe";

            string? vsMsBuild = RunAndReadLines(vswhere, vswhereArgs).FirstOrDefault(File.Exists);
            if (vsMsBuild != null)
                return LogResolved("vswhere", vsMsBuild);
        }

        string? sdkMsBuild = GetDotNetSdkMsBuildPath();
        if (sdkMsBuild != null)
            return LogResolved(".NET SDK", sdkMsBuild);

        string? pathMsBuild = RunAndReadLines("where.exe", "MSBuild.exe").FirstOrDefault(File.Exists);
        if (pathMsBuild != null)
            return LogResolved("PATH", pathMsBuild);

        _logger?.LogWarning(
            "MSBuild not found via vswhere, .NET SDK or PATH. Scanning drive C:, this may take several minutes. "
                + "Set src.msBuildPath in acu.json.user to skip the search"
        );
        string? scannedMsBuild = RunAndReadLines("where.exe", @"/R C:\ MSBuild.exe").FirstOrDefault(File.Exists);
        if (scannedMsBuild != null)
            return LogResolved("disk scan", scannedMsBuild);

        throw new FileNotFoundException(
            "MSBuild not found. Install Visual Studio, Build Tools or .NET SDK, or set src.msBuildPath in acu.json.user"
        );
    }

    /// <summary>
    /// Returns MSBuild.exe of the highest installed .NET SDK, based on "dotnet --list-sdks"
    /// (lines like "10.0.401 [C:\Program Files\dotnet\sdk]").
    /// </summary>
    private static string? GetDotNetSdkMsBuildPath()
    {
        return RunAndReadLines("dotnet", "--list-sdks")
            .Select(line =>
            {
                int bracket = line.IndexOf(" [", StringComparison.Ordinal);
                if (bracket <= 0 || !line.EndsWith(']'))
                    return null;

                string versionText = line[..bracket];
                string sdkRoot = line[(bracket + 2)..^1];
                return !versionText.Contains('-') && Version.TryParse(versionText, out Version? version)
                    ? new
                    {
                        Version = version,
                        Path = Path.Combine(sdkRoot, versionText, "MSBuild.exe"),
                    }
                    : null;
            })
            .Where(sdk => sdk != null && File.Exists(sdk.Path))
            .OrderByDescending(sdk => sdk!.Version)
            .Select(sdk => sdk!.Path)
            .FirstOrDefault();
    }

    private string LogResolved(string source, string path)
    {
        _logger?.LogInformation("MSBuild resolved via {Source}: {Path}", source, path);
        return path;
    }

    /// <summary>
    /// Runs a console tool and returns its non-empty stdout lines.
    /// Returns an empty list if the tool is not installed or cannot be started.
    /// </summary>
    private static List<string> RunAndReadLines(string fileName, string arguments)
    {
        try
        {
            using Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
            };

            // stderr is drained asynchronously so tools like where.exe ("INFO: Could not find ...")
            // neither pollute the console nor block on a full pipe
            proc.ErrorDataReceived += (_, _) => { };
            proc.Start();
            proc.BeginErrorReadLine();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            return output
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return [];
        }
    }
}
