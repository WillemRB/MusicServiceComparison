using System;
using System.Linq;
using System.Web.Mvc;
using MusicServiceComparison.Models;
using MusicServiceComparison.MusicGatherer;
using MusicServiceComparison.Repository;

namespace MusicServiceComparison.Controllers
{
    public class ArtistController : Controller
    {
        private readonly IArtistRepository repository;

        private readonly IMusicGatherer gatherer;

        public ArtistController(IArtistRepository repository, IMusicGatherer gatherer)
        {
            this.repository = repository;
            this.gatherer = gatherer;
        }

        [HttpGet, Route("artist/{artist}")]
        public ActionResult GetArtist(string artist)
        {
            var model = repository.Get(artist);

            if (model != null)
            {
                CheckForUpdate(model);

                model.Albums = model.Albums.OrderBy(m => m.Name).ToList();
            }

            return View("ArtistView", model);
        }

        [HttpGet, Route("artist/itunes/{id}")]
        public ActionResult GetArtistByITunesId(int id)
        {
            var model = repository.Get(id.ToString());

            if (model == null)
            {
                model = gatherer.GetFromITunes(id);
            }

            CheckForUpdate(model);

            return View("ArtistView", model);
        }

        [HttpGet, Route("artist/spotify/{id}")]
        public ActionResult GetArtistBySpotifyId(string id)
        {
            var model = repository.Get(id.ToString());

            if (model == null)
            {
                model = gatherer.GetFromSpotify(id);
            }

            CheckForUpdate(model);

            return View("ArtistView", model);
        }

        private void CheckForUpdate(ArtistModel model)
        {
            if (model.LastUpdate.AddDays(7) < DateTime.Now)
            {
                model = gatherer.Update(model);
                repository.Update(model);
            }
        }
    }
}