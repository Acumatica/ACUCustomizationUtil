using System.Text.Json.Serialization;

namespace ACUCustomizationUtils.Helpers.RestModel.Request
{
    internal class GetProject
    {
        [JsonPropertyName("IsAutoResolveConflicts")]
        public required bool IsAutoResolveConflicts { get; init; }

        [JsonPropertyName("projectName")]
        public required string ProjectName { get; init; }
    }
}
