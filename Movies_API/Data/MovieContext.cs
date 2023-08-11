using Microsoft.EntityFrameworkCore;
using Movies_API.Models;

namespace Movies_API.Data {
    public class MovieContext : DbContext {
        public MovieContext(DbContextOptions<MovieContext> opts) : base(opts) { }

        public DbSet<Movie> Movies { get; set; }
    }
}
