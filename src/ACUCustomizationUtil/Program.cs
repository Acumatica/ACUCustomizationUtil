using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using ACUCustomizationUtils.Builders.Commands;
using Serilog;
using System.Reflection;
using ACUCustomizationUtils.Builders.DI;
using ACUCustomizationUtils.Builders.Log;
using ACUCustomizationUtils.Common;
using Spectre.Console;

[assembly: AssemblyVersion("23.08.21.*")]

namespace ACUCustomizationUtils;
/// <summary>
/// This class is the point of building an application and the start of user input processing
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public static class Program
{
    private static async Task<int> Main(string[] args)
    {
        SerilogBuilder.Build();
        try
        {
            using var host = HostBuilder.Build(args);
            var rootCommand = new RootCommandBuilder(host.Services).BuildCommand();

            var parser = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .UseHelp(ctx =>
                {
                    ctx.HelpBuilder.CustomizeLayout(
                        _ =>
                            HelpBuilder.Default
                                .GetLayout()
                                .Skip(1) // Skip the default command description section.
                                .Prepend(_ =>
                                {
                                    //AnsiConsole.Write(new FigletText(Messages.Acu));
                                    AnsiConsole.WriteLine(rootCommand.Description!);
                                    AnsiConsole.WriteLine(Messages.Copyright);
                                }));
                })
                .Build();
            
            
            //return await rootCommand.InvokeAsync(args);
            return await parser.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}