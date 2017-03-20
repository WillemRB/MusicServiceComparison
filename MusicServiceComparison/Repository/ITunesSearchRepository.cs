using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MusicServiceComparison.MusicGatherer.Service;

namespace MusicServiceComparison.Repository
{
    public class ITunesSearchRepository : ISearchRepository
    {
        private readonly ITunesService service;

        public ITunesSearchRepository(ITunesService service)
        {
            this.service = service;
        }

        public void Search(string query)
        {
            //var itunesId = int.Parse(id);

            service.ArtistSearch(query);
        }
    }
}