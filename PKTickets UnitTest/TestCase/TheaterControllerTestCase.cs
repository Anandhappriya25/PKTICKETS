using Blazorise;
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
            Theater theater2 = new Theater { TheaterId = 4, TheaterName = "Priya Cinemas", Location = "Kolathur", IsActive = true };
            List<Theater> theaters = new List<Theater>();
            theaters.Add(theater);
            theaters.Add(theater2);
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.GetTheaters()).Returns(theaters);
            var controller = new TheatersController(mockservice.Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Theater>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(theaters, lists);
            Assert.StrictEqual(theaters.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
            Assert.Equal(theater.TheaterId, lists[0].TheaterId);
        }

        [Fact]
        public void GetById_OK()
        {
            Theater theater = new Theater { TheaterId = 7, TheaterName = "Vijay cinemas", Location = "Vellore", IsActive = true };
            Theater theater1 = null;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterById(It.IsAny<int>())).Returns(theater);
            var controller = new TheatersController(mockservice.Object);
            var okResult = controller.GetById(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as Theater;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(theater, result);
            Assert.NotNull(result);
            Assert.Null(theater1);
            Assert.NotEqual(3, result.TheaterId);
            Assert.True(result.IsActive);
            Assert.StrictEqual(200, list.StatusCode);
            Assert.StrictEqual(theater.TheaterId, result.TheaterId);
        }

        [Fact]
        public void GetById_Null()
        {
            Theater theater = null;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterById(It.IsAny<int>())).Returns(theater);
            var controller = new TheatersController(mockservice.Object);
            var result = controller.GetById(30);
            var check = result as StatusCodeResult;
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, check.StatusCode);
        }
        [Fact]
        public void GetByLocation_Success()
        {
            Theater theater = new Theater { TheaterId = 7, TheaterName = "Vijay cinemas", Location = "Vellore", IsActive = true };
            List<Theater> theaters = new List<Theater>();
            theaters.Add(theater);
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterByLocation(It.IsAny<string>())).Returns(theaters);
            var controller = new TheatersController(mockservice.Object);
            var okResult = controller.GetById(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as Theater;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(theater, result);
            Assert.NotNull(result);
            Assert.NotEqual(3, result.TheaterId);
            Assert.True(result.IsActive);
            Assert.StrictEqual(200, list.StatusCode);
            Assert.StrictEqual(theater.Location, result.Location);
        }

        [Fact]
        public void GetByLocation_Null()
        {
            Theater theater = null;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterById(It.IsAny<int>())).Returns(theater);
            var controller = new TheatersController(mockservice.Object);
            var result = controller.GetById(30);
            var check = result as StatusCodeResult;
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, check.StatusCode);
        }

    }
}
