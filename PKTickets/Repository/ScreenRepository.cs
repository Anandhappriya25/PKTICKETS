using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

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
                return Request.Bad("Enter the Theater Id field");
            }
            var theaterExist = db.Theaters.Where(x => x.IsActive).FirstOrDefault(x => x.TheaterId == screen.TheaterId);
            var screenExist = db.Screens.Where(x => x.IsActive).Where(x => x.TheaterId == screen.TheaterId).
                FirstOrDefault(x => x.ScreenName == screen.ScreenName);
            return (theaterExist == null) ? Request.Not($"Theater Id{screen.TheaterId} is Not Registered.")
               : (screenExist != null) ? Request.Conflict($"Screen Name {screen.ScreenName} is Already Registered.")
             : Create(screen);
        }

        public Messages UpdateScreen(Screen screen)
        {
            if (screen.ScreenId == 0)
            {
                return Request.Bad("Enter the Screen Id field");
            }
            var ScreenExist = ScreenById(screen.ScreenId);
            var nameExist = db.Screens.Where(x => x.IsActive).Where(x => x.TheaterId == screen.TheaterId).
         FirstOrDefault(x => x.ScreenName == screen.ScreenName);
            return (ScreenExist == null) ? Request.Not($"Screen Id {screen.ScreenId} is not found")
               : (nameExist != null && nameExist.ScreenId != screen.ScreenId) ? Request.Conflict($"Screen Name {screen.ScreenName} is Already Registered.")
             : Update(screen, ScreenExist);
        }
        public Messages RemoveScreen(int screenId)
        {
            var screenExist = ScreenById(screenId);
            var screen = db.Schedules.Where(x => x.ScreenId == screenId).FirstOrDefault();
            return (screenExist == null) ? Request.Not($"Screen Id {screenId} is not found")
              : (screen != null) ? Request.Conflict($"This Screen {screenExist.ScreenName} is Already scheduled, so you can't delete the screen")
            : Remove(screenExist);
        }


        #region
        private Messages messages = new Messages() { Success = true };
        private Messages Create(Screen screen)
        {
            db.Screens.Add(screen);
            db.SaveChanges();
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
            messages.Status = Statuses.Success;
            messages.Message = $"Screen {screen.ScreenName} is succssfully Updated";
            return messages;
        }
        private Messages Remove(Screen screenExist)
        {
            screenExist.IsActive = false;
            db.SaveChanges();
            messages.Status = Statuses.Success;
            messages.Message = $"Screen {screenExist.ScreenName} is succssfully Removed";
            return messages;
        }
        #endregion


    }
}
