using Blazorise;
using Microsoft.AspNetCore.Mvc;
using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace PKTickets.Repository
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly PKTicketsDbContext db;
        public ScheduleRepository(PKTicketsDbContext db)
        {
            this.db = db;
        }
        public List<Theater> TheaterByMovieId(int id)
        {
            var details= DetailsByMovieId(id);
            List<Theater> theater = new List<Theater>();
            foreach(var item in details.Theaters)
            {
                var obj=db.Theaters.FirstOrDefault(x=>x.TheaterId==item.TheaterId);
                theater.Add(obj);
            }
            return theater;
        }
        public List<Screen> ScreenByMovieAndTheaterId(int mId,int tId)
        {
            var details = DetailsByMovieId(mId);
            var theater = details.Theaters.FirstOrDefault(x=>x.TheaterId==tId);
            List<Screen> screen = new List<Screen>();
            foreach (var item in theater.Screens)
            {
                var obj = db.Screens.FirstOrDefault(x => x.ScreenId == item.ScreenId);
                screen.Add(obj);
            }
            return screen;
        }
        public List<SchedulesDTO> ScheduleByMovieAndScreenId(int mId, int sId)
        {
            var list = SchedulesListScreenId(sId).Where(x => x.MovieId == mId).ToList();
            return list;
        }
        public List<Schedule> SchedulesList()
        {
            return db.Schedules.ToList();
        }
        public List<Schedule> AvailableSchedulesList()
        {
            DateTime date = DateTime.Now;
            var timeValue = TimesValue(date);
            var list = db.Schedules.Where(x => x.IsActive).Where(x => x.Date == date.Date).ToList();
            var scheduleList = new List<Schedule>();
            foreach (Schedule movie in list)
            {
                var times = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == movie.ShowTimeId);
                if (times.ShowTiming > timeValue)
                {
                    scheduleList.Add(movie);
                }
            }
            var list2 = db.Schedules.Where(x => x.IsActive).Where(x => x.Date > date.Date).ToList();
            if(list2.Count()>0)
            {
                foreach (Schedule movie in list2)
                {
                    scheduleList.Add(movie);
                }
            }
            return scheduleList;
        }
        public Movie MovieById(int id)
        {
            var movie = db.Movies.Where(x => x.IsPlaying).FirstOrDefault(x => x.MovieId == id);
            return movie;
        }
        public Schedule ScheduleById(int id)
        {
            var schedule = db.Schedules.Where(x => x.IsActive).FirstOrDefault(x => x.ScheduleId == id);
            return schedule;
        }

        public List<Schedule> SchedulesByMovieId(int id)
        {
            var movie=db.Movies.FirstOrDefault(x => x.MovieId == id);
            List<Schedule> scheduleList = new List<Schedule>();  
            if (movie == null)
            {
                scheduleList[1]= null;
                return scheduleList;
            }
            scheduleList = AvailableSchedulesList().Where(x => x.MovieId == id).ToList();
            return scheduleList;
        }


        public Messages DeleteSchedule(int id)
        {
            Messages messages = new Messages();
            messages.Success = false;
            DateTime date = DateTime.Now;
            var timeValue = TimesValue(date);
            var scheduleExist = ScheduleById(id);
            if (scheduleExist == null)
            {
                return NotFound($"The Schedule Id {id} is not found");
            }
            var reservation=db.Reservations.Where(x => x.ScheduleId == id).Where(x => x.IsActive == true).FirstOrDefault();
            if (reservation != null)
            {
                return UpdateConflict(0,$"The Reservation is Already Started to Schedule Id {id} ,so can't Delete");
            }
            var timeExist = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == scheduleExist.ShowTimeId);
            return (date.Date > scheduleExist.Date) ? UpdateConflict(0,$"The Show is Started for this Schedule Id {id} ,so can't Delete")
                : (date.Date == scheduleExist.Date && timeExist.ShowTiming < timeValue) ? UpdateConflict(0,$"The Show is Started for this Schedule Id {id} ,so can't Delete")
                : Delete(scheduleExist, id);
        }
        
        public Messages CreateSchedule(Schedule schedule)
        {
            if(BadSchedule(schedule).Status==Statuses.BadRequest)
            {
                return BadSchedule(schedule);
            }
            else if (NotFoundSchedule(schedule).Status==Statuses.NotFound)
            {
                return NotFoundSchedule(schedule);
            }
            var screen = db.Screens.Where(x => x.IsActive == true).FirstOrDefault(x => x.ScreenId == schedule.ScreenId);
            var scheduleExist = SchedulesByMovieId(schedule.MovieId).Where(x => x.ScreenId == schedule.ScreenId).Where
               (x => x.Date == schedule.Date).FirstOrDefault(x => x.ShowTimeId == schedule.ShowTimeId);
            if (scheduleExist != null)
            {
                return ExistConflict();
            }
            return (TimeCheck(schedule).Status == Statuses.Conflict) ? TimeCheck(schedule)
                : Create(schedule, screen);
        }

        public Messages UpdateSchedule(Schedule schedule)
        {
            Messages messages = new Messages();
            messages.Success = false;
            if(schedule.ScheduleId==0)
            {
                return BadRequest.MSG("Enter the Schedule Id Field");
            }
            if (BadSchedule(schedule).Status == Statuses.BadRequest)
            {
                return BadSchedule(schedule);
            }
            var scheduleExist = ScheduleById(schedule.ScheduleId);
            if (scheduleExist == null)
            {
                messages.Message = "The Schedule Id is not found";
                messages.Status = Statuses.NotFound;
                return messages;
            }
            return (TimeCheck(schedule).Status==Statuses.Conflict)? TimeCheck(schedule)
                :Update(schedule,scheduleExist);
        }

      
        public Screen ScreenById(int id)
        {
            var screen = db.Screens.Where(x => x.IsActive).FirstOrDefault(x => x.ScreenId == id);
            return screen;
        }
       
        public Theater TheaterById(int id)
        {
            var theater = db.Theaters.Where(x => x.IsActive).FirstOrDefault(x => x.TheaterId == id);
            return theater;
        }

        public MovieDTO DetailsByMovieId(int id)
        {
            MovieDTO movie = new MovieDTO();
            var moviename = db.Movies.Where(x => x.IsPlaying).FirstOrDefault(x => x.MovieId == id);
            movie.MovieName = moviename.Title;
            if(moviename == null)
            {
                return movie;
            }
            List<Schedule> schedulesList = AvailableSchedulesList().Where(x=> x.MovieId == id).ToList();    
                //db.Schedules.Where(x => x.IsActive == true).Where(x => x.MovieId == id).ToList();
            List<Schedule> uniqueSchedulesByScreen = schedulesList.DistinctBy(x => x.ScreenId).ToList();
            List<Screen> screenList = new List<Screen>();
            foreach (var schedule in uniqueSchedulesByScreen)
            {
                Screen screen = db.Screens.Where(x => x.IsActive).FirstOrDefault(x => x.ScreenId == schedule.ScreenId);
                screenList.Add(screen);
            }
            List<Screen> uniqueScreen = screenList.DistinctBy(x => x.ScreenId).ToList();
            List<Theater> theaterList = new List<Theater>();
            foreach (var screen in uniqueScreen)
            {
                Theater theater = db.Theaters.Where(x => x.IsActive).FirstOrDefault(x => x.TheaterId == screen.TheaterId);
                theaterList.Add(theater);
            }
            List<Theater> uniqueTheater = theaterList.DistinctBy(x => x.TheaterId).ToList();
            foreach (var theater in uniqueTheater)
            {
                TheaterDetails theaters = new TheaterDetails();
                theaters.TheaterId = theater.TheaterId;
                theaters.TheaterName = theater.TheaterName;
                List<Screen> theaterScreens = new List<Screen>();
                foreach (var screen in uniqueScreen)
                {
                    if (screen.TheaterId == theater.TheaterId)
                    {
                        theaterScreens.Add(screen);
                    }
                }
                foreach (var screen in theaterScreens)
                {
                    ScreenDetails newScreen = new ScreenDetails();
                    newScreen.ScreenId = screen.ScreenId;
                    newScreen.ScreenName = screen.ScreenName;
                    newScreen.PremiumCapacity = screen.PremiumCapacity;
                    newScreen.EliteCapacity = screen.EliteCapacity;
                    //newScreen.Schedules=newScreen.Schedules == null ? new List<ScheduleDetails>() : newScreen.Schedules;
                    foreach (var schedule in schedulesList)
                    {
                        if (schedule.ScreenId == screen.ScreenId)
                        {
                            ScheduleDetails schedules = new ScheduleDetails();
                            schedules.ScheduleId = schedule.ScheduleId;
                            schedules.Date = schedule.Date;
                            var time = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == schedule.ShowTimeId);
                            schedules.ShowTime = TimingConvert.ConvertToString(time.ShowTiming);
                            schedules.AvailablePremiumSeats = schedule.AvailablePreSeats;
                            schedules.AvailableEliteSeats = schedule.AvailableEliSeats;
                            newScreen.Schedules.Add(schedules);
                        }
                    }
                    theaters.Screens.Add(newScreen);
                }
                movie.Theaters.Add(theaters);
            }
            return movie;
        }
        public SchedulesListDTO SchedulesListByScreenId(int id)
        {
            var screens = db.Screens.Where(x => x.IsActive).FirstOrDefault(x => x.ScreenId == id);
            var theater = db.Theaters.FirstOrDefault(x => x.TheaterId == screens.TheaterId);

            SchedulesListDTO list = new SchedulesListDTO();
            list.TheaterName = theater.TheaterName;
            list.ScreenName = screens.ScreenName;
            list.PremiumCapacity = screens.PremiumCapacity;
            list.EliteCapacity = screens.EliteCapacity;
            list.Schedules = SchedulesListScreenId(id);
            return list;
        }
        #region PrivateMethods

        private Messages messages = new Messages() { Status = Statuses.Created, Success = false };
        private Messages Create(Schedule schedule,Screen screen)
        {
            schedule.PremiumSeats = screen.PremiumCapacity;
            schedule.EliteSeats = screen.EliteCapacity;
            schedule.AvailablePreSeats = screen.PremiumCapacity;
            schedule.AvailableEliSeats = screen.EliteCapacity;
            db.Schedules.Add(schedule);
            db.SaveChanges();
            messages.Success = true;
            messages.Message = $"Schedule Id {schedule.ScheduleId} Is added Successfully";
            return messages;
        }
        private Messages Update(Schedule schedule, Schedule scheduleExist)
        {
            scheduleExist.MovieId = schedule.MovieId;
            db.SaveChanges();
            messages.Success = true;
            messages.Status = Statuses.Success;
            messages.Message = $"The Schedule Id {schedule.ScheduleId} is Successfully Updated";
            return messages;
        }
        private Messages TimeCheck(Schedule schedule)
        {
            DateTime date = DateTime.Now;
            var timeValue = TimesValue(date);
            var timeExist = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == schedule.ShowTimeId);
            if (schedule.ScheduleId == 0)
            {
                return (date.Date > schedule.Date) ? Conflict(schedule.Date)
                    : (date.Date == schedule.Date && timeExist.ShowTiming < timeValue) ? Conflict(schedule.Date)
                    : messages;
            }
            else
            {
                return (date.Date > schedule.Date) ? UpdateConflict(schedule.ScheduleId,"")
               : (date.Date == schedule.Date && timeExist.ShowTiming > timeValue) ? UpdateConflict(schedule.ScheduleId,"")
               : messages;
            }
        }
        private Messages UpdateConflict(int id,string txt)
        {
            if(id==0)
            {
                messages.Message = txt;
            }
            else
            {
                messages.Message = $"The Reservation Is Already started fot this Schedule Id {id} So can't Update";
            }
            messages.Status = Statuses.Conflict;
            return messages;
        }
        private Messages Conflict(DateTime date)
        {
            messages.Message = $"The date {date} entered is Invalid,Kindly Check the Date.";
            messages.Status = Statuses.Conflict;
            return messages;
        }
        private Messages BadSchedule(Schedule schedule)
        {
            return (schedule.ScreenId == 0) ? BadRequest.MSG("Enter the Screen Id Field")
                : (schedule.MovieId == 0) ? BadRequest.MSG("Enter the Movie Id Field")
                : (schedule.ShowTimeId == 0) ? BadRequest.MSG("Enter the Show Time Id Field")
                : messages;
        }
        private Messages NotFoundSchedule(Schedule schedule)
        {
            var screen = db.Screens.Where(x => x.IsActive == true).FirstOrDefault(x => x.ScreenId == schedule.ScreenId);
            var movie = db.Movies.Where(x => x.IsPlaying == true).FirstOrDefault(x => x.MovieId == schedule.MovieId);
            var showtime = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == schedule.ShowTimeId);
            return (screen == null) ? NotFound($"The Screen Id {schedule.ScreenId} is not Registered")
                : (movie == null) ? NotFound($"The Movie Id {schedule.MovieId} is not Registered")
                : (showtime == null) ? NotFound($"The Show Time Id {schedule.ShowTimeId} is not Registered")
                : messages;
        }
        private Messages ExistConflict()
        {
            messages.Status = Statuses.Conflict;
            messages.Message = "This Schedule is already Registered Please check the Fields";
            return messages;
        }
        private Messages NotFound(string txt)
        {
            messages.Status=Statuses.NotFound;
            messages.Message=txt;
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
                           where screen.ScreenId == id &&  schedule.IsActive == true &&((schedule.Date==date.Date && showTime.ShowTiming >timeValue) || (schedule.Date>date.Date) )
                           select new SchedulesDTO()
                           {
                               ScheduleId = schedule.ScheduleId,
                               MovieName=movie.Title,
                               MovieId=movie.MovieId,
                               Date = schedule.Date,
                               ShowTime = TimingConvert.ConvertToString(showTime.ShowTiming),
                               AvailablePremiumSeats = schedule.AvailablePreSeats,
                               AvailableEliteSeats = schedule.AvailableEliSeats
                           }).ToList();

            return screens;
        }
        private Messages Delete(Schedule scheduleExist,int id)
        {
            scheduleExist.IsActive = false;
            db.SaveChanges();
            messages.Success = true;
            messages.Status = Statuses.Success;
            messages.Message = $"The Schedule Id {id} is Removed From reservation";
            return messages;
        }
        private int TimesValue(DateTime date)
        {
            TimeSpan time = new TimeSpan(date.Hour, date.Minute, 0);
            return (TimingConvert.ConvertToInt(Convert.ToString(time)));
        }
        #endregion
    }
}
