using Microsoft.AspNetCore.Mvc.Rendering;

namespace PKTickets.Models.DTO
{
    public class BookTicketDTO
    {
        public int ReservationId { get; set; }
         public List<SelectListItem> Users { get; set; }
        public int UserId { get; set; }
        public int ScheduleId { get; set; }
        public int EliteTickets { get; set; }
        public int PremiumTickets { get; set; }
    }
 
}
