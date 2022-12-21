namespace PKTickets.Models.DTO
{
    public class TheatersSchedulesDTO
    {
        public string TheaterName { get; set; }
        public int ScreensCount { get; set; }
        public List<ScreenSchedulesDTO> Screens { get; set; }
  

    }

  
    public class ScreenSchedulesDTO
    {
        public int ScreenId { get; set; }
        public string ScreenName { get; set; }
        public int PremiumCapacity { get; set; }
        public int EliteCapacity { get; set; }
        public List<SchedulesDTO> Schedules { get; set; }
    }

 }
