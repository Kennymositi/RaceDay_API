using System.Text.Json.Serialization;

namespace RaceDay_MVC.Models
{
    public class ApiAuthResponse
    {
        public string? Message { get; set; }
        public string? Token { get; set; }
        public ApiUser? User { get; set; }
    }

    public class ApiUser
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = "";

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = "";

        [JsonPropertyName("email")]
        public string Email { get; set; } = "";

        [JsonPropertyName("role")]
        public string Role { get; set; } = "";
    }
}