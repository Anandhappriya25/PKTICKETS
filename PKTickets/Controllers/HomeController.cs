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