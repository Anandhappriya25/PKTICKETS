using Blazorise;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PKTickets.Controllers;
using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
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
        }

        [Fact]
        public void List_NullOk()
        {
            List<Theater> theaters = new List<Theater>();
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.GetTheaters()).Returns(theaters);
            var controller = new TheatersController(mockservice.Object);
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
            Theater theater = new Theater { TheaterId = 7, TheaterName = "Vijay cinemas", Location = "Vellore", IsActive = true };
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.TheaterById(It.IsAny<int>())).Returns(theater);
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
            Assert.Null(theater);
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
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.GetTheaters()).Returns(theaters);
            var controller = new TheatersController(mockservice.Object);
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
        public void Add_Success()
        {
            Theater theater = new Theater { TheaterId = 9, TheaterName = "Priyan cinemas", Location = "Chennai", IsActive = true };
            Messages message = new Messages();
            message.Message = "Theater Priyan Cinemas, is Successfully Added";
            message.Success = true;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.CreateTheater(theater)).Returns(message);
            var controller = new TheatersController(mockservice.Object);
            var output = controller.Add(theater);
            var result = output as CreatedResult;
            Assert.IsType<CreatedResult>(output);
            Assert.StrictEqual(message.Message, result.Value);
            Assert.StrictEqual("https://localhost:7221/api/Theaters/9", result.Location);
            Assert.StrictEqual(201, result.StatusCode);
        }

        [Fact]
        public void Add_Conflict()
        {
            Theater theater = new Theater { TheaterId = 9, TheaterName = "Priyan cinemas", Location = "Chennai", IsActive = true };
            Messages message = new Messages();
            message.Message = "Theater Priyan Cinemas, is already registered";
            message.Success = false;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.CreateTheater(theater)).Returns(message);
            var controller = new TheatersController(mockservice.Object);
            var output = controller.Add(theater);
            var result = output as ConflictObjectResult;
            Assert.StrictEqual(message.Message, result.Value);
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }
        [Fact]
        public void Update_BadRequest()
        {
            Theater theater = new Theater { TheaterId = 0 };
            var mockservice = new Mock<ITheaterRepository>();
            var controller = new TheatersController(mockservice.Object);
            var output = controller.Update(theater);
            var result = output as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.StrictEqual("Enter the Theater Id field", result.Value);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.True(theater.TheaterId == 0);
        }
        [Fact]
        public void Update_NotFound()
        {
            Theater theater = new Theater { TheaterId = 15, TheaterName = "Dharshini cinemas", Location = "Chennai", IsActive = true };
            Messages message = new Messages();
            message.Message = "Theater Id is not found";
            message.Success = false;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.UpdateTheater(theater)).Returns(message);
            var controller = new TheatersController(mockservice.Object);
            var output = controller.Update(theater);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual(message.Message, result.Value);
            Assert.StrictEqual(404, result.StatusCode);
        }

        [Fact]
        public void Update_NameConflict()
        {
            Theater theater = new Theater { TheaterId = 9, TheaterName = "Priyan cinemas", Location = "Chennai", IsActive = true };
            Messages message = new Messages();
            message.Message = "Theater Priyan Cinemas, is already registered";
            message.Success = false;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.UpdateTheater(theater)).Returns(message);
            var controller = new TheatersController(mockservice.Object);
            var output = controller.Update(theater);
            var result = output as ConflictObjectResult;
            Assert.Equal(message.Message, result.Value);
            Assert.StrictEqual(409, result.StatusCode);
            Assert.IsType<ConflictObjectResult>(output);
        }
        
        [Fact]
        public void Update_SuccessOk()
        {
            Theater theater = new Theater { TheaterId = 9, TheaterName = "Priyan cinemas", Location = "Chennai", IsActive = true };
            Messages message = new Messages();
            message.Message = "Theater Priyan Cinemas, is Successfully Updated";
            message.Success = true;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.UpdateTheater(theater)).Returns(message);
            var controller = new TheatersController(mockservice.Object);
            var output = controller.Update(theater);
            var result = output as OkObjectResult;
            Assert.Equal(message.Message, result.Value);
            Assert.StrictEqual(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(output);
        }
        [Fact]
        public void Remove_SucessOk()
        {
            Theater theater = new Theater { TheaterId = 9, TheaterName = "Priyan cinemas", Location = "Chennai", IsActive = true };
            Messages message = new Messages();
            message.Message = "Theater Priyan Cinemas is Successfully removed";
            message.Success = true;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.DeleteTheater(It.IsAny<int>())).Returns(message);
            var controller = new TheatersController(mockservice.Object);
            var output = controller.Remove(9);
            Assert.IsType<OkObjectResult>(output);
            var result = output as OkObjectResult;
            Assert.Equal(message.Message, result.Value);
            Assert.StrictEqual(200, result.StatusCode);

        }

        [Fact]
        public void Remove_NotFound()
        {
            Messages message = new Messages();
            message.Message = "Theater Id (3) is not found";
            message.Success = false;
            var mockservice = new Mock<ITheaterRepository>();
            mockservice.Setup(x => x.DeleteTheater(It.IsAny<int>())).Returns(message);
            var controller = new TheatersController(mockservice.Object);
            var output = controller.Remove(3);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual(message.Message, result.Value);
            Assert.StrictEqual(404, result.StatusCode);
        }
    }
}
