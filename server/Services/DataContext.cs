using BCI.SLAPS.Server.Helpers;
using BCI.SLAPS.Server.Model.Displays;
using BCI.SLAPS.Server.Model.Media;
using BCI.SLAPS.Server.Model.Settings;
using BCI.SLAPS.Server.Model.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

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

            // Set default values
            model.Entity<Slide>()
                .Property(s => s.Order)
                .ValueGeneratedNever()
                .HasDefaultValue(999);

            model.Entity<Slide>()
                .Property(s => s.DisplaySeconds)
                .ValueGeneratedNever()
                .HasDefaultValue(20);
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

            #region DateTimeOffset converter
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                // NB: Taken from https://blog.dangl.me/archive/handling-datetimeoffset-in-sqlite-with-entity-framework-core/

                // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
                // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
                // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
                // use the DateTimeOffsetToBinaryConverter
                // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
                // This only supports millisecond precision, but should be sufficient for most use cases.
                foreach (var entityType in model.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                                || p.PropertyType == typeof(DateTimeOffset?));
                    foreach (var property in properties)
                    {
                        model.Entity(entityType.Name)
                             .Property(property.Name)
                             .HasConversion(new DateTimeOffsetToBinaryConverter());
                    }
                }
            }
            #endregion
        }

        public DbSet<SettingsEntry> Settings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<SlideSet> SlideSets { get; set; }
        public DbSet<Display> Displays { get; set; }
    }
}
