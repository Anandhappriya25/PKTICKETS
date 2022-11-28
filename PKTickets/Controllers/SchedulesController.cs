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
    public class SchedulesController : ControllerBase
    {

        private readonly IScheduleRepository scheduleRepository;

        public SchedulesController(IScheduleRepository _showRepository)
        {
            scheduleRepository = _showRepository;
        }


        [HttpGet("")]
        public IActionResult List()
        {
            return Ok(scheduleRepository.SchedulesList());
        }
        

        [HttpGet("Available")]
        public IActionResult AvailableList()
        {
            return Ok(scheduleRepository.AvailableSchedulesList());
        }


        [HttpGet("{id}")]

        public ActionResult GetById(int id)
        {
            var show = scheduleRepository.ScheduleById(id);
            if (show == null)
            {
                return NotFound();
            }
            return Ok(show);
        }

        [HttpGet("Movie/{id}")]
        public IActionResult ListByMovieId(int id)
        {
            var movie= scheduleRepository.MovieById(id);
            if(movie == null)
            {
                return NotFound("Movie Id is Not Found");
            }
            return Ok(scheduleRepository.SchedulesByMovieId(id));
        }


        [HttpPost("")]
        public IActionResult Add(Schedule schedule)
        {
            var result = scheduleRepository.CreateSchedule(schedule);
            if (result.Success == false)
            {
                return Conflict(result.Message);
            }
            return Created(""+ TimingConvert.LocalHost("Schedules") + schedule.ScheduleId + "", result.Message);
        }


        [HttpPut("")]
        public ActionResult Update(Schedule schedule)
        {
            if (schedule.ScheduleId == 0)
            {
                return BadRequest("Enter the Schedule Id field");
            }
            var result = scheduleRepository.UpdateSchedule(schedule);
            if (result.Message == "The Schedule Id is not found")
            {
                return NotFound(result.Message);
            }
            else if (result.Success == false)
            {
                return Conflict(result.Message);
            }
            return Ok(result.Message);
        }



        [HttpDelete("{id}")]

        public IActionResult Remove(int id)
        {
            var schedule = scheduleRepository.ScheduleById(id);
            if(schedule == null)
            {
                return NotFound("The Schedule Id is notfound");
            }
            var result = scheduleRepository.DeleteSchedule(id);
            if (result.Success == false)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }


        [HttpGet("ScreenId/{id}")]
        public IActionResult ListByScreenId(int id)
        {
            var screen= scheduleRepository.ScreenById(id);
            if(screen == null)
            {
                return NotFound("Screen Id is notfound");
            }
            return Ok(scheduleRepository.SchedulesListByScreenId(id));
        }


        [HttpGet("TheaterId/{id}")]
        public IActionResult ListByTheaterId(int id)
        {
            var theater = scheduleRepository.TheaterById(id);
            if (theater == null)
            {
                return NotFound("Theater Id is notfound");
            }
            return Ok(scheduleRepository.TheaterSchedulesById(id));
        }

        [HttpGet("Details/Movie/{id}")]
        public IActionResult DetailsByMovieId(int id)
        {
            var movie = scheduleRepository.MovieById(id);
            if (movie == null)
            {
                return NotFound("Movie Id is Not Found");
            }
            return Ok(scheduleRepository.DetailsByMovieId(id));
        }
    }
}
