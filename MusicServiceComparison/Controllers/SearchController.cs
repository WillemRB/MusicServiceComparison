using System.Web.Mvc;
using MusicServiceComparison.MusicGatherer.Service;

namespace MusicServiceComparison.Controllers
{
    public class SearchController : Controller
    {
        private readonly ITunesService itunes;

        private readonly SpotifyService spotify;

        public SearchController(ITunesService itunes, SpotifyService spotify)
        {
            this.itunes = itunes;
            this.spotify = spotify;
        }
        
        [HttpGet, Route("search/itunes")]
        public ActionResult SearchOnITunes()
        {
            var query = Request.QueryString["q"];

            var results = itunes.ArtistSearch(query);

            return View("ITunesSearchResults", results);
        }

        [HttpGet, Route("search/spotify")]
        public ActionResult SearchOnSpotify()
        {
            var query = Request.QueryString["q"];

            var results = spotify.ArtistSearch(query);

            return View("SpotifySearchResults", results);
        }
    }
}