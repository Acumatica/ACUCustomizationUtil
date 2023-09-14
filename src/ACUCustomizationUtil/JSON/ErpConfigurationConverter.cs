using System.Text.Json;
using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.Erp;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.JSON;

public class ErpConfigurationConverter : JsonConverter<IErpConfiguration>
{
    public override IErpConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        var erp = new ErpConfiguration();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return erp;
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected PropertyName token");
            var propName = reader.GetString();
            reader.Read();
            switch (propName?.FirstCharToUpper())
            {
                case nameof(erp.ErpVersion):
                    erp.ErpVersion = reader.GetString();
                    break;
                case nameof(erp.Url):
                    erp.Url = reader.GetString() != null ? new Uri(reader.GetString()!, UriKind.Absolute) : null;
                    break;
                case nameof(erp.DestinationDirectory):
                    erp.DestinationDirectory = reader.GetString().NormalizeEnvVariables();
                    break;
                case nameof(erp.InstallationFileName):
                    erp.InstallationFileName = reader.GetString();
                    break;
            }
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, IErpConfiguration value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}