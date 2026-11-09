using System.Text.Json.Serialization;
namespace RaceDay_MVC.Models
{
    public class EnrollmentViewModel
    {
        [JsonPropertyName("enrollment_id")]
        public int EnrollmentId { get; set; }
        [JsonPropertyName("participant_id")]
        public int ParticipantId { get; set; }
        [JsonPropertyName("event_id")]
        public int EventId { get; set; }
        [JsonPropertyName("category_id")]
        public int CategoryId { get; set; }
        [JsonPropertyName("enrollment_date")]
        public DateTime EnrollmentDate { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; } = "";
    }
}