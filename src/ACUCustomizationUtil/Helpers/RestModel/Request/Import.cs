using System.Text.Json.Serialization;

namespace ACUCustomizationUtils.Helpers.RestModel.Request
{
    internal class Import
    {
        [JsonPropertyName("projectLevel")]
        public required int ProjectLevel { get; init; }

        [JsonPropertyName("isReplaceIfExists")]
        public required bool IsReplaceIfExists { get; init; }

        [JsonPropertyName("projectName")]
        public required string ProjectName { get; init; }

        [JsonPropertyName("projectDescription")]
        public string? ProjectDescription { get; init; }

        [JsonPropertyName("projectContentBase64")]
        public required string ProjectContentBase64 { get; init; }
    }
}
