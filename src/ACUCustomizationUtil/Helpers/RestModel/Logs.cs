using System.Text.Json.Serialization;

namespace ACUCustomizationUtils.Helpers.RestModel
{
    public class Logs
    {
        [JsonPropertyName("log")]
        public Log[]? Log { get; init; }
    }

    public class Log
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; init; }

        [JsonPropertyName("logType")]
        public string? LogType { get; init; }

        [JsonPropertyName("message")]
        public string? Message { get; init; }
    }
}
