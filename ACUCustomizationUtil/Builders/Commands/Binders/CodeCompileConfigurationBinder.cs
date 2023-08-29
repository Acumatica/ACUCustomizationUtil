using System.CommandLine;
using System.CommandLine.Binding;
using ACUCustomizationUtils.Builders.Commands.Common;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Code;

namespace ACUCustomizationUtils.Builders.Commands.Binders;

public class CodeCompileConfigurationBinder : CommandParametersBinder
{
    public CodeCompileConfigurationBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions) : base(configFile, userConfigFile, commandOptions)
    {
    }

    protected override IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions)
    {
        var msBuildSolutionFilePath = bindingContext.ParseResult.GetValueForOption(commandOptions[0]!);
        var msBuildTargetDirectoryPath = bindingContext.ParseResult.GetValueForOption(commandOptions[1]!);
        var msBuildAssemblyFileName = bindingContext.ParseResult.GetValueForOption(commandOptions[2]!);


        return new AcuConfiguration
        {
            Code = new CodeConfiguration
            {
                MsBuildSolutionFile = msBuildSolutionFilePath,
                MsBuildTargetDirectory = msBuildTargetDirectoryPath,
                MsBuildAssemblyName = msBuildAssemblyFileName
            }
        };
    }
}