using PKTickets.Models;
using PKTickets.Models.DTO;

namespace PKTickets.Interfaces
{
    public interface IScreenRepository
    {
        public List<Screen> GetAllScreens();
        public Screen ScreenById(int id);
        public Messages AddScreen(Screen screen);
        public Messages UpdateScreen(Screen screen);
        public Messages RemoveScreen(int screenId);
        public ScreensListDTO TheaterScreens(int id);
        public Theater TheaterById(int id);
    }
}
