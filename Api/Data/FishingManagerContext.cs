using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class FishingManagerContext : DbContext
    {
        public FishingManagerContext(DbContextOptions<FishingManagerContext> options) : base(options)
        {
        }
    }
}