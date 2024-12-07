using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyTestApp.Models
{
    public class StudySession
    {
        [Key] // Marks this property as the primary key
        public int Id { get; set; } // Auto-incrementing ID

        public string UserId { get; set; }
        public DateTime StudyDate { get; set; }
        public string SongAudioFeaturesJson { get; set; } // Store song features in a dictionary
        public List<string> MusicHistory { get; set; } // List of song names or ids
        public int Productivity { get; set; } // 1-10 scale for productivity
        public string Genre { get; set; } // Most played song genre

        [NotMapped] //Don't map this to a column
        public Dictionary<string, double> SongAudioFeatures
        {
            get
            {
                return string.IsNullOrEmpty(SongAudioFeaturesJson)
                    ? new Dictionary<string, double>()
                    : JsonConvert.DeserializeObject<Dictionary<string, double>>(SongAudioFeaturesJson);
            }
            set
            {
                SongAudioFeaturesJson = JsonConvert.SerializeObject(value);
            }
        }
    }
}
