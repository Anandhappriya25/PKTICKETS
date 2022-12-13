using PKTickets.Repository;
using PKTickets.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using PKTickets.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PKTickets.Interfaces;
using PKTickets.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.Data;

namespace PKTickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ITheaterRepository _theaterRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IScreenRepository _screenRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IShowTimeRepository _showTimeRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IWebHostEnvironment WebHostEnvironment;


        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, IMovieRepository movieRepository,
            ITheaterRepository theaterRepository, IScreenRepository screenRepository,
            IScheduleRepository scheduleRepository, IShowTimeRepository showTimeRepository,
            IReservationRepository reservationRepository,
            IWebHostEnvironment _webHostEnvironment)
        {
            _logger = logger;
            _userRepository = userRepository;
            _movieRepository = movieRepository;
            _theaterRepository = theaterRepository;
            _screenRepository = screenRepository;
            _scheduleRepository = scheduleRepository;
            _showTimeRepository = showTimeRepository;
            _reservationRepository = reservationRepository;
            WebHostEnvironment = _webHostEnvironment;
        }


        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
       
        public IActionResult UserList()
        {
            var usersList = _userRepository.GetAllUsers();
            return View(usersList);
        }
        public IActionResult DeleteUser(int id)
        {
            var ticket = _userRepository.DeleteUser(id);
            return Json(ticket);
        }
        public IActionResult CreateUser()
        {
            User user = new User();
            return View(user);
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _userRepository.UserById(id);
            return View("CreateUser", user);
        }

        [HttpPost]
        public IActionResult Save(User user)
        {
            if (user.UserId == 0)
            {
                return Json(_userRepository.CreateUser(user));
            }
            else
            {
                return Json(_userRepository.UpdateUser(user));
            }

        }
        public IActionResult AddTheater()
        {
            Theater theater = new Theater();
            return View(theater);
        }
        [HttpGet]
        public IActionResult EditTheater(int id)
        {
            var theater = _theaterRepository.TheaterById(id);
            return View("AddTheater", theater);
        }
        [HttpPost]
        public IActionResult SaveTheater(Theater theater)
        {
            if (theater.TheaterId == 0)
            {
                return Json(_theaterRepository.CreateTheater(theater));
            }
            else
            {
                return Json(_theaterRepository.UpdateTheater(theater));
            }

        }
        public IActionResult TheatersList()
        {
            var theater = _theaterRepository.GetTheaters();
            return View(theater);
        }
        [HttpGet]
        public IActionResult TheaterScreens(int id)
        {
            var theater = _theaterRepository.TheaterScreens(id);
            ViewBag.csd = id;
            return View(theater);
        }
        public IActionResult RemoveTheater(int id)
        {
            var theater = _theaterRepository.DeleteTheater(id);
            return Json(theater);
        }

        //[HttpGet]
        //public IActionResult ScreenSchedules(int id)
        //{
        //    var schedule = _scheduleRepository.SchedulesListByScreenId(id);
        //    ViewBag.csd = id;
        //    return View(schedule);
        //}
        public IActionResult ReservationsByUserId(int id)
        {
            var usersList = _reservationRepository.ReservationsByUserId(id);
            return View(usersList);
        }
        public IActionResult CancelTicket(int id)
        {
            var ticket = _reservationRepository.DeleteReservation(id);
            return Json(ticket);
        }
        public IActionResult AddScreen(int id)
        {
            Screen screen = new Screen();
            screen.TheaterId = id;
            return View(screen);
        }

        [HttpPost]
        public IActionResult SaveScreen(Screen screen)
        {
            if (screen.ScreenId == 0)
            {
                return Json(_screenRepository.AddScreen(screen));
            }
            else
            {
                return Json(_screenRepository.UpdateScreen(screen));
            }
        }

        [HttpGet]
        public IActionResult EditScreen(int id)
        {
            var screen = _screenRepository.ScreenById(id);
            return View("AddScreen", screen);
        }
        public IActionResult RemoveScreen(int id)
        {
            var screen = _screenRepository.RemoveScreen(id);
            return Json(screen);
        }

        //[HttpGet]
        //public IActionResult ScreenScheduleById(int id)
        //{
        //    var schedule = _scheduleRepository.SchedulesListByScreenId(id);
        //    ViewBag.csd = id;
        //    return View("ScreenSchedules", schedule);
        //}
        public IActionResult ScreenSchedules()
        {
            return View();
        }

        public IActionResult CreateSchedule(int id)
        {
            CreateSchedule schedule = new CreateSchedule();
            schedule.ScreenId = id;
            schedule.Movies=_movieRepository.GetAllMovies().Select(a => new SelectListItem
              {
                      Text = a.Title + "(" + a.Language + ")",
                      Value = a.MovieId.ToString()
                  }).ToList();
            schedule.Movies.Insert(0, new SelectListItem { Text = "Select the Movie", Value = null });
            schedule.Times = _showTimeRepository.GetAllShowTimes().Select(a => new SelectListItem
            {
                Text = a.ShowTiming ,
                Value = a.ShowTimeId.ToString()
            }).ToList();
            schedule.Times.Insert(0, new SelectListItem { Text = "Select the Time", Value = null });
            return View(schedule);
        }

        [HttpPost]
        public IActionResult SaveSchedule(CreateSchedule scheduleDTO)
        {
            Schedule schedule=new Schedule();
            schedule.ScheduleId = scheduleDTO.ScheduleId;
            schedule.ScreenId = scheduleDTO.ScreenId;
            schedule.MovieId = scheduleDTO.MovieId;
            schedule.ShowTimeId = scheduleDTO.ShowTimeId;
            schedule.Date=Convert.ToDateTime(scheduleDTO.Date);
            if (schedule.ScheduleId == 0)
            {
                return Json(_scheduleRepository.CreateSchedule(schedule));
            }
            else
            {
                return Json(_scheduleRepository.UpdateSchedule(schedule));
            }
        }

        [HttpGet]
        public IActionResult EditSchedule(int id)
        {
            var schedules = _scheduleRepository.ScheduleById(id);
            CreateSchedule schedule = new CreateSchedule();
            schedule.ScreenId = schedules.ScreenId;
            schedule.Movies = _movieRepository.GetAllMovies().Select(a => new SelectListItem
            {
                Text = a.Title + "(" + a.Language + ")",
                Value = a.MovieId.ToString()
            }).ToList();
            schedule.Movies.Insert(0, new SelectListItem { Text = "Select the Movie", Value = null });
            schedule.Times = _showTimeRepository.GetAllShowTimes().Select(a => new SelectListItem
            {
                Text = a.ShowTiming,
                Value = a.ShowTimeId.ToString()
            }).ToList();
            schedule.Times.Insert(0, new SelectListItem { Text = "Select the Time", Value = null });
            schedule.ScheduleId = schedules.ScheduleId;
            schedule.MovieId = schedules.MovieId;
            schedule.ShowTimeId = schedules.ShowTimeId;
            schedule.Date = schedules.Date.ToString("yyyy-MM-dd"); 
            schedule.AvailablePreSeats = schedules.AvailablePreSeats;
            schedule.AvailableEliSeats = schedules.AvailableEliSeats;
            return View("CreateSchedule", schedule);
        }
        public IActionResult RemoveSchedule(int id)
        {
            var schedule = _scheduleRepository.DeleteSchedule(id);
            return Json(schedule);
        }
        public IActionResult Movies()
        {
            var movie = _movieRepository.GetAllMovies();
            return View(movie);
        }
        public IActionResult MovieList()
        {
            var movie = _movieRepository.ScheduledMovies();
            return View(movie);
        }


        public IActionResult AddMovie()
        {
            Movie movie = new Movie();
            return View(movie);
        }
        [HttpGet]
        public IActionResult EditMovie(int id)
        {
            var movie = _movieRepository.MovieById(id);
            return View("AddMovie", movie);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Movie movie)
        {

            var uploadDirectory = "Css/Image/";
            var uploadPath = Path.Combine(WebHostEnvironment.WebRootPath, uploadDirectory);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);
            var fileName = Guid.NewGuid() + Path.GetExtension(movie.CoverPhoto.FileName);
            var imagePath = Path.Combine(uploadPath, fileName);
            await movie.CoverPhoto.CopyToAsync(new FileStream(imagePath, FileMode.Create));
            movie.ImagePath = fileName;

            if (movie.MovieId > 0)
            {
                var obj = _movieRepository.UpdateMovie(movie);
                return RedirectToAction("Movies");
            }
            else
            {
                var obj = _movieRepository.CreateMovie(movie);
                return RedirectToAction("Movies");
            }
        }

        public IActionResult ReservationsList()
        {
            var list = _reservationRepository.ListOfReservations();
            return View(list);
        }
        [HttpGet]
        public IActionResult InvoiceById(int id)
        {
            var invoice = _reservationRepository.InvoiceById(id);

            return View("Invoice", invoice);
        }
        public IActionResult Invoice()
        {
            Invoice invoice = new Invoice();
            return View(invoice);
        }
        public IActionResult CancelReservation(int id)
        {
            var ticket = _reservationRepository.DeleteReservation(id);
            return Json(ticket);
        }
        public IActionResult RemoveMovie(int id)
        {
            var movie = _movieRepository.DeleteMovie(id);
            return Json(movie);
        }
        [HttpGet]
        public IActionResult MovieById(int id)
        {
            var movie = _movieRepository.MovieById(id);
            return View("MovieInfo", movie);
        }
        public IActionResult MovieInfo()
        {
            Movie movie = new Movie();
            return View(movie);
        }

        [HttpGet]
        public IActionResult MovieTheaters(int id)
        {
            var theaters = _scheduleRepository.TheaterByMovieId(id);
            ViewBag.mId = id;
            return View(theaters);
        }
        [HttpGet]
        public IActionResult MovieScreens(int tId, int mId)
        {
            var screens = _scheduleRepository.ScreenByMovieAndTheaterId(mId, tId);
            ViewBag.mId = mId;
            ViewBag.tId = tId;
            return View(screens);
        }
        [HttpGet]
        public IActionResult MovieSchedules(int sId, int mId)
        {
            var schedules = _scheduleRepository.ScheduleByMovieAndScreenId(mId, sId);
            return View(schedules);
        }
        public IActionResult BookTicket(int id)
        {
            BookTicketDTO reservation = new BookTicketDTO();
            reservation.ScheduleId = id;
            reservation.Users = _userRepository.GetAllUsers().Select(a => new SelectListItem
            {
                Text = a.UserName + "(" + a.PhoneNumber + ")",
                Value = a.UserId.ToString()
            }).ToList();
            reservation.Users.Insert(0, new SelectListItem { Text = "Select the User", Value = null });
            return View(reservation);
        }
        public IActionResult SaveTicket(BookTicketDTO booking)
        {
            Reservation reservation = new Reservation();
            reservation.ScheduleId = booking.ScheduleId;
            reservation.PremiumTickets = booking.PremiumTickets;
            reservation.EliteTickets = booking.EliteTickets;
            reservation.UserId = booking.UserId;
             
            if(reservation.ReservationId==0)
            {
                return Json(_reservationRepository.CreateReservation(reservation));
            }
            else
            {
                return Json(_reservationRepository.UpdateReservation(reservation));
            }
        }
    }
}