using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using System.Linq;
using System.Xml.Linq;

namespace PKTickets.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly PKTicketsDbContext db;
        public ReservationRepository(PKTicketsDbContext db)
        {
            this.db = db;
        }

        public List<Reservation> ReservationList()
        {
            return db.Reservations.Where(x => x.IsActive == true).ToList();
        }
 

        public Reservation ReservationById(int id)
        {
            var reservation = db.Reservations.Where(x => x.IsActive == true).FirstOrDefault(x => x.ReservationId == id);
            return reservation;
        }
        public List<Reservation> ReservationsByShowId(int id)
        {
            return ReservationList().Where(x => x.ScheduleId == id).ToList();
        }

        public Messages DeleteReservation(int id)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var reservationExist = ReservationById(id);
            if (reservationExist == null)
            {
                messages.Message = "Reservation Id is not found";
            }
            DateTime date = DateTime.Now;
            TimeSpan time = new TimeSpan( date.Hour, date.Minute,0);
            var timing = TimingConvert.ConvertToInt(Convert.ToString(time));
            var schedule = db.Schedules.FirstOrDefault(x => x.ScheduleId == reservationExist.ScheduleId);
            if (date.Date > schedule.Date)
            {
                messages.Message = "You are UpTo Time ,so can't Cancel The Reservation";
                return messages;
            }
            var showTiming = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == schedule.ShowTimeId);
            if (date.Date < schedule.Date)
            {
                return DeleteSave(reservationExist, schedule);
            }
            else
            {
                if (timing > showTiming.ShowTiming)
                {
                    messages.Message = "You are UpTo Time ,so can't Cancel The Reservation";
                    return messages;
                }
                else
                {
                    return DeleteSave(reservationExist,schedule);
                }
            }
               
        }

        public Messages CreateReservation(Reservation reservation)
        {
            Messages messages = new Messages();
            messages.Success = false;
            DateTime date = DateTime.Now;
            TimeSpan time = new TimeSpan(date.Hour, date.Minute, 0);
            var timing = TimingConvert.ConvertToInt(Convert.ToString(time));
            var schedule = db.Schedules.Where(x => x.IsActive == true).FirstOrDefault(x => x.ScheduleId == reservation.ScheduleId);
            if (schedule == null)
            {
                messages.Message = "Schedule Id(" + reservation.ScheduleId + ") is Not found";
                return messages;
            }
            var user = db.Users.Where(x => x.IsActive == true).FirstOrDefault(x => x.UserId == reservation.UserId);
            if (user == null)
            {
                messages.Message = "User Id(" + reservation.UserId + ") is Not found";
                return messages;
            }
            else if (reservation.PremiumTickets==0 && reservation.EliteTickets== 0)
            {
                messages.Message = "Atleast Please reaserve a seat";
                return messages;
            }
           
            var showTime = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == schedule.ShowTimeId);
            if (date.Date > schedule.Date)
            {
                messages.Message = "You are UpTo Time ,so can't Book The Reservation";
                return messages;
            }
            else if(schedule.AvailablePreSeats- reservation.PremiumTickets < 0)
            {
                messages.Message = "Only "+schedule.AvailablePreSeats+" Premium Tickets are Available";
                return messages;
            }
            else if (schedule.AvailableEliSeats - reservation.EliteTickets < 0)
            {
                messages.Message = "Only " + schedule.AvailableEliSeats + " Elite Tickets are Available";
                return messages;
            }
            else
            {
                if (date.Date == schedule.Date && timing > showTime.ShowTiming)
                {
                    messages.Message = "You are UpTo Time ,so can't Book The Reservation";
                    return messages;
                }
                else
                {
                    var tickets = reservation.PremiumTickets + reservation.EliteTickets;
                    schedule.AvailablePreSeats = schedule.AvailablePreSeats - reservation.PremiumTickets;
                    schedule.AvailableEliSeats = schedule.AvailableEliSeats - reservation.EliteTickets;
                    db.Reservations.Add(reservation);
                    db.SaveChanges();
                    messages.Success = true;
                    messages.Message = "Successfully " + tickets + " Tickets are Reserved";
                    return messages;
                }
            }
            
        }


        public Messages UpdateReservation(Reservation reservation)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var reservationExist = ReservationById(reservation.ReservationId);
            if (reservationExist == null)
            {
                messages.Message = "Reservation Id is Not found";
                return messages;
            }
            DateTime date = DateTime.Now;
            TimeSpan time = new TimeSpan(date.Hour, date.Minute,0);
            var timing = TimingConvert.ConvertToInt(Convert.ToString(time));
            var schedule = db.Schedules.Where(x => x.IsActive == true).FirstOrDefault(x => x.ScheduleId == reservation.ScheduleId);
            if(date.Date > schedule.Date)
            {
                messages.Message = "You are UpTo Time ,so can't Update The Reservation";
                return messages;
            }
            var showTime = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == schedule.ShowTimeId);
            if (date.Date < schedule.Date)
            {
                return UpdateSave(reservation, reservationExist, schedule);
            }
            else 
            {
                if (timing > showTime.ShowTiming)
                {
                    messages.Message = "You are UpTo Time ,so can't Update The Reservation";
                    return messages;
                }
                else
                {
                        return UpdateSave(reservation,reservationExist,schedule);
                }
             }
                    
        }
        public UserDTO ReservationsByUserId(int id)
        {
            var user = db.Users.Where(x => x.IsActive == true).FirstOrDefault(x => x.UserId == id);
            UserDTO details = new UserDTO();
            details.UserName = user.UserName;
            details.ReservationDetail= ReservationDetailsByUserId(id);
            return details;
        }

        public User UserById(int id)
        {
            var user = db.Users.Where(x => x.IsActive == true).FirstOrDefault(x => x.UserId == id);
            return user;
        }
        public Schedule ScheduleById(int id)
        {
            var schedule = db.Schedules.Where(x => x.IsActive == true).FirstOrDefault(x => x.ScheduleId == id);
            return schedule;
        }

        public List<ReservationDTO> ListOfReservations()
        {
            var reservations = (from reservation in db.Reservations
                                join user in db.Users on reservation.UserId equals user.UserId
                                join schedule in db.Schedules on reservation.ScheduleId equals schedule.ScheduleId
                                join screen in db.Screens on schedule.ScreenId equals screen.ScreenId
                                join theater in db.Theaters on screen.TheaterId equals theater.TheaterId
                                join movie in db.Movies on schedule.MovieId equals movie.MovieId
                                join time in db.ShowTimes on schedule.ShowTimeId equals time.ShowTimeId
                                where  user.IsActive == true && reservation.IsActive == true
                                select new ReservationDTO()
                                {
                                    ReservationId = reservation.ReservationId,
                                    TheaterName = theater.TheaterName,
                                    UserName = user.UserName,
                                    MovieName = movie.Title,
                                    Date = schedule.Date,
                                    ShowTiming = TimingConvert.ConvertToString(time.ShowTiming),
                                    Tickets = reservation.PremiumTickets,
                                      }).ToList();
            return reservations;
        }
        #region Private Methods

        private List<ReservationDetails> ReservationDetailsByUserId(int id)
        {
            var reservations = (from user in db.Users
                           join reservation in db.Reservations on user.UserId equals reservation.UserId
                                join schedule in db.Schedules on reservation.ScheduleId equals schedule.ScheduleId
                                join screen in db.Screens on schedule.ScreenId equals screen.ScreenId
                                join theater in db.Theaters on screen.TheaterId equals theater.TheaterId
                                join movie in db.Movies on schedule.MovieId equals movie.MovieId
                                join time in db.ShowTimes on schedule.ShowTimeId equals time.ShowTimeId
                                where user.UserId == id && user.IsActive==true && reservation.IsActive==true
                           select new ReservationDetails()
                           {
                           ReservationId= reservation.ReservationId,
                               TheaterName=theater.TheaterName,
                               ScreenName=screen.ScreenName,
                               MovieName=movie.Title,
                               Date=schedule.Date,
                               ShowTime=TimingConvert.ConvertToString(time.ShowTiming),
                               PremiumTickets =reservation.PremiumTickets,
                               EliteTickets=reservation.EliteTickets,
                               PremiumPrice=screen.PremiumPrice,
                               ElitePrice=screen.ElitePrice,
                               TotalAmount= (reservation.PremiumTickets * screen.PremiumPrice)+(reservation.EliteTickets * screen.ElitePrice),
                           }).ToList();
            return reservations;
        }
      private Messages UpdateSave(Reservation reservation, Reservation reservationExist, Schedule schedule)
      {
        Messages messages = new Messages();
        messages.Success = false;
        var premiumSeats = schedule.AvailablePreSeats + (reservationExist.PremiumTickets - reservation.PremiumTickets);
        var eliteSeats = schedule.AvailableEliSeats + (reservationExist.EliteTickets - reservation.EliteTickets);
        var premium = schedule.AvailablePreSeats - reservationExist.PremiumTickets;
        var elite = schedule.AvailableEliSeats - reservationExist.EliteTickets;
        if (premiumSeats < 0)
        {
            messages.Message = "This Show Do not have that much of Premium seats,only " + premium + "Premium Tickets available";
            return messages;
        }
        else if (eliteSeats < 0)
        {
            messages.Message = "This Show Do not have that much of Elite seats,only " + elite + "Elite Tickets available";
            return messages;
        }
        else
        {
            schedule.AvailablePreSeats = premiumSeats;
            schedule.AvailableEliSeats = eliteSeats;
            reservationExist.PremiumTickets = reservation.PremiumTickets;
            reservationExist.EliteTickets = reservation.EliteTickets;
            db.SaveChanges();
            messages.Success = true;
            messages.Message = " Tickets are Successfully Updated";
            return messages;


        }
      }
        private Messages DeleteSave( Reservation reservationExist, Schedule schedule)
        {
            Messages messages = new Messages();
            schedule.AvailablePreSeats = schedule.AvailablePreSeats + reservationExist.PremiumTickets;
            schedule.AvailableEliSeats = schedule.AvailableEliSeats + reservationExist.EliteTickets;
            reservationExist.IsActive = false;
            db.SaveChanges();
            messages.Success = true;
            messages.Message = "Reservation is succssfully deleted";
            return messages;
        }

        #endregion

    }
}

