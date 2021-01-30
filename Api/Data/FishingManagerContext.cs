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

            modelBuilder.Entity<DataBaseLog>().Property(l => l.Type).HasMaxLength(50);

            modelBuilder.Entity<Address>().Property(a => a.Title).HasMaxLength(30);
            modelBuilder.Entity<Address>().Property(a => a.City).HasMaxLength(80);
            modelBuilder.Entity<Address>().Property(a => a.Phone).HasMaxLength(50);
            modelBuilder.Entity<Address>().Property(a => a.Street).HasMaxLength(150);
            modelBuilder.Entity<Address>().Property(a => a.AddressAddition).HasMaxLength(200);

            modelBuilder.Entity<FishingClub>().Property(c => c.Name).HasMaxLength(200);
            modelBuilder.Entity<FishingClub>().Property(c => c.Website).HasMaxLength(150);
            modelBuilder.Entity<FishingClub>().Property(c => c.PictureUrl).HasMaxLength(250);

            modelBuilder.Entity<Right>().Property(r => r.Name).HasMaxLength(50);

            modelBuilder.Entity<User>().Property(u => u.FirstName).HasMaxLength(80);
            modelBuilder.Entity<User>().Property(u => u.LastName).HasMaxLength(80);
            modelBuilder.Entity<User>().Property(u => u.Email).HasMaxLength(150);
            modelBuilder.Entity<User>().Property(u => u.PictureUrl).HasMaxLength(250);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
            modelBuilder.Entity<Licence>().Property(l => l.LicenseName).HasMaxLength(100);
            modelBuilder.Entity<Licence>()
                .HasOne(c => c.User)
                .WithMany(u => u.Licences);
            modelBuilder.Entity<Licence>()
                .HasOne(l => l.Creator)
                .WithMany()
                .HasForeignKey(l => l.CreatorId)
                .OnDelete(DeleteBehavior.NoAction);

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