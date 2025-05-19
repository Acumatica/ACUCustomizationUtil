using System.Text.Json;
using System.Text.Json.Serialization;

using ACUCustomizationUtils.Configuration.ACU;
using ACUCustomizationUtils.Configuration.Erp;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.JSON;

public class AcuConfigurationConverter : JsonConverter<IAcuConfiguration>
{
    public override IAcuConfiguration Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        AcuConfiguration configuration = new AcuConfiguration();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                configuration.OnDeserialized();
                return configuration;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName token");

            string? propertyName = reader.GetString()?.FirstCharToUpper();
            reader.Read();

            switch (propertyName)
            {
                case nameof(configuration.Erp):
                    ErpConfigurationConverter converter = new ErpConfigurationConverter();
                    IErpConfiguration erp = converter.Read(ref reader, typeof(IErpConfiguration), options);
                    configuration.Erp.CopyValues(erp);
                    break;

                case nameof(configuration.Site):
                    SiteConfigurationConverter siteConverter = new SiteConfigurationConverter();
                    Configuration.Site.ISiteConfiguration site = siteConverter.Read(ref reader, typeof(IErpConfiguration), options);
                    configuration.Site.CopyValues(site);
                    break;

                case nameof(configuration.Pkg):
                    PackageConfigurationConverter packageConverter = new PackageConfigurationConverter();
                    Configuration.Package.IPackageConfiguration package = packageConverter.Read(
                        ref reader,
                        typeof(IErpConfiguration),
                        options
                    );
                    configuration.Pkg.CopyValues(package);
                    break;

                case nameof(configuration.Src):
                    SrcConfigurationConverter projectConverter = new SrcConfigurationConverter();
                    Configuration.Src.ISrcConfiguration project = projectConverter.Read(
                        ref reader,
                        typeof(IErpConfiguration),
                        options
                    );
                    configuration.Src.CopyValues(project);
                    break;
            }
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(
        Utf8JsonWriter writer,
        IAcuConfiguration value,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }
}
