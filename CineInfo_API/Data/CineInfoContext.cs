using Microsoft.EntityFrameworkCore;
using CineInfo_API.Models;

namespace CineInfo_API.Data; 
public class CineInfoContext : DbContext {
    public CineInfoContext(DbContextOptions<CineInfoContext> opts) : base(opts) { }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Session> Sessions { get; set; }
}
