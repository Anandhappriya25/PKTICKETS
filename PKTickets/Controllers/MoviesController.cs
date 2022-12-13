using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using PKTickets.Repository;

namespace PKTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {

        private readonly IMovieRepository movieRepository;

        public MoviesController(IMovieRepository _movieRepository)
        {
            movieRepository = _movieRepository;
        }


        [HttpGet("")]
        public IActionResult List()
        {
            return Ok(movieRepository.GetAllMovies());
        }
        [HttpGet("Available")]
        public IActionResult AvailableSchedules()
        {
            return Ok(movieRepository.ScheduledMovies());
        }
        [HttpGet("Title/{title}")]
        public IActionResult ListByTitle(string title)
        {
            var list = movieRepository.MovieByTitle(title);
            if(list.Count() == 0)
            {
                return NotFound("This Movie Titles is Not Registered");
            }
            return Ok(list);
        }

        [HttpGet("Genre/{Genre}")]
        public IActionResult ListByGenre(string Genre)
        {
            var list = movieRepository.MovieByGenre(Genre);
            if (list.Count() == 0)
            {
                return NotFound("This Genre of Movies is Not Registered");
            }
            return Ok(list);
        }

        [HttpGet("{movieId}")]

        public ActionResult GetById(int movieId)
        {
            var movie = movieRepository.MovieById(movieId);
            if (movie == null)
            {
                return NotFound("This Movie Id is Not Registered");
            }
            return Ok(movie);
        }


        [HttpPost("")]
        public IActionResult Add(Movie movie)
        {
            var result = movieRepository.CreateMovie(movie);
            if (result.Success == false)
            {
                return Conflict(result.Message);
            }
            return Created(""+ TimingConvert.LocalHost("Movies") + movie.MovieId + "", result.Message);
        }


        [HttpPut("")]
        public ActionResult Update(Movie movie)
        {
            if (movie.MovieId == 0)
            {
                return BadRequest("Enter the Movie Id field");
            }
            var result = movieRepository.UpdateMovie(movie);
            if (result.Message == "Movie Id is not found")
            {
                return NotFound(result.Message);
            }
            else if (result.Success == false)
            {
                return Conflict(result.Message);
            }
            return Ok(result.Message);
        }


        [HttpDelete("{movieId}")]

        public IActionResult Remove(int movieId)
        {
            var result = movieRepository.DeleteMovie(movieId);
            if (result.Success == false)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Message);
        }
    }
}
