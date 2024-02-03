using Microsoft.EntityFrameworkCore;
using SecretSantaApi.Models;

namespace SecretSantaApi.Data
{
    public class SecretSantaApiDbContext : DbContext
    {
        public SecretSantaApiDbContext(DbContextOptions<SecretSantaApiDbContext> options) : base(options)
        {
        }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GiftList> Lists { get; set; }
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserEmail)
                .IsUnique();
        }
    }
}

   
