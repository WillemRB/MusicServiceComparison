using MusicServiceComparison.Models;

namespace MusicServiceComparison.MusicGatherer
{
    public interface IMusicGatherer
    {
        ArtistModel Update(string name);

        ArtistModel Update(ArtistModel model);

        ArtistModel GetFromITunes(int id);

        ArtistModel GetFromSpotify(string id);
    }
}
