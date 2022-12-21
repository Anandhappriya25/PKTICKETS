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
            return db.Reservations.Where(x => x.IsActive).ToList();
        }


        public Reservation ReservationById(int id)
        {
            var reservation = db.Reservations.Where(x => x.IsActive).FirstOrDefault(x => x.ReservationId == id);
            return reservation;
        }
        public List<Reservation> ReservationsByShowId(int id)
        {
            return ReservationList().Where(x => x.ScheduleId == id).ToList();
        }

        public Messages DeleteReservation(int id)
        {
            var reservationExist = ReservationById(id);
            if (reservationExist == null)
            {
                return Request.Not("Reservation Id is not found");
            }
            DateTime date = DateTime.Now;
            TimeSpan time = new TimeSpan(date.Hour, date.Minute, 0);
            var timing = TimingConvert.ConvertToInt(Convert.ToString(time));
            var schedule = db.Schedules.FirstOrDefault(x => x.ScheduleId == reservationExist.ScheduleId);
            var showTiming = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == schedule.ShowTimeId);
            return (date.Date > schedule.Date) ? Request.Conflict("You are UpTo Time ,so can't Cancel The Reservation")
                : (date.Date < schedule.Date) ? DeleteSave(reservationExist, schedule)
                : (date.Date == schedule.Date && timing > showTiming.ShowTiming) ? Request.Conflict("You are UpTo Time ,so can't Cancel The Reservation")
                : DeleteSave(reservationExist, schedule);
        }

       
        public Messages CreateReservation(Reservation reservation)
        {
            Messages messages = new Messages();
            messages.Success = false;
            DateTime date = DateTime.Now;
            TimeSpan time = new TimeSpan(date.Hour, date.Minute, 0);
            var timing = TimingConvert.ConvertToInt(Convert.ToString(time));
            if(NotFounds(reservation).Status==Statuses.NotFound)
            {
                return NotFounds(reservation);
            }
            var schedule = db.Schedules.Where(x => x.IsActive).FirstOrDefault(x => x.ScheduleId == reservation.ScheduleId);
            if (reservation.PremiumTickets == 0 && reservation.EliteTickets == 0)
            {
                return Request.Bad("Please reaserve atleast a seat");
            }
            var showTime = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == schedule.ShowTimeId);
            return (date.Date > schedule.Date) ? ScheduleFalse(schedule)
                : (schedule.AvailablePreSeats - reservation.PremiumTickets < 0) ? Request.Conflict
                ("Only " + schedule.AvailablePreSeats + " Premium Tickets are Available")
                : (schedule.AvailableEliSeats - reservation.EliteTickets < 0) ? Request.Conflict
                ("Only " + schedule.AvailableEliSeats + " Elite Tickets are Available")
                : (date.Date == schedule.Date && timing > showTime.ShowTiming) ? ScheduleFalse(schedule)
                : Create(reservation, schedule);
        }

        public Messages UpdateReservation(Reservation reservation)
        {
            if (reservation.ReservationId == 0)
            {
                return Request.Bad("Enter the Reservation Id Field");
            }
            var reservationExist = ReservationById(reservation.ReservationId);
            if (reservationExist == null)
            {
                return Request.Not("Reservation Id is Not found");
            }
            DateTime date = DateTime.Now;
            TimeSpan time = new TimeSpan(date.Hour, date.Minute, 0);
            var timing = TimingConvert.ConvertToInt(Convert.ToString(time));
            var schedule = db.Schedules.Where(x => x.IsActive).FirstOrDefault(x => x.ScheduleId == reservation.ScheduleId);
            var showTime = db.ShowTimes.FirstOrDefault(x => x.ShowTimeId == schedule.ShowTimeId);
            return (date.Date > schedule.Date)? Request.Conflict("You are UpTo Time ,so can't Update The Reservation")
                : (date.Date < schedule.Date) ? UpdateSave(reservation, reservationExist, schedule)
                : (date.Date == schedule.Date && timing > showTime.ShowTiming) ? Request.Conflict("You are UpTo Time ,so can't Update The Reservation")
                : UpdateSave(reservation, reservationExist, schedule);
        }
        public UserDTO ReservationsByUserId(int id)
        {
            var user = db.Users.Where(x => x.IsActive).FirstOrDefault(x => x.UserId == id);
            UserDTO details = new UserDTO();
            if (user == null)
            {
                return details;
            }
            details.UserName = user.UserName;
            details.ReservationDetail = ReservationDetailsByUserId(id);
            return details;
        }
        public User UserById(int id)
        {
            var user = db.Users.Where(x => x.IsActive).FirstOrDefault(x => x.UserId == id);
            return user;
        }
        public Schedule ScheduleById(int id)
        {
            var schedule = db.Schedules.Where(x => x.IsActive).FirstOrDefault(x => x.ScheduleId == id);
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
                                where user.IsActive == true && reservation.IsActive
                                select new ReservationDTO()
                                {
                                    ReservationId = reservation.ReservationId,
                                    TheaterName = theater.TheaterName,
                                    UserId = user.UserId,
                                    UserName = user.UserName,
                                    MovieName = movie.Title,
                                    Date = schedule.Date,
                                    ShowTiming = TimingConvert.ConvertToString(time.ShowTiming),
                                    Tickets = reservation.PremiumTickets + reservation.EliteTickets,
                                }).ToList();
            return reservations;
        }
        public Invoice InvoiceById(int id)
        {
            var invoice = (from reservation in db.Reservations
                           join user in db.Users on reservation.UserId equals user.UserId
                           join schedule in db.Schedules on reservation.ScheduleId equals schedule.ScheduleId
                           join screen in db.Screens on schedule.ScreenId equals screen.ScreenId
                           join theater in db.Theaters on screen.TheaterId equals theater.TheaterId
                           join movie in db.Movies on schedule.MovieId equals movie.MovieId
                           join time in db.ShowTimes on schedule.ShowTimeId equals time.ShowTimeId
                           where reservation.ReservationId == id && reservation.IsActive
                           select new Invoice()
                           {
                               ReservationId = reservation.ReservationId,
                               UserName = user.UserName,
                               TheaterName = theater.TheaterName,
                               ScreenName = screen.ScreenName,
                               MovieName = movie.Title,
                               Language = movie.Language,
                               Date = schedule.Date,
                               ShowTime = TimingConvert.ConvertToString(time.ShowTiming),
                               PremiumTicket = reservation.PremiumTickets,
                               EliteTicket = reservation.EliteTickets,
                               PremiumPrice = screen.PremiumPrice,
                               ElitePrice = screen.ElitePrice,
                               TotalAmount = (reservation.PremiumTickets * screen.PremiumPrice) + (reservation.EliteTickets * screen.ElitePrice),
                           }).ToList();
            return invoice.ToList()[0];
        }

        #region Private Methods
        private void SeatCheck(Schedule schedule)
        {
            if (schedule.AvailablePreSeats == 0 && schedule.AvailableEliSeats == 0)
            {
                schedule.IsActive = false;
                db.SaveChanges();
            }
        }
        private Messages messages = new Messages() { Success = true };
        private Messages ScheduleFalse(Schedule schedule)
        {
            messages.Success = false;
            schedule.IsActive = false;
            db.SaveChanges();
            return Request.Conflict("You are UpTo Time ,so can't Book The Reservation");
        }
        private List<ReservationDetails> ReservationDetailsByUserId(int id)
        {
            var reservations = (from user in db.Users
                                join reservation in db.Reservations on user.UserId equals reservation.UserId
                                join schedule in db.Schedules on reservation.ScheduleId equals schedule.ScheduleId
                                join screen in db.Screens on schedule.ScreenId equals screen.ScreenId
                                join theater in db.Theaters on screen.TheaterId equals theater.TheaterId
                                join movie in db.Movies on schedule.MovieId equals movie.MovieId
                                join time in db.ShowTimes on schedule.ShowTimeId equals time.ShowTimeId
                                where user.UserId == id && user.IsActive == true && reservation.IsActive
                                select new ReservationDetails()
                                {
                                    ReservationId = reservation.ReservationId,
                                    TheaterName = theater.TheaterName,
                                    ScreenName = screen.ScreenName,
                                    MovieName = movie.Title,
                                    Date = schedule.Date,
                                    ShowTime = TimingConvert.ConvertToString(time.ShowTiming),
                                    PremiumTickets = reservation.PremiumTickets,
                                    EliteTickets = reservation.EliteTickets,
                                    PremiumPrice = screen.PremiumPrice,
                                    ElitePrice = screen.ElitePrice,
                                    TotalAmount = (reservation.PremiumTickets * screen.PremiumPrice) + (reservation.EliteTickets * screen.ElitePrice),
                                }).ToList();
            return reservations;
        }
        private Messages NotFounds(Reservation reservation)
        {
            var schedule = db.Schedules.Where(x => x.IsActive).FirstOrDefault(x => x.ScheduleId == reservation.ScheduleId);
            var user = db.Users.Where(x => x.IsActive).FirstOrDefault(x => x.UserId == reservation.UserId);
            return (schedule == null) ? Request.Not("Schedule Id is Not found")
                : (user == null) ? Request.Not("User Id is Not found")
                : messages;
        }
        private Messages Create(Reservation reservation,Schedule schedule)
        {
            var tickets = reservation.PremiumTickets + reservation.EliteTickets;
            schedule.AvailablePreSeats = schedule.AvailablePreSeats - reservation.PremiumTickets;
            schedule.AvailableEliSeats = schedule.AvailableEliSeats - reservation.EliteTickets;
            db.Reservations.Add(reservation);
            db.SaveChanges();
            messages.Message = "Successfully " + tickets + " Tickets are Reserved";
            messages.Status = Statuses.Created;
            SeatCheck(schedule);
            return messages;
        }

        private Messages UpdateSave(Reservation reservation, Reservation reservationExist, Schedule schedule)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var premiumSeats = schedule.AvailablePreSeats + (reservationExist.PremiumTickets - reservation.PremiumTickets);
            var eliteSeats = schedule.AvailableEliSeats + (reservationExist.EliteTickets - reservation.EliteTickets);
            var premium = schedule.AvailablePreSeats - reservationExist.PremiumTickets;
            var elite = schedule.AvailableEliSeats - reservationExist.EliteTickets;
            return (premiumSeats < 0)? Request.Conflict("This Show Do not have that much of Premium seats,only " + premium + "Premium Tickets available")
                : (eliteSeats < 0)? Request.Conflict("This Show Do not have that much of Elite seats,only " + elite + "Elite Tickets available")
                : Update(reservation, reservationExist, schedule, premiumSeats, eliteSeats);
        }
        private Messages Update(Reservation reservation, Reservation reservationExist, Schedule schedule,int premiumSeats, int eliteSeats)
        {
            schedule.AvailablePreSeats = premiumSeats;
            schedule.AvailableEliSeats = eliteSeats;
            reservationExist.PremiumTickets = reservation.PremiumTickets;
            reservationExist.EliteTickets = reservation.EliteTickets;
            db.SaveChanges();
            messages.Message = " Tickets are Successfully Updated";
            messages.Status = Statuses.Success;
            SeatCheck(schedule);
            return messages;
        }
        private Messages DeleteSave(Reservation reservationExist, Schedule schedule)
        {
            schedule.AvailablePreSeats = schedule.AvailablePreSeats + reservationExist.PremiumTickets;
            schedule.AvailableEliSeats = schedule.AvailableEliSeats + reservationExist.EliteTickets;
            reservationExist.IsActive = false;
            db.SaveChanges();
            messages.Message = "Reservation is succssfully deleted";
            messages.Status = Statuses.Success;
            return messages;
        }

        #endregion
    }
}

