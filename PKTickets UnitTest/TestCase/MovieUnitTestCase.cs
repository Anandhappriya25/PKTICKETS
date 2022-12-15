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
    public class MovieUnitTestCase
    {
        private Mock<IMovieRepository> Mock()
        {
            var mockservice = new Mock<IMovieRepository>();
            return mockservice;
        }
        private Mock<IMovieRepository> TitleMock(string name,List<Movie> movies)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.MovieByTitle(It.IsAny<string>())).Returns(movies);
            return mockservice;
        }
        private Mock<IMovieRepository> GenreMock(string name, List<Movie> movies)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.MovieByGenre(It.IsAny<string>())).Returns(movies);
            return mockservice;
        }
        private Mock<IMovieRepository> GetAllMock(List<Movie> movies)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.GetAllMovies()).Returns(movies);
            return mockservice;
        }
        private Mock<IMovieRepository> AvailableScheduleMock(List<Movie> movies)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.ScheduledMovies()).Returns(movies);
            return mockservice;
        }
        private Mock<IMovieRepository> GetByIdMock(Movie movie)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.MovieById(It.IsAny<int>())).Returns(movie);
            return mockservice;
        }
        private Mock<IMovieRepository> AddMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.CreateMovie(It.IsAny<Movie>())).Returns(message);
            return mockservice;
        }
        private Mock<IMovieRepository> UpdateMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.UpdateMovie(It.IsAny<Movie>())).Returns(message);
            return mockservice;
        }
        private Mock<IMovieRepository> DeleteMock(Messages message)
        {
            var mockservice = Mock();
            mockservice.Setup(x => x.DeleteMovie(It.IsAny<int>())).Returns(message);
            return mockservice;
        }
        private Movie TestMovie => new Movie()
        { MovieId = 3, Title = "Theri", Duration =120,Genre="Action", CastAndCrew ="vijay and samantha",Language="tamil",Director="atlee",ImagePath="css",IsPlaying=true};

        
        [Fact]
        public void List_Ok()
        {
            List<Movie> movies = new List<Movie>();
            movies.Add(TestMovie);
            var controller = new MoviesController(GetAllMock(movies).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Movie>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(movies, lists);
            Assert.NotEmpty(lists);
            Assert.StrictEqual(movies.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void List_NullOk()
        {
            List<Movie> movies = new List<Movie>();
            var controller = new MoviesController(GetAllMock(movies).Object);
            var okResult = controller.List();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Movie>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(movies, lists);
            Assert.Empty(lists);
            Assert.StrictEqual(movies.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void AvailableSchedules_Ok()
        {
            List<Movie> movies = new List<Movie>();
            movies.Add(TestMovie);
            var controller = new MoviesController(AvailableScheduleMock(movies).Object);
            var okResult = controller.AvailableSchedules();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Movie>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(movies, lists);
            Assert.NotEmpty(lists);
            Assert.StrictEqual(movies.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void AvailableSchedules_NullOk()
        {
            List<Movie> movies = new List<Movie>();
            var controller = new MoviesController(AvailableScheduleMock(movies).Object);
            var okResult = controller.AvailableSchedules();
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Movie>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(movies, lists);
            Assert.Empty(lists);
            Assert.StrictEqual(movies.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void ListByTitle_Ok()
        {
            List<Movie> movies = new List<Movie>();
            movies.Add(TestMovie);
            var controller = new MoviesController(TitleMock("Theri", movies).Object);
            var okResult = controller.ListByTitle("Theri");
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Movie>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(movies, lists);
            Assert.NotEmpty(lists);
            Assert.StrictEqual(movies.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void ListByTitle_NotFound()
        {
            List<Movie> movies = new List<Movie>();
            var controller = new MoviesController(TitleMock("Theri", movies).Object);
            var okResult = controller.ListByTitle("Theri");
            var result = okResult as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(okResult);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void ListByGenre_Ok()
        {
            List<Movie> movies = new List<Movie>();
            movies.Add(TestMovie);
            var controller = new MoviesController(GenreMock("Action", movies).Object);
            var okResult = controller.ListByGenre("Action");
            var list = okResult as OkObjectResult;
            var lists = list.Value as List<Movie>;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(movies, lists);
            Assert.NotEmpty(lists);
            Assert.StrictEqual(movies.Count(), lists.Count());
            Assert.StrictEqual(200, list.StatusCode);
        }

        [Fact]
        public void ListByGenre_NotFound()
        {
            List<Movie> movies = new List<Movie>();
            var controller = new MoviesController(GenreMock("Action", movies).Object);
            var okResult = controller.ListByGenre("Action");
            var result = okResult as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(okResult);
            Assert.Equal(404, result.StatusCode);
        }
        [Fact]
        public void GetById_ok()
        {
            var controller = new MoviesController(GetByIdMock(TestMovie).Object);
            var okResult = controller.GetById(3);
            var list = okResult as OkObjectResult;
            var result = list.Value as Movie;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.Equal(TestMovie.Title, result.Title);
            Assert.NotNull(result);
            Assert.StrictEqual(3, result.MovieId);
            Assert.True(result.IsPlaying);
            Assert.StrictEqual(200, list.StatusCode);
        }
        [Fact]
        public void GetById_NotFound()
        {
            Movie movie = null;
            var controller = new MoviesController(GetByIdMock(movie).Object);
            var result = controller.GetById(3);
            var check = result as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, check.StatusCode);
        }

        [Fact]
        public void Add_AlreadyExistConflict()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.Conflict;
            var controller = new MoviesController(AddMock(message).Object);
            var output = controller.Add(TestMovie);
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
            var controller = new MoviesController(AddMock(message).Object);
            var output = controller.Add(TestMovie);
            var result = output as CreatedResult;
            Assert.IsType<CreatedResult>(output);
            Assert.StrictEqual("https://localhost:7221/api/Movies/3", result.Location);
            Assert.StrictEqual(201, result.StatusCode);
        }

        [Fact]
        public void Update_BadRequest()
        {
            Messages message = new Messages();
            message.Success = true;
            message.Status = Statuses.BadRequest;
            Movie movie = new Movie { MovieId = 0 };
            var controller = new MoviesController(UpdateMock(message).Object);
            var output = controller.Update(movie);
            var result = output as BadRequestObjectResult;
            Assert.IsType<BadRequestObjectResult>(output);
            Assert.StrictEqual(400, result.StatusCode);
            Assert.True(movie.MovieId == 0);
        }
        [Fact]
        public void Update_NotFound()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.NotFound;
            var controller = new MoviesController(UpdateMock(message).Object);
            var output = controller.Update(TestMovie);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual(404, result.StatusCode);
        }

        [Fact]
        public void Update_DirectorConflict()
        {
            Messages message = new Messages();
            message.Success = false;
            message.Status = Statuses.Conflict;
            var controller = new MoviesController(UpdateMock(message).Object);
            var output = controller.Update(TestMovie);
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
            var controller = new MoviesController(UpdateMock(message).Object);
            var output = controller.Update(TestMovie);
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
            var controller = new MoviesController(DeleteMock(message).Object);
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
            var controller = new MoviesController(DeleteMock(message).Object);
            var output = controller.Remove(3);
            var result = output as NotFoundObjectResult;
            Assert.IsType<NotFoundObjectResult>(output);
            Assert.StrictEqual(404, result.StatusCode);
        }
    }
}
