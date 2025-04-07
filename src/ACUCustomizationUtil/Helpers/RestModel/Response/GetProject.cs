using System.Text.Json.Serialization;

namespace ACUCustomizationUtils.Helpers.RestModel.Response
{
    public class GetProject : Logs
    {
        [JsonPropertyName("projectContentBase64")]
        public required string ProjectContentBase64 { get; init; }

        [JsonPropertyName("hasConflicts")]
        public required bool HasConflicts { get; init; }
    }
}
