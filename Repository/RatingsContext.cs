using Microsoft.EntityFrameworkCore;
using Ratings.Models;

namespace Ratings.Repository
{
    public class RatingsContext : DbContext
    {
        public RatingsContext(DbContextOptions<RatingsContext> opts) : base(opts) { }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<Rating> Ratings { get; set; }
    }
}
