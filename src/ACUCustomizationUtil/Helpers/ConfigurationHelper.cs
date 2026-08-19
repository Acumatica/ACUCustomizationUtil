using System.Reflection;
using System.Text.Json;

using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Extensions;

using Microsoft.Extensions.Logging;

namespace ACUCustomizationUtils.Helpers;

public static class ConfigurationHelper
{
    // Placeholder rendered in the console/log in place of any config property marked [Secret].
    private const string MaskText = "***";

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
        IAcuConfiguration result;
        if (config.IsNotNull)
        {
            config.CopyValues(userConfig).CopyValues(userInput);
            result = config;
        }
        else if (userConfig.IsNotNull)
        {
            userConfig.CopyValues(userInput);
            result = userConfig;
        }
        else if (userInput != null)
        {
            result = userInput;
        }
        else
        {
            throw new Exception($"ACU configuration is null or empty");
        }

        //Derive the DB connection string after the merge so that discrete DB fields
        //(dbProvider, sqlServerName, dbName, dbUser, dbPassword) from any source are respected
        result.Site.DeriveDbConnectionString();

        return result;
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
        IEnumerable<PropertyInfo> properties = t.GetProperties().Where(prop => prop is { CanRead: true, CanWrite: true });
        foreach (PropertyInfo? prop in properties)
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
                    res.Add((currentConfigType, prop.Name,
                        prop.GetCustomAttribute<SecretAttribute>() is not null ? MaskText : value));
            }
    }
}
