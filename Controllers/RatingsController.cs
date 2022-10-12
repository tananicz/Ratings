using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ratings.Filters;
using Ratings.Models;
using Ratings.Repository;

namespace Ratings.Controllers
{
    [RoleAdderFilter]
    public class RatingsController : Controller
    {
        private RatingsRepository _repository;

        public RatingsController(RatingsRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ShowArtists()
        {
            return View(_repository.GetArtists());
        }

        public IActionResult ShowArtistAndWorks(int id)
        {
            ArtistViewModel viewModel = new ArtistViewModel
            {
                Artist = _repository.GetArtist(id),
                Works = _repository.GetArtistsWorks(id)
            };
            return View(viewModel);
        }

        [Authorize(Roles = "user,moderator,admin")]
        public IActionResult RateAWork(int id)
        {
            return View(_repository.GetRating(id, User.Identity.Name));
        }
    }
}
