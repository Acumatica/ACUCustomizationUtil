using System.Text.Json;
using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.JSON;

public class PackageConfigurationConverter : JsonConverter<IPackageConfiguration>
{
    public override IPackageConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        var package = new PackageConfiguration();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return package;
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected PropertyName token");
            var propName = reader.GetString();
            reader.Read();
            switch (propName?.FirstCharToUpper())
            {
                case nameof(package.Url):
                    var str = reader.GetString();
                    package.Url = str != null ? new Uri(str) : null;
                    break;
                case nameof(package.Login):
                    package.Login = reader.GetString();
                    break;
                case nameof(package.Password):
                    package.Password = reader.GetString();
                    break;
                case nameof(package.Tenant):
                    package.Tenant = reader.GetString();
                    break;
                case nameof(package.PackageName):
                    package.PackageName = reader.GetString();
                    break;
                case nameof(package.PackageDirectory):
                    package.PackageDirectory = reader.GetString();
                    break;
            }
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, IPackageConfiguration value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}