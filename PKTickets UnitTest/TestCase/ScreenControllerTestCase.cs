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
    public class ScreenControllerTestCase
    {
        private Mock<IScreenRepository> Mock()
        {
            var mockservice = new Mock<IScreenRepository>();
            return mockservice;
        }
        private Mock<IScreenRepository> GetAllMock(List<Screen> screens)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.GetAllScreens()).Returns(screens);
            return mockservice;
        }
        private Mock<IScreenRepository> GetByIdMock(Screen screen)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.ScreenById(It.IsAny<int>())).Returns(screen);
            return mockservice;
        }
        private Mock<IScreenRepository> AddMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.AddScreen(It.IsAny<Screen>())).Returns(message);
            return mockservice;
        }
        private Mock<IScreenRepository> UpdateMock(Messages message)
        {
            var mockservice = Mock(); 
            mockservice.Setup(x => x.UpdateScreen(It.IsAny<Screen>())).Returns(message);
            return mockservice;
        }
        private Mock<IScreenRepository> DeleteMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.RemoveScreen(It.IsAny<int>())).Returns(message);
            return mockservice;
        }
        private Screen TestScreen => new Screen()
        { ScreenId = 3, ScreenName = "Vijay", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250, IsActive = true };
        

            [Fact]
        public void List_Ok()
        {
            List<Screen> screens = new List<Screen>();
            screens.Add(TestScreen);
            var controller = new ScreensController(GetAllMock(screens).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Screen>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(screens, lists);
            Assert.NotEmpty(lists);
            Assert.StrictEqual(screens.Count(),lists.Count());
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
        public void GetById_NotFound()
        {
            Screen screen = null;
            var controller = new ScreensController(GetByIdMock(screen).Object);
            var result = controller.GetById(3);
            var check = result as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, check.StatusCode);
            Assert.Null(screen);
        }

        [Fact]
        public void Add_TheaterIdConflict()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.Conflict;
            var controller = new ScreensController(AddMock(message).Object);
            var output = controller.Add(TestScreen);
            var result = output as ConflictObjectResult;
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }

        [Fact]
        public void Add_NameConflict()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.Conflict;
            var controller = new ScreensController(AddMock(message).Object);
            var output = controller.Add(TestScreen);
            var result = output as ConflictObjectResult;
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }

        [Fact]
        public void Add_Success()
        {
            Messages message = new Messages();
            message.Success = true;
            message.Status = Statuses.Created;
            var controller = new ScreensController(AddMock(message).Object);
            var output = controller.Add(TestScreen);
            var result = output as CreatedResult;
            Assert.IsType<CreatedResult>(output);
            Assert.StrictEqual("https://localhost:7221/api/Screens/3", result.Location);
            Assert.StrictEqual(201, result.StatusCode);
        }

        [Fact]
        public void Update_BadRequest()
        {
            Messages message = new Messages();
            message.Success = true;
            message.Status = Statuses.BadRequest;
            Screen screen = new Screen { ScreenId = 0 };
            var controller = new ScreensController(UpdateMock(message).Object);
            var output = controller.Update(screen);
            var result = output as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.True(screen.ScreenId == 0);
        }
        [Fact]
        public void Update_NotFound()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.NotFound;
            var controller = new ScreensController(UpdateMock(message).Object);
            var output = controller.Update(TestScreen);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual(404, result.StatusCode);
        }

        [Fact]
        public void Update_NameConflict()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.Conflict;
            var controller = new ScreensController(UpdateMock(message).Object);
            var output = controller.Update(TestScreen);
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
            var controller = new ScreensController(UpdateMock(message).Object);
            var output = controller.Update(TestScreen);
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
            var controller = new ScreensController(DeleteMock(message).Object);
            var output = controller.Remove(3);
            Assert.IsType<OkObjectResult>(output);
            var result = output as OkObjectResult;
            Assert.StrictEqual(200, result.StatusCode);

        }

        [Fact]
        public void Remove_IdNotFound()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.NotFound;
            var controller = new ScreensController(DeleteMock(message).Object);
            var output = controller.Remove(3);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual(404, result.StatusCode);
        }
        [Fact]
        public void Remove_AlreadyStarted()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.Conflict;
            var controller = new ScreensController(DeleteMock(message).Object);
            var output = controller.Remove(3);
            var result = output as ConflictObjectResult;
            Assert.IsType<ConflictObjectResult>(output);
            Assert.StrictEqual(409, result.StatusCode);
        }
    }
}
