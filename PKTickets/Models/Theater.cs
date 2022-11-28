using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PKTickets.Models
{
    public class Theater
    {
        [Key]
        public int TheaterId { get;set; }

        [Required(ErrorMessage = "Please Enter the Theater Name !!!")]
        [StringLength(20, ErrorMessage = "Theater Name length must be below 20 characters")]
        public string TheaterName { get;set; }

        [Required(ErrorMessage = "Please Enter the Location !!!")]
        [StringLength(20, ErrorMessage = "Location length must be below 20 characters")]
        public string Location { get;set; } 
        public bool IsActive { get; set; } = true;
    }
}
