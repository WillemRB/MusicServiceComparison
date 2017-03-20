namespace MusicServiceComparison.Models
{
    public class AlbumModel
    {
        public string Name { get; set; }

        public bool Hide { get; set; }

        public string Art { get; set; }

        public int? ITunesId { get; set; }

        public string SpotifyId { get; set; }

        public int? DeezerId { get; set; }

        public string GrooveId { get; set; }
    }
}