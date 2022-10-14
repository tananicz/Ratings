using Microsoft.EntityFrameworkCore;
using Ratings.Models;
using System.Linq;

namespace Ratings.Repository
{
    public class RatingsRepository
    {
        private RatingsContext _context;

        public RatingsRepository(RatingsContext context)
        {
            _context = context;
        }

        public List<Artist> GetArtists()
        {
            return _context.Artists.OrderBy(a => a.Surname).ToList();
        }

        public async Task<Artist> GetArtist(int id)
        {
            return await _context.Artists.Where(a => a.Id == id).SingleOrDefaultAsync();
        }

        public async Task AddArtist(Artist artist)
        {
            await _context.Artists.AddAsync(artist);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateArtist(Artist artist)
        {
            _context.Artists.Update(artist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteArtist(Artist artist)
        {
            IEnumerable<Work> worksToRemove = _context.Works.Where(w => w.ArtistId == artist.Id);
            foreach(Work work in worksToRemove)
            {
                IEnumerable<Rating> ratings = _context.Ratings.Where(r => r.WorkId == work.Id);
                _context.Ratings.RemoveRange(ratings);
                await _context.SaveChangesAsync();
            }

            _context.Works.RemoveRange(worksToRemove);
            await _context.SaveChangesAsync();

            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
        }

        public List<Work> GetArtistsWorks(int id)
        {
            return _context.Works.Where(w => w.ArtistId == id).ToList();
        }

        public async Task<Work> GetWork(int id, bool includeArtist)
        {
            if (includeArtist)
            {
                return await _context.Works.Include(w => w.Artist).Where(w => w.Id == id).SingleOrDefaultAsync();
            }
            else
            { 
                return await _context.Works.Where(w => w.Id == id).SingleOrDefaultAsync();
            }
        }

        public async Task UpdateWork(Work work)
        {
            _context.Works.Update(work);
            await _context.SaveChangesAsync();
        }

        public async Task AddWork(Work work)
        {
            await _context.Works.AddAsync(work);
            await _context.SaveChangesAsync();
        }

        public async Task<Rating> GetRating(int workId, string userName)
        {
            return await _context.Ratings.Include(r => r.Work).ThenInclude(w => w.Artist).Where(r => r.UserName == userName && r.WorkId == workId).SingleOrDefaultAsync();
        }

        public async Task<decimal> ComputeRatingForWork(int workId)
        {
            return await _context.Ratings.Where(r => r.WorkId == workId).AverageAsync(r => r.RatingValue);
        }

        public async Task AddRating(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRating(Rating rating)
        {
            _context.Ratings.Update(rating);
            await _context.SaveChangesAsync();
        }
    }
}
