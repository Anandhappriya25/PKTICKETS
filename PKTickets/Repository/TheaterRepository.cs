using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace PKTickets.Repository
{
    public class TheaterRepository : ITheaterRepository
    {
        private readonly PKTicketsDbContext db;
        public TheaterRepository(PKTicketsDbContext db)
        {
            this.db = db;
        }

        public List<Theater> GetTheaters()
        {
            return db.Theaters.Where(x => x.IsActive).ToList();
        }

        public List<Theater> TheaterByLocation(string location)
        {
            var theaters = db.Theaters.Where(x => x.IsActive).Where(x => x.Location == location).ToList();
            return theaters;
        }

        public Theater TheaterById(int id)
        {
            var theater = db.Theaters.Where(x => x.IsActive).FirstOrDefault(x => x.TheaterId == id);
            return theater;
        }
        public Theater TheaterByName(string name)
        {
            var theater = db.Theaters.Where(x => x.IsActive).FirstOrDefault(x => x.TheaterName == name);
            return theater;
        }


        public Messages DeleteTheater(int theaterId)
        {
            var theater = TheaterById(theaterId);
            var theaters = db.Screens.Where(x => x.TheaterId == theaterId).Where(x=>x.IsActive).FirstOrDefault();
            return (theater == null) ? TheaterNotFound(theaterId)
               : (theaters != null) ? ScheduleConflict(theater.TheaterName)
               : Delete(theater);
        }

        public Messages CreateTheater(Theater theater)
        {
            var theaterExist = db.Theaters.FirstOrDefault(x => x.TheaterName == theater.TheaterName);
            return (theaterExist != null) ? NameConflict(theater.TheaterName)
              : Create(theater);
        }

        public Messages UpdateTheater(Theater theater)
        {
            if (theater.TheaterId == 0)
            {
                return BadRequest.MSG("Enter the Theater Id field");
            }
            var theaterExist = TheaterById(theater.TheaterId);
            var nameExist = db.Theaters.FirstOrDefault(x => x.TheaterName == theater.TheaterName);
            return (theaterExist == null) ? TheaterNotFound(theater.TheaterId)
                : (nameExist != null && nameExist.TheaterId != theaterExist.TheaterId) ? NameConflict(theater.TheaterName)
              : Update(theater,theaterExist);
        }

        public ScreensListDTO TheaterScreens(int id)
        {
            var theater = db.Theaters.Where(x => x.IsActive).FirstOrDefault(x => x.TheaterId == id);
            var screens = db.Screens.Where(x => x.IsActive).Where(x => x.TheaterId == id).ToList();
            ScreensListDTO list = new ScreensListDTO();
            if (theater == null)
            {
                return list;
            }
            list.TheaterName = theater.TheaterName;
            list.ScreensCount = screens.Count();
            list.Screens = Screens(id);
            return list;
        }

        public TheatersSchedulesDTO TheaterSchedulesById(int id)
        {
            var theater = db.Theaters.FirstOrDefault(x => x.TheaterId == id);
            var screens = db.Screens.Where(x => x.IsActive).Where(x => x.TheaterId == id).ToList();

            TheatersSchedulesDTO list = new TheatersSchedulesDTO();
            if (theater == null)
            {
                return list;
            }
            list.TheaterName = theater.TheaterName;
            list.ScreensCount = screens.Count();

            List<ScreenSchedulesDTO> schedules = new List<ScreenSchedulesDTO>();
            foreach (var screen in screens)
            {
                ScreenSchedulesDTO scheduleList = new ScreenSchedulesDTO();
                scheduleList.ScreenId = screen.ScreenId;
                scheduleList.ScreenName = screen.ScreenName;
                scheduleList.PremiumCapacity = screen.PremiumCapacity;
                scheduleList.EliteCapacity = screen.EliteCapacity;
                scheduleList.Schedules = SchedulesListScreenId(screen.ScreenId);
                schedules.Add(scheduleList);
            }
            list.Screens = schedules;
            return list;
        }

        #region PrivateMethods
        private Messages messages = new Messages() { Status = Statuses.Conflict, Success = false };
        private Messages TheaterNotFound(int id)
        {
            messages.Message = $"Theater Id {id} is not found";
            messages.Status = Statuses.NotFound;
            return messages;
        }
        private Messages ScheduleConflict(string name)
        {
            messages.Message = $"This Theater {name} is Already scheduled, so you can't delete the theater";
            return messages;
        }
        private Messages NameConflict(string name)
        {
            messages.Message = $"Theater Name {name} is already Registered.";
            return messages;
        }
        private Messages Delete(Theater theater)
        {
            theater.IsActive = false;
            db.SaveChanges();
            messages.Success = true;
            messages.Message = $"Theater {theater.TheaterName} is Successfully Removed";
            messages.Status = Statuses.Success;
            return messages;
        }
        private Messages Create(Theater theater)
        {
            db.Theaters.Add(theater);
            db.SaveChanges();
            messages.Success = true;
            messages.Message = $"Theater {theater.TheaterName} is Successfully Added";
            messages.Status = Statuses.Created;
            return messages;
        }
        private Messages Update(Theater theater,Theater theaterExist)
        {
            theaterExist.TheaterName = theater.TheaterName;
            theaterExist.Location = theater.Location;
            db.SaveChanges();
            messages.Success = true;
            messages.Message = $"Theater {theater.TheaterName} is Successfully Updated";
            messages.Status = Statuses.Success;
            return messages;
        }
        private List<SchedulesDTO> SchedulesListScreenId(int id)
        {
            DateTime date = DateTime.Now;
            var timeValue = TimesValue(date);
            var screens = (from screen in db.Screens
                           join schedule in db.Schedules on screen.ScreenId equals schedule.ScreenId
                           join showTime in db.ShowTimes on schedule.ShowTimeId equals showTime.ShowTimeId
                           join movie in db.Movies on schedule.MovieId equals movie.MovieId
                           where screen.ScreenId == id && schedule.IsActive && ((schedule.Date == date.Date && showTime.ShowTiming > timeValue) || (schedule.Date > date.Date))
                           select new SchedulesDTO()
                           {
                               ScheduleId = schedule.ScheduleId,
                               MovieName = movie.Title,
                               Date = schedule.Date,
                               ShowTime = TimingConvert.ConvertToString(showTime.ShowTiming),
                               AvailablePremiumSeats = schedule.AvailablePreSeats,
                               AvailableEliteSeats = schedule.AvailableEliSeats
                           }).ToList();

            return screens;
        }
        private List<ScreensDTO> Screens(int id)
        {
            var screens = (from theater in db.Theaters
                           join screen in db.Screens on theater.TheaterId equals screen.TheaterId
                           where theater.TheaterId == id && screen.IsActive
                           select new ScreensDTO()
                           {
                               ScreenId = screen.ScreenId,
                               ScreenName = screen.ScreenName,
                               PremiumCapacity = screen.PremiumCapacity,
                               EliteCapacity = screen.EliteCapacity,
                               PremiumPrice = screen.PremiumPrice,
                               ElitePrice = screen.ElitePrice,
                           }).ToList();

            return screens;
        }

        private int TimesValue(DateTime date)
        {
            TimeSpan time = new TimeSpan(date.Hour, date.Minute, 0);
            return (TimingConvert.ConvertToInt(Convert.ToString(time)));
        }
        #endregion

    }
}
