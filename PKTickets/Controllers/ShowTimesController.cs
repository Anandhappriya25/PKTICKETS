using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PKTickets.Interfaces;
using PKTickets.Models.DTO;
using PKTickets.Models;
using PKTickets.Repository;

namespace PKTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowTimesController : ControllerBase
    {
        private readonly IShowTimeRepository showTimeRepository;

        public ShowTimesController(IShowTimeRepository _showTimeRepository)
        {
            showTimeRepository = _showTimeRepository;
        }

        [HttpGet("")]
        public IActionResult List()
        {
            return Ok(showTimeRepository.GetAllShowTimes());
        }

        [HttpGet("{showTimeId}")]

        public ActionResult GetById(int showTimeId)
        {
            var showTime = showTimeRepository.ShowTimeasStringById(showTimeId);
            if (showTime == null)
            {
                return NotFound("This ShowTime Id is not registered");
            }
            return Ok(showTime);
        }


        [HttpPost("")]
        public IActionResult Add(ShowTimeDTO showTime)
        {
            var result = showTimeRepository.CreateShowTime(showTime);
            if (result.Status == Statuses.Conflict)
            {
                return Conflict(result.Message);
            }
            return Created("" + TimingConvert.LocalHost("ShowTimes") + showTime.ShowTimeId + "", result.Message);
        }


        [HttpPut("")]
        public ActionResult Update(ShowTimeDTO showTime)
        {
           
            var result = showTimeRepository.UpdateShowTime(showTime);
            if (result.Status == Statuses.BadRequest)
            {
                return BadRequest(result.Message);
            }
           else if (result.Status == Statuses.NotFound)
            {
                return NotFound(result.Message);
            }
            else if (result.Status == Statuses.Conflict)
            {
                return Conflict(result.Message);
            }
            return Ok(result.Message);
        }


    

    }
}
