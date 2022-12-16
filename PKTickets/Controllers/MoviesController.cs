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


        [HttpGet]
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

        public IActionResult GetById(int movieId)
        {
            var movie = movieRepository.MovieById(movieId);
            return (movie == null) ? NotFound("This Movie Id is Not Registered") : Ok(movie);
        }


        [HttpPost]
        public IActionResult Add(Movie movie)
        {
            var result = movieRepository.CreateMovie(movie);
            return (result.Status == Statuses.Created) ? Created($"{TimingConvert.LocalHost("Movies")}{movie.MovieId}", result.Message) :
                Conflict(result.Message);
        }


        [HttpPut("")]
        public IActionResult Update(Movie movie)
        {
            var result = movieRepository.UpdateMovie(movie);
            return OutPut(result);
        }


        [HttpDelete("{movieId}")]

        public IActionResult Remove(int movieId)
        {
            var result = movieRepository.DeleteMovie(movieId);
            return OutPut(result);
        }

        private IActionResult OutPut(Messages result)
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
