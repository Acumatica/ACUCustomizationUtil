using System.Text.Json.Serialization;
 
namespace ACUCustomizationUtils.Helpers.RestModel.Request
{
    internal class Login
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("password")]
        public required string Password { get; init; }

        [JsonPropertyName("tenant")]
        public string? Tenant { get; init; }

        [JsonPropertyName("branch")]
        public string? Branch { get; init; }

        [JsonPropertyName("locale")]
        public string? Locale { get; init; }
    }
}
