namespace SpotifyTestApp.Models
{
    public class BrainstormingPlaylist
    {
        //Brainstorming Playlist - Recommended ambient noise --> Genre: Ambient
        //User Specific - Different genres and artists

        //Ranges were recommended by ChatGPT
        public float? MinAcousticness { get; set; } = 0.4f;
        public float? MaxAcousticness { get; set; } = 0.9f;
        public float? MinDanceability { get; set; } = 0.2f; 
        public float? MaxDanceability { get; set; } = 0.4f; 
        public float? MinEnergy { get; set; } = 0.1f;
        public float? MaxEnergy { get; set; } = 0.3f;
        public float? MinInstrumentalness { get; set; } = 0.7f;
        public float? MaxInstrumentalness { get; set; } = 1;
        public float? MinLoudness { get; set; } = -25;
        public float? MaxLoudness { get; set; } = -10;
        public float? MinSpeechiness { get; set; } = 0.02f;
        public float? MaxSpeechiness { get; set; } = 0.1f;
        public float? MinTempo { get; set; } = 40;
        public float? MaxTempo { get; set; } = 90;
        public float? MinValence { get; set; } = 0.2f;
        public float? MaxValence { get; set; } = 0.5f;

        //More flexible ranges
        public float? TargetAcousticness { get; set; } = 0.65f;
        public float? TargetDanceability { get; set; } = 0.3f;
        public float? TargetEnergy { get; set; } = 0.2f;
        public float? TargetInstrumentalness { get; set; } = 0.85f;
        public float? TargetLoudness { get; set; } = -17.5f;
        public float? TargetSpeechiness { get; set; } = 0.06f;
        public float? TargetTempo { get; set; } = 65;
        public float? TargetValence { get; set; } = 0.35f;

    }
}
