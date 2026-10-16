using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{//start of namespace

    [Table("EVENTTYPE")]
    public class Event_Type
    {//start of class

        [Key]
        public int event_type_id { get; set; }

        [Required]
        [StringLength(30)]
        public string type_name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? description { get; set; }

        public ICollection<Event> Events { get; set; }

            = new List<Event>();

    }//end of class
}//end of namespace
