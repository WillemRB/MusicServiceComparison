using System.Collections.Generic;
using System.Text;
using System.Web;
using LiteDB;
using MusicServiceComparison.Models;

namespace MusicServiceComparison.Repository
{
    public class LiteDBArtistRepository : IArtistRepository
    {
        private readonly LiteDatabase db;
        public LiteDBArtistRepository()
        {
            #if DEBUG
                db = new LiteDatabase(@"artists.db");
            #else
                db = new LiteDatabase(@"D:\home\data\artists.db");
            #endif
        }

        public ArtistModel Get(string id)
        {
            var artists = db.GetCollection<ArtistModel>("artists");
            
            return artists.FindOne(artist => artist.Id == id);

        }

        public void Insert(ArtistModel artistModel)
        {
            var artists = db.GetCollection<ArtistModel>("artists");
            
            if (string.IsNullOrEmpty(artistModel.Id))
            {
                var baseId = HttpUtility.UrlEncode(artistModel.Name, Encoding.GetEncoding("iso-8859-7"));
                var id = baseId;

                var count = 1;

                while (artists.Exists(a => a.Id == id))
                {
                    id = baseId + "-" + count;
                    count++;
                }

                artistModel.Id = id;
            }

            // Sort before saving would save CPU, but does it work?
            //artistModel.Albums = artistModel.Albums.OrderBy(a => a.Name).ToList();

            artists.Insert(artistModel);

            artists.EnsureIndex(artist => artist.Name, new IndexOptions { IgnoreCase = true, RemoveAccents = true, TrimWhitespace = true });
            artists.EnsureIndex(artist => artist.Id);
            artists.EnsureIndex(artist => artist.ITunesId);
            //artists.EnsureIndex(artist => artist.SpotifyId);
            //artists.EnsureIndex(artist => artist.DeezerId);
        }

        public IEnumerable<ArtistModel> Search(string query)
        {
            var artists = db.GetCollection<ArtistModel>("artists");

            return artists.Find(artist => artist.Name.Contains(query.ToLower()), limit: 5);
        }
        
        public void Update(ArtistModel artistModel)
        {
            var artists = db.GetCollection<ArtistModel>("artists");

            artists.Update(artistModel);
        }
    }
}