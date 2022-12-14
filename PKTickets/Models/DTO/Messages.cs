namespace PKTickets.Models.DTO
{
    public class Messages
    {
        public string Message { get; set; }
        public Statuses Status { get; set; }  

        public bool Success { get; set; }

    }
    public enum Statuses
    {
        Success,
        BadRequest,
        NotFound,
        Conflict
    }
}
