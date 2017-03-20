using System.Collections.Generic;
using MusicServiceComparison.Models;

namespace MusicServiceComparison.Repository
{
    public interface IArtistRepository
    {
        IEnumerable<ArtistModel> Search(string query);

        ArtistModel Get(string id);

        void Insert(ArtistModel artistModel);

        void Update(ArtistModel artistModel);
    }
}
