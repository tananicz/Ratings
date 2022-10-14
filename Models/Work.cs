using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ratings.Models
{
    public class Work
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwę utworu")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Rok")]
        public int Year { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        public decimal AvgRating { get; set; }

        public int ArtistId { get; set; }

        public Artist Artist { get; set; }
    }
}
