using DomainDrivenDesign.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenDesign.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(user => user.Id);

                entity.Property(user => user.Email)
                    .HasMaxLength(320)
                    .IsRequired();

                entity.Property(user => user.NormalizedEmail)
                    .HasMaxLength(320)
                    .IsRequired();

                entity.Property(user => user.DisplayName)
                    .HasMaxLength(120)
                    .IsRequired();

                entity.Property(user => user.PasswordHash)
                    .HasMaxLength(512)
                    .IsRequired();

                entity.HasIndex(user => user.NormalizedEmail)
                    .IsUnique();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(role => role.Id);

                entity.Property(role => role.Name)
                    .HasMaxLength(80)
                    .IsRequired();

                entity.Property(role => role.NormalizedName)
                    .HasMaxLength(80)
                    .IsRequired();

                entity.HasIndex(role => role.NormalizedName)
                    .IsUnique();

                entity.HasData(
                    new Role { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                    new Role { Id = 2, Name = "Author", NormalizedName = "AUTHOR" },
                    new Role { Id = 3, Name = "Reader", NormalizedName = "READER" });
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(userRole => new { userRole.UserId, userRole.RoleId });

                entity.HasOne(userRole => userRole.User)
                    .WithMany(user => user.UserRoles)
                    .HasForeignKey(userRole => userRole.UserId);

                entity.HasOne(userRole => userRole.Role)
                    .WithMany(role => role.UserRoles)
                    .HasForeignKey(userRole => userRole.RoleId);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(refreshToken => refreshToken.Id);

                entity.Property(refreshToken => refreshToken.TokenHash)
                    .HasMaxLength(256)
                    .IsRequired();

                entity.HasIndex(refreshToken => refreshToken.TokenHash)
                    .IsUnique();

                entity.HasOne(refreshToken => refreshToken.User)
                    .WithMany(user => user.RefreshTokens)
                    .HasForeignKey(refreshToken => refreshToken.UserId);
            });
        }
    }
}
