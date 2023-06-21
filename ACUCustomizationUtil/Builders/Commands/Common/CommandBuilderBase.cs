using System.CommandLine;

namespace ACUCustomizationUtils.Builders.Commands.Common;
/// <summary>
/// This class is the base command builder
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class CommandBuilderBase : ICommandBuilder
{
    protected Option<FileInfo?>? ConfigOption;
    protected Option<FileInfo?>? UserConfigOption;
    public abstract Command BuildCommand();

    public ICommandBuilder SetGlobalOptions(Option<FileInfo?> configOption, Option<FileInfo?> userConfigOption)
    {
        ConfigOption = configOption;
        UserConfigOption = userConfigOption;
        return this;
    }
}