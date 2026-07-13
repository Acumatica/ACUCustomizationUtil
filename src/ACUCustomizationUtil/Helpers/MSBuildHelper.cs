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
    private readonly MetaDataHelper _metaDataHelper;

    public MsBuildHelper(IAcuConfiguration config, StatusContext ctx)
    {
        _config = config;
        _ctx = ctx;
        _packageSourceBin = _config.Src.PkgSourceBinDirectory!;
        _msBuildTargetDirectory = _config.Src.MsBuildTargetDirectory;
        _msBuildAssemblyFileName = _config.Src.MsBuildAssemblyName;
        _metaDataHelper = new MetaDataHelper(_config);
    }

    public void Execute()
    {
        _metaDataHelper.SetBuildVersion();
        _metaDataHelper.SetBuildMetadata();
        //Build solution
        _msbuildPath = GetMsbuildPath();
        _msbuildArgs = GetMsBuildArgs();
        ProcessHelper process = new ProcessHelper(_msbuildPath, _msbuildArgs, _ctx);
        process.Execute();
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

        return $"{buildConfiguration} {buildTarget} {solutionFilePath}";
    }

    private string GetMsbuildPath()
    {
        if (_config.Src.MsBuildPath != null && File.Exists(_config.Src.MsBuildPath))
            return _config.Src.MsBuildPath;

        Process proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "where.exe",
                Arguments = "/R C:\\ MSBuild.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            },
        };

        proc.Start();
        while (!proc.StandardOutput.EndOfStream)
        {
            string? line = proc.StandardOutput.ReadLine();
            if (line is null || !File.Exists(line))
                continue;
            return line;
        }

        throw new FileNotFoundException("MSBuild not found");
    }
}
