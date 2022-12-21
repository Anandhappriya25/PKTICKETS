using Microsoft.EntityFrameworkCore;

namespace PKTickets.Models
{
    public class PKTicketsDbContext :DbContext
    {
        public PKTicketsDbContext(DbContextOptions<PKTicketsDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ShowTime> ShowTimes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
    }
}
