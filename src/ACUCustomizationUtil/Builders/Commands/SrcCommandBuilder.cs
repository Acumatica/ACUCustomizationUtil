using System.CommandLine;

using ACUCustomizationUtils.Builders.Commands.Binders;
using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Common;
using ACUCustomizationUtils.Services.Src;

namespace ACUCustomizationUtils.Builders.Commands;

/// <summary>
/// This class is the point of building an application commands routing (Code subcommand)
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public class SrcCommandBuilder(ISrcService projectService) : CommandBuilderBase
{
    private readonly ISrcService _projectService = projectService;

    public override Command BuildCommand()
    {
        Command srcCommand = BuildSrcCommand();
        Command makeCommand = BuildMakeCommand();
        Command compileCommand = BuildCompileCommand();

        Command pkgCommand = new Command("src", "Work with a source code of customization.")
        {
            srcCommand,
            makeCommand,
            compileCommand,
        };

        return pkgCommand;
    }

    private Command BuildMakeCommand()
    {
        Option<string> projectDescription = GetProjectDescriptionOption();
        Option<string> projectLevel = GetProjectLevelOption();
        Option<string> sourceDirectory = GetSourceDirectoryOption();
        Option<string> packageName = GetPackageNameOption();
        Option<string> packageDirectory = GetPackageDirectoryOption();
        Option<string> packageSuffix = GetPackageSuffixOption();
        Option<string> makeMode = BuildMakeModeOption();

        Command command = new Command("make", "Create customization package from source code")
        {
            sourceDirectory,
            packageName,
            packageDirectory,
            packageSuffix,
            makeMode,
        };

        command.SetHandler(
            _projectService.MakeProjectFromSource,
            new CodeMakeConfigurationBinder(
                ConfigOption!,
                UserConfigOption!,
                packageName,
                sourceDirectory,
                packageDirectory,
                projectDescription,
                projectLevel,
                makeMode,
                packageSuffix
            )
        );

        return command;
    }

    private Command BuildSrcCommand()
    {
        Option<string> packageName = GetPackageNameOption();
        Option<string> dbConnection = GetDBConnectionStringOption();
        Option<string> instancePath = GetInstancePathOption();
        Option<string> sourceDirectory = GetSourceDirectoryOption();

        Command command = new Command("get", "Get customization project source")
        {
            packageName,
            dbConnection,
            instancePath,
            sourceDirectory,
        };

        command.SetHandler(
            _projectService.GetProjectSource,
            new CodeSrcConfigurationBinder(
                ConfigOption!,
                UserConfigOption!,
                packageName,
                dbConnection,
                instancePath,
                sourceDirectory
            )
        );
        return command;
    }

    private Command BuildCompileCommand()
    {
        Option<string> msBuildSolutionFilePath = GetMsBuildSolutionFileNameOption();
        Option<string> msBuildTargetDirectory = GetMsBuildTargetDirectoryOption();
        Option<string> msBuildAssemblyFile = GetMsBuildAssemblyFileNameOption();
        Option<string> msBuildPath = GetMsBuildPathOption();
        Option<string> msAssemblyInfoPath = GetMsBuildAssemblyInfoPathOption();

        Command command = new Command("build", "Build dll from extension library source code")
        {
            msBuildSolutionFilePath,
            msBuildTargetDirectory,
            msBuildAssemblyFile,
            msBuildPath,
            msAssemblyInfoPath,
        };

        command.SetHandler(
            _projectService.CompileSolution,
            new CodeCompileConfigurationBinder(
                ConfigOption!,
                UserConfigOption!,
                msBuildSolutionFilePath,
                msBuildTargetDirectory,
                msBuildAssemblyFile,
                msBuildPath,
                msAssemblyInfoPath
            )
        );

        return command;
    }

    private static Option<string> GetInstancePathOption()
    {
        return new Option<string>("--instancePath", "Acumatica instance physical path");
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
        return new Option<string>("--pkgName", "Package name");
    }

    private static Option<string> GetPackageSuffixOption()
    {
        return new Option<string>("--pkgSuffix", "Package suffix");
    }

    private static Option<string> GetPackageDirectoryOption()
    {
        return new Option<string>("--pkgDirectory", "Package destination directory");
    }

    private static Option<string> GetMsBuildPathOption()
    {
        return new Option<string>("--msBuildPath", "MSBuild full path");
    }

    private static Option<string> GetMsBuildAssemblyInfoPathOption()
    {
        return new Option<string>(
            "--assemblyInfoPath",
            "External code assembly info file full name"
        );
    }

    private static Option<string> GetMsBuildSolutionFileNameOption()
    {
        return new Option<string>("--solutionFile", "External code solution file full name");
    }

    private static Option<string> GetMsBuildTargetDirectoryOption()
    {
        return new Option<string>("--targetDirectory", "External code build target directory");
    }

    private static Option<string> GetMsBuildAssemblyFileNameOption()
    {
        return new Option<string>("--assemblyName", "External code build ");
    }

    private static Option<string> BuildMakeModeOption()
    {
        return new Option<string>(
            name: "--mode",
            description: "Mode for make package: QA|ISV",
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                    return Messages.MakeModeBase;
                string optionValue = result.Tokens.Single().Value;
                if (
                    optionValue != Messages.MakeModeBase
                    && optionValue != Messages.MakeModeQA
                    && optionValue != Messages.MakeModeISV
                )
                    return Messages.MakeModeBase;

                return optionValue;
            }
        );
    }
}
