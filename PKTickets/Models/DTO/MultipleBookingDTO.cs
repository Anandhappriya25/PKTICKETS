using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PKTickets.Models.DTO
{
    public class MultipleBookingDTO
    {
        public int UserId { get; set; }
        public List<Detail> MultipleReservations { get; set; }
        public MultipleBookingDTO()
        {
            this.MultipleReservations = new List<Detail>();
        }
    }
    public class Detail
    {
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "Please enter the PremiumTickets")]
        [Range(1, 10, ErrorMessage = "PremiumTickets seats must be between 1 to 10")]
        public int PremiumTickets { get; set; }

        [Required(ErrorMessage = "Please enter the EliteTickets")]
        [Range(1, 10, ErrorMessage = "EliteTickets seats must be between 1 to 10")]
        public int EliteTickets { get; set; }

    }
    public class Result
    {
        public List<Output> Messages { get; set; }
        public  Result()
        {
            this.Messages=new List<Output>();
        }

    }
    public class Output
    {
        public string Message { get; set; }
    }
  }
