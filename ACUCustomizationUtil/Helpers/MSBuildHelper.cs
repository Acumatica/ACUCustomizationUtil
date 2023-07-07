using System.Diagnostics;
using ACUCustomizationUtils.Configuration;
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
    private string? _packageSourceBin;
    private string? _packageName;

    public MsBuildHelper(IAcuConfiguration config, StatusContext ctx)
    {
        _config = config;
        _ctx = ctx;
    }

    public async Task Execute()
    {
        //Build solution
        _msbuildPath = GetMsbuildPath();
        _msbuildArgs = GetMsBuildArgs(_config);
        _packageSourceBin = _config.Code.PkgSourceBinDirectory!;
        _packageName = _config.Package.PackageName;
        
        var process = new ProcessHelper(_msbuildPath, _msbuildArgs, _ctx);
        await process.Execute();


        if (Directory.Exists(_packageSourceBin))
        {
            //Copy dll`s
            var files = Directory.GetFiles(_packageSourceBin);
            foreach (var file in files.Where(f => f.EndsWith(".dll")))
            {
                var sourceFile = Path.Combine(_config.Code.MsBuildTargetDirectory!, new FileInfo(file).Name);
                if (!File.Exists(sourceFile))
                    throw new InvalidOperationException($"Source file {sourceFile} not copied!");
                File.Copy(sourceFile, file, true);
                if (!File.Exists(file)) throw new InvalidOperationException($"Target file {file} not copied!");
            }
        }
        else
        {
            if (_packageName != null)
            {
                var targetFile = Path.Combine(_packageSourceBin, $"{_packageName}.dll");;
                var sourceFile = Path.Combine(_config.Code.MsBuildTargetDirectory!, $"{_packageName}.dll");
                if (!File.Exists(sourceFile))
                    throw new InvalidOperationException($"Source dll file {sourceFile} not copied!");
                targetFile.TryCheckFileDirectory();
                File.Copy(sourceFile, targetFile, true);
                if (!File.Exists(targetFile)) throw new InvalidOperationException($"Package dll file {targetFile} not copied!");
            }
        }
        
        
    }


    private static string GetMsBuildArgs(IAcuConfiguration config)
    {

        var version = $"{config.Erp.ErpVersion?[..6]}.{DateTime.Now:yMd.HHmm}";
        var solutionFilePath = config.Code.MsBuildSolutionFile;
        var versionProperty = $"/property:Version={version}";
        
        const string buildConfiguration = "/property:Configuration=Release";
        const string buildTarget = "/target:Rebuild";
        

        return $"{buildConfiguration} {versionProperty} {buildTarget} {solutionFilePath}";
    }

    private string GetMsbuildPath()
    {
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "where.exe",
                Arguments = "msbuild",
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
            _ctx.Status($"Found MSBuild: {line}");
            return line;
        }

        throw new FileNotFoundException("MSBuild not found");
    }
}