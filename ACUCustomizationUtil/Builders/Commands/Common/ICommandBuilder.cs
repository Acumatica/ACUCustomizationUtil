using System.CommandLine;

namespace ACUCustomizationUtils.Builders.Commands.Common;

public interface ICommandBuilder
{
    Command BuildCommand();
    ICommandBuilder SetGlobalOptions(Option<FileInfo?> configOption, Option<FileInfo?> userConfigOption);
}