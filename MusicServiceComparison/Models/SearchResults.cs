using System.Collections.Generic;
using MusicServiceComparison.MusicGatherer.Models;

namespace MusicServiceComparison.Models
{
    public class SearchResults
    {
        public IEnumerable<ArtistModel> Results { get; set; }

        public string Query { get; set; }

        public string Id { get; set; }
    }

    public class ITunesSearchResults
    {
        public IEnumerable<ITunesArtistModel> Results { get; set; }

        public string Query { get; set; }
    }
}