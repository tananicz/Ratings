using System.ComponentModel.DataAnnotations;

namespace Ratings.Models
{
    public class Artist
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Musisz podać nazwisko")]
        [MaxLength(50)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Musisz podać imię lub imiona")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(4096)]
        public string Bio { get; set; }

        public byte[] Photo { get; set; }
    }
}
