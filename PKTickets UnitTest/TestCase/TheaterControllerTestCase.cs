using Blazorise;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using PKTickets.Controllers;
using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKTickets_UnitTest.TestCase
{
    public class TheaterControllerTestCase
    {
        private Mock<ITheaterRepository> Mock()
        {
            var mockservice = new Mock<ITheaterRepository>();
            return mockservice;
        }
        private Mock<ITheaterRepository> GetAllMock(List<Theater> theaters)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.GetTheaters()).Returns(theaters);
            return mockservice;
        }
        private Mock<ITheaterRepository> GetByIdMock(Theater theater)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.TheaterById(It.IsAny<int>())).Returns(theater);
            return mockservice;
        }
        
        private Mock<ITheaterRepository> GetByLocationMock(string s,List<Theater> theaters)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.TheaterByLocation(It.IsAny<string>())).Returns(theaters);
            return mockservice;
        }

        private Mock<ITheaterRepository> AddMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.CreateTheater(It.IsAny<Theater>())).Returns(message);
            return mockservice;
        }
        private Mock<ITheaterRepository> UpdateMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.UpdateTheater(It.IsAny<Theater>())).Returns(message);
            return mockservice;
        }
        private Mock<ITheaterRepository> RemoveMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.DeleteTheater(It.IsAny<int>())).Returns(message);
            return mockservice;
        }
        private Theater TestTheater => new()
        { TheaterId = 3, TheaterName = "Vijaya Cinemas", Location = "Maduravoyal", IsActive = true };
        
        [Fact]
        public void TheatersList_Ok()
        {
            List<Theater> theaters = new List<Theater>();
            theaters.Add(TestTheater);
            var controller = new TheatersController(GetAllMock(theaters).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Theater>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(theaters, lists);
            Assert.StrictEqual(theaters.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void List_NullOk()
        {
            List<Theater> theaters = new List<Theater>();
            var controller = new TheatersController(GetAllMock(theaters).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Theater>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(theaters, lists);
            Assert.Empty(lists);
            Assert.StrictEqual(theaters.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void GetById_OK()
        {
            var controller = new TheatersController(GetByIdMock(TestTheater).Object);
            var okResult = controller.GetById(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as Theater;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(TestTheater.TheaterName, result.TheaterName);
            Assert.NotNull(result);
            Assert.NotEqual(9, result.TheaterId);
            Assert.True(result.IsActive);
            Assert.StrictEqual(200, list.StatusCode);
            Assert.StrictEqual(TestTheater.TheaterId, result.TheaterId);
        }

        [Fact]
        public void GetById_NotFound()
        {
            Theater theater = null;
            var controller = new TheatersController(GetByIdMock(theater).Object);
            var result = controller.GetById(30);
            var check = result as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Null(theater);
            Assert.Equal(404, check.StatusCode);
        }

        [Fact]
        public void GetByLocation_Success()
        {
            List<Theater> theaters = new List<Theater>();
            var controller = new TheatersController(GetByLocationMock("Vellore",theaters).Object);
            var okResult = controller.GetByLocation("Vellore");
            var list = okResult as OkObjectResult;
            var results = list.Value as List<Theater>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(theaters, results);
            Assert.StrictEqual(200, list.StatusCode);
            Assert.StrictEqual(theaters.Count(), results.Count());
        }

        [Fact]
        public void GetByLocation_NullOk()
        {
            List<Theater> theaters = new List<Theater>();
            var controller = new TheatersController(GetByLocationMock("Kolathur",theaters).Object);
            var okResult = controller.GetByLocation("Vellore");
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Theater>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.StrictEqual(theaters.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void Add_Success()
        {
            Messages message = new Messages();
            message.Success = true;
            message.Status = Statuses.Created;
            var controller = new TheatersController(AddMock(message).Object);
            var output = controller.Add(TestTheater);
            var result = output as CreatedResult;
            Assert.IsType<CreatedResult>(output);
            Assert.StrictEqual("https://localhost:7221/api/Theaters/3", result.Location);
            Assert.StrictEqual(201, result.StatusCode);
        }

        [Fact]
        public void Add_NameConflict()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.Conflict;
            var controller = new TheatersController(AddMock(message).Object);
            var output = controller.Add(TestTheater);
            var result = output as ConflictObjectResult;
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }
        [Fact]
        public void Update_BadRequest()
        {
            Theater theater = new Theater { TheaterId = 0 };
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.BadRequest;
            var controller = new TheatersController(UpdateMock(message).Object);
            var output = controller.Update(theater);
            var result = output as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.True(theater.TheaterId == 0);
        }
        [Fact]
        public void Update_NotFound()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.NotFound;
            var controller = new TheatersController(UpdateMock(message).Object);
            var output = controller.Update(TestTheater);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual(message.Message, result.Value);
            Assert.NotEqual("Theater Id is not founds", result.Value);
            Assert.StrictEqual(404, result.StatusCode);
        }

        [Fact]
        public void Update_NameConflict()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.Conflict;
            var controller = new TheatersController(UpdateMock(message).Object);
            var output = controller.Update(TestTheater);
            var result = output as ConflictObjectResult;
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }
        
        [Fact]
        public void Update_SuccessOk()
        {
            Messages message = new Messages();
            message.Success = true;
            message.Status = Statuses.Success;
            var controller = new TheatersController(UpdateMock(message).Object);
            var output = controller.Update(TestTheater);
            var result = output as OkObjectResult;
            Assert.StrictEqual(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(output);
        }
        [Fact]
        public void Remove_SucessOk()
        {
            Messages message = new Messages();
            message.Success = true;
            message.Status = Statuses.Success;
            var controller = new TheatersController(RemoveMock(message).Object);
            var output = controller.Remove(9);
            Assert.IsType<OkObjectResult>(output);
            var result = output as OkObjectResult;
            Assert.StrictEqual(200, result.StatusCode);

        }

        [Fact]
        public void Remove_NotFound()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.NotFound;
            var controller = new TheatersController(RemoveMock(message).Object);
            var output = controller.Remove(3);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual(404, result.StatusCode);
        }

        [Fact]
        public void GetScreensByTheaterId_Ok()
        {
            ScreensListDTO screensListDTO = new ScreensListDTO() { TheaterName = "Vijaya Cinemas", ScreensCount = 1 };
            List<ScreensDTO> screens = new List<ScreensDTO>();
            ScreensDTO screen = new ScreensDTO()
            { ScreenId = 3, ScreenName = "Vijay", PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250 };
            screens.Add(screen);
            screensListDTO.Screens = screens;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterById(It.IsAny<int>())).Returns(TestTheater);
            mockservice.Setup(x => x.TheaterScreens(It.IsAny<int>())).Returns(screensListDTO);
            var controller = new TheatersController(mockservice.Object);
            var okResult = controller.GetScreensByTheaterId(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as ScreensListDTO;
            Assert.IsType<OkObjectResult>(list);
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void GetScreensByTheaterId_NullOk()
        {
            ScreensListDTO screensListDTO = new ScreensListDTO();
            screensListDTO.TheaterName = "priya";
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterScreens(It.IsAny<int>())).Returns(screensListDTO);
            var controller = new TheatersController(mockservice.Object);
            var okResult = controller.GetScreensByTheaterId(3);
            var list = okResult as OkObjectResult;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void GetScreensByTheaterId_NotFound()
        {
            ScreensListDTO screensListDTO = new ScreensListDTO();
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterScreens(It.IsAny<int>())).Returns(screensListDTO);
            var controller = new TheatersController(mockservice.Object);
            var okResult = controller.GetScreensByTheaterId(3);
            var list = okResult as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(okResult);
            Assert.StrictEqual(404, list.StatusCode);
        }

        [Fact]
        public void ScheduleListByTheaterId_Ok()
        {
            TheatersSchedulesDTO theater = new TheatersSchedulesDTO();
            theater.TheaterName = "Vijaya Cinemas";
            theater.ScreensCount = 1;
            ScreenSchedulesDTO screens = new ScreenSchedulesDTO();
            screens.ScreenId = 1;
            screens.ScreenName = "Silver Screen";
            screens.PremiumCapacity = 150;
            screens.EliteCapacity = 100;
            List<SchedulesDTO> schedulesDTOs = new List<SchedulesDTO>();
            SchedulesDTO schedule = new SchedulesDTO();
            schedule.ScheduleId = 1;
            schedule.MovieName = "Theri";
            schedule.MovieId = 1;   
                schedule.Date= DateTime.Now;
            schedule.ShowTime = "3:00 pm";
            schedule.AvailablePremiumSeats = 150;
            schedule.AvailableEliteSeats = 100;
            schedulesDTOs.Add(schedule);
            screens.Schedules = schedulesDTOs;
            List<ScreenSchedulesDTO> screenSchedulesDTOs= new List<ScreenSchedulesDTO>();
            screenSchedulesDTOs.Add(screens);
            theater.Screens = screenSchedulesDTOs;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterSchedulesById(It.IsAny<int>())).Returns(theater);
            var controller = new TheatersController(mockservice.Object);
            var okResult = controller.ListByTheaterId(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as TheatersSchedulesDTO;
            Assert.IsType<OkObjectResult>(list);
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void ScheduleListByTheaterId_NullOk()
        {
            TheatersSchedulesDTO theater = new TheatersSchedulesDTO();
            theater.TheaterName = "hari";
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterSchedulesById(It.IsAny<int>())).Returns(theater);
            var controller = new TheatersController(mockservice.Object);
            var okResult = controller.ListByTheaterId(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as TheatersSchedulesDTO;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void ScheduleListByTheaterId_NotFound()
        {
            TheatersSchedulesDTO theater = new TheatersSchedulesDTO();
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterSchedulesById(It.IsAny<int>())).Returns(theater);
            var controller = new TheatersController(mockservice.Object);
            var okResult = controller.ListByTheaterId(3);
            var list = okResult as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(okResult);
            Assert.StrictEqual(404, list.StatusCode);
        }

    }
}
