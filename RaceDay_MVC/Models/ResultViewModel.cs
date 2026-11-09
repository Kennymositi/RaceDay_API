using System.Text.Json.Serialization;
namespace RaceDay_MVC.Models
{
    public class ResultViewModel
    {
        [JsonPropertyName("result_id")]
        public int ResultId { get; set; }
        [JsonPropertyName("enrollment_id")]
        public int EnrollmentId { get; set; }
        [JsonPropertyName("finishing_position")]
        public int FinishingPosition { get; set; }
        [JsonPropertyName("finish_time")]
        public TimeSpan? FinishTime { get; set; }
    }
}