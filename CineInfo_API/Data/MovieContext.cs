using Microsoft.EntityFrameworkCore;
using CineInfo_API.Models;

namespace CineInfo_API.Data; 
public class MovieContext : DbContext {
    public MovieContext(DbContextOptions<MovieContext> opts) : base(opts) { }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
}
