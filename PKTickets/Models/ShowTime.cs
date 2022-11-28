using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PKTickets.Models
{
    public class ShowTime
    {
        [Key]
        public int ShowTimeId { get; set; }
        [Required(ErrorMessage = "Please enter the ShowTiming")]
        public int ShowTiming { get; set; }

        
    }
}
