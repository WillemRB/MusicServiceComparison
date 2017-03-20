using System;
using System.Text.RegularExpressions;
using MusicServiceComparison.Models;
using MusicServiceComparison.MusicGatherer.Service;

namespace MusicServiceComparison.MusicGatherer
{
    public class Gatherer : IMusicGatherer
    {
        private readonly SpotifyService spotify;

        private readonly ITunesService itunes;

        private readonly DeezerService deezer;

        private readonly GrooveService groove;

        public Gatherer(SpotifyService spotify, ITunesService itunes, DeezerService deezer, GrooveService groove)
        {
            this.spotify = spotify;
            this.itunes = itunes;
            this.deezer = deezer;
            this.groove = groove;
        }

        public ArtistModel Update(string name)
        {
            return Update(new ArtistModel { Name = name });
        }

        public ArtistModel GetFromITunes(int id)
        {
            try
            {
                var artist = itunes.ArtistLookup(id).Results[0];

                return Update(new ArtistModel { ITunesId = artist.ArtistId, Name = artist.ArtistName });
            }
            catch
            {
                throw;
            }
        }

        public ArtistModel GetFromSpotify(string id)
        {
            try
            {
                var artist = spotify.ArtistLookup(id);

                return Update(new ArtistModel { SpotifyId = artist.Id, Name = artist.Name });
            }
            catch
            {
                throw;
            }
        }

        public ArtistModel Update(ArtistModel model)
        {
            itunes.Update(model);
            spotify.Update(model);
            deezer.Update(model);
            //groove.Update(model);

            model.LastUpdate = DateTime.Now;

            return model;
        }
    }

    public class AlbumTitle
    {
        private static Regex removeUnwantedCharacters = new Regex(@"[^a-z0-9]", RegexOptions.Compiled);

        private static Regex removeBrackets = new Regex(@"\(.*?\)|\[.*?\]", RegexOptions.Compiled);

        /// <summary>
        /// Compares two titles and returns whether they can be considered equal.
        /// </summary>
        /// <remarks>
        /// Special cases to consider:
        /// Artist - Meat Loaf:
        /// - Bat out of Hell
        /// - Bat out of Hell II
        /// - Bat out of Hell 3
        /// and
        /// - Couldn´t Have Said It Better
        /// - Couldn't Have Said It Better
        /// (Note the difference in Couldn't)
        /// 
        /// Artist - Dark Forest:
        /// iTunes lists both as seperate bands.
        /// Spotify had 'Dark Forest' and 'The Dark Forest'
        /// Deezer combines both bands on one page
        /// </remarks>
        /// <param name="title1"></param>
        /// <param name="title2"></param>
        /// <returns></returns>
        public static bool Compare(string title1, string title2)
        {
            // Initial version of compare.
            //return title1.ToLower() == title2.ToLower();

            // Version 1
            //var shortest = title1.Length < title2.Length ? title1 : title2;
            //var longest = title1.Length < title2.Length ? title2 : title1;

            //return longest.ToLower().Contains(shortest.ToLower());
            
            title1 = title1.ToLower();
            title2 = title2.ToLower();

            title1 = removeBrackets.Replace(title1, string.Empty);
            title2 = removeBrackets.Replace(title2, string.Empty);

            // Don't remove unwanted characters before brackets!
            title1 = removeUnwantedCharacters.Replace(title1, string.Empty);
            title2 = removeUnwantedCharacters.Replace(title2, string.Empty);

            return title1 == title2;
        }

        public static string BestTitle(string title1, string title2)
        {
            if (title1.Length < title2.Length)
            {
                return title1;
            }

            return title2;
        }
    }
}