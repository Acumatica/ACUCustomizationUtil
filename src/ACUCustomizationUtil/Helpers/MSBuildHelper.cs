using System.Diagnostics;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;
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

    public MsBuildHelper(IAcuConfiguration config, StatusContext ctx)
    {
        _config = config;
        _ctx = ctx;
        _packageSourceBin = _config.Src.PkgSourceBinDirectory!;
        _msBuildTargetDirectory = _config.Src.MsBuildTargetDirectory;
        _msBuildAssemblyFileName = _config.Src.MsBuildAssemblyName;
    }

    public async Task Execute()
    {
        //Build solution
        _msbuildPath = GetMsbuildPath();
        _msbuildArgs = GetMsBuildArgs(_config);
        var process = new ProcessHelper(_msbuildPath, _msbuildArgs, _ctx);
        await process.Execute();
    }

    public async Task CopyAssemblyToPackageBinAsync()
    {
        await Task.Run(() =>
        {
            var assemblyDllFile = Path.Combine(_msBuildTargetDirectory!, _msBuildAssemblyFileName!);
            if (File.Exists(assemblyDllFile))
            {
                var packageDllFile = Path.Combine(_packageSourceBin!, _msBuildAssemblyFileName!);
                packageDllFile.TryCheckFileDirectory();
                File.Copy(assemblyDllFile, packageDllFile, true);
                if (!File.Exists(packageDllFile))
                    throw new InvalidOperationException($"Source file {assemblyDllFile} not copied to {packageDllFile}!");
            }
            else
            {
                throw new FileNotFoundException($"Assembly file {assemblyDllFile} not found");
            }
        });
    }


    private static string GetMsBuildArgs(IAcuConfiguration config)
    {
        var datePart = GetDateVersion();
        var version = $"{config.Erp.ErpVersion?[..6]}.{datePart}";
        var solutionFilePath = config.Src.MsBuildSolutionFile;
        var versionProperty = $"/property:Version={version}";
        
        const string buildConfiguration = "/property:Configuration=Release";
        const string buildTarget = "/target:Rebuild";
        

        return $"{buildConfiguration} {versionProperty} {buildTarget} {solutionFilePath}";
    }

    private static string GetDateVersion()
    {
        var firstDate = new DateTime(DateTime.Now.Year, 1, 1);
        var days = Math.Truncate((DateTime.Now - firstDate).TotalDays).ToString("000");
        return $"{DateTime.Now:yy}{days}.{DateTime.Now:HHmm}";
    }

    private string GetMsbuildPath()
    {
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "where.exe",
                Arguments = "/R C:\\ MSBuild.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        proc.Start();
        while (!proc.StandardOutput.EndOfStream)
        {
            var line = proc.StandardOutput.ReadLine();
            if (line is null || !File.Exists(line)) continue;
            return line;
        }

        throw new FileNotFoundException("MSBuild not found");
    }
}