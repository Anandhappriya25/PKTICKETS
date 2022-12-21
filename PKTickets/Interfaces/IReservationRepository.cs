using PKTickets.Models;
using PKTickets.Models.DTO;

namespace PKTickets.Interfaces
{
    public interface IReservationRepository
    {

        public List<Reservation> ReservationList();
        public Reservation ReservationById(int id);
        public Messages DeleteReservation(int id);
        public Messages CreateReservation(Reservation reservation);
        public Messages UpdateReservation(Reservation reservation);
        public List<Reservation> ReservationsByShowId(int id);
        public UserDTO ReservationsByUserId(int id);
        public User UserById(int id);
        public Schedule ScheduleById(int id);

        public List<ReservationDTO> ListOfReservations();
        public Invoice InvoiceById(int id);
    }
}
