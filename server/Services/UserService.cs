using BCI.SLAPS.Server.Model.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace BCI.SLAPS.Server.Services
{
    public interface IUserService
    {
        // CRUD
        Task<User> AddUserAsync(User user, CancellationToken ct = default);
        Task<List<User>> GetUsersAsync(bool tracking = false, CancellationToken ct = default);
        Task<User> GetUserAsync(Guid id, bool tracking = false, CancellationToken ct = default); 
        Task<User> GetUserAsync(string username, bool tracking = false, CancellationToken ct = default);
        Task<User> UpdateUserAsync(User user, CancellationToken ct = default);
        Task<User> DeleteUserAsync(User user, CancellationToken ct = default);

        // Token Generation
        Task<string> GenerateAdminTokenAsync(User user, CancellationToken ct = default);

        Task RefreshCacheAsync(CancellationToken ct = default);
        Task<bool> CheckUsernameAvailableAsync(string username, CancellationToken ct = default);
    }

    public class UserService : IUserService
    {
        private const string CACHE_KEY_USERS = "_userService:Users";

        private readonly IMemoryCache _cache;
        private readonly DataContext _db;
        private readonly ISettingsService _settings;

        public UserService(IMemoryCache cache, DataContext db, ISettingsService settings)
        {
            _cache = cache;
            _db = db;
            _settings = settings;
        }

        #region CRUD
        public async Task<User> AddUserAsync(User user, CancellationToken ct = default)
        {
            var entEntry = _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
            await PopulateUsersCacheAsync(ct);
            return entEntry.Entity;
        }

        public async Task<List<User>> GetUsersAsync(bool tracking = false, CancellationToken ct = default)
        {
            if (tracking)
                return await _db.Users.AsTracking()
                                      .ToListAsync(ct);
            else
            {
                IEnumerable<User> users;
                if (!_cache.TryGetValue(CACHE_KEY_USERS, out users))
                    users = await PopulateUsersCacheAsync(ct);
                return users.ToList();
            }
        }

        public async Task<User> GetUserAsync(Guid id, bool tracking = false, CancellationToken ct = default)
        {
            if (tracking)
                return await _db.Users.AsTracking()
                                      .Where(u => !u.Historic)
                                      .FirstOrDefaultAsync(u => u.Id == id, ct);
            else
            {
                IEnumerable<User> users;
                if (!_cache.TryGetValue(CACHE_KEY_USERS, out users))
                    users = await PopulateUsersCacheAsync(ct);
                return users.Where(u => !u.Historic)
                            .FirstOrDefault(u => u.Id == id);
            }
        }

        public async Task<User> GetUserAsync(string username, bool tracking = false, CancellationToken ct = default)
        {
            if (tracking)
                return await _db.Users.AsTracking()
                                      .Where(u => !u.Historic)
                                      .FirstOrDefaultAsync(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase), ct);
            else
            {
                IEnumerable<User> users;
                if (!_cache.TryGetValue(CACHE_KEY_USERS, out users))
                    users = await PopulateUsersCacheAsync(ct);
                return users.Where(u => !u.Historic)
                            .FirstOrDefault(u => u.Username == username);
            }
        }

        public async Task<User> UpdateUserAsync(User user, CancellationToken ct = default)
        {
            var entEntry = _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
            await PopulateUsersCacheAsync(ct);
            return entEntry.Entity;
        }

        public async Task<User> DeleteUserAsync(User user, CancellationToken ct = default)
        {
            user.Historic = true;
            var entEntry = _db.Users.Update(user);
            await _db.SaveChangesAsync(ct);
            await PopulateUsersCacheAsync(ct);
            return entEntry.Entity;
        }
        #endregion

        #region Token Generation
        public async Task<string> GenerateAdminTokenAsync(User user, CancellationToken ct = default)
        {
            // Generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _settings.GetJwtIssuer(),
                Audience = _settings.GetJwtAudience(),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, Role.Admin)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_settings.GetJwtSecret()), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
        #endregion

        public async Task RefreshCacheAsync(CancellationToken ct = default)
        {
            _ = await PopulateUsersCacheAsync(ct);
        }

        public async Task<bool> CheckUsernameAvailableAsync(string username, CancellationToken ct = default)
        {
            // Update latest from cache
            await PopulateUsersCacheAsync(ct);

            IEnumerable<User> users;
            if (!_cache.TryGetValue(CACHE_KEY_USERS, out users))
                users = await PopulateUsersCacheAsync(ct);
            return !users.Any(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        }

        // Private functions
        private async Task<IEnumerable<User>> PopulateUsersCacheAsync(CancellationToken ct = default)
        {
            _cache.Remove(CACHE_KEY_USERS);
            var users = await _db.Users.AsNoTracking()
                                       .ToListAsync(ct);
            _cache.Set(CACHE_KEY_USERS, users);
            return users;
        }
    }
}
