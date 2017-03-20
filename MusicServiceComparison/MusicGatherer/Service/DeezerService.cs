using System;
using System.Linq;
using MusicServiceComparison.Models;
using MusicServiceComparison.MusicGatherer.Models;
using RestSharp;

namespace MusicServiceComparison.MusicGatherer.Service
{
    public class DeezerService
    {
        private readonly RestClient client;

        // Default image for Deezer is a gray image.
        private const string DefaultImage = "https://cdns-images.dzcdn.net/images/artist//250x250-000000-80-0-0.jpg";

        private const string MP3AlbumString = "(MP3 Album)";

        public DeezerService()
        {
            client = new RestClient
            {
                BaseUrl = new Uri("https://api.deezer.com/")
            };

            client.AddDefaultHeader("accept", "application/json");
        }

        public ArtistModel Update(ArtistModel model)
        {
            if (model.DeezerId == null)
            {
                var artists = ArtistSearch(model.Name);

                if (artists.Total > 0)
                {
                    var artist = artists.Data.FirstOrDefault(a => a.Name.ToLower() == model.Name.ToLower());

                    if (artist == null)
                    {
                        // Artist likely doesn't exist on Deezer
                        return model;
                    }
                    
                    model.DeezerId = artist.Id;
                    
                    // Ignore the Deezer default image. This is already used when no image is available.
                    if (string.IsNullOrEmpty(model.Image) && artist.Picture_medium != DefaultImage)
                    {
                        model.Image = artist.Picture_medium;
                    }
                }
            }

            var albums = AlbumSearch(model.DeezerId);

            albums.Data.ForEach(album =>
            {
                // Filter out unwanted results:
                // - If an album already exists we skip it.
                // - Record_type must be "album". Singles will have "single".
                if (model.Albums.Any(a => a.DeezerId == album.Id) || album.Record_type != "album")
                {
                    return;
                }

                // For some reason Deezer has a lot of albums ending in 'MP3 Album' and starting with the artist name.
                // This is useless data because it will never match other albums, so first we clean it up.
                if (album.Title.EndsWith(MP3AlbumString))
                {
                    album.Title = album.Title.Replace(MP3AlbumString, string.Empty);

                    album.Title = album.Title.Substring(album.Title.IndexOf('-') + 1).Trim();
                }

                var albumByName = model.Albums.FirstOrDefault(a => AlbumTitle.Compare(a.Name, album.Title));
                if (albumByName != null)
                {
                    albumByName.Name = AlbumTitle.BestTitle(album.Title, albumByName.Name);
                    albumByName.DeezerId = album.Id;
                }
                else
                {
                    model.Albums.Add(new AlbumModel { Name = album.Title, DeezerId = album.Id, Art = album.Cover_medium });
                }
            });

            var removed = model.Albums.Where(a => a.DeezerId.HasValue).Select(a => (int)a.DeezerId)
                .Except(albums.Data.Select(a => a.Id)).ToList();

            removed.ForEach(id =>
            {
                var album = model.Albums.First(a => a.DeezerId == id);
                album.DeezerId = null;
            });

            return model;
        }

        public DeezerArtistModel ArtistSearch(string name)
        {
            var request = new RestRequest($"search/artist?q={name}", Method.GET);

            var response = client.Execute<DeezerArtistModel>(request);

            return response.Data;
        }

        public DeezerAlbumModel AlbumSearch(int? artistId)
        {
            if (!artistId.HasValue)
            {
                return new DeezerAlbumModel();
            }

            var request = new RestRequest($"artist/{artistId}/albums", Method.GET);

            var response = client.Execute<DeezerAlbumModel>(request);

            return response.Data;
        }
    }
}