using System.CommandLine;
using System.CommandLine.Binding;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Helpers;
using ACUCustomizationUtils.Services;

namespace ACUCustomizationUtils.Builders.Commands.Common;

public abstract class CommandParametersBinder : BinderBase<IAcuConfiguration>
{
    private readonly Option<FileInfo> _configFile;
    private readonly Option<FileInfo> _userConfigFile;
    private readonly Option<string>?[] _commandOptions;

    protected CommandParametersBinder(Option<FileInfo> configFile, Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions)
    {
        _configFile = configFile;
        _userConfigFile = userConfigFile;
        _commandOptions = commandOptions;
    }

    protected override IAcuConfiguration GetBoundValue(BindingContext bindingContext)
    {
        var configFile = bindingContext.ParseResult.GetValueForOption(_configFile);
        var userConfigFile = bindingContext.ParseResult.GetValueForOption(_userConfigFile);
        var userInput = GetUserConfiguration(bindingContext, _commandOptions);
        userInput.OnDeserialized();
        var acuConfiguration = ConfigurationHelper.GetConfiguration(configFile, userConfigFile, userInput);

        return acuConfiguration;
    }

    protected abstract IAcuConfiguration GetUserConfiguration(BindingContext bindingContext,
        Option<string>?[] commandOptions);
}