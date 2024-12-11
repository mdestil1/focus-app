using Microsoft.EntityFrameworkCore;
using StudyApp.Server.Models;
using System.Text.Json;



namespace StudyApp.Server.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        // Define your DbSets (tables)
        public DbSet<StudySession> StudySessions { get; set; }
    }
    
    
    
}