using Microsoft.EntityFrameworkCore;
using SpotifyTestApp.Models;

namespace SpotifyTestApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Define your DbSets (tables)
        public DbSet<StudySession> StudySessions { get; set; }
    }
}
