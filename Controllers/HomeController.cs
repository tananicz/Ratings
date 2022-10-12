using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ratings.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View((Object) "To jest strona główna");
        }

        public IActionResult SomeOtherPage()
        {
            return View("index", "To jest jakaś inna, nie-główna strona, z jakąś tam inną zawartością...");
        }

        [Authorize(Roles = "admin")]
        public IActionResult ForAdmin()
        {
            return View("index", "Do tej strony mają dostęp wyłącznie administratorzy");
        }

        [Authorize(Roles = "moderator,admin")]
        public IActionResult ForModerator()
        {
            return View("index", "Do tej strony mają dostęp wyłącznie moderatorzy i administratorzy");
        }

        [Authorize(Roles = "user,moderator,admin")]
        public IActionResult ForUser()
        {
            return View("index", "Ta strona może być wyświetlona zalogowanym użytkownikom");
        }
    }
}
