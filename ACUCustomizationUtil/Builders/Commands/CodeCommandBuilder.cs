﻿using System.CommandLine;
using ACUCustomizationUtils.Builders.Commands.Binders;
using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Services;

namespace ACUCustomizationUtils.Builders.Commands;
/// <summary>
/// This class is the point of building an application commands routing (Code subcommand)
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class CodeCommandBuilder : CommandBuilderBase
{
    private readonly ICodeService _projectService;

    public CodeCommandBuilder(ICodeService projectService)
    {
        _projectService = projectService;
    }

    public override Command BuildCommand()
    {
        var srcCommand = BuildSrcCommand();
        var makeCommand = BuildMakeCommand();
        var compileCommand = BuildCompileCommand();

        var pkgCommand = new Command("code", "Work with a customization code.")
        {
            srcCommand,
            makeCommand,
            compileCommand
        };

        return pkgCommand;
    }

    private Command BuildMakeCommand()
    {
        var projectDescription = GetProjectDescriptionOption();
        var projectLevel = GetProjectLevelOption();
        var sourceDirectory = GetSourceDirectoryOption();
        var packageName = GetPackageNameOption();
        var packageDirectory = GetPackageDirectoryOption();
        var makeQA = BuildQAOption();
        var makeISV = BuildISVOption();

        var command = new Command("make", "Create customization package from source code")
        {
            sourceDirectory, packageName, packageDirectory, makeQA, makeISV
        };

        command.SetHandler(_projectService.MakeProjectFromSource,
            new CodeMakeConfigurationBinder
            (
                ConfigOption!,
                UserConfigOption!,
                packageName,
                sourceDirectory,
                packageDirectory,
                projectDescription,
                projectLevel,
                makeQA,
                makeISV
            ));

        return command;
    }

    private Command BuildSrcCommand()
    {
        var packageName = GetPackageNameOption();
        var dbConnection = GetDBConnectionStringOption();
        var sitePhysicalPath = GetSitePhysicalPathOption();
        var sourceDirectory = GetSourceDirectoryOption();

        var command = new Command("src", "Get customization project source")
        {
            packageName, dbConnection, sitePhysicalPath, sourceDirectory
        };

        command.SetHandler(_projectService.GetProjectSource,
            new CodeSrcConfigurationBinder
            (
                ConfigOption!,
                UserConfigOption!,
                packageName,
                dbConnection,
                sitePhysicalPath,
                sourceDirectory
            ));
        return command;
    }

    private Command BuildCompileCommand()
    {
        var msBuildSolutionFilePath = GetMsBuildSolutionFileNameOption();
        var msBuildTargetDirectory = GetMsBuildTargetDirectoryOption();
        var command = new Command("compile", "Compile external library source code")
        {
            msBuildSolutionFilePath, msBuildTargetDirectory
        };

        command.SetHandler(_projectService.CompileSolution,
            new CodeCompileConfigurationBinder(
                ConfigOption!,
                UserConfigOption!,
                msBuildSolutionFilePath,
                msBuildTargetDirectory
            ));

        return command;
    }

    private static Option<string> GetSitePhysicalPathOption()
    {
        return new Option<string>("--sitePath", "Acumatica instance physical path");
    }

    private static Option<string> GetDBConnectionStringOption()
    {
        return new Option<string>("--dbConnectionString", "Database connection string");
    }

    private static Option<string> GetProjectDescriptionOption()
    {
        return new Option<string>("--projectDescription", "Customization project description");
    }

    private static Option<string> GetProjectLevelOption()
    {
        return new Option<string>("--projectLevel", "Customization project level");
    }

    private static Option<string> GetSourceDirectoryOption()
    {
        return new Option<string>("--sourceDirectory", "Customization source items directory");
    }

    private static Option<string> GetPackageNameOption()
    {
        return new Option<string>("--packageName", "Package name");
    }

    private static Option<string> GetPackageDirectoryOption()
    {
        return new Option<string>("--packageDirectory", "Package destination directory");
    }

    private static Option<string> GetMsBuildSolutionFileNameOption()
    {
        return new Option<string>("--solutionFile", "External code solution file full name");
    }

    private static Option<string> GetMsBuildTargetDirectoryOption()
    {
        return new Option<string>("--targetDirectory", "External code build target directory");
    }


    private static Option<string> BuildQAOption()
    {
        return new Option<string>(
            name: "--qa",
            description: "Make package for QA");
    }

    private static Option<string> BuildISVOption()
    {
        return new Option<string>(
            name: "--isv",
            description: "Make package for ISV");
    }
}