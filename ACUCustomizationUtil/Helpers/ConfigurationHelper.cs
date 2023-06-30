using System.Text.Json;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;
using ACUCustomizationUtils.Helpers.Proxy;
using Microsoft.Extensions.Logging;

namespace ACUCustomizationUtils.Helpers;

public static class ConfigurationHelper 
{
    public static void WriteConfig(IAcuConfiguration config)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        var configJson = JsonSerializer.Serialize(config, options);
        File.WriteAllText("acu.json", configJson);
    }

    public static IAcuConfiguration GetConfiguration(FileSystemInfo? configFile, FileSystemInfo? userConfigFile,
        IAcuConfiguration? userInput = null)
    {
        //Read configurations
        var config = ReadConfig(configFile?.FullName);
        var userConfig = ReadConfig(userConfigFile?.FullName);

        //Merge configurations
        config.CopyValues(userConfig).CopyValues(userInput);
        return config;
    }

    public static void PrintConfiguration<T>(IAcuConfiguration config, ILogger<T> logger, params string[]? types)
    {
        var res = new List<(string?, string, object)>();
        ReadConfigurationValues(config, nameof(IAcuConfiguration), res);
        if (!res.Any()) return;

        logger.LogInformation("Current configuration parameters:");
        foreach (var (type, name, value) in res.Where(r => types?.Contains(r.Item1) ?? true))
            logger.LogInformation("[{Type}] {Key} : {Value}", type, name, value);
    }

    private static IAcuConfiguration ReadConfig(string? filename)
    {
        if (filename == null || !File.Exists(filename)) return AcuNullConfiguration.Instance;
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var data = File.ReadAllText(filename);
        var config = JsonSerializer.Deserialize<IAcuConfiguration>(data, options);
        return config!;
    }

    private static void ReadConfigurationValues(object config, string? currentConfigType,
        ICollection<(string?, string, object)> res)
    {
        var t = config.GetType();
        var properties = t.GetProperties().Where(prop => prop is { CanRead: true, CanWrite: true });
        foreach (var prop in properties)
            if (prop.PropertyType.Assembly == t.Assembly)
            {
                var sConfig = prop.GetValue(config);
                var configType = prop.Name;
                ReadConfigurationValues(sConfig!, configType, res);
            }
            else
            {
                var value = prop.GetValue(config, null);
                if (value != null) res.Add((currentConfigType, prop.Name, value));
            }
    }
}