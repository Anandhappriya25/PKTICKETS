using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Xml.Linq;

namespace PKTickets.Repository
{
    public class ScreenRepository : IScreenRepository
    {
        private readonly PKTicketsDbContext db;
        public ScreenRepository(PKTicketsDbContext db)
        {
            this.db = db;
        }

        public List<Screen> GetAllScreens()
        {
            return db.Screens.Where(x => x.IsActive).ToList();
        }

        public Screen ScreenById(int id)
        {
            var screen = db.Screens.Where(x => x.IsActive).FirstOrDefault(x => x.ScreenId == id);
            return screen;
        }


        public Messages AddScreen(Screen screen)
        {
            if (screen.TheaterId == 0)
            {
                return BadRequest.MSG("Enter the Theater Id field");
            }
            var theaterExist = db.Theaters.Where(x => x.IsActive).FirstOrDefault(x => x.TheaterId == screen.TheaterId);
            var screenExist = db.Screens.Where(x => x.IsActive).Where(x => x.TheaterId == screen.TheaterId).
                FirstOrDefault(x => x.ScreenName == screen.ScreenName);
            return (theaterExist == null) ? TheaterNotFound(screen.TheaterId)
               : (screenExist != null) ? NameConflict(screen.ScreenName)
             : Create(screen);
        }

        public Messages UpdateScreen(Screen screen)
        {
            if (screen.ScreenId == 0)
            {
                return BadRequest.MSG("Enter the Screen Id field");
            }
            var ScreenExist = ScreenById(screen.ScreenId);
            var nameExist = db.Screens.Where(x => x.IsActive).Where(x => x.TheaterId == screen.TheaterId).
         FirstOrDefault(x => x.ScreenName == screen.ScreenName);
            return (ScreenExist == null) ? ScreenNotFound(screen.ScreenId)
               : (nameExist != null && nameExist.ScreenId != screen.ScreenId) ? NameConflict(screen.ScreenName)
             : Update(screen, ScreenExist);
        }
        public Messages RemoveScreen(int screenId)
        {
            var screenExist = ScreenById(screenId);
            var screen = db.Schedules.Where(x => x.ScreenId == screenId).FirstOrDefault();
            return (screenExist == null) ? ScreenNotFound(screenId)
              : (screen != null) ? ScheduleConflict(screenExist.ScreenName)
            : Remove(screenExist);
        }


        #region
        private Messages messages = new Messages() { Status = Statuses.Conflict, Success = false };
        private Messages TheaterNotFound(int id)
        {
            messages.Message = $"Theater Id{id} is Not Registered.";
            messages.Status = Statuses.NotFound;
            return messages;
        }
        private Messages ScreenNotFound(int id)
        {
            messages.Message = $"Screen Id {id} is not found";
            messages.Status = Statuses.NotFound;
            return messages;
        }
        private Messages ScheduleConflict(string name)
        {
            messages.Message = $"This Screen {name} is Already scheduled, so you can't delete the screen";
            return messages;
        }
        private Messages NameConflict(string name)
        {
            messages.Message = $"Screen Name {name} is Already Registered.";
            return messages;
        }
        private Messages Create(Screen screen)
        {
            db.Screens.Add(screen);
            db.SaveChanges();
            messages.Success = true;
            messages.Message = $"Screen {screen.ScreenName} is succssfully Added";
            messages.Status = Statuses.Created;
            return messages;
        }
        private Messages Update(Screen screen, Screen ScreenExist)
        {
            ScreenExist.ScreenName = screen.ScreenName;
            ScreenExist.ElitePrice = screen.ElitePrice;
            ScreenExist.PremiumPrice = screen.PremiumPrice;
            db.SaveChanges();
            messages.Success = true;
            messages.Status = Statuses.Success;
            messages.Message = $"Screen {screen.ScreenName} is succssfully Updated";
            return messages;
        }
        private Messages Remove(Screen screenExist)
        {
            screenExist.IsActive = false;
            db.SaveChanges();
            messages.Success = true;
            messages.Status = Statuses.Success;
            messages.Message = $"Screen {screenExist.ScreenName} is succssfully Removed";
            return messages;
        }
        #endregion


    }
}
