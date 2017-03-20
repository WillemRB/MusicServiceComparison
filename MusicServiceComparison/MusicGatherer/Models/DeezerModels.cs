using System.Collections.Generic;

namespace MusicServiceComparison.MusicGatherer.Models
{
    #region Artist Search
    public class DeezerArtistModel
    {
        public int Total { get; set; }

        public List<DeezerArtistItem> Data { get; set; } = new List<DeezerArtistItem>();
    }

    public class DeezerArtistItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Picture_medium { get; set; }
    }
    #endregion

    #region Album Search
    public class DeezerAlbumModel
    {
        public int Total { get; set; }

        public List<DeezerAlbumItem> Data { get; set; } = new List<DeezerAlbumItem>();
    }

    public class DeezerAlbumItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Cover_medium { get; set; }

        public string Record_type { get; set; }
    }
    #endregion
}