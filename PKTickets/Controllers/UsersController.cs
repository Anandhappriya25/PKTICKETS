using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;

namespace PKTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserRepository userRepository;

        public UsersController(IUserRepository _userRepository)
        {
            userRepository = _userRepository;
        }


        [HttpGet("")]
        public IActionResult List()
        {
            return Ok(userRepository.GetAllUsers());
        }

        [HttpGet("{userId}")]

        public ActionResult GetById(int userId)
        {
            var user = userRepository.UserById(userId);
            return (user == null) ? NotFound("This User Id is not Registered") : Ok(user);            
        }


        [HttpPost("")]

        public IActionResult Add(User user)
        {
            var result = userRepository.CreateUser(user);
            return (result.Status == Statuses.Created) ? Created($"{TimingConvert.LocalHost("Users")}{user.UserId}", result.Message) : OutPut(result);            
        }


        [HttpPut("")]
        public IActionResult Update(User user)
        {            
            var result = userRepository.UpdateUser(user);
            return OutPut(result);
        }


        [HttpDelete("{userId}")]

        public IActionResult Remove(int userId)
        {
            var result = userRepository.DeleteUser(userId);
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
