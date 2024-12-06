namespace SpotifyTestApp.Models
{
    public class ReviewingPlaylist
    {
        //Reviewing Playlist - Recommended relaxing music/instrumental music --> Genre in Spotify: Relaxative
        //User Specific - Different genres and artists

        //Ranges were recommended by ChatGPT
        public float? MinAcousticness { get; set; } = 0.5f;
        public float? MaxAcousticness { get; set; } = 1;
        public float? MinDanceability { get; set; } = 0.3f;
        public float? MaxDanceability { get; set; } = 0.5f;
        public float? MinEnergy { get; set; } = 0.2f;
        public float? MaxEnergy { get; set; } = 0.5f;
        public float? MinInstrumentalness { get; set; } = 0.5f;
        public float? MaxInstrumentalness { get; set; } = 1;
        public float? MinLoudness { get; set; } = -20;
        public float? MaxLoudness { get; set; } = -10;
        public float? MinSpeechiness { get; set; } = 0.02f;
        public float? MaxSpeechiness { get; set; } = 0.2f;
        public float? MinTempo { get; set; } = 60;
        public float? MaxTempo { get; set; } = 120;
        public float? MinValence { get; set; } = 0.3f;
        public float? MaxValence { get; set; } = 0.6f;

        //More flexible ranges
        public float? TargetAcousticness { get; set; } = 0.75f;
        public float? TargetDanceability { get; set; } = 0.4f;
        public float? TargetEnergy { get; set; } = 0.35f;
        public float? TargetInstrumentalness { get; set; } = 0.75f;
        public float? TargetLoudness { get; set; } = -15f;
        public float? TargetSpeechiness { get; set; } = 0.11f;
        public float? TargetTempo { get; set; } = 90;
        public float? TargetValence { get; set; } = 0.45f;
    }
}
