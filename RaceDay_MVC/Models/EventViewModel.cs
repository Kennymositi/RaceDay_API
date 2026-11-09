using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RaceDay_MVC.Models
{
    public class EventViewModel
    {
        [JsonPropertyName("event_id")]
        public int EventId { get; set; }

        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("event_date")]
        [Required]
        public DateTime EventDate { get; set; }

        [JsonPropertyName("location")]
        [Required]
        public string Location { get; set; } = "";

        [JsonPropertyName("distance")]
        [Range(0.01, 9999.99)]
        public decimal Distance { get; set; }

        [JsonPropertyName("event_type_id")]
        [Range(1, int.MaxValue)]
        public int EventTypeId { get; set; }

        [JsonPropertyName("organiser_id")]
        public int OrganiserId { get; set; }
    }

    public class EventTypeViewModel
    {
        [JsonPropertyName("event_type_id")]
        public int EventTypeId { get; set; }

        [JsonPropertyName("type_name")]
        public string TypeName { get; set; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}