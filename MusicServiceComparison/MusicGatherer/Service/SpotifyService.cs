using System;
using System.Linq;
using MusicServiceComparison.Models;
using MusicServiceComparison.MusicGatherer.Models;
using RestSharp;

namespace MusicServiceComparison.MusicGatherer.Service
{
    public class SpotifyService
    {
        private readonly RestClient client;

        public SpotifyService()
        {
            client = new RestClient
            {
                BaseUrl = new Uri("https://api.spotify.com/v1/")
            };

            client.AddDefaultHeader("accept", "application/json");
        }

        public ArtistModel Update(ArtistModel model)
        {
            if (model.SpotifyId == null)
            {
                var artists = ArtistSearch(model.Name);

                if (artists.Artists.Total > 0)
                {
                    var artist = artists.Artists.Items.FirstOrDefault(a => a.Name.ToLower() == model.Name.ToLower());

                    if (artist == null)
                    {
                        // Artist likely doesn't exist on Spotify
                        return model;
                    }
                    
                    model.SpotifyId = artist.Id;

                    if (string.IsNullOrEmpty(model.Image) && artist.Images.Any())
                    {
                        model.Image = artist.Images.First().Url;
                    }
                }
            }
            
            var albums = AlbumSearch(model.SpotifyId);

            albums.Items.ForEach(album =>
            {
                if (model.Albums.Any(a => a.SpotifyId == album.Id))
                {
                    return;
                }
                
                var albumByName = model.Albums.FirstOrDefault(a => AlbumTitle.Compare(a.Name, album.Name));
                if (albumByName != null)
                {
                    albumByName.Name = AlbumTitle.BestTitle(album.Name, albumByName.Name);
                    albumByName.SpotifyId = album.Id;
                }
                else
                {
                    var img = (album.Images.First() ?? new SpotifyImage()).Url;
                    model.Albums.Add(new AlbumModel { Name = album.Name, SpotifyId = album.Id, Art = img });
                }
            });

            var removed = model.Albums.Select(a => a.SpotifyId)
                .Except(albums.Items.Select(a => a.Id)).ToList();

            removed.ForEach(id =>
            {
                var album = model.Albums.First(a => a.SpotifyId == id);
                album.SpotifyId = null;
            });

            return model;
        }

        public SpotifyArtistModel ArtistSearch(string name)
        {
            var request = new RestRequest($"search?q={name}&type=artist", Method.GET);

            var response = client.Execute<SpotifyArtistModel>(request);

            return response.Data;
        }

        public SpotifyArtistResultItem ArtistLookup(string artistId)
        {
            var request = new RestRequest($"artists/{artistId}", Method.GET);

            var response = client.Execute<SpotifyArtistResultItem>(request);

            return response.Data;
        }

        public SpotifyAlbumModel AlbumSearch(string artistId)
        {
            var request = new RestRequest($"artists/{artistId}/albums?album_type=album&limit=50", Method.GET);

            var response = client.Execute<SpotifyAlbumModel>(request);

            return response.Data;
        }
    }
}