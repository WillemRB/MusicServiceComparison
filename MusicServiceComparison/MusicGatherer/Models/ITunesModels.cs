using System.Collections.Generic;

namespace MusicServiceComparison.MusicGatherer.Models
{
    #region Artist Search
    public class ITunesArtistModel
    {
        public int ResultCount { get; set; }

        public List<ITunesArtistResult> Results { get; set; } = new List<ITunesArtistResult>();
    }

    public class ITunesArtistResult
    {
        public string ArtistType { get; set; }

        public string ArtistName { get; set; }

        public int ArtistId { get; set; }
    }
    #endregion

    #region Album Search
    public class ITunesAlbumModel
    {
        public int ResultCount { get; set; }

        public List<ITunesAlbumResult> Results { get; set; } = new List<ITunesAlbumResult>();
    }

    public class ITunesAlbumResult
    {
        public string WrapperType { get; set; }

        public string CollectionType { get; set; }

        public int CollectionId { get; set; }

        public string CollectionName { get; set; }

        public string ArtworkUrl100 { get; set; }

        public int TrackCount { get; set; }
    }
    #endregion
}