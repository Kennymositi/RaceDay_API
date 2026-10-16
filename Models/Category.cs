using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{

    [Table("CATEGORY")]
    public class Category
    {

        [Key]
        public int category_id { get; set; }

        [Required]
        public int event_id { get; set; }

        [Required]
        [StringLength(50)]
        public string category_name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string category_type { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string category_value { get; set; } = string.Empty;

        public string? description { get; set; }

        [ForeignKey(nameof(event_id))]
        public Event? Event { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
            = new List<Enrollment>();

    }
}
