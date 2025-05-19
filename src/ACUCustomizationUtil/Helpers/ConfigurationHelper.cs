using System.Text.Json;

using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;

using Microsoft.Extensions.Logging;

namespace ACUCustomizationUtils.Helpers;

public static class ConfigurationHelper
{
    public static void WriteConfig(IAcuConfiguration config)
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };
        string configJson = JsonSerializer.Serialize(config, options);
        File.WriteAllText("acu.json", configJson);
    }

    public static IAcuConfiguration GetConfiguration(
        FileSystemInfo? configFile,
        FileSystemInfo? userConfigFile,
        IAcuConfiguration? userInput = null
    )
    {
        //Read configurations
        IAcuConfiguration config = ReadConfig(configFile?.FullName);
        IAcuConfiguration userConfig = ReadConfig(userConfigFile?.FullName);

        //Merge configurations
        if (config.IsNotNull)
        {
            config.CopyValues(userConfig).CopyValues(userInput);
            return config;
        }
        else if (userConfig.IsNotNull)
        {
            userConfig.CopyValues(userInput);
            return userConfig;
        }
        else if (userInput != null)
        {
            return userInput;
        }
        else
        {
            throw new Exception($"ACU configuration is null or empty");
        }
    }

    public static void PrintConfiguration<T>(
        IAcuConfiguration config,
        ILogger<T> logger,
        params string[]? types
    )
    {
        List<(string?, string, object)> res = new List<(string?, string, object)>();
        ReadConfigurationValues(config, nameof(IAcuConfiguration), res);
        if (!res.Any())
            return;

        logger.LogInformation("Current configuration parameters:");
        foreach ((string type, string name, object value) in res.Where(r => types?.Contains(r.Item1) ?? true))
            logger.LogInformation("[{Type}] {Key} : {Value}", type, name, value);
    }

    private static IAcuConfiguration ReadConfig(string? filename)
    {
        if (filename == null || !File.Exists(filename))
            return AcuNullConfiguration.Instance;
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        string data = File.ReadAllText(filename);
        IAcuConfiguration? config = JsonSerializer.Deserialize<IAcuConfiguration>(data, options);
        return config!;
    }

    private static void ReadConfigurationValues(
        object config,
        string? currentConfigType,
        ICollection<(string?, string, object)> res
    )
    {
        Type t = config.GetType();
        IEnumerable<System.Reflection.PropertyInfo> properties = t.GetProperties().Where(prop => prop is { CanRead: true, CanWrite: true });
        foreach (System.Reflection.PropertyInfo? prop in properties)
            if (prop.PropertyType.Assembly == t.Assembly)
            {
                object? sConfig = prop.GetValue(config);
                string configType = prop.Name;
                ReadConfigurationValues(sConfig!, configType, res);
            }
            else
            {
                object? value = prop.GetValue(config, null);
                if (value != null)
                    res.Add((currentConfigType, prop.Name, value));
            }
    }
}
