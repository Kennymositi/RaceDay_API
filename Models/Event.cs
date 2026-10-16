using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{//start of namespace

    [Table("EVENT")]
    public class Event
    {//start of class

        [Key]
        public int event_id { get; set; }

        [Required]
        public int organiser_id { get; set; }

        [Required]
        public int event_type_id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? description { get; set; }

        [Required]
        public DateTime event_date { get; set; }

        [Required]
        [StringLength(200)]
        public string location { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal distance { get; set; }

        public ICollection<Category> Categories { get; set; }

            = new List<Category>();

        public ICollection<Enrollment> Enrollments { get; set; }

            = new List<Enrollment>();

    }//end of class
}//end of namespace
