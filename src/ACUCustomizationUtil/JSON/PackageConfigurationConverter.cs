using ACUCustomizationUtils.Configuration.Package;
using ACUCustomizationUtils.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ACUCustomizationUtils.JSON;

public class PackageConfigurationConverter : JsonConverter<IPackageConfiguration>
{
    public override IPackageConfiguration Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        PackageConfiguration package = new PackageConfiguration();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return package;
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName token");
            string? propName = reader.GetString();
            reader.Read();
            switch (propName?.FirstCharToUpper())
            {
                case nameof(package.Url):
                    string? rest = reader.GetString();
                    package.Url = rest != null ? new Uri(rest.EnsureTrailingSlash()) : null;
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
                case nameof(package.Branch):
                    package.Branch = reader.GetString();
                    break;
                case nameof(package.PkgName):
                    package.PkgName = reader.GetString();
                    break;
                case nameof(package.PkgDirectory):
                    package.PkgDirectory = reader.GetString().NormalizeEnvVariables();
                    break;
                case nameof(package.PkgSuffix):
                    package.PkgSuffix = reader.GetString().NormalizeEnvVariables();
                    break;
            }
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(
        Utf8JsonWriter writer,
        IPackageConfiguration value,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }
}
