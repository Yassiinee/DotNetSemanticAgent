using System.Text.Json.Serialization;

namespace DotNetSemanticAgent
{
    public class LightModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("is_on")]
        public bool? IsOn { get; set; }
    }
}
