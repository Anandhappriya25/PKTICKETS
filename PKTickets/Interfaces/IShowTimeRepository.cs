using PKTickets.Models;
using PKTickets.Models.DTO;

namespace PKTickets.Interfaces
{
    public interface IShowTimeRepository
    {
        public ShowTimeDTO ShowTimeasStringById(int id);
        public List<ShowTimeDTO> GetAllShowTimes();
        public ShowTime TimeById(int id);
        public ShowTime DetailsByTiming(int time);
        public Messages CreateShowTime(ShowTimeDTO showTimeDTO);
        public Messages UpdateShowTime(ShowTimeDTO showTimeDTO);

    }
}
