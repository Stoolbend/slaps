using BCI.SLAPS.Server.Model.Displays;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BCI.SLAPS.Server.Services
{
    public interface IDisplayService
    {
        #region Displays
        Task<Display> AddDisplayAsync(Display display, CancellationToken ct = default);
        Task<List<Display>> GetDisplaysAsync(bool tracking = false, CancellationToken ct = default);
        Task<Display> GetDisplayAsync(Guid id, bool tracking = false, CancellationToken ct = default);
        Task<Display> UpdateDisplayAsync(Display display, CancellationToken ct = default);
        Task<Display> DeleteDisplayAsync(Display display, CancellationToken ct = default);
        #endregion
    }

    public class DisplayService : IDisplayService
    {
        private const string CACHE_KEY_DISPLAYS = "_displayService:Displays";

        private readonly IMemoryCache _cache;
        private readonly DataContext _db;

        public DisplayService(IMemoryCache cache, DataContext db)
        {
            _cache = cache;
            _db = db;
        }

        #region Displays
        public async Task<Display> AddDisplayAsync(Display display, CancellationToken ct = default)
        {
            var entEntry = _db.Displays.Add(display);
            await _db.SaveChangesAsync(ct);
            await RefreshCacheAsync(ct);
            return entEntry.Entity;
        }

        public async Task<List<Display>> GetDisplaysAsync(bool tracking = false, CancellationToken ct = default)
        {
            if (tracking)
                return await _db.Displays.AsTracking()
                                          .Include(d => d.SlideSet)
                                          .ToListAsync(ct);
            else
            {
                IEnumerable<Display> displays;
                if (!_cache.TryGetValue(CACHE_KEY_DISPLAYS, out displays))
                    displays = await RefreshCacheAsync(ct);
                return displays.ToList();
            }
        }

        public async Task<Display> GetDisplayAsync(Guid id, bool tracking = false, CancellationToken ct = default)
        {
            if (tracking)
                return await _db.Displays.AsTracking()
                                          .Include(d => d.SlideSet)
                                          .FirstOrDefaultAsync(d => d.Id == id, ct);
            else
            {
                IEnumerable<Display> displays;
                if (!_cache.TryGetValue(CACHE_KEY_DISPLAYS, out displays))
                    displays = await RefreshCacheAsync(ct);
                return displays.FirstOrDefault(ss => ss.Id == id);
            }
        }

        public async Task<Display> UpdateDisplayAsync(Display display, CancellationToken ct = default)
        {
            var entEntry = _db.Displays.Update(display);
            await _db.SaveChangesAsync(ct);
            await RefreshCacheAsync(ct);
            return entEntry.Entity;
        }

        public async Task<Display> DeleteDisplayAsync(Display display, CancellationToken ct = default)
        {
            var entEntry = _db.Displays.Remove(display);
            await _db.SaveChangesAsync(ct);
            await RefreshCacheAsync(ct);
            return entEntry.Entity;
        }
        #endregion

        private async Task<IEnumerable<Display>> RefreshCacheAsync(CancellationToken ct = default)
        {
            _cache.Remove(CACHE_KEY_DISPLAYS);
            var data = await _db.Displays.AsNoTracking()
                                         .ToListAsync(ct);
            _cache.Set(CACHE_KEY_DISPLAYS, data);
            return data;
        }
    }
}
