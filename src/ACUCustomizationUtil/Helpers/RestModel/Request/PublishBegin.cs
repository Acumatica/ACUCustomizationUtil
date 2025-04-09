using System.Text.Json.Serialization;

namespace ACUCustomizationUtils.Helpers.RestModel.Request
{
    internal class PublishBegin
    {
        [JsonPropertyName("isMergeWithExistingPackages")]
        public required bool IsMergeWithExistingPackages { get; init; }

        [JsonPropertyName("isOnlyValidation")]
        public required bool IsOnlyValidation { get; init; }

        [JsonPropertyName("isOnlyDbUpdates")]
        public required bool IsOnlyDbUpdates { get; init; }

        [JsonPropertyName("isReplayPreviouslyExecutedScripts")]
        public required bool IsReplayPreviouslyExecutedScripts { get; init; }

        [JsonPropertyName("projectNames")]
        public required string[] ProjectNames { get; init; }

        [JsonPropertyName("tenantMode")]
        public required string TenantMode { get; init; }

        [JsonPropertyName("tenantLoginNames")]
        public string[]? TenantLoginNames { get; init; }
    }
}
