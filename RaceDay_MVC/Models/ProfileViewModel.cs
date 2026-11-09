using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace RaceDay_MVC.Models
{
    public class ProfileViewModel
    {
        [JsonPropertyName("first_name")]
        [Required]
        public string FirstName { get; set; } = "";
        [JsonPropertyName("last_name")]
        [Required]
        public string LastName { get; set; } = "";
        [JsonPropertyName("email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }
    }
}