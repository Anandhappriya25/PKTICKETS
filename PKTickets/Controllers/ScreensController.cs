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
            return (screen == null) ? NotFound("This Screen Id is not Registered") : Ok(screen);
        }


        [HttpPost("")]

        public IActionResult Add(Screen screen)
        {
            var result = screenRepository.AddScreen(screen);
            return (result.Success == false) ? OutPut(result) :
                Created($"{TimingConvert.LocalHost("Screens")}{ screen.ScreenId}", result.Message);
        }


        [HttpPut("")]
        public IActionResult Update(Screen screen)
        {
            var result = screenRepository.UpdateScreen(screen);
            return OutPut(result);
        }


        [HttpDelete("{screenId}")]
        public IActionResult Remove(int screenId)
        {
            var result = screenRepository.RemoveScreen(screenId);
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
