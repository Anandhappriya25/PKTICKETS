using PKTickets.Models;
using PKTickets.Models.DTO;

namespace PKTickets.Interfaces
{
    public interface IMovieRepository
    {
        public List<Movie> GetAllMovies();
        public Movie MovieById(int id);
        public List<Movie> MovieByTitle(string title);
        public List<Movie> MovieByGenre(string genre);
        public Messages CreateMovie(Movie movie);
        public Messages DeleteMovie(int movieId);
        public Messages UpdateMovie(Movie movie);
        public List<Movie> ScheduledMovies();
    }
}
