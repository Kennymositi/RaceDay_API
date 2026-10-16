using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{

    [Table("RESULT")]
    public class Result
    {

        [Key]
        public int result_id { get; set; }

        [Required]
        public int enrollment_id { get; set; }

        [Required]
        public TimeSpan finish_time { get; set; }

        [Required]
        public int finishing_position { get; set; }

        [ForeignKey(nameof(enrollment_id))]
        public Enrollment? Enrollment { get; set; }
    }
}
