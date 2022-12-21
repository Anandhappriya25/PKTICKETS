namespace PKTickets.Models.DTO
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public List<ReservationDetails> ReservationDetail { get; set; }
    }
    public class ReservationDetails
    {
        public int ReservationId { get; set; }
        public string TheaterName { get; set; }
        public string ScreenName { get; set; }
        public string MovieName { get; set; }
        public DateTime Date { get; set; }
        public string ShowTime { get; set; }
        public int PremiumTickets { get; set; }
        public int EliteTickets { get; set; }
        public int PremiumPrice { get; set; }
        public int ElitePrice { get; set; }
        public int TotalAmount { get; set; }
    }
}
