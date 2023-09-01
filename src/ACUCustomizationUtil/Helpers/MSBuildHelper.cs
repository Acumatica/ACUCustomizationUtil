﻿using System.Diagnostics;
using ACUCustomizationUtils.Configuration.ACU;
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
        _packageSourceBin = _config.Code.PkgSourceBinDirectory!;
        _msBuildTargetDirectory = _config.Code.MsBuildTargetDirectory;
        _msBuildAssemblyFileName = _config.Code.MsBuildAssemblyName;
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
            return line;
        }

        throw new FileNotFoundException("MSBuild not found");
    }
}