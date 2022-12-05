using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PKTickets.Models.DTO
{
    public class CreateSchedule
    {
        public int ScheduleId { get; set; }
        public int ScreenId { get; set; }
        public string Date { get; set; }
        public int ShowTimeId { get; set; }
        public List<SelectListItem> Times { get; set; }
        public int MovieId { get; set; }
        public List<SelectListItem> Movies { get; set; }
        public int PremiumSeats { get; set; }

        public int EliteSeats { get; set; }

        public int AvailablePreSeats { get; set; }

        public int AvailableEliSeats { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
