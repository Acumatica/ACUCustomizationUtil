using System.Text.Json.Serialization;

namespace ACUCustomizationUtils.Helpers.RestModel.Request
{
    internal class UnpublishAll
    {
        [JsonPropertyName("tenantMode")]
        public required string TenantMode { get; init; }

        [JsonPropertyName("tenantLoginNames")]
        public string[]? TenantLoginNames { get; init; }
    }
}
