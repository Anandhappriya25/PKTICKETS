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
            return db.Theaters.Where(x => x.IsActive == true).ToList();
        }

        public List<Theater> TheaterByLocation(string location)
        {
            var theaters = db.Theaters.Where(x => x.IsActive == true).Where(x => x.Location == location).ToList();
            return theaters;
        }

        public Theater TheaterById(int id)
        {
            var theater = db.Theaters.Where(x => x.IsActive == true).FirstOrDefault(x => x.TheaterId == id);
            return theater;
        }
        public Theater TheaterByName(string name)
        {
            var theater = db.Theaters.Where(x => x.IsActive == true).FirstOrDefault(x => x.TheaterName == name);
            return theater;
        }


        public Messages DeleteTheater(int theaterId)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var theater = TheaterById(theaterId);
            if (theater == null)
            {
                messages.Message = $"Theater Id {theaterId} is not found";
                messages.Status = Statuses.NotFound;
                return messages;
            }
            var theaters = db.Screens.Where(x => x.TheaterId == theaterId).FirstOrDefault();
            if (theaters != null)
            {
                messages.Message = $"This Theater {theater.TheaterName} is Already scheduled, so you can't delete the theater";
                messages.Status = Statuses.Conflict;
                return messages;
            }
            else
            {
                theater.IsActive = false;
                db.SaveChanges();
                messages.Success = true;
                messages.Message = $"Theater {theater.TheaterName} is Successfully Removed";
                messages.Status = Statuses.Success;
                return messages;
            }
            
        }

        public Messages CreateTheater(Theater theater)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var theaterExist = db.Theaters.FirstOrDefault(x => x.TheaterName == theater.TheaterName);
            if (theaterExist != null)
            {
               
                messages.Status = Statuses.Conflict;
                messages.Message = $"Theater Name {theater.TheaterName} is already Registered.";
                return messages;
            }
            else
            {
                db.Theaters.Add(theater);
                db.SaveChanges();
                messages.Success = true;
                messages.Message = $"Theater {theater.TheaterName} is Successfully Added";
                messages.Status = Statuses.Created;
                return messages;
            }
            
        }

        public Messages UpdateTheater(Theater theater)
        {
            Messages messages = new Messages();
            messages.Success = false;
            if (theater.TheaterId == 0)
            {
                messages.Message = "Enter the Theater Id field";
                messages.Status = Statuses.BadRequest;
                return messages;
            }
            var theaterExist = TheaterById(theater.TheaterId);
            var nameExist = db.Theaters.FirstOrDefault(x => x.TheaterName == theater.TheaterName);
            if (theaterExist == null)
            {
                messages.Message = "Theater Id is not found";
                messages.Status = Statuses.NotFound;
            }
            else if (nameExist != null && nameExist.TheaterId != theaterExist.TheaterId)
            {
                messages.Message = $"Theater Name {theater.TheaterName} is already registered";
                messages.Status = Statuses.Conflict;
            }
            else
            {
                theaterExist.TheaterName = theater.TheaterName;
                theaterExist.Location = theater.Location;
                db.SaveChanges();
                messages.Success = true;
                messages.Message = $"Theater {theater.TheaterName} is Successfully Updated";
                messages.Status = Statuses.Success;
            }
            return messages;
        }

        public ScreensListDTO TheaterScreens(int id)
        {
            var theater = db.Theaters.Where(x => x.IsActive == true).FirstOrDefault(x => x.TheaterId == id);
            var screens = db.Screens.Where(x => x.IsActive == true).Where(x => x.TheaterId == id).ToList();
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
            var screens = db.Screens.Where(x => x.IsActive == true).Where(x => x.TheaterId == id).ToList();

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

        private List<SchedulesDTO> SchedulesListScreenId(int id)
        {
            DateTime date = DateTime.Now;
            var timeValue = TimesValue(date);
            var screens = (from screen in db.Screens
                           join schedule in db.Schedules on screen.ScreenId equals schedule.ScreenId
                           join showTime in db.ShowTimes on schedule.ShowTimeId equals showTime.ShowTimeId
                           join movie in db.Movies on schedule.MovieId equals movie.MovieId
                           where screen.ScreenId == id && schedule.IsActive == true && ((schedule.Date == date.Date && showTime.ShowTiming > timeValue) || (schedule.Date > date.Date))
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
                           where theater.TheaterId == id && screen.IsActive == true
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
