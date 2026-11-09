using System.Text.Json.Serialization;

namespace RaceDay_MVC.Models
{
    public class CategoryViewModel
    {
        [JsonPropertyName("category_id")]
        public int CategoryId { get; set; }

        [JsonPropertyName("event_id")]
        public int EventId { get; set; }

        [JsonPropertyName("category_name")]
        public string CategoryName { get; set; } = "";

        [JsonPropertyName("category_type")]
        public string CategoryType { get; set; } = "";

        [JsonPropertyName("category_value")]
        public string CategoryValue { get; set; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}