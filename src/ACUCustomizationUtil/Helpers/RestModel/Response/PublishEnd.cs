using System.Text.Json.Serialization;

namespace ACUCustomizationUtils.Helpers.RestModel.Response
{
    public class PublishEnd : Logs
    {
        [JsonPropertyName("isCompleted")]
        public required bool IsCompleted { get; init; }

        [JsonPropertyName("isFailed")]
        public required bool IsFailed { get; init; }
    }
}
