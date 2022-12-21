namespace PKTickets.Models.DTO
{
    public class MovieDTO
    {
        public string MovieName { get; set; }
        public List<TheaterDetails> Theaters { get; set; }
        public MovieDTO()
        {
            this.Theaters = new List<TheaterDetails>();
        }
    }
    public class TheaterDetails
    {
        public int TheaterId { get; set; }
        public string TheaterName { get; set; }
        public List<ScreenDetails> Screens { get; set; }
        public TheaterDetails()
        {
            this.Screens = new List<ScreenDetails>();
        }
    }
    public class ScreenDetails
    {
        public int ScreenId { get; set; }
        public string ScreenName { get; set; }
        public int PremiumCapacity { get; set; }
        public int EliteCapacity { get; set; }
        public  List<ScheduleDetails> Schedules { get; set; }
        public ScreenDetails()
        {
            this.Schedules = new List<ScheduleDetails>();
        }
    }
    public class ScheduleDetails
    {
        public int ScheduleId { get; set; }
        public DateTime Date { get; set; }
        public string ShowTime { get; set; }
        public int AvailablePremiumSeats { get; set; }
        public int AvailableEliteSeats { get; set; }
       
    }
}
