using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using PKTickets.Repository;
using System.Collections.Generic;

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
            var list= reservationRepository.ReservationsByShowId(id);
            return (list.Count() == 0) ? NotFound("This Shedule Id don't have any Reservations") : Ok(list);
        }

        [HttpGet("{id}")]

        public ActionResult GetById(int id)
        {
            var reservation = reservationRepository.ReservationById(id);
            return (reservation == null) ? NotFound("This Reservation Id is Not Registered") : Ok(reservation);
        }
        [HttpPost("")]

        public IActionResult Add(Reservation reservation)
        {

            var result = reservationRepository.CreateReservation(reservation);
            return (result.Status == Statuses.Created) ? Created($"{TimingConvert.LocalHost("Reservations")}{reservation.ReservationId}", result.Message) :
                OutPut(result);
        }


        [HttpPut("")]
        public IActionResult Update(Reservation reservation)
        {                            
            var result = reservationRepository.UpdateReservation(reservation);
            return OutPut(result);
        }


        [HttpDelete("{id}")]

        public IActionResult Cancel(int id)
        {
            var result = reservationRepository.DeleteReservation(id);
            return OutPut(result);
        }
        [HttpGet("UserId/{id}")]
        public IActionResult ListByUserId(int id)
        {
            var list= reservationRepository.ReservationsByUserId(id);
            return (list.UserName == null) ? NotFound("User Id is notfound") : Ok(list);
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
