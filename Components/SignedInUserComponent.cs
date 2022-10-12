using Microsoft.AspNetCore.Mvc;

namespace Ratings.Components
{
    public class SignedInUserComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}