using PKTickets.Interfaces;
using PKTickets.Models;
using PKTickets.Models.DTO;
using System.Collections.Generic;

namespace PKTickets.Repository
{
    public class MovieRepository : IMovieRepository
    {

        private readonly PKTicketsDbContext db;
        private readonly IScheduleRepository schedules;
        public MovieRepository(PKTicketsDbContext db, IScheduleRepository _schedules)
        {
            this.db = db;
            this.schedules = _schedules;
        }

        public List<Movie> GetAllMovies()
        {
            return db.Movies.Where(x => x.IsPlaying).ToList();
        }
        public List<Movie> ScheduledMovies()
        {
            var list = schedules.AvailableSchedulesList();
            List<Movie> movies= new List<Movie>();
            foreach (var schedule in list)
            {
                var movie = db.Movies.FirstOrDefault(x=>x.MovieId==schedule.MovieId);
                movies.Add(movie);
            }
            var movielist= movies.DistinctBy(x=> x.MovieId).ToList();
            return movielist;
        }

        public Movie MovieById(int id)
        {
            var movieExist = db.Movies.Where(x => x.IsPlaying).FirstOrDefault(x => x.MovieId == id);
            return movieExist;
        }

        public List<Movie> MovieByTitle(string title)
        {
            var movieExists = db.Movies.Where(x => x.IsPlaying).Where(x => x.Title == title).ToList();
            return movieExists;
        }

        public List<Movie> MovieByGenre(string genre)
        {
            var movieExists = db.Movies.Where(x => x.IsPlaying).Where(x => x.Genre == genre).ToList();
            return movieExists;
        }

      

        public Messages CreateMovie(Movie movie)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var movieExist = MovieByTitle(movie.Title).FirstOrDefault(x => x.Director == movie.Director);
            return (movieExist != null)? Request.Conflict($"{movie.Title} Movie is already Registered.")
                : Create(movie);
        }

       
        public Messages DeleteMovie(int movieId)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var movie = MovieById(movieId);
            return (movie == null)? Request.Not($"Movie Id {movieId} is not found")
                : Delete(movie); 
        }

        public Messages UpdateMovie(Movie movie)
        {
            if (movie.MovieId == 0)
            {
                return Request.Bad("Enter the Movie Id Field");
            }
            var movieExist = MovieById(movie.MovieId);
            var directorExist = MovieByTitle(movie.Title).FirstOrDefault(x => x.Director == movie.Director);
            return (movieExist == null) ? Request.Bad("Movie Id is not found")
                : (directorExist != null && directorExist.MovieId != directorExist.MovieId)? Request.Conflict($"This Movie is already registered with Director {movie.Director}")
                : Update(movie, movieExist);
        }

        #region
        private Messages messages = new Messages() { Success = true };
        private Messages Update(Movie movie,Movie movieExist)
        {
            movieExist.Director = movie.Director;
            movieExist.Title = movie.Title;
            movieExist.Duration = movie.Duration;
            movieExist.Genre = movie.Genre;
            movieExist.CastAndCrew = movie.CastAndCrew;
            movieExist.ImagePath = movie.ImagePath;
            db.SaveChanges();
            messages.Message = $"{movie.Title} Movie is Successfully Updated";
            messages.Status = Statuses.Success;
            return messages;
        }
        private Messages Delete(Movie movie)
        {
            movie.IsPlaying = false;
            db.SaveChanges();
            messages.Message = $"{movie.Title} Movie is Successfully Removed";
            messages.Status = Statuses.Success;
            return messages;
        }
        private Messages Create(Movie movie)
        {
            db.Movies.Add(movie);
            db.SaveChanges();
            messages.Status = Statuses.Created;
            messages.Message = $"{movie.Title} Movie is Successfully Added";
            return messages;
        }
        #endregion

    }
}
