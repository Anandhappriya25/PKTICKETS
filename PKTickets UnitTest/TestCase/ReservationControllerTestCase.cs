using Blazorise;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.Common.Interfaces;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using PKTickets.Controllers;
using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PKTickets_UnitTest.TestCase
{
    public class ReservationControllerTestCase
    {
        private Mock<IReservationRepository> Mock()
        {
            var mockservice = new Mock<IReservationRepository>();
            return mockservice;
        }
        private Mock<IReservationRepository> GetAllMock(List<Reservation> reservation)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.ReservationList()).Returns(reservation);
            return mockservice;
        }
        private Mock<IReservationRepository> GetByIdMock(Reservation reservation)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.ReservationById(It.IsAny<int>())).Returns(reservation);
            return mockservice;
        }
        private Mock<IReservationRepository> AddMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.CreateReservation(It.IsAny<Reservation>())).Returns(message);
            return mockservice;
        }
        private Mock<IReservationRepository> UpdateMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.UpdateReservation(It.IsAny<Reservation>())).Returns(message);
            return mockservice;
        }
        private Mock<IReservationRepository> DeleteMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.DeleteReservation(It.IsAny<int>())).Returns(message);
            return mockservice;
        }
        private Reservation TestReservation => new()
        { UserId = 3, ReservationId = 1, ScheduleId = 2, PremiumTickets = 3, EliteTickets = 3, IsActive = true };


        [Fact]
        public void List_Ok()
        {
            List<Reservation> reservations = new List<Reservation>();
            reservations.Add(TestReservation);
            var controller = new ReservationsController(GetAllMock(reservations).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Reservation>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(reservations, lists);
            Assert.NotEmpty(lists);
            Assert.StrictEqual(reservations.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void List_NullOk()
        {
            List<Reservation> reservations = new List<Reservation>();
            var controller = new ReservationsController(GetAllMock(reservations).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Reservation>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(reservations, lists);
            Assert.Empty(lists);
            Assert.StrictEqual(reservations.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }
        [Fact]
        public void GetById_ok()
        {
            var controller = new ReservationsController(GetByIdMock(TestReservation).Object);
            var okResult = controller.GetById(1);
            var list = okResult as OkObjectResult;
            var result = list.Value as Reservation;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(TestReservation.UserId, result.UserId);
            Assert.NotNull(result);
            Assert.StrictEqual(1, result.ReservationId);
            Assert.True(result.IsActive);
            Assert.StrictEqual(200, list.StatusCode);
        }
        [Fact]
        public void GetById_Null()
        {
            Reservation reservation = null;
            var controller = new ReservationsController(GetByIdMock(reservation).Object);
            var result = controller.GetById(3);
            var check = result as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, check.StatusCode);
            Assert.Equal("This Reservation Id is Not Registered", check.Value);
        }

        [Fact]
        public void Add_ScheduleNotFound()
        {
            Messages message = new Messages();
            TestReservation.ScheduleId = 0;
            message.Message = "Schedule Id is Not found";
            message.Success = false;
            var controller = new ReservationsController(AddMock(message).Object);
            var output = controller.Add(TestReservation);
            var result = output as NotFoundObjectResult;
            Assert.Equal("Schedule Id is Not found", result.Value);
            Assert.StrictEqual(404, result.StatusCode);
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.True(message.Success == false);
        }

        [Fact]
        public void Add_UserNotFound()
        {
            Messages message = new Messages();
            TestReservation.UserId = 0;
            message.Message = "User Id is Not found";
            message.Success = false;
            var controller = new ReservationsController(AddMock(message).Object);
            var output = controller.Add(TestReservation);
            var result = output as NotFoundObjectResult;
            Assert.Equal("User Id is Not found", result.Value);
            Assert.StrictEqual(404, result.StatusCode);
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.True(message.Success==false);
        }

        [Fact]
        public void Add_BadRequest()
        {
            Messages message = new Messages();
            TestReservation.PremiumTickets = 0;
            TestReservation.EliteTickets = 0;
            message.Message = "Please reaserve atleast a seat";
            message.Success = false;
            var controller = new ReservationsController(AddMock(message).Object);
            var output = controller.Add(TestReservation);
            var result = output as BadRequestObjectResult;
            Assert.Equal("Please reaserve atleast a seat", result.Value);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.False(message.Success);
        }
        [Fact]
        public void Add_PremiumTicketBadRequest()
        {
            Messages message = new Messages();
            message.Message = "Only 3 Premium Tickets are Available";
            message.Success = false;
            var controller = new ReservationsController(AddMock(message).Object);
            var output = controller.Add(TestReservation);
            var result = output as BadRequestObjectResult;
            Assert.Equal("Only 3 Premium Tickets are Available", result.Value);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.False(message.Success);
        }

        [Fact]
        public void Add_EliteTicketBadRequest()
        {
            Messages message = new Messages();
            message.Message = "Only 3 Elite Tickets are Available";
            message.Success = false;
            var controller = new ReservationsController(AddMock(message).Object);
            var output = controller.Add(TestReservation);
            var result = output as BadRequestObjectResult;
            Assert.Equal("Only 3 Elite Tickets are Available", result.Value);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.False(message.Success);
        }

        [Fact]
        public void Add_UpToTimeBadRequest()
        {
            Messages message = new Messages();
            message.Message = "You are UpTo Time ,so can't Book The Reservation";
            message.Success = false;
            var controller = new ReservationsController(AddMock(message).Object);
            var output = controller.Add(TestReservation);
            var result = output as BadRequestObjectResult;
            Assert.Equal("You are UpTo Time ,so can't Book The Reservation", result.Value);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.False(message.Success);
        }
        
        [Fact]
        public void Add_Success()
        {
            Messages message = new Messages();
            message.Message = "Successfully 6 Tickets are Reserved";
            message.Success = true;
            var controller = new ReservationsController(AddMock(message).Object);
            var output = controller.Add(TestReservation);
            var result = output as CreatedResult;
            Assert.IsType<CreatedResult>(output);
            Assert.StrictEqual("Successfully 6 Tickets are Reserved", result.Value);
            Assert.StrictEqual("https://localhost:7221/api/Reservations/1", result.Location);
            Assert.StrictEqual(201, result.StatusCode);
        }

        //[Fact]
        //public void Update_BadRequest()
        //{
        //    User newUser = new User { UserId = 0 };
        //    var controller = new ReservationsController(Mock().Object);
        //    var output = controller.Update(newUser);
        //    var result = output as BadRequestObjectResult;
        //    Assert.IsType<BadRequestObjectResult>(output);
        //    Assert.StrictEqual("Enter the User Id field", result.Value);
        //    Assert.StrictEqual(400, result.StatusCode);
        //    Assert.True(newUser.UserId == 0);
        //}
        //[Fact]
        //public void Update_NotFound()
        //{
        //    Messages message = new Messages();
        //    message.Message = "User Id is not found";
        //    message.Success = false;
        //    var controller = new ReservationsController(UpdateMock(message).Object);
        //    var output = controller.Update(TestUser);
        //    var result = output as NotFoundObjectResult;
        //    Assert.IsType<NotFoundObjectResult>(output);
        //    Assert.StrictEqual("User Id is not found", result.Value);
        //    Assert.StrictEqual(404, result.StatusCode);
        //}

        //[Fact]
        //public void Update_PhoneConflict()
        //{
        //    Messages message = new Messages();
        //    message.Message = "The (9443004834), PhoneNumber is already Registered.";
        //    message.Success = false;
        //    var controller = new ReservationsController(UpdateMock(message).Object);
        //    var output = controller.Update(TestUser);
        //    var result = output as ConflictObjectResult;
        //    Assert.Equal("The (9443004834), PhoneNumber is already Registered.", result.Value);
        //    Assert.StrictEqual(409, result.StatusCode);
        //    Assert.IsType<ConflictObjectResult>(output);
        //}
        //[Fact]
        //public void Update_EmailConflict()
        //{
        //    Messages message = new Messages();
        //    message.Message = "The (karth56@gmail.com), EmailId is already Registered.";
        //    message.Success = false;
        //    var controller = new ReservationsController(UpdateMock(message).Object);
        //    var output = controller.Update(TestUser);
        //    var result = output as ConflictObjectResult;
        //    Assert.Equal("The (karth56@gmail.com), EmailId is already Registered.", result.Value);
        //    Assert.StrictEqual(409, result.StatusCode);
        //    Assert.IsType<ConflictObjectResult>(output);
        //}

        //[Fact]
        //public void Update_SuccessOk()
        //{
        //    Messages message = new Messages();
        //    message.Message = "The Vijay Account is Successfully Updated";
        //    message.Success = true;
        //    var controller = new ReservationsController(UpdateMock(message).Object);
        //    var output = controller.Update(TestUser);
        //    var result = output as OkObjectResult;
        //    Assert.Equal("The Vijay Account is Successfully Updated", result.Value);
        //    Assert.StrictEqual(200, result.StatusCode);
        //    Assert.IsType<OkObjectResult>(output);
        //}

        //[Fact]
        //public void Remove_SucessOk()
        //{Messages message = new Messages();
        //    message.Message = "The Vijay Account is Successfully removed";
        //    message.Success = true;
        //    var controller = new ReservationsController(DeleteMock(message).Object);
        //    var output = controller.Remove(3);
        //    Assert.IsType<OkObjectResult>(output);
        //    var result = output as OkObjectResult;
        //    Assert.Equal("The Vijay Account is Successfully removed", result.Value);
        //    Assert.StrictEqual(200, result.StatusCode);

        //}

        //[Fact]
        //public void Remove_NotFound()
        //{
        //    Messages message = new Messages();
        //    message.Message = "User Id (3) is not found";
        //    message.Success = false;
        //    var controller = new ReservationsController(DeleteMock(message).Object);
        //    var output = controller.Remove(3);
        //    var result = output as NotFoundObjectResult;
        //    Assert.IsType<NotFoundObjectResult>(output);
        //    Assert.StrictEqual("User Id (3) is not found", result.Value);
        //    Assert.StrictEqual(404, result.StatusCode);
        //}
    }
}
