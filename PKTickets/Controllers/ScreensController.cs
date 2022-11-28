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
    public class ScreensController : ControllerBase
    {

        private readonly IScreenRepository screenRepository;

        public ScreensController(IScreenRepository _screenRepository)
        {
            screenRepository = _screenRepository;
        }


        [HttpGet("")]
        public IActionResult List()
        {
            return Ok(screenRepository.GetAllScreens());
        }

        [HttpGet("{screenId}")]

        public ActionResult GetById(int screenId)
        {
            var screen = screenRepository.ScreenById(screenId);
            if (screen == null)
            {
               return NotFound();
            }
            return Ok(screen);
        }


        [HttpPost("")]

        public IActionResult Add(Screen screen)
        {
            var result = screenRepository.AddScreen(screen);
            if (result.Success == false)
            {
                return Conflict(result.Message);
            }
            return Created("" + TimingConvert.LocalHost("Screens") + screen.ScreenId + "", result.Message);
        }


        [HttpPut("")]
        public ActionResult Update(Screen screen)
        {
            if (screen.ScreenId == 0)
            {
                return BadRequest("Enter the Screen Id field");
            }
            var result = screenRepository.UpdateScreen(screen);
            if (result.Message == "Screen Id is not found")
            {
                return NotFound(result.Message);
            }
            else if (result.Success == false)
            {
                return Conflict(result.Message);
            }
            return Ok(result.Message);
        }


        [HttpDelete("{screenId}")]
        public IActionResult Remove(int screenId)
        {
            var result = screenRepository.RemoveScreen(screenId);
            if (result.Success == false)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpGet("Theater/{id}")]
        public IActionResult GetScreensByTheaterId(int id)
        {
            var theater=screenRepository.TheaterById(id);
            if(theater == null)
            {
                return NotFound();
            }
            return Ok(screenRepository.TheaterScreens(id));
        }

    }
}
