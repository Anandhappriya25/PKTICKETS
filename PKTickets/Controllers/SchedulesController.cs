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


        [HttpGet]
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
            return (schedule.Count() == 0) ? NotFound("Movie is Not Registered in any Schedules") : Ok(schedule);
        }

        [HttpPost]
        public IActionResult Add(Schedule schedule)
        {
            var result = scheduleRepository.CreateSchedule(schedule);
            return (result.Status == Statuses.Created) ? Created($"{TimingConvert.LocalHost("Schedules")}{schedule.ScheduleId}", result.Message) :
               OutPut(result);
        }

        [HttpPut]
        public IActionResult Update(Schedule schedule)
        {
            var result = scheduleRepository.UpdateSchedule(schedule);
            return OutPut(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
        {
            var result = scheduleRepository.DeleteSchedule(id);
            return OutPut(result);
        }       

        [HttpGet("Movie/{id}")]
        public IActionResult DetailsByMovieId(int id)
        {
            var list = scheduleRepository.DetailsByMovieId(id);
            return (list.MovieName == null) ? NotFound("Movie Id is Not Found") : Ok(list);
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
