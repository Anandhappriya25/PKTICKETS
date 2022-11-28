using Microsoft.AspNetCore.Mvc.Rendering;

namespace PKTickets.Models.DTO
{
    public class ScreensDTO
    {
       
        public int ScreenId { get; set; }
        public string ScreenName { get; set; }
        public int PremiumCapacity { get; set; }
        public int EliteCapacity { get; set; }
        public int PremiumPrice { get; set; }
        public int ElitePrice { get; set; }



    }
    public class ScreensListDTO
    {
        public string TheaterName { get; set; }
        public int ScreensCount { get; set; }
        public List<ScreensDTO> Screens { get; set; }
    }
}
