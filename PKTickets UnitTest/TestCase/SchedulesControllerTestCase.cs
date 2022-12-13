using Blazorise;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
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
    public class SchedulesControllerTestCase
    {
        private Mock<IScheduleRepository> Mock()
        {
            var mockservice = new Mock<IScheduleRepository>();
            return mockservice;
        }
        private Mock<IScheduleRepository> GetAllMock(List<Schedule> schedules)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.SchedulesList()).Returns(schedules);
            return mockservice;
        }
        private Mock<IScheduleRepository> GetByIdMock(Schedule schedule)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.ScheduleById(It.IsAny<int>())).Returns(schedule);
            return mockservice;
        }
        private Mock<IScheduleRepository> AddMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.CreateSchedule(It.IsAny<Schedule>())).Returns(message);
            return mockservice;
        }
        private Mock<IScheduleRepository> UpdateMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.UpdateSchedule(It.IsAny<Schedule>())).Returns(message);
            return mockservice;
        }
        private Mock<IScheduleRepository> DeleteMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.DeleteSchedule(It.IsAny<int>())).Returns(message);
            return mockservice;
        }
        private Schedule TestScreen => new()
        { ScheduleId = 3, };

        [Fact]
        public void List_Ok()
        {
            Screen screen1 = new Screen { ScreenId = 3, ScreenName = "Vijay", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250, IsActive = true };
            Screen screen2 = new Screen { ScreenId = 2, ScreenName = "theri", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250, IsActive = true };
            List<Screen> screens = new List<Screen>();
            screens.Add(screen2);
            screens.Add(screen1);
            var controller = new ScreensController(GetAllMock(screens).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Screen>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(screens, lists);
            Assert.NotEmpty(lists);
            Assert.StrictEqual(screens.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }
        [Fact]
        public void List_NullOk()
        {
            List<Screen> screens = new List<Screen>();
            var controller = new ScreensController(GetAllMock(screens).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Screen>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(screens, lists);
            Assert.Empty(lists);
            Assert.StrictEqual(screens.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }
        [Fact]
        public void GetById_ok()
        {

            var controller = new ScreensController(GetByIdMock(TestScreen).Object);
            var okResult = controller.GetById(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as Screen;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(TestScreen.ScreenName, result.ScreenName);
            Assert.NotNull(result);
            Assert.StrictEqual(3, result.ScreenId);
            Assert.True(result.IsActive);
            Assert.StrictEqual(200, list.StatusCode);
        }
        [Fact]
        public void GetById_Null()
        {
            Screen screen = null;
            var controller = new ScreensController(GetByIdMock(screen).Object);
            var result = controller.GetById(3);
            var check = result as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, check.StatusCode);
            Assert.Null(screen);
            Assert.Equal("This Screen Id is not Registered", check.Value);
        }

        [Fact]
        public void Add_TheaterIdConflict()
        {
            Messages message = new Messages();
            message.Message = "Theater Id(1) is Not Registered.";
            message.Success = false;
            var controller = new ScreensController(AddMock(message).Object);
            var output = controller.Add(TestScreen);
            var result = output as ConflictObjectResult;
            Assert.Equal("Theater Id(1) is Not Registered.", result.Value);
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }

        [Fact]
        public void Add_NameConflict()
        {
            Screen newscreen = new Screen { ScreenId = 3, ScreenName = "Vijay", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250, IsActive = true };
            Messages message = new Messages();
            message.Message = "Screen Name(Vijay) is Already Registered.";
            message.Success = false;
            var controller = new ScreensController(AddMock(message).Object);
            var output = controller.Add(TestScreen);
            var result = output as ConflictObjectResult;
            Assert.Equal("Screen Name(Vijay) is Already Registered.", result.Value);
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }

        [Fact]
        public void Add_Success()
        {
            Messages message = new Messages();
            message.Message = "Screen (Vijay) is succssfully Added";
            message.Success = true;
            var controller = new ScreensController(AddMock(message).Object);
            var output = controller.Add(TestScreen);
            var result = output as CreatedResult;
            Assert.IsType<CreatedResult>(output);
            Assert.StrictEqual("Screen (Vijay) is succssfully Added", result.Value);
            Assert.StrictEqual("https://localhost:7221/api/Screens/3", result.Location);
            Assert.StrictEqual(201, result.StatusCode);
        }

        [Fact]
        public void Update_BadRequest()
        {
            Screen screen = new Screen { ScreenId = 0 };
            var controller = new ScreensController(Mock().Object);
            var output = controller.Update(screen);
            var result = output as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.StrictEqual("Enter the Screen Id field", result.Value);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.True(screen.ScreenId == 0);
        }
        [Fact]
        public void Update_NotFound()
        {
            Messages message = new Messages();
            message.Message = "Screen Id is not found";
            message.Success = false;
            var controller = new ScreensController(UpdateMock(message).Object);
            var output = controller.Update(TestScreen);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual("Screen Id is not found", result.Value);
            Assert.StrictEqual(404, result.StatusCode);
        }

        [Fact]
        public void Update_NameConflict()
        {
            Messages message = new Messages();
            message.Message = "Screen Name(Vijay) is Already Registered.";
            message.Success = false;
            var controller = new ScreensController(UpdateMock(message).Object);
            var output = controller.Update(TestScreen);
            var result = output as ConflictObjectResult;
            Assert.Equal("Screen Name(Vijay) is Already Registered.", result.Value);
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }

        [Fact]
        public void Update_SuccessOk()
        {
            Messages message = new Messages();
            message.Message = "Screen Vijay is succssfully Updated";
            message.Success = true;
            var controller = new ScreensController(UpdateMock(message).Object);
            var output = controller.Update(TestScreen);
            var result = output as OkObjectResult;
            Assert.Equal("Screen Vijay is succssfully Updated", result.Value);
            Assert.StrictEqual(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(output);
        }

        [Fact]
        public void Remove_SucessOk()
        {
            Messages message = new Messages();
            message.Message = "Screen (Vijay) is succssfully Removed";
            message.Success = true;
            var controller = new ScreensController(DeleteMock(message).Object);
            var output = controller.Remove(3);
            Assert.IsType<OkObjectResult>(output);
            var result = output as OkObjectResult;
            Assert.Equal("Screen (Vijay) is succssfully Removed", result.Value);
            Assert.StrictEqual(200, result.StatusCode);

        }

        [Fact]
        public void Remove_IdNotFound()
        {
            Messages message = new Messages();
            message.Message = "Screen Id(3) is not found";
            message.Success = false;
            var controller = new ScreensController(DeleteMock(message).Object);
            var output = controller.Remove(3);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual("Screen Id(3) is not found", result.Value);
            Assert.StrictEqual(404, result.StatusCode);
        }
        [Fact]
        public void Remove_AlreadyStarted()
        {
            Messages message = new Messages();
            message.Message = "This Screen(3) is Already scheduled, so you can't delete the screen";
            message.Success = false;
            var controller = new ScreensController(DeleteMock(message).Object);
            var output = controller.Remove(3);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual("This Screen(3) is Already scheduled, so you can't delete the screen", result.Value);
            Assert.StrictEqual(404, result.StatusCode);
        }
    }
}

