using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class FishingManagerContext : DbContext
    {
        public FishingManagerContext(DbContextOptions<FishingManagerContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FishingClub> FishingClubs { get; set; }
        public DbSet<Licence> Licences { get; set; }
        public DbSet<DataBaseLog> DataBaseLogs { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Right> Rights { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<Infringement> Infringements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Licence>()
                .HasOne(c => c.User)
                .WithMany(u => u.Licences);

            modelBuilder.Entity<Infringement>()
                .HasOne(i => i.Creator)
                .WithMany()
                .HasForeignKey(i => i.CreatorId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Infringement>()
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Licence>()
                .HasOne(l => l.Creator)
                .WithMany()
                .HasForeignKey(l => l.CreatorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Statistic>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Statistic>()
                .HasOne(s => s.Licence)
                .WithMany()
                .HasForeignKey(s => s.LicenceId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}