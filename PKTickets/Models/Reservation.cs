using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PKTickets.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

       

        [Required(ErrorMessage = "Please Enter the User Id !!!")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public User? User { get; set; }

       
        [Required(ErrorMessage = "Please Enter the Schedule Id !!!")]
        public int ScheduleId { get; set; }

        [ForeignKey("ScheduleId")]
        [JsonIgnore]
        public Schedule? Schedule { get; set; }


        [Required(ErrorMessage = "Please enter the PremiumTickets")]
        [Range(1, 10, ErrorMessage = "PremiumTickets seats must be between 1 to 10")]
        public int PremiumTickets { get; set; }
        [Required(ErrorMessage = "Please enter the EliteTickets")]
        [Range(1, 10, ErrorMessage = "EliteTickets seats must be between 1 to 10")]
        public int EliteTickets { get; set; }
        public bool IsActive { get; set; }=true;

    }
}