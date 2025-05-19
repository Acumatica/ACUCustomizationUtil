using System.CommandLine;
using System.CommandLine.Binding;

using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Helpers;

namespace ACUCustomizationUtils.Builders.Commands.Common;

public abstract class CommandParametersBinder : BinderBase<IAcuConfiguration>
{
    private readonly Option<FileInfo> _configFile;
    private readonly Option<FileInfo> _userConfigFile;
    private readonly Option<string>?[] _commandOptions;

    protected CommandParametersBinder(
        Option<FileInfo> configFile,
        Option<FileInfo> userConfigFile,
        params Option<string>?[] commandOptions
    )
    {
        _configFile = configFile;
        _userConfigFile = userConfigFile;
        _commandOptions = commandOptions;
    }

    protected override IAcuConfiguration GetBoundValue(BindingContext bindingContext)
    {
        FileInfo? configFile = bindingContext.ParseResult.GetValueForOption(_configFile);
        FileInfo? userConfigFile = bindingContext.ParseResult.GetValueForOption(_userConfigFile);
        IAcuConfiguration userInput = GetUserConfiguration(bindingContext, _commandOptions);
        userInput.OnDeserialized();
        IAcuConfiguration acuConfiguration = ConfigurationHelper.GetConfiguration(
            configFile,
            userConfigFile,
            userInput
        );

        return acuConfiguration;
    }

    protected abstract IAcuConfiguration GetUserConfiguration(
        BindingContext bindingContext,
        Option<string>?[] commandOptions
    );
}
