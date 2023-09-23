using DUTC.Models.DTO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DUTC.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie>Movies { get; set; }
        public DbSet<MovieGenre> MoviesGenres { get; set;}
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showtime>Showtimes { get; set; }
        public DbSet<Ticket>Tickets { get; set; }
        public DbSet<SeatBuy> SeatsBuy { get; set; }
        public DbSet<Review>Reviews { get; set; }
        public DbSet<RevenueMovie>Revenues { get; set; }
        public DbSet<CustomerReview> CustomerReviews { get; set; }
    }
}