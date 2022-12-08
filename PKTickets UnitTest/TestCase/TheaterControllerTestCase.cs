using Castle.Core.Resource;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PKTickets.Controllers;
using PKTickets.Interfaces;
using PKTickets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKTickets_UnitTest.TestCase
{
    public class TheaterControllerTestCase
    {
        [Fact]
        public void TheatersList_Ok()
        {
            Theater theater = new Theater { TheaterId = 3, TheaterName = "Vijaya Cinemas", Location = "Maduravoyal", IsActive = true};
            List<Theater> theaters = new List<Theater>();
            theaters.Add(theater);
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.GetTheaters()).Returns(theaters);
            var controller = new TheatersController(mockservice.Object);
            var okResult = controller.List();
            Assert.IsType<OkObjectResult>(okResult);
        }

        [Fact]
        public void GetById_OK()
        {
            Theater theater = new Theater { TheaterId = 8, TheaterName = "Kavitha Cinemas", Location = "Korattur", IsActive = true };
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterById(It.IsAny<int>())).Returns(theater);
            var controller = new TheatersController(mockservice.Object);
            var okResult = controller.GetById(8) as OkObjectResult;
            Assert.NotNull(okResult);
            var model = okResult.Value as Theater;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.StrictEqual(theater.TheaterId, model.TheaterId);
        }

        [Fact]
        public void GetById_NotFound()
        {
            Theater theater = null;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterById(It.IsAny<int>())).Returns(theater);
            var controller = new TheatersController(mockservice.Object);
            var result = controller.GetById(30);
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
