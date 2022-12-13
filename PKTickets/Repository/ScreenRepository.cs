using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
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
            return db.Screens.Where(x => x.IsActive == true).ToList();
        }

        public Screen ScreenById(int id)
        {
            var screen = db.Screens.Where(x => x.IsActive == true).FirstOrDefault(x => x.ScreenId == id);
            return screen;
        }


        public Messages AddScreen(Screen screen)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var theaterExist = db.Theaters.Where(x => x.IsActive == true).FirstOrDefault(x => x.TheaterId == screen.TheaterId);
           
            if (theaterExist == null)
            {
                messages.Message = "Theater Id("+screen.TheaterId+") is Not Registered.";
                return messages;
            }
            var screenExist = db.Screens.Where(x => x.IsActive == true).Where(x => x.TheaterId == screen.TheaterId).
                FirstOrDefault(x => x.ScreenName == screen.ScreenName);
            if(screenExist != null)
            {
                messages.Message = "Screen Name(" + screen.ScreenName + ") is Already Registered.";
                return messages;
            }
            else
            {
                db.Screens.Add(screen);
                db.SaveChanges();
                messages.Success = true;
                messages.Message = "Screen ("+screen.ScreenName+") is succssfully Added";
                return messages;
            }
        }

        public Messages UpdateScreen(Screen screen)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var ScreenExist = ScreenById(screen.ScreenId);
            if (ScreenExist == null)
            {
                messages.Message = "Screen Id is not found";
                return messages;
            }
            var nameExist = db.Screens.Where(x => x.IsActive == true).Where(x => x.TheaterId == screen.TheaterId).
                FirstOrDefault(x => x.ScreenName == screen.ScreenName);
            if(nameExist!=null && nameExist.ScreenId != screen.ScreenId)
            {
                messages.Message = "Screen Name(" + screen.ScreenName + ") is Already Registered.";
                return messages;
            }
            else
            {
                ScreenExist.ScreenName = screen.ScreenName;
                ScreenExist.ElitePrice=screen.ElitePrice;
                ScreenExist.PremiumPrice = screen.PremiumPrice;
                db.SaveChanges();
                messages.Success = true;
                messages.Message = "Screen "+ screen.ScreenName + " is succssfully Updated";
                return messages;
            }
        }
        public Messages RemoveScreen(int screenId)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var screenExist = ScreenById(screenId);
            if (screenExist == null)
            {
                messages.Message = "Screen Id(" + screenId + ") is not found";
                return messages;
            }
            var screen = db.Schedules.Where(x => x.ScreenId == screenId).FirstOrDefault();
            if(screen != null)
            {
                messages.Message = "This Screen(" + screenExist.ScreenName+") is Already scheduled, so you can't delete the screen";
                return messages;
            }
            else
            {
                screenExist.IsActive = false;
                db.SaveChanges();
                messages.Success = true;
                messages.Message = "Screen ("+ screenExist .ScreenName+ ") is succssfully Removed";
                return messages;
            }
        }

        
       
     
     
    }
}
