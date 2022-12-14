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
            return (show == null) ? NotFound("This Schedule Id is not Registered") : Ok(show);
        }

        [HttpGet("Movie/{id}")]
        public IActionResult ListByMovieId(int id)
        {
            var schedule= scheduleRepository.SchedulesByMovieId(id);
            if (schedule.Count() == 0)
            {
                return NotFound("Movie is Not Registered in any Schedules");
            }
            return Ok(schedule);
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
            if (schedule == null)
            {
                return NotFound("Schedule Id(3) is not found");
            }
            var result = scheduleRepository.DeleteSchedule(id);
            if (result.Success == false)
            {
                return BadRequest(result.Message);
                
            }
            return Ok(result.Message);
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
