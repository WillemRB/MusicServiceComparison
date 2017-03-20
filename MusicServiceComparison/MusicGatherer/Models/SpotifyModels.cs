using System.Collections.Generic;

namespace MusicServiceComparison.MusicGatherer.Models
{
    #region Artist Search
    public class SpotifyArtistModel
    {
        public SpotifyArtistResult Artists { get; set; }
    }

    public class SpotifyArtistResult
    {
        public int Limit { get; set; }

        public int Total { get; set; }

        public List<SpotifyArtistResultItem> Items { get; set; } = new List<SpotifyArtistResultItem>();
    }

    public class SpotifyArtistResultItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<SpotifyImage> Images { get; set; } = new List<SpotifyImage>();
    }
    #endregion

    #region Album Search
    public class SpotifyAlbumModel
    {
        public List<SpotifyAlbumModelItem> Items { get; set; } = new List<SpotifyAlbumModelItem>();

        public int Total { get; set; }

        public int Offset { get; set; }

        public string Next { get; set; }
    }

    public class SpotifyAlbumModelItem
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Id { get; set; }

        public List<SpotifyImage> Images { get; set; } = new List<SpotifyImage>();
    }
    #endregion

    public class SpotifyImage
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public string Url { get; set; }
    }
}