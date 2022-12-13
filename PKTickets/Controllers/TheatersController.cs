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
            if (result.Success == false)
            {
                return Conflict(result.Message);
            }
            return Created("" + TimingConvert.LocalHost("Theaters") + theater.TheaterId +"", result.Message);
        }


        [HttpPut("")]
        public ActionResult Update(Theater theater)
        {
            if (theater.TheaterId == 0)
            {
                return BadRequest("Enter the Theater Id field");
            }
            var result = theaterRepository.UpdateTheater(theater);
            if (result.Message == "Theater Id is not found")
            {
                return NotFound(result.Message);
            }
            else if (result.Success == false)
            {
                return Conflict(result.Message);
            }
            return Ok(result.Message);
        }


        [HttpDelete("{theaterId}")]

        public IActionResult Remove(int theaterId)
        {
            var result = theaterRepository.DeleteTheater(theaterId);
            if (result.Success == false)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Message);
        }
        [HttpGet("{id}/Screens")]
        public IActionResult GetScreensByTheaterId(int id)
        {
            var theater = theaterRepository.TheaterById(id);
            if (theater == null)
            {
                return NotFound("This Theater Id is not Registered");
            }
            return Ok(theaterRepository.TheaterScreens(id));
        }

        [HttpGet("{id}/Schedules")]
        public IActionResult ListByTheaterId(int id)
        {
            var theater = theaterRepository.TheaterById(id);
            if (theater == null)
            {
                return NotFound("Theater Id is notfound");
            }
            return Ok(theaterRepository.TheaterSchedulesById(id));
        }
    }
}
