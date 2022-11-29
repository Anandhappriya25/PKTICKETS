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

namespace PKTickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ITheaterRepository _theaterRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IScreenRepository _screenRepository;
        private readonly IScheduleRepository _seatRepository;
        private readonly IShowTimeRepository _showTimeRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IWebHostEnvironment WebHostEnvironment;


        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, IMovieRepository movieRepository,
            ITheaterRepository theaterRepository, IScreenRepository screenRepository,
            IScheduleRepository seatRepository, IShowTimeRepository showTimeRepository,
            IReservationRepository reservationRepository,
            IWebHostEnvironment _webHostEnvironment)
        {
            _logger = logger;
            _userRepository = userRepository;
            _movieRepository = movieRepository;
            _theaterRepository = theaterRepository;
            _screenRepository = screenRepository;
            _seatRepository = seatRepository;
            _showTimeRepository = showTimeRepository;
            _reservationRepository = reservationRepository;
            WebHostEnvironment = _webHostEnvironment;
        }

       
        public IActionResult Index()
        {
            return View();
        }

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
            User user=new User();
            return View(user);
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _userRepository.UserById(id);
            return View("CreateUser",user);
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

        public IActionResult TheatersList()
        {
            var theater = _theaterRepository.GetTheaters();
            return View(theater);
        }
        public IActionResult RemoveTheater (int id)
        {
            var theater = _theaterRepository.DeleteTheater(id);
            return Json(theater);
        }
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

        //public async Task<IActionResult> Add(Movie movie)
        //{

        //    var uploadDirectory = "Css/Image/";
        //    string location = "~wwwroot/Css/Image/";
        //    var uploadPath = Path.Combine(WebHostEnvironment.WebRootPath, uploadDirectory);
        //    if (!Directory.Exists(uploadPath))
        //        Directory.CreateDirectory(uploadPath);
        //    var fileName = Guid.NewGuid() + Path.GetExtension(movie.CoverPhoto.FileName);
        //    var imagePath = Path.Combine(uploadPath, fileName);
        //    await movie.CoverPhoto.CopyToAsync(new FileStream(imagePath, FileMode.Create));
        //    movie.ImagePath = fileName;
        //    if (movie.MovieId > 0)
        //    {
        //        return Json(_movieRepository.UpdateMovie(movie));
        //    }
        //    else
        //    {
        //        return Json(_movieRepository.CreateMovie(movie));
        //    }
        //}


    }
}