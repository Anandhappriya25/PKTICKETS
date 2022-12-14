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
            return (theater == null) ? NotFound("This Theater Id is not Registered") : Ok(theater);
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
            return (result.Status == Statuses.Created) ? Created($"{TimingConvert.LocalHost("Theaters")}{theater.TheaterId}", result.Message) :
             Conflict(result.Message) ;
        
        }

        [HttpPut("")]
        public IActionResult Update(Theater theater)
        {
            var result = theaterRepository.UpdateTheater(theater);
            return OutPut(result);
        }


        [HttpDelete("{theaterId}")]
        public IActionResult Remove(int theaterId)
        {
            var result = theaterRepository.DeleteTheater(theaterId);
            return OutPut(result);
        }

        [HttpGet("{id}/Screens")]
        public IActionResult GetScreensByTheaterId(int id)
        {
            var theater = theaterRepository.TheaterScreens(id);
            return (theater.TheaterName == null) ? NotFound("The Theater Id is NotFound") : Ok(theater);
        }

        [HttpGet("{id}/Schedules")]
        public IActionResult ListByTheaterId(int id)
        {
            var theater = theaterRepository.TheaterSchedulesById(id);
            return (theater.TheaterName == null) ? NotFound("The Theater Id is NotFound") : Ok(theater);
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
