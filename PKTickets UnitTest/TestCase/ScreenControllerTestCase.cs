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
        private Screen TestScreen => new()
        { ScreenId = 3, ScreenName = "Vijay", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250, IsActive = true };
        

            [Fact]
        public void List_Ok()
        {
            Screen screen1 = new Screen { ScreenId = 3, ScreenName = "Vijay", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150,ElitePrice=250, IsActive = true };
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
        public void GetById_Null()
        {
            Screen screen = null;
            var controller = new ScreensController(GetByIdMock(screen).Object);
            var result = controller.GetById(3);
            var check = result as StatusCodeResult;
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, check.StatusCode);
            Assert.Null(screen);
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
            var mockservice = new Mock<IScreenRepository>();
            var controller = new ScreensController(mockservice.Object);
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
            Screen screen = new Screen { ScreenId = 3, ScreenName = "Vijay", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250, IsActive = true };
            Messages message = new Messages();
            message.Message = "Screen Id is not found";
            message.Success = false;
            var mockservice = new Mock<IScreenRepository>();
            mockservice.Setup(x => x.UpdateScreen(It.IsAny<Screen>())).Returns(message);
            var controller = new ScreensController(mockservice.Object);
            var output = controller.Update(screen);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual("Screen Id is not found", result.Value);
            Assert.StrictEqual(404, result.StatusCode);
        }

        [Fact]
        public void Update_NameConflict()
        {
            Screen screen = new Screen { ScreenId = 3, ScreenName = "Vijay", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250, IsActive = true };
            Messages message = new Messages();
            message.Message = "Screen Name(Vijay) is Already Registered.";
            message.Success = false;
            var mockservice = new Mock<IScreenRepository>();
            mockservice.Setup(x => x.UpdateScreen(It.IsAny<Screen>())).Returns(message);
            var controller = new ScreensController(mockservice.Object);
            var output = controller.Update(screen);
            var result = output as ConflictObjectResult;
            Assert.Equal("Screen Name(Vijay) is Already Registered.", result.Value);
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }

        [Fact]
        public void Update_SuccessOk()
        {
            Screen screen = new Screen { ScreenId = 3, ScreenName = "Vijay", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250, IsActive = true };
            Messages message = new Messages();
            message.Message = "Screen Vijay is succssfully Updated";
            message.Success = true;
            var mockservice = new Mock<IScreenRepository>();
            mockservice.Setup(x => x.UpdateScreen(It.IsAny<Screen>())).Returns(message);
            var controller = new ScreensController(mockservice.Object);
            var output = controller.Update(screen);
            var result = output as OkObjectResult;
            Assert.Equal(message.Message, result.Value);
            Assert.StrictEqual(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(output);
        }

        [Fact]
        public void Remove_SucessOk()
        {
            Screen screen = new Screen { ScreenId = 3, ScreenName = "Vijay", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250, IsActive = true };
            Messages message = new Messages();
            message.Message = "Screen (Vijay) is succssfully Removed";
            message.Success = true;
            var mockservice = new Mock<IScreenRepository>();
            mockservice.Setup(x => x.RemoveScreen(It.IsAny<int>())).Returns(message);
            var controller = new ScreensController(mockservice.Object);
            var output = controller.Remove(3);
            Assert.IsType<OkObjectResult>(output);
            var result = output as OkObjectResult;
            Assert.Equal(message.Message, result.Value);
            Assert.StrictEqual(200, result.StatusCode);

        }

        [Fact]
        public void Remove_NotFound()
        {
            Messages message = new Messages();
            message.Message = "Screen Id(3) is not found";
            message.Success = false;
            var mockservice = new Mock<IScreenRepository>();
            mockservice.Setup(x => x.RemoveScreen(It.IsAny<int>())).Returns(message);
            var controller = new ScreensController(mockservice.Object);
            var output = controller.Remove(3);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual(message.Message, result.Value);
            Assert.StrictEqual(404, result.StatusCode);
        }
    }
}
