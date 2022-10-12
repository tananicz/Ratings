using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ratings.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        public decimal RatingValue { get; set; }

        [MaxLength(4096)]
        public string Review { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public int WorkId { get; set; }

        public Work Work { get; set; }
    }
}
