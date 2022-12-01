namespace PKTickets.Models.DTO
{
    public class ReservationDTO
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string MovieName { get; set; }
        public string TheaterName { get; set; }
        public DateTime Date { get; set; }
        public string ShowTiming { get; set; }
        public int Tickets { get; set; }
    }
}
