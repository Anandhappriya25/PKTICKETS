using Blazorise;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [Fact]
        public void List_Ok()
        {
            Screen screen1 = new Screen { ScreenId = 3, ScreenName = "Vijay", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150,ElitePrice=250, IsActive = true };
            Screen screen2 = new Screen { ScreenId = 2, ScreenName = "theri", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250, IsActive = true };
            List<Screen> screens = new List<Screen>();
            screens.Add(screen2);
            screens.Add(screen1);
            var mockservice = new Mock<IScreenRepository>();
            mockservice.Setup(x => x.GetAllScreens()).Returns(screens);
            var controller = new ScreensController(mockservice.Object);
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
            var mockservice = new Mock<IScreenRepository>();
            mockservice.Setup(x => x.GetAllScreens()).Returns(screens);
            var controller = new ScreensController(mockservice.Object);
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
            Screen screen = new Screen { ScreenId = 3, ScreenName = "Vijay", TheaterId = 1, PremiumCapacity = 200, EliteCapacity = 150, PremiumPrice = 150, ElitePrice = 250, IsActive = true };
            var mockservice = new Mock<IScreenRepository>();
            mockservice.Setup(x => x.ScreenById(It.IsAny<int>())).Returns(screen);
            var controller = new ScreensController(mockservice.Object);
            var okResult = controller.GetById(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as Screen;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(screen, result);
            Assert.NotNull(result);
            Assert.StrictEqual(3, result.ScreenId);
            Assert.True(result.IsActive);
            Assert.StrictEqual(200, list.StatusCode);
        }
        [Fact]
        public void GetById_Null()
        {
            Screen screen = null;
            var mockservice = new Mock<IScreenRepository>();
            mockservice.Setup(x => x.ScreenById(It.IsAny<int>())).Returns(screen); ;
            var controller = new ScreensController(mockservice.Object);
            var result = controller.GetById(3);
            var check = result as StatusCodeResult;
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, check.StatusCode);
            Assert.Null(screen);
        }

        //[Fact]
        //public void Add_PhoneConflict()
        //{
        //    User newUser = new User { UserName = "Vijay", PhoneNumber = "9441004834", Location = "Vellore", EmailId = "karth56@gmail.com", Password = "123456", IsActive = true };
        //    Messages message = new Messages();
        //    message.Message = "The (9441004834) , PhoneNumber is already Registered.";
        //    message.Success = false;
        //    var mockservice = new Mock<IUserRepository>();
        //    mockservice.Setup(x => x.CreateUser(newUser)).Returns(message);
        //    var controller = new UsersController(mockservice.Object);
        //    var output = controller.Add(newUser);
        //    var result = output as ConflictObjectResult;
        //    Assert.Equal(message.Message, result.Value);
        //    Assert.StrictEqual(409, result.StatusCode);
        //    Assert.IsType<ConflictObjectResult>(output);
        //}

        //[Fact]
        //public void Add_EmailConflict()
        //{
        //    User newUser = new User { UserName = "Vijay", PhoneNumber = "9443004834", Location = "Vellore", EmailId = "karth56@gmail.com", Password = "123456", IsActive = true };
        //    Messages message = new Messages();
        //    message.Message = "The (karth56@gmail.com), EmailId is already Registered.";
        //    message.Success = false;
        //    var mockservice = new Mock<IUserRepository>();
        //    mockservice.Setup(x => x.CreateUser(newUser)).Returns(message);
        //    var controller = new UsersController(mockservice.Object);
        //    var output = controller.Add(newUser);
        //    var result = output as ConflictObjectResult;
        //    Assert.Equal(message.Message, result.Value);
        //    Assert.StrictEqual(409, result.StatusCode);
        //    Assert.IsType<ConflictObjectResult>(output);
        //}

        //[Fact]
        //public void Add_Success()
        //{
        //    User newUser = new User { UserId = 3, UserName = "Vijay", PhoneNumber = "9443004834", Location = "Vellore", EmailId = "karth56@gmail.com", Password = "123456", IsActive = true };
        //    Messages message = new Messages();
        //    message.Message = "Vijay, Your Account is Successfully Registered";
        //    message.Success = true;
        //    var mockservice = new Mock<IUserRepository>();
        //    mockservice.Setup(x => x.CreateUser(newUser)).Returns(message);
        //    var controller = new UsersController(mockservice.Object);
        //    var output = controller.Add(newUser);
        //    var result = output as CreatedResult;
        //    Assert.IsType<CreatedResult>(output);
        //    Assert.StrictEqual(message.Message, result.Value);
        //    Assert.StrictEqual("https://localhost:7221/api/Users/3", result.Location);
        //    Assert.StrictEqual(201, result.StatusCode);
        //}

        //[Fact]
        //public void Update_BadRequest()
        //{
        //    User newUser = new User { UserId = 0 };
        //    var mockservice = new Mock<IUserRepository>();
        //    var controller = new UsersController(mockservice.Object);
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
        //    User newUser = new User { UserId = 3, UserName = "Vijay", PhoneNumber = "9443004834", Location = "Vellore", EmailId = "karth56@gmail.com", Password = "123456", IsActive = true };
        //    Messages message = new Messages();
        //    message.Message = "User Id is not found";
        //    message.Success = false;
        //    var mockservice = new Mock<IUserRepository>();
        //    mockservice.Setup(x => x.UpdateUser(newUser)).Returns(message);
        //    var controller = new UsersController(mockservice.Object);
        //    var output = controller.Update(newUser);
        //    var result = output as NotFoundObjectResult;
        //    Assert.IsType<NotFoundObjectResult>(output);
        //    Assert.StrictEqual(message.Message, result.Value);
        //    Assert.StrictEqual(404, result.StatusCode);
        //}

        //[Fact]
        //public void Update_PhoneConflict()
        //{
        //    User newUser = new User { UserId = 3, UserName = "Vijay", PhoneNumber = "9443004832", Location = "Vellore", EmailId = "karth56@gmail.com", Password = "123456", IsActive = true };
        //    Messages message = new Messages();
        //    message.Message = "The (9443004834), PhoneNumber is already Registered.";
        //    message.Success = false;
        //    var mockservice = new Mock<IUserRepository>();
        //    mockservice.Setup(x => x.UpdateUser(newUser)).Returns(message);
        //    var controller = new UsersController(mockservice.Object);
        //    var output = controller.Update(newUser);
        //    var result = output as ConflictObjectResult;
        //    Assert.Equal(message.Message, result.Value);
        //    Assert.StrictEqual(409, result.StatusCode);
        //    Assert.IsType<ConflictObjectResult>(output);
        //}
        //[Fact]
        //public void Update_EmailConflict()
        //{
        //    User newUser = new User { UserId = 3, UserName = "Vijay", PhoneNumber = "9443004834", Location = "Vellore", EmailId = "karth5@gmail.com", Password = "123456", IsActive = true };
        //    Messages message = new Messages();
        //    message.Message = "The (karth56@gmail.com), EmailId is already Registered.";
        //    message.Success = false;
        //    var mockservice = new Mock<IUserRepository>();
        //    mockservice.Setup(x => x.UpdateUser(newUser)).Returns(message);
        //    var controller = new UsersController(mockservice.Object);
        //    var output = controller.Update(newUser);
        //    var result = output as ConflictObjectResult;
        //    Assert.Equal(message.Message, result.Value);
        //    Assert.StrictEqual(409, result.StatusCode);
        //    Assert.IsType<ConflictObjectResult>(output);
        //}

        //[Fact]
        //public void Update_SuccessOk()
        //{
        //    User newUser = new User { UserId = 3, UserName = "Vijay", PhoneNumber = "9443004835", Location = "Vellore", EmailId = "karth56@gmail.com", Password = "123456", IsActive = true };
        //    Messages message = new Messages();
        //    message.Message = "The Vijay Account is Successfully Updated";
        //    message.Success = true;
        //    var mockservice = new Mock<IUserRepository>();
        //    mockservice.Setup(x => x.UpdateUser(newUser)).Returns(message);
        //    var controller = new UsersController(mockservice.Object);
        //    var output = controller.Update(newUser);
        //    var result = output as OkObjectResult;
        //    Assert.Equal(message.Message, result.Value);
        //    Assert.StrictEqual(200, result.StatusCode);
        //    Assert.IsType<OkObjectResult>(output);
        //}

        //[Fact]
        //public void Remove_SucessOk()
        //{
        //    User newUser = new User { UserId = 3, UserName = "Vijay", PhoneNumber = "9443004835", Location = "Vellore", EmailId = "karth56@gmail.com", Password = "123456", IsActive = true };
        //    Messages message = new Messages();
        //    message.Message = "The Vijay Account is Successfully removed";
        //    message.Success = true;
        //    var mockservice = new Mock<IUserRepository>();
        //    mockservice.Setup(x => x.DeleteUser(It.IsAny<int>())).Returns(message);
        //    var controller = new UsersController(mockservice.Object);
        //    var output = controller.Remove(3);
        //    Assert.IsType<OkObjectResult>(output);
        //    var result = output as OkObjectResult;
        //    Assert.Equal(message.Message, result.Value);
        //    Assert.StrictEqual(200, result.StatusCode);

        //}

        //[Fact]
        //public void Remove_NotFound()
        //{
        //    Messages message = new Messages();
        //    message.Message = "User Id (3) is not found";
        //    message.Success = false;
        //    var mockservice = new Mock<IUserRepository>();
        //    mockservice.Setup(x => x.DeleteUser(It.IsAny<int>())).Returns(message);
        //    var controller = new UsersController(mockservice.Object);
        //    var output = controller.Remove(3);
        //    var result = output as NotFoundObjectResult;
        //    Assert.IsType<NotFoundObjectResult>(output);
        //    Assert.StrictEqual(message.Message, result.Value);
        //    Assert.StrictEqual(404, result.StatusCode);
        //}
    }
}
