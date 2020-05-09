using BCI.SLAPS.Server.Helpers;
using BCI.SLAPS.Server.Model.Media;
using BCI.SLAPS.Server.Model.Settings;
using BCI.SLAPS.Server.Model.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace BCI.SLAPS.Server.Services
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration _config;

        public DataContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=" + _config["DatabasePath"]);

        protected override void OnModelCreating(ModelBuilder model)
        {
            #region Model configuration
            // Set certain columns to be *case insensitive*
            model.Entity<User>()
                .Property(u => u.Username)
                .HasColumnType("TEXT COLLATE NOCASE");
            model.Entity<SettingsEntry>()
                .Property(s => s.Key)
                .HasColumnType("TEXT COLLATE NOCASE");

            // Set custom primary keys
            #endregion

            #region Settings seeding
            #endregion

            #region Users seeding
            // Generate an admin user with a default known password.
            var adminUserSalt = PasswordUtils.GenerateSalt();
            model.Entity<User>().HasData(
                new User()
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Salt = adminUserSalt,
                    Hash = PasswordUtils.GenerateHash("letmeslaps", adminUserSalt)
                });
            #endregion
        }

        public DbSet<SettingsEntry> Settings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<SlideSet> SlideSets { get; set; }
    }
}
