using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{//start of namespace

    [Table("USER")]
    public class User
    {//start of class

        [Key]
        public int user_id { get; set; }

        [Required]
        [StringLength(50)]
        public string first_name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string last_name { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string email { get; set; } = string.Empty;

        [StringLength(30)]
        public string? phone { get; set; }

        [Required]
        [StringLength(20)]
        public string role { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string password_hash { get; set; } = string.Empty;

        public ICollection<Event> EventsOrganised { get; set; }

            = new List<Event>();

        public ICollection<Enrollment> Enrollments { get; set; }

            = new List<Enrollment>();

    }//end of class
}//end of namespace
