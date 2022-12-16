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
        private Mock<IScheduleRepository> GetAvailableMock(List<Schedule> schedules)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.AvailableSchedulesList()).Returns(schedules);
            return mockservice;
        }
        private Mock<IScheduleRepository> GetByIdMock(Schedule schedule)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.ScheduleById(It.IsAny<int>())).Returns(schedule);
            return mockservice;
        }
        private Mock<IScheduleRepository> ListByMovieIdMock(List<Schedule> schedule)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.SchedulesByMovieId(It.IsAny<int>())).Returns(schedule);
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
        private Schedule TestSchedule => new Schedule()
        { ScheduleId = 3, ScreenId=2, MovieId=3, ShowTimeId=3, Date= DateTime.Now, PremiumSeats=200, EliteSeats=150, AvailablePreSeats=200, AvailableEliSeats=150, IsActive=true};


        [Fact]
        public void List_Ok()
        {
            List<Schedule> schedules = new List<Schedule>();
            schedules.Add(TestSchedule);
            var controller = new SchedulesController(GetAllMock(schedules).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Schedule>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(schedules, lists);
            Assert.StrictEqual(schedules.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void List_NullOk()
        {
            List<Schedule> schedules = new List<Schedule>();
            var controller = new SchedulesController(GetAllMock(schedules).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Schedule>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(schedules, lists);
            Assert.Empty(lists);
            Assert.StrictEqual(schedules.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void AvailableList_Ok()
        {
            List<Schedule> schedules = new List<Schedule>();
            schedules.Add(TestSchedule);
            var controller = new SchedulesController(GetAvailableMock(schedules).Object);
            var okResult = controller.AvailableList();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Schedule>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(schedules, lists);
            Assert.StrictEqual(schedules.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void AvailableList_NullOk()
        {
            List<Schedule> schedules = new List<Schedule>();
            var controller = new SchedulesController(GetAvailableMock(schedules).Object);
            var okResult = controller.AvailableList();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Schedule>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(schedules, lists);
            Assert.StrictEqual(schedules.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void GetById_ok()
        {
            var controller = new SchedulesController(GetByIdMock(TestSchedule).Object);
            var okResult = controller.GetById(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as Schedule;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.NotEqual(TestSchedule, result);
            Assert.NotNull(result);
            Assert.StrictEqual(3, result.ScheduleId);
            Assert.True(result.IsActive);
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void GetById_NotFound()
        {
            Schedule schedule = null;
            var controller = new SchedulesController(GetByIdMock(schedule).Object);
            var result = controller.GetById(3);
            var check = result as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, check.StatusCode);
            Assert.Null(schedule);
        }

        [Fact]
        public void ListByMovieId_Ok()
        {
            Schedule schedule = new Schedule(){ ScheduleId = 6, ScreenId = 7, MovieId = 3, ShowTimeId = 2, Date = DateTime.Now, PremiumSeats = 200, EliteSeats = 150, AvailablePreSeats = 200, AvailableEliSeats = 150, IsActive = true };
            Schedule schedule2 = new Schedule(){ ScheduleId = 3, ScreenId = 2, MovieId = 3, ShowTimeId = 3, Date = DateTime.Now, PremiumSeats = 200, EliteSeats = 150, AvailablePreSeats = 200, AvailableEliSeats = 150, IsActive = true };
            List<Schedule> schedules = new List<Schedule>();
            schedules.Add(schedule);
            schedules.Add(schedule2);
            var controller = new SchedulesController(ListByMovieIdMock(schedules).Object);
            var result = controller.ListByMovieId(3);
            var check = result as OkObjectResult;
            var lists = check.Value as List<Schedule>;
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, check.StatusCode);
            Assert.Equal(schedules.Count(), lists.Count());
        }

        [Fact]
        public void ListByMovieId_NullOk()
        {
            Schedule schedule = new Schedule() { MovieId = 30 };
            List<Schedule> schedules = new List<Schedule>();
            var mockservice = new Mock<IScheduleRepository>();
            mockservice.Setup(x => x.SchedulesByMovieId(It.IsAny<int>())).Returns(schedules);
            var controller = new SchedulesController(mockservice.Object);
            var result = controller.ListByMovieId(3);
            var check = result as OkObjectResult;
            var lists = check.Value as List<Schedule>;
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, check.StatusCode);
            Assert.Equal(schedules.Count(), lists.Count());
        }
        [Fact]
        public void Add_Success()
        {
            Messages message = new Messages();
            message.Success = true;
            message.Status = Statuses.Created;
            var controller = new SchedulesController(AddMock(message).Object);
            var output = controller.Add(TestSchedule);
            var result = output as CreatedResult;
            Assert.IsType<CreatedResult>(output);
            Assert.StrictEqual("https://localhost:7221/api/Schedules/3", result.Location);
            Assert.StrictEqual(201, result.StatusCode);
        }

        [Fact]
        public void Add_BadRequest()
        {
            Schedule schedule = new Schedule { MovieId = 0 , ScreenId = 0, ShowTimeId = 0 };
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.BadRequest;
            var controller = new SchedulesController(AddMock(message).Object);
            var output = controller.Add(schedule);
            var result = output as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.True(schedule.MovieId == 0 || schedule.ScreenId == 0 || schedule.ShowTimeId == 0);
        }

        [Fact]
        public void Add_NotFound()
        {
            Schedule schedule = new Schedule { MovieId = 3, ScreenId = 2, ShowTimeId = 3 };
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.NotFound;
            var controller = new SchedulesController(AddMock(message).Object);
            var output = controller.Add(schedule);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual(404, result.StatusCode);
            Assert.True(schedule.MovieId == 3 || schedule.ScreenId == 2 || schedule.ShowTimeId == 3);
        }

        //[Fact]
        //public void Add_Conflict()
        //{
        //    Schedule schedule = new Schedule { Date = DateTime.Now}; 
        //    Messages message = new Messages();
        //    message.Success = false;
        //    message.Status = Statuses.Conflict;
        //    var controller = new SchedulesController(AddMock(message).Object);
        //    var output = controller.Add(schedule);
        //    var result = output as ConflictObjectResult;
        //    Assert.IsType<ConflictObjectResult>(output); 
        //    Assert.StrictEqual(409, result.StatusCode);
        //    Assert.True(schedule.MovieId == 3 || schedule.ScreenId == 2 || schedule.ShowTimeId == 3);
        //}


        [Fact]
        public void Update_BadRequest()
        {
            Schedule schedule = new Schedule { ScheduleId = 0, MovieId = 0, ScreenId = 0, ShowTimeId = 0 };
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.BadRequest;
            var controller = new SchedulesController(UpdateMock(message).Object);
            var output = controller.Update(schedule);
            var result = output as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.True(schedule.ScheduleId == 0 || schedule.MovieId == 0 || schedule.ScreenId == 0 || schedule.ShowTimeId == 0);
        }

        [Fact]
        public void Update_NotFound()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.NotFound;
            var controller = new SchedulesController(UpdateMock(message).Object);
            var output = controller.Update(TestSchedule);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual(message.Message, result.Value);
            Assert.StrictEqual(404, result.StatusCode);
        }
         
        //[Fact]
        //public void Update_NameConflict()
        //{
        //    Messages message = new Messages();
        //    message.Message = "Screen Name(Vijay) is Already Registered.";
        //    message.Success = false;
        //    var controller = new ScreensController(UpdateMock(message).Object);
        //    var output = controller.Update(TestScreen);
        //    var result = output as ConflictObjectResult;
        //    Assert.Equal("Screen Name(Vijay) is Already Registered.", result.Value);
        //    Assert.StrictEqual(409, result.StatusCode);
        //    Assert.IsType<ConflictObjectResult>(output);
        //}

        [Fact]
        public void Update_SuccessOk()
        {
            Messages message = new Messages();
            message.Success = true;
            message.Status = Statuses.Success;
            var controller = new SchedulesController(UpdateMock(message).Object);
            var output = controller.Update(TestSchedule);
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
            var controller = new SchedulesController(DeleteMock(message).Object);
            var output = controller.Remove(3);
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
            var controller = new SchedulesController(DeleteMock(message).Object);
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
            var controller = new SchedulesController(DeleteMock(message).Object);
            var output = controller.Remove(3);
            var result = output as ConflictObjectResult;
            Assert.IsType<ConflictObjectResult>(output);
            Assert.StrictEqual(409, result.StatusCode);
        }
    }
}

