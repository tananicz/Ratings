namespace Ratings.Models
{
    public class ArtistViewModel
    {
        public Artist Artist { get; set; }
        public IEnumerable<Work> Works { get; set; }
    }
}
