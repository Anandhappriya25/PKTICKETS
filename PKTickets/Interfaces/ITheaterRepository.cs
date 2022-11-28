using PKTickets.Models;
using PKTickets.Models.DTO;

namespace PKTickets.Interfaces
{
    public interface ITheaterRepository
    {
        public List<Theater> GetTheaters();
        public List<Theater> TheaterByLocation(string location);
        public Theater TheaterById(int id);
        public Theater TheaterByName(string name);
        public Messages DeleteTheater(int theaterId);
        public Messages CreateTheater(Theater theater);
        public Messages UpdateTheater(Theater theater);
    }
}
