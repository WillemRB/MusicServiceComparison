using System;
using System.Linq;
using System.Collections.Generic;
using LiteDB;

namespace MusicServiceComparison.Models
{
    public class ArtistModel
    {
        public ArtistModel()
        {
            LastUpdate = DateTime.Now;
            Albums = new List<AlbumModel>();
        }
        
        public string Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        /// <summary>
        /// Timestamp of the last update.
        /// </summary>
        public DateTime LastUpdate { get; set; }

        public int? ITunesId { get; set; }

        public string SpotifyId { get; set; }

        public int? DeezerId { get; set; }

        public string GrooveId { get; set; }

        public IList<AlbumModel> Albums { get; set; }

        [BsonIgnore]
        public IList<AlbumModel> SortedAlbums
        {
            get { return Albums.OrderBy(m => m.Name).ToList(); }
        }
    }
}