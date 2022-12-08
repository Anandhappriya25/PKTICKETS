using Blazorise.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PKTickets.Controllers;
using PKTickets.Interfaces;
using PKTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PKTicketsUnitTest.XUnitTest
{
    public class UserController
    {

        [Fact]
        public void List_Ok()
        {
            //User user = new User { UserId = 3, UserName = "Vijaya", PhoneNumber = "9441009834", Location = "Vellore",EmailId="karthi56@gmail.com",Password="123456",IsActive=true };
            User user2 = new User { UserId = 3, UserName = "Vijay", PhoneNumber = "9441004834", Location = "Vellore", EmailId = "karth56@gmail.com", Password = "123456", IsActive = true };
            List<User> customers = new List<User>();
            customers.Add(user2);
            var mockservice = new Mock<IUserRepository>();
            mockservice.Setup(x => x.GetAllUsers()).Returns(customers);
            var controller = new UsersController(mockservice.Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<User>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(customers, lists);
            lists.Count().CompareTo(1);
        }
        [Fact]
        public void GetById_ok()
        {
            User user = new User { UserId = 3, UserName = "Vijay", PhoneNumber = "9441004834", Location = "Vellore", EmailId = "karth56@gmail.com", Password = "123456", IsActive = true }; 
            var mockservice = new Mock<IUserRepository>();
            mockservice.Setup(x => x.UserById(It.IsAny<int>())).Returns(user);
            var controller = new UsersController(mockservice.Object);
            var okResult = controller.GetById(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as User;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(user, result);
            Assert.NotNull(result);
            Assert.StrictEqual(3, result.UserId);
            Assert.True(result.IsActive);
        }
        [Fact]
        public void GetById_Null()
        {
            User user = null;
            var mockservice = new Mock<IUserRepository>();
            mockservice.Setup(x => x.UserById(It.IsAny<int>())).Returns(user);
            var controller = new UsersController(mockservice.Object);
            var result = controller.GetById(3);
            var check = result as StatusCodeResult;
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, check.StatusCode);
        }
    }
}
