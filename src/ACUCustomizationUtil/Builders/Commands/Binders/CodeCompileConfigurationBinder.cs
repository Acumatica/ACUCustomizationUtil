using System.CommandLine;
using System.CommandLine.Binding;

using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Src;

namespace ACUCustomizationUtils.Builders.Commands.Binders;

public class CodeCompileConfigurationBinder : CommandParametersBinder
{
    public CodeCompileConfigurationBinder(
        Option<FileInfo> configFile,
        Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions
    )
        : base(configFile, userConfigFile, commandOptions) { }

    protected override IAcuConfiguration GetUserConfiguration(
        BindingContext bindingContext,
        Option<string>?[] commandOptions
    )
    {
        string? msBuildSolutionFilePath = bindingContext.ParseResult.GetValueForOption(
            commandOptions[0]!
        );
        string? msBuildTargetDirectoryPath = bindingContext.ParseResult.GetValueForOption(
            commandOptions[1]!
        );
        string? msBuildAssemblyFileName = bindingContext.ParseResult.GetValueForOption(
            commandOptions[2]!
        );
        string? msBuildPath = bindingContext.ParseResult.GetValueForOption(commandOptions[3]!);
        string? msAssemblyInfoPath = bindingContext.ParseResult.GetValueForOption(commandOptions[4]!);

        return new AcuConfiguration
        {
            Src = new SrcConfiguration
            {
                MsBuildSolutionFile = msBuildSolutionFilePath,
                MsBuildTargetDirectory = msBuildTargetDirectoryPath,
                MsBuildAssemblyName = msBuildAssemblyFileName,
                MsBuildPath = msBuildPath,
                AssemblyInfoPath = msAssemblyInfoPath,
            },
        };
    }
}
