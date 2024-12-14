using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyApp.Server.Models
{
    public class StudySession
    {
        [Key] // Marks this property as the primary key
        public int Id { get; set; } // Auto-incrementing ID
        public string UserId { get; set; }
        public string Token { get; set; }
        public string StudyDate { get; set; }
        public TimeSpan Duration { get; set; }
        public string TasksJson { get; set; }
        public string MusicHistoryJson { get; set; } // List of song names or ids
        public int Productivity { get; set; } // 1-10 scale for productivity
        public string Genre { get; set; } // Most played song genre
    }
}
