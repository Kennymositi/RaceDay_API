using System.ComponentModel.DataAnnotations;

namespace RaceDay_MVC.Models
{
    public class RecordResultViewModel
    {
        [Required]
        public int EnrollmentId { get; set; }

        [Required(ErrorMessage = "Please enter the finishing time.")]
        public string FinishTime { get; set; } = "";

        [Range(1, int.MaxValue,
            ErrorMessage = "Finishing position must be at least 1.")]
        public int FinishingPosition { get; set; }
    }
}