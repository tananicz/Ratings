using System.ComponentModel.DataAnnotations;

namespace Ratings.Models
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Wprowadź jakąś wartość")]
        [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "Login powinien składać się jedynie z wielkich i małych liter, cyfr oraz znaków podkreślenia")]
        [MinLength(5, ErrorMessage = "Login powinien składać się z minimum 5 znaków")]
        [MaxLength(50, ErrorMessage = "Nazwa użytkownika powinna mieć nie więcej niż 50 znaków")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Wprowadź jakąś wartość")]
        [MinLength(8, ErrorMessage = "Hasło powinno składać się z minimum 8 znaków")]
        [MaxLength(50, ErrorMessage = "Hasło powinno mieć nie więcej niż 50 znaków")]
        public string Password { get; set; }
        
        public string CofirmPassword { get; set; }
        
        public bool AcceptRules { get; set; }

        public string Role { get; set; }

        public string IdentityId { get; set; }
    }
}
