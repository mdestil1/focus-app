namespace SpotifyTestApp.Models
{
    public class RewatchingPlaylist
    {
        //Rewatching Playlist - Recommended classical music/instrumental music --> Genre in Spotify: Classical
        //User Specific - Different genres and artists

        //Ranges were recommended by ChatGPT
        public float? MinAcousticness { get; set; } = 0.8f;
        public float? MaxAcousticness { get; set; } = 1;
        public float? MinDanceability { get; set; } = 0.1f;
        public float? MaxDanceability { get; set; } = 0.4f;
        public float? MinEnergy { get; set; } = 0.1f;
        public float? MaxEnergy { get; set; } = 0.7f;
        public float? MinInstrumentalness { get; set; } = 0.9f;
        public float? MaxInstrumentalness { get; set; } = 1;
        public float? MinLoudness { get; set; } = -30;
        public float? MaxLoudness { get; set; } = -10;
        public float? MinSpeechiness { get; set; } = 0.02f;
        public float? MaxSpeechiness { get; set; } = 0.15f;
        public float? MinTempo { get; set; } = 40;
        public float? MaxTempo { get; set; } = 180;
        public float? MinValence { get; set; } = 0.2f;
        public float? MaxValence { get; set; } = 0.6f;

        //More flexible ranges
        public float? TargetAcousticness { get; set; } = 0.9f;
        public float? TargetDanceability { get; set; } = 0.25f;
        public float? TargetEnergy { get; set; } = 0.4f;
        public float? TargetInstrumentalness { get; set; } = 0.95f;
        public float? TargetLoudness { get; set; } = -20f;
        public float? TargetSpeechiness { get; set; } = 0.085f;
        public float? TargetTempo { get; set; } = 110;
        public float? TargetValence { get; set; } = 0.4f;
    }
}
