using System.Text.Json;
using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration;
using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Erp;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.JSON;

public class AcuConfigurationConverter : JsonConverter<IAcuConfiguration>
{
    public override IAcuConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        var configuration = new AcuConfiguration();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                configuration.OnDeserialized();
                return configuration;
            }

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected PropertyName token");

            var propertyName = reader.GetString()?.FirstCharToUpper();
            reader.Read();

            switch (propertyName)
            {
                case nameof(configuration.Erp):
                    var converter = new ErpConfigurationConverter();
                    var erp = converter.Read(ref reader, typeof(IErpConfiguration), options);
                    configuration.Erp.CopyValues(erp);
                    break;

                case nameof(configuration.Site):
                    var siteConverter = new SiteConfigurationConverter();
                    var site = siteConverter.Read(ref reader, typeof(IErpConfiguration), options);
                    configuration.Site.CopyValues(site);
                    break;

                case nameof(configuration.Package):
                    var packageConverter = new PackageConfigurationConverter();
                    var package = packageConverter.Read(ref reader, typeof(IErpConfiguration), options);
                    configuration.Package.CopyValues(package);
                    break;

                case nameof(configuration.Code):
                    var projectConverter = new CodeConfigurationConverter();
                    var project = projectConverter.Read(ref reader, typeof(IErpConfiguration), options);
                    configuration.Code.CopyValues(project);
                    break;
            }
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, IAcuConfiguration value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}