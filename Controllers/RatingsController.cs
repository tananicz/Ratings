using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ratings.Filters;
using Ratings.Helpers;
using Ratings.ModelBinders;
using Ratings.Models;
using Ratings.Repository;
using System.Text.RegularExpressions;

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

        public async Task<IActionResult> ShowArtistAndWorks(int id)
        {
            ArtistViewModel viewModel = new ArtistViewModel
            {
                Artist = await _repository.GetArtist(id),
                Works = _repository.GetArtistsWorks(id)
            };
            return View(viewModel);
        }

        public async Task<IActionResult> ShowReviews([FromRoute(Name = "id")] int workId)
        {
            return View(new ReviewsViewModel
            {
                Work = await _repository.GetWork(workId, true),
                Reviews = await _repository.GetReviewsForWork(workId)
            });
        }

        [Authorize(Roles = "user,moderator,admin")]
        public async Task<IActionResult> RateAWork([FromRoute(Name = "id")] int workId)
        {
            ViewBag.WorkId = workId;
            return View(await _repository.GetRating(workId, User.Identity.Name));
        }

        [HttpPost]
        [Authorize(Roles = "user,moderator,admin")]
        public async Task<IActionResult> SaveRating([ModelBinder(BinderType = typeof(CustomRatingModelBinder))] Rating rating)
        {
            if (ModelState.IsValid)
            {
                rating.UserName = User.Identity.Name;
                if (rating.Review != null)
                { 
                    rating.Review = Regex.Replace(rating.Review, @"\s+", " ");
                    rating.Review = Regex.IsMatch(rating.Review, @"^\s+$") ? "" : rating.Review;
                }

                if (rating.Id == 0)
                {
                    await _repository.AddRating(rating);
                }
                else
                {
                    await _repository.UpdateRating(rating);
                }

                Work work = await _repository.GetWork(rating.WorkId, true);
                work.AvgRating = await _repository.ComputeRatingForWork(rating.WorkId);
                await _repository.UpdateWork(work);

                return RedirectToAction("ShowArtistAndWorks", new { id = work.ArtistId });
            }
            return View("RateAWork", rating);
        }

        [Authorize(Roles = "moderator,admin")]
        public IActionResult AddArtist()
        {
            return View("AddOrEditArtist", new ArtistViewModel());
        }

        [Authorize(Roles = "moderator,admin")]
        public async Task<IActionResult> EditArtist(int id, string returnUrl)
        {
            Artist artist = await _repository.GetArtist(id);
            if (artist != null)
            { 
                ViewBag.ReturnUrl = returnUrl;
                ArtistViewModel viewModel = new ArtistViewModel
                {
                    Artist = artist,
                    Works = _repository.GetArtistsWorks(id)
                };
                return View("AddOrEditArtist", viewModel);
            }
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        [HttpPost]
        [Authorize(Roles = "moderator,admin")]
        public async Task<IActionResult> PerformAddOrEditArtist([FromForm(Name = "Artist")] Artist artist, string returnUrl, IFormFile image)
        {
            if (image != null)
            { 
                SetModelStateForFile(image);
            }

            if (ModelState.IsValid)
            {
                bool continueWithAction = true;

                if (image != null)
                {
                    try 
                    { 
                        artist.Photo = await AppHelper.GetImageBytes(image.OpenReadStream());
                    }
                    catch
                    {
                        ModelState.AddModelError("Artist.Photo", "Wystąpił problem z załadowaniem obrazka");
                        continueWithAction = false;
                    }
                }

                if (continueWithAction)
                { 
                    if (artist.Id == 0)
                    {
                        await _repository.AddArtist(artist);
                    }
                    else
                    {
                        await _repository.UpdateArtist(artist);
                    }

                    return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("ShowArtists");
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View("AddOrEditArtist", new ArtistViewModel 
            { 
                Artist = artist,
                Works = _repository.GetArtistsWorks(artist.Id)
            });
        }

        [HttpPost]
        [Authorize(Roles = "moderator,admin")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            Artist artistToRemove = await _repository.GetArtist(id);
            if (artistToRemove != null)
            {
                await _repository.DeleteArtist(artistToRemove);
                return RedirectToAction("ShowArtists");
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }

        [Authorize(Roles = "moderator,admin")]
        public async Task<IActionResult> AddWork([FromRoute(Name = "id")] int artistId, string returnUrlForEdit)
        {
            ViewBag.ReturnUrlForEdit = returnUrlForEdit;
            Artist artist = await _repository.GetArtist(artistId);
            if (artist != null)
            {
                return View("AddOrEditWork", new Work
                {
                    Artist = artist,
                });
            }
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        [Authorize(Roles = "moderator,admin")]
        public async Task<IActionResult> EditWork(int id, string returnUrlForEdit)
        {
            ViewBag.ReturnUrlForEdit = returnUrlForEdit;
            Work work = await _repository.GetWork(id, true);
            if (work != null)
            {
                return View("AddOrEditWork", work);
            }
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        [HttpPost]
        [Authorize(Roles = "moderator,admin")]
        public async Task<IActionResult> PerformAddOrEditWork(Work work, string returnUrlForEdit)
        {
            if (work.Year < 1 || work.Year > DateTime.Now.Year)
            {
                ModelState.AddModelError(nameof(Work.Year), "Rok wydania musi należeć do ery nowożytnej i nie może pochodzić z przyszłości");
            }

            if (ModelState.IsValid)
            {
                if (work.Id == 0)
                { 
                    await _repository.AddWork(work);
                }
                else
                {
                    await _repository.UpdateWork(work);
                }
                return RedirectToAction("EditArtist", new { id = work.ArtistId, returnUrl = returnUrlForEdit });
            }
            ViewBag.ReturnUrlForEdit = returnUrlForEdit;
            work.Artist = await _repository.GetArtist(work.ArtistId);
            return View("AddOrEditWork", work);
        }

        [HttpPost]
        [Authorize(Roles = "moderator,admin")]
        public async Task<IActionResult> DeleteWork(int id, string returnUrlForEdit)
        {
            Work workToRemove = await _repository.GetWork(id, false);
            if (workToRemove != null)
            {
                int artistId = workToRemove.ArtistId;
                await _repository.DeleteWork(workToRemove);
                return RedirectToAction("EditArtist", new { id = artistId, returnUrl = returnUrlForEdit });
            }
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        private void SetModelStateForFile(IFormFile file)
        {
            string ext = Path.GetExtension(file.FileName);
            if (!Regex.IsMatch(ext, @"^(.jpg|.jpeg)$"))
            {
                ModelState.AddModelError(nameof(Artist.Photo), "Zdjęcie musi mieć format .jpg");
            }

            if (file.Length > 2097152)
            {
                ModelState.AddModelError(nameof(Artist.Photo), "Zdjęcie musi mieć wielkość mniejszą niż 2 MB");
            }
        }
    }
}
