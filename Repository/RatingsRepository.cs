﻿using Microsoft.EntityFrameworkCore;
using Ratings.Models;

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
            return _context.Artists.ToList();
        }

        public Artist GetArtist(int id)
        {
            return _context.Artists.Where(a => a.Id == id).SingleOrDefault();
        }

        public List<Work> GetArtistsWorks(int id)
        {
            return _context.Works.Where(w => w.ArtistId == id).ToList();
        }

        public async Task<Rating> GetRating(int workId, string userName)
        {
            return await _context.Ratings.Include(r => r.Work).Where(r => r.UserName == userName && r.WorkId == workId).SingleOrDefaultAsync();
        }
    }
}
