using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using PKTickets.Repository;
using System.Collections.Generic;

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
            return (list.Count() == 0) ? NotFound("This Movie Titles is Not Registered") : Ok(list);
        }

        [HttpGet("Genre/{Genre}")]
        public IActionResult ListByGenre(string Genre)
        {
            var list = movieRepository.MovieByGenre(Genre);
            return (list.Count() == 0) ? NotFound("This Genre of Movies is Not Registered") : Ok(list);
        }

        [HttpGet("{movieId}")]

        public ActionResult GetById(int movieId)
        {
            var movie = movieRepository.MovieById(movieId);
            return (movie == null) ? NotFound("This Movie Id is Not Registered") : Ok(movie);
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

        public IActionResult OutPut(Messages result)
        {
            switch (result.Status)
            {
                case Statuses.BadRequest:
                    return BadRequest(result.Message);
                case Statuses.NotFound:
                    return NotFound(result.Message);
                case Statuses.Conflict:
                    return Conflict(result.Message);
            }
            return Ok(result.Message);
        }
    }
}
