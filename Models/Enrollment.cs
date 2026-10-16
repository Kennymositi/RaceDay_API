using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{

    [Table("ENROLLMENT")]
    public class Enrollment
    {

        [Key]
        public int enrollment_id { get; set; }

        [Required]
        public int participant_id { get; set; }

        [Required]
        public int event_id { get; set; }

        [Required]
        public int category_id { get; set; }

        [Required]
        public DateTime enrollment_date { get; set; }

        [StringLength(20)]
        public string status { get; set; } = "ENTERED";

        [ForeignKey(nameof(participant_id))]
        public User? User { get; set; }

        [ForeignKey(nameof(event_id))]
        public Event? Event { get; set; }

        [ForeignKey(nameof(category_id))]
        public Category? Category { get; set; }

        public Result? Result { get; set; }

    }
}
