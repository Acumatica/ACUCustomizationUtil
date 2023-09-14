using System.Text.Json;
using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.Src;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.JSON;

public class SrcConfigurationConverter : JsonConverter<ISrcConfiguration>
{
    public override ISrcConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        var code = new SrcConfiguration();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return code;
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected PropertyName token");
            var propName = reader.GetString();
            _ = reader.Read();
            switch (propName?.FirstCharToUpper())
            {
                case nameof(code.PkgSourceDirectory):
                    code.PkgSourceDirectory = reader.GetString().NormalizeEnvVariables();
                    break;
                case nameof(code.PkgDescription):
                    code.PkgDescription = reader.GetString();
                    break;
                case nameof(code.PkgLevel):
                    code.PkgLevel = reader.GetString();
                    break;
                case nameof(code.MsBuildSolutionFile):
                    code.MsBuildSolutionFile = reader.GetString().NormalizeEnvVariables();
                    break;
                case nameof(code.MsBuildTargetDirectory):
                    code.MsBuildTargetDirectory = reader.GetString().NormalizeEnvVariables();
                    break;
                case nameof(code.MsBuildAssemblyName):
                    code.MsBuildAssemblyName = reader.GetString();
                    break;
                case nameof(code.MakeMode):
                    code.MakeMode = reader.GetString();
                    break;
            }
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, ISrcConfiguration value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}