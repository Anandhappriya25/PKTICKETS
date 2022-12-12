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
    public class UserControllerTestCase
    {
        private Mock<IUserRepository> Mock()
        {
            var mockservice = new Mock<IUserRepository>();
            return mockservice;
        }
        private Mock<IUserRepository> GetAllMock(List<User> users)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.GetAllUsers()).Returns(users);
            return mockservice;
        }
        private Mock<IUserRepository> GetByIdMock(User user)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.UserById(It.IsAny<int>())).Returns(user);
            return mockservice;
        }
        private Mock<IUserRepository> AddMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.CreateUser(It.IsAny<User>())).Returns(message);
            return mockservice;
        }
        private Mock<IUserRepository> UpdateMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.UpdateUser(It.IsAny<User>())).Returns(message);
            return mockservice;
        }
        private Mock<IUserRepository> DeleteMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.DeleteUser(It.IsAny<int>())).Returns(message);
            return mockservice;
        }
        private User TestUser => new()
        { UserId = 3, UserName = "Vijay", PhoneNumber = "9441004834", Location = "Vellore", EmailId = "karth56@gmail.com", Password = "123456", IsActive = true };


        [Fact]
        public void List_Ok()
        {
            List<User> customers = new List<User>();
            customers.Add(TestUser);
            var controller = new UsersController(GetAllMock(customers).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<User>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(customers, lists);
            Assert.NotEmpty(lists);
            Assert.StrictEqual(customers.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void List_NullOk()
        {
            List<User> customers = new List<User>();
            var controller = new UsersController(GetAllMock(customers).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<User>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(customers, lists);
            Assert.Empty(lists);
            Assert.StrictEqual(customers.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }
        [Fact]
        public void GetById_ok()
        {
            var controller = new UsersController(GetByIdMock(TestUser).Object);
            var okResult = controller.GetById(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as User;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(TestUser.UserName, result.UserName);
            Assert.NotNull(result);
            Assert.StrictEqual(3, result.UserId);
            Assert.True(result.IsActive);
            Assert.StrictEqual(200, list.StatusCode);
        }
        [Fact]
        public void GetById_Null()
        {
            User user = null;
            var controller = new UsersController(GetByIdMock(user).Object);
            var result = controller.GetById(3);
            var check = result as StatusCodeResult;
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, check.StatusCode);
        }

        [Fact]
        public void Add_PhoneConflict()
        {Messages message = new Messages();
            message.Message = "The (9441004834) , PhoneNumber is already Registered.";
            message.Success = false;
            var controller = new UsersController(AddMock(message).Object);
            var output = controller.Add(TestUser);
            var result = output as ConflictObjectResult;
            Assert.Equal("The (9441004834) , PhoneNumber is already Registered.", result.Value);
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }

        [Fact]
        public void Add_EmailConflict()
        {
            Messages message = new Messages();
            message.Message = "The (karth56@gmail.com), EmailId is already Registered.";
            message.Success = false;
            var controller = new UsersController(AddMock(message).Object);
            var output = controller.Add(TestUser);
            var result = output as ConflictObjectResult;
            Assert.Equal("The (karth56@gmail.com), EmailId is already Registered.", result.Value);
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }

        [Fact]
        public void Add_Success()
        {
            Messages message = new Messages();
            message.Message = "Vijay, Your Account is Successfully Registered";
            message.Success = true;
            var controller = new UsersController(AddMock(message).Object);
            var output = controller.Add(TestUser);
            var result = output as CreatedResult;
            Assert.IsType<CreatedResult>(output);
            Assert.StrictEqual("Vijay, Your Account is Successfully Registered", result.Value);
            Assert.StrictEqual("https://localhost:7221/api/Users/3", result.Location);
            Assert.StrictEqual(201, result.StatusCode);
        }

        [Fact]
        public void Update_BadRequest()
        {
            User newUser = new User { UserId = 0 };
            var controller = new UsersController(Mock().Object);
            var output = controller.Update(newUser);
            var result = output as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.StrictEqual("Enter the User Id field", result.Value);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.True(newUser.UserId == 0);
        }
        [Fact]
        public void Update_NotFound()
        {
            Messages message = new Messages();
            message.Message = "User Id is not found";
            message.Success = false;
            var controller = new UsersController(UpdateMock(message).Object);
            var output = controller.Update(TestUser);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual("User Id is not found", result.Value);
            Assert.StrictEqual(404, result.StatusCode);
        }

        [Fact]
        public void Update_PhoneConflict()
        {
            Messages message = new Messages();
            message.Message = "The (9443004834), PhoneNumber is already Registered.";
            message.Success = false;
            var controller = new UsersController(UpdateMock(message).Object);
            var output = controller.Update(TestUser);
            var result = output as ConflictObjectResult;
            Assert.Equal("The (9443004834), PhoneNumber is already Registered.", result.Value);
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }
        [Fact]
        public void Update_EmailConflict()
        {
            Messages message = new Messages();
            message.Message = "The (karth56@gmail.com), EmailId is already Registered.";
            message.Success = false;
            var controller = new UsersController(UpdateMock(message).Object);
            var output = controller.Update(TestUser);
            var result = output as ConflictObjectResult;
            Assert.Equal("The (karth56@gmail.com), EmailId is already Registered.", result.Value);
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }

        [Fact]
        public void Update_SuccessOk()
        {
            Messages message = new Messages();
            message.Message = "The Vijay Account is Successfully Updated";
            message.Success = true;
            var controller = new UsersController(UpdateMock(message).Object);
            var output = controller.Update(TestUser);
            var result = output as OkObjectResult;
            Assert.Equal("The Vijay Account is Successfully Updated", result.Value);
            Assert.StrictEqual(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(output);
        }

        [Fact]
        public void Remove_SucessOk()
        {Messages message = new Messages();
            message.Message = "The Vijay Account is Successfully removed";
            message.Success = true;
            var controller = new UsersController(DeleteMock(message).Object);
            var output = controller.Remove(3);
            Assert.IsType<OkObjectResult>(output);
            var result = output as OkObjectResult;
            Assert.Equal("The Vijay Account is Successfully removed", result.Value);
            Assert.StrictEqual(200, result.StatusCode);

        }

        [Fact]
        public void Remove_NotFound()
        {
            Messages message = new Messages();
            message.Message = "User Id (3) is not found";
            message.Success = false;
            var controller = new UsersController(DeleteMock(message).Object);
            var output = controller.Remove(3);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual("User Id (3) is not found", result.Value);
            Assert.StrictEqual(404, result.StatusCode);
        }
    }
}
