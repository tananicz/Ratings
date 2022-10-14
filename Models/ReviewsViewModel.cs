namespace Ratings.Models
{
    public class ReviewsViewModel
    {
        public Work Work { get; set; }
        public IEnumerable<Rating> Reviews { get; set; }
    }
}
