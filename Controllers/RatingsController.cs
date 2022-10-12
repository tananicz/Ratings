using Microsoft.AspNetCore.Mvc;
using Ratings.Models;
using Ratings.Repository;

namespace Ratings.Controllers
{
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
    }
}
