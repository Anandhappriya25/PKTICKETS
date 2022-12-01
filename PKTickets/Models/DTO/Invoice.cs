namespace PKTickets.Models.DTO
{
    public class Invoice
    {
        public int ReservationId { get; set; }
        public string UserName { get; set; }
        public string TheaterName { get; set; }
        public string ScreenName { get; set; }
        public string MovieName { get; set; }
        public string Language { get; set; }
        public DateTime Date { get; set; }
        public string ShowTime { get; set; }
        public int PremiumTicket { get; set; }
        public int EliteTicket { get; set; }
        public int PremiumPrice { get; set; }
        public int ElitePrice { get; set; }
        public int TotalAmount { get; set; }

    }
}
