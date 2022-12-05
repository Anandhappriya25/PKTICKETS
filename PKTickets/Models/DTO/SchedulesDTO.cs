namespace PKTickets.Models.DTO
{
    public class SchedulesDTO
    {
        public int ScheduleId { get; set; }
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public DateTime Date { get; set; }
        public string ShowTime { get; set; }
       
        public int AvailablePremiumSeats { get; set; }
        public int AvailableEliteSeats { get; set; }
        public List<ShowTime> ShowTimes { get; set; }
    }
    public class SchedulesListDTO
    {
        public string TheaterName { get; set; }
        public string ScreenName { get; set; }
        public int PremiumCapacity { get; set; }
        public int EliteCapacity { get; set; }
        public List<SchedulesDTO> Schedules { get; set; }

    }
}
