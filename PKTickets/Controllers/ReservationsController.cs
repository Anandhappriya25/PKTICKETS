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
    public class ReservationsController : ControllerBase
    {

        private readonly IReservationRepository reservationRepository;

        public ReservationsController(IReservationRepository _reservationRepository)
        {
            reservationRepository = _reservationRepository;
        }


        [HttpGet("")]
        public IActionResult List()
        {
            return Ok(reservationRepository.ReservationList());
        }

        [HttpGet("ScheduleId/{id}")]
        public IActionResult ListByScheduleId(int id)
        {
            var schedule=reservationRepository.ScheduleById(id);
            if(schedule==null)
            {
                return NotFound("Schedule Id is notfound");
            }
            return Ok(reservationRepository.ReservationsByShowId(id));
        }

        [HttpGet("{id}")]

        public ActionResult GetById(int id)
        {
            var reservation = reservationRepository.ReservationById(id);
            if (reservation == null)
            {
               return NotFound();
            }
            return Ok(reservation);
        }


        [HttpPost("")]

        public IActionResult Add(Reservation reservation)
        {

            var result = reservationRepository.CreateReservation(reservation);
            if (result.Message == "Atleast Please reaserve a seat")
            {
                return BadRequest(result.Message);
            }
            else if (result.Success == false)
            {
                return NotFound(result.Message);
            }
            return Created(""+ TimingConvert.LocalHost("Reservations") + reservation.ReservationId + "", result.Message);
        }


        [HttpPut("")]
        public ActionResult Update(Reservation reservation)
        {
            if (reservation.ReservationId == 0)
            {
                return BadRequest("Enter the Reservation Id field");
            }
            var result = reservationRepository.UpdateReservation(reservation);
            if (result.Message == "Reservation Id is Not found")
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

        public IActionResult Cancel(int id)
        {
            var result = reservationRepository.DeleteReservation(id);
            if (result.Message == "Reservation Id is not found")
            {
                return NotFound(result.Message);
            }
            else if (result.Success == false)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }
        [HttpGet("UserId/{id}")]
        public IActionResult ListByUserId(int id)
        {
            var user=reservationRepository.UserById(id);
            if(user == null)
            {
                return NotFound("User Id is notfound");
            }
            return Ok(reservationRepository.ReservationsByUserId(id));
        }

        [HttpPost("Multiple")]
        public IActionResult Multiple(MultipleBookingDTO obj)
        {
            var user = reservationRepository.UserById(obj.UserId);
            if (user == null)
            {
                return NotFound("User Id is notfound");
            }
            return Ok(reservationRepository.MultipleBooking(obj));
        }
    }
}
