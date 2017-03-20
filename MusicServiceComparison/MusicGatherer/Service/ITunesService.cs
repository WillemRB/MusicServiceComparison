using System;
using System.Linq;
using System.Text.RegularExpressions;
using MusicServiceComparison.Models;
using MusicServiceComparison.MusicGatherer.Models;
using RestSharp;

namespace MusicServiceComparison.MusicGatherer.Service
{
    public class ITunesService
    {
        private readonly RestClient client;

        private readonly Regex singleRegex = new Regex(@"-\s?[Ss]ingle$", RegexOptions.Compiled);

        // Strips the following album notations:
        // - EP
        private readonly Regex cleanupRegex = new Regex(@"-\s?([Ee][Pp])", RegexOptions.Compiled);

        public ITunesService()
        {
            client = new RestClient
            {
                BaseUrl = new Uri("https://itunes.apple.com/")
            };

            client.AddDefaultHeader("accept", "application/json");
        }

        public ArtistModel Update(ArtistModel model)
        {
            if (model.ITunesId == null)
            {
                var artists = ArtistSearch(model.Name);

                if (artists.ResultCount > 0)
                {
                    var artist = artists.Results.FirstOrDefault(a => a.ArtistName.ToLower() == model.Name.ToLower());

                    if (artist == null)
                    {
                        // Artist likely doesn't exist on iTunes
                        return model;
                    }
                    
                    model.ITunesId = artist.ArtistId;

                    // iTunes doesn't have images for artists.
                }
            }

            var albums = AlbumSearch(model.ITunesId);
            albums.Results.ForEach(album =>
                {
                    // Do some checks to prevent too filter out unwanted results from the search results.
                    // - WrapperType must be collection, otherwise this process will fail.
                    // - There is no difference between singles and albums on iTunes. So we only consider albums with at least 5 tracks
                    //   to be actual albums.
                    // - ITunes tends to denote single by ending the name with '- Single' or variations.
                    if (model.Albums.Any(a => a.ITunesId == album.CollectionId) 
                        || (album.WrapperType != "collection" && album.TrackCount < 5)
                        || singleRegex.IsMatch(album.CollectionName))
                    {
                        return;
                    }

                    album.CollectionName = cleanupRegex.Replace(album.CollectionName, string.Empty).Trim();

                    var albumByName = model.Albums.FirstOrDefault(a => AlbumTitle.Compare(a.Name, album.CollectionName));
                    if (albumByName != null)
                    {
                        albumByName.Name = AlbumTitle.BestTitle(album.CollectionName, albumByName.Name);
                        albumByName.ITunesId = album.CollectionId;
                    }
                    else
                    {
                        model.Albums.Add(new AlbumModel { Name = album.CollectionName, ITunesId = album.CollectionId, Art = album.ArtworkUrl100 });
                    }
                });

            var removed = model.Albums.Where(a => a.ITunesId.HasValue).Select(a => (int)a.ITunesId)
                .Except(albums.Results.Where(a => a.WrapperType == "collection").Select(a => a.CollectionId)).ToList();

            removed.ForEach(id =>
            {
                var album = model.Albums.First(a => a.ITunesId == id);
                album.ITunesId = null;
            });


            return model;
        }

        public ITunesArtistModel ArtistSearch(string name)
        {
            var request = new RestRequest($"search?term={name}&entity=musicArtist", Method.GET);

            var response = client.Execute<ITunesArtistModel>(request);

            return response.Data;
        }

        public ITunesArtistModel ArtistLookup(int artistId)
        {
            var request = new RestRequest($"lookup?id={artistId}", Method.GET);

            var response = client.Execute<ITunesArtistModel>(request);

            return response.Data;
        }

        public ITunesAlbumModel AlbumSearch(int? artistId)
        {
            var request = new RestRequest($"lookup?id={artistId}&entity=album", Method.GET);

            var response = client.Execute<ITunesAlbumModel>(request);

            return response.Data;
        }
    }
}