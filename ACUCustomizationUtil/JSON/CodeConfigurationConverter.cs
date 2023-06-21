using System.Text.Json;
using System.Text.Json.Serialization;
using ACUCustomizationUtils.Configuration.Code;
using ACUCustomizationUtils.Extensions;

namespace ACUCustomizationUtils.JSON;

public class CodeConfigurationConverter : JsonConverter<ICodeConfiguration>
{
    public override ICodeConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        var code = new CodeConfiguration();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return code;
            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected PropertyName token");
            var propName = reader.GetString();
            _ = reader.Read();
            switch (propName?.FirstCharToUpper())
            {
                case nameof(code.PkgSourceDirectory):
                    code.PkgSourceDirectory = reader.GetString();
                    break;
                case nameof(code.PkgDescription):
                    code.PkgDescription = reader.GetString();
                    break;
                case nameof(code.PkgLevel):
                    code.PkgLevel = reader.GetString();
                    break;
                case nameof(code.MsBuildSolutionFile):
                    code.MsBuildSolutionFile = reader.GetString();
                    break;
                case nameof(code.MsBuildTargetDirectory):
                    code.MsBuildTargetDirectory = reader.GetString();
                    break;
                case nameof(code.MakeQA):
                    var qaStr = reader.GetString();
                    _ = bool.TryParse(qaStr, out var qa);
                    code.MakeQA = qa;
                    break;
                case nameof(code.MakeISV):
                    var isvStr = reader.GetString();
                    _ = bool.TryParse(isvStr, out var isv);
                    code.MakeISV = isv;
                    break;
            }
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, ICodeConfiguration value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}