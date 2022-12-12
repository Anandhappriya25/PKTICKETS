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
            return db.Movies.Where(x => x.IsPlaying == true).ToList();
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
            var movieExist = db.Movies.Where(x => x.IsPlaying == true).FirstOrDefault(x => x.MovieId == id);
            return movieExist;
        }

        public List<Movie> MovieByTitle(string title)
        {
            var movieExists = db.Movies.Where(x => x.IsPlaying == true).Where(x => x.Title == title).ToList();
            return movieExists;
        }

        public List<Movie> MovieByGenre(string genre)
        {
            var movieExists = db.Movies.Where(x => x.IsPlaying == true).Where(x => x.Genre == genre).ToList();
            return movieExists;
        }

        public List<Movie> MovieByLanguage(string language)
        {
            var movieExists = db.Movies.Where(x => x.IsPlaying == true).Where(x => x.Genre == language).ToList();
            return movieExists;
        }

        public List<Movie> MovieByDirector(string name)
        {
            var movieExists = db.Movies.Where(x => x.IsPlaying == true).Where(x => x.Director == name).ToList();
            return movieExists;
        }

        public Messages CreateMovie(Movie movie)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var movieExist = MovieByTitle(movie.Title).FirstOrDefault(x => x.Director == movie.Director);
            if (movieExist != null)
            {
                messages.Message = ""+movie.Title+" Movie is already Registered.";
                return messages;
            }
            else
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                messages.Success = true;
                messages.Message = "" + movie.Title +" Movie is Successfully Added";
                return messages;
            }
        }

       
        public Messages DeleteMovie(int movieId)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var movie = MovieById(movieId);
            if (movie == null)
            {
                messages.Message = "Movie Id (" + movieId + ") is not found";
                return messages;
            }
           
            else
            {
                movie.IsPlaying = false;
                db.SaveChanges();
                messages.Success = true;
                messages.Message = "" + movie.Title + " Movie is Successfully Removed";
                return messages;
            }
        }

        public Messages UpdateMovie(Movie movie)
        {
            Messages messages = new Messages();
            messages.Success = false;
            var movieExist = MovieById(movie.MovieId);
            var directorExist = MovieByTitle(movie.Title).FirstOrDefault(x => x.Director == movie.Director);
            if (movieExist == null)
            {
                messages.Message = "Movie Id is not found";
                return messages;
            }
            else if (directorExist != null && directorExist.MovieId != directorExist.MovieId)
            {
                messages.Message = "This Movie is already registered with Director"+movie.Director+"";
                return messages;
            }
            else
            {
                movieExist.Director = movie.Director;
                movieExist.Title = movie.Title;
                movieExist.Duration = movie.Duration;
                movieExist.Genre = movie.Genre;
                movieExist.CastAndCrew = movie.CastAndCrew;
                movieExist.ImagePath= movie.ImagePath;
                db.SaveChanges();
                messages.Success = true;
                messages.Message = "" + movie.Title + " Movie is Successfully Updated";
                return messages;
            }
        }
       
        

    }
}
