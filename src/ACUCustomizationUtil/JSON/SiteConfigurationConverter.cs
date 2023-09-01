using System.Text.Json;
using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.Site;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.JSON;

public class SiteConfigurationConverter : JsonConverter<ISiteConfiguration>
{
    public override ISiteConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        var site = new SiteConfiguration();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return site;
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected PropertyName token");
            var propName = reader.GetString();
            reader.Read();
            switch (propName?.FirstCharToUpper())
            {
                case nameof(site.AcumaticaToolPath):
                    site.AcumaticaToolPath = reader.GetString();
                    break;

                case nameof(site.InstanceName):
                    site.InstanceName = reader.GetString();
                    break;

                case nameof(site.InstancePath):
                    site.InstancePath = reader.GetString();
                    break;

                case nameof(site.SqlServerName):
                    site.SqlServerName = reader.GetString();
                    break;

                case nameof(site.DbName):
                    site.DbName = reader.GetString();
                    break;

                case nameof(site.DbConnectionString):
                    site.DbConnectionString = reader.GetString();
                    break;

                case nameof(site.IisAppPool):
                    site.IisAppPool = reader.GetString();
                    break;

                case nameof(site.IisDbUsername):
                    site.IisDbUsername = reader.GetString();
                    break;

                case nameof(site.IisWebSite):
                    site.IisWebSite = reader.GetString();
                    break;

                case nameof(site.AcumaticaAdminName):
                    site.AcumaticaAdminName = reader.GetString();
                    break;

                case nameof(site.AcumaticaAdminPassword):
                    site.AcumaticaAdminPassword = reader.GetString();
                    break;
                
            }
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, ISiteConfiguration value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}