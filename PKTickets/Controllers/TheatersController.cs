using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using PKTickets.Repository;
using System;

namespace PKTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheatersController : ControllerBase
    {

        private readonly ITheaterRepository theaterRepository;

        public TheatersController(ITheaterRepository _theaterRepository)
        {
            theaterRepository = _theaterRepository;
        }


        [HttpGet("")]
        public IActionResult List()
        {
            return Ok(theaterRepository.GetTheaters());
        }

        [HttpGet("{theaterId}")]

        public ActionResult GetById(int theaterId)
        {
            var theater = theaterRepository.TheaterById(theaterId);
            if (theater == null)
            {
                return NotFound("This Theater Id is not Registered");
            }
            return Ok(theater);
        }

        [HttpGet("Location/{location}")]

        public ActionResult GetByLocation(string location)
        {
            return Ok(theaterRepository.TheaterByLocation(location));
        }


        [HttpPost("")]

        public IActionResult Add(Theater theater)
        {
            var result = theaterRepository.CreateTheater(theater);
            if (result.Status == Statuses.Conflict)
            {
                return Conflict(result.Message);
            }
            return Created("" + TimingConvert.LocalHost("Theaters") + theater.TheaterId +"", result.Message);
        }


        [HttpPut("")]
        public ActionResult Update(Theater theater)
        {
            var result = theaterRepository.UpdateTheater(theater);
            if (result.Status == Statuses.NotFound)
            {
                return NotFound(result.Message);
            }
            else if (result.Status == Statuses.Conflict)
            {
                return Conflict(result.Message);
            }
            return Ok(result.Message);
        }


        [HttpDelete("{theaterId}")]
        public IActionResult Remove(int theaterId)
        {
            var result = theaterRepository.DeleteTheater(theaterId);
            if (result.Status == Statuses.NotFound)
            {
                return NotFound(result.Message);
            }
            else if (result.Status == Statuses.Conflict)
            {
                return Conflict(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpGet("{id}/Screens")]
        public IActionResult GetScreensByTheaterId(int id)
        {
            var theater = theaterRepository.TheaterScreens(id);
            if (theater.TheaterName == null)
            {
                return NotFound("This Theater Id is not Registered");
            }
            return Ok(theater);
        }

        [HttpGet("{id}/Schedules")]
        public IActionResult ListByTheaterId(int id)
        {
            var theater = theaterRepository.TheaterSchedulesById(id);
            if (theater.TheaterName == null)
            {
                return NotFound("Theater Id is notfound");
            }
            return Ok(theater);
        }
    }
}
