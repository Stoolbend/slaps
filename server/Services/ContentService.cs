using BCI.SLAPS.Server.Model.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BCI.SLAPS.Server.Services
{
    public interface IContentService
    {
        #region SlideSets
        Task<SlideSet> AddSlideSetAsync(SlideSet slideSet, CancellationToken ct = default);
        Task<List<SlideSet>> GetSlideSetsAsync(bool tracking = false, CancellationToken ct = default);
        Task<SlideSet> GetSlideSetAsync(Guid id, bool tracking = false, CancellationToken ct = default);
        Task<SlideSet> UpdateSlideSetAsync(SlideSet slideSet, CancellationToken ct = default);
        Task<SlideSet> DeleteSlideSetAsync(SlideSet slideSet, CancellationToken ct = default);
        #endregion

        #region Slides
        Task<Slide> AddSlideAsync(Slide slide, CancellationToken ct = default);
        Task<List<Slide>> GetSlidesAsync(bool tracking = false, CancellationToken ct = default);
        Task<Slide> GetSlideAsync(Guid id, bool tracking = false, CancellationToken ct = default);
        Task<Slide> UpdateSlideAsync(Slide slide, CancellationToken ct = default);
        Task<Slide> DeleteSlideAsync(Slide slide, CancellationToken ct = default);
        #endregion
    }

    public class ContentService : IContentService
    {
        private const string CACHE_KEY_SLIDES = "_contentService:Slides";
        private const string CACHE_KEY_SLIDESETS = "_contentService:SlideSets";

        private readonly IMemoryCache _cache;
        private readonly DataContext _db;

        public ContentService(IMemoryCache cache, DataContext db)
        {
            _cache = cache;
            _db = db;
        }

        #region SlideSets
        public async Task<SlideSet> AddSlideSetAsync(SlideSet slideSet, CancellationToken ct = default)
        {
            var entEntry = _db.SlideSets.Add(slideSet);
            await _db.SaveChangesAsync(ct);
            await RefreshSlideSetCacheAsync(ct);
            return entEntry.Entity;
        }

        public async Task<List<SlideSet>> GetSlideSetsAsync(bool tracking = false, CancellationToken ct = default)
        {
            if (tracking)
                return await _db.SlideSets.AsTracking()
                                          .Include(ss => ss.Slides)
                                          .ToListAsync(ct);
            else
            {
                IEnumerable<SlideSet> slideSets;
                if (!_cache.TryGetValue(CACHE_KEY_SLIDESETS, out slideSets))
                    slideSets = await RefreshSlideSetCacheAsync(ct);
                return slideSets.ToList();
            }
        }

        public async Task<SlideSet> GetSlideSetAsync(Guid id, bool tracking = false, CancellationToken ct = default)
        {
            if (tracking)
                return await _db.SlideSets.AsTracking()
                                          .Include(ss => ss.Slides)
                                          .FirstOrDefaultAsync(ss => ss.Id == id, ct);
            else
            {
                IEnumerable<SlideSet> slideSets;
                if (!_cache.TryGetValue(CACHE_KEY_SLIDESETS, out slideSets))
                    slideSets = await RefreshSlideSetCacheAsync(ct);
                return slideSets.FirstOrDefault(ss => ss.Id == id);
            }
        }

        public async Task<SlideSet> UpdateSlideSetAsync(SlideSet slideSet, CancellationToken ct = default)
        {
            var entEntry = _db.SlideSets.Update(slideSet);
            await _db.SaveChangesAsync(ct);
            await RefreshSlideSetCacheAsync(ct);
            return entEntry.Entity;
        }

        public async Task<SlideSet> DeleteSlideSetAsync(SlideSet slideSet, CancellationToken ct = default)
        {
            var entEntry = _db.SlideSets.Remove(slideSet);
            await _db.SaveChangesAsync(ct);
            await RefreshSlideCacheAsync(ct);
            await RefreshSlideSetCacheAsync(ct);
            return entEntry.Entity;
        }
        #endregion

        #region Slides
        public async Task<Slide> AddSlideAsync(Slide slide, CancellationToken ct = default)
        {
            var entEntry = _db.Slides.Add(slide);
            await _db.SaveChangesAsync(ct);
            await RefreshSlideCacheAsync(ct);
            await RefreshSlideSetCacheAsync(ct);
            return entEntry.Entity;
        }

        public async Task<List<Slide>> GetSlidesAsync(bool tracking = false, CancellationToken ct = default)
        {
            if (tracking)
                return await _db.Slides.AsTracking()
                                       .ToListAsync(ct);
            else
            {
                IEnumerable<Slide> slides;
                if (!_cache.TryGetValue(CACHE_KEY_SLIDESETS, out slides))
                    slides = await RefreshSlideCacheAsync(ct);
                return slides.ToList();
            }
        }

        public async Task<Slide> GetSlideAsync(Guid id, bool tracking = false, CancellationToken ct = default)
        {
            if (tracking)
                return await _db.Slides.AsTracking()
                                       .FirstOrDefaultAsync(s => s.Id == id, ct);
            else
            {
                IEnumerable<Slide> slides;
                if (!_cache.TryGetValue(CACHE_KEY_SLIDESETS, out slides))
                    slides = await RefreshSlideCacheAsync(ct);
                return slides.FirstOrDefault(s => s.Id == id);
            }
        }

        public async Task<Slide> UpdateSlideAsync(Slide slide, CancellationToken ct = default)
        {
            var entEntry = _db.Slides.Update(slide);
            await _db.SaveChangesAsync(ct);
            await RefreshSlideCacheAsync(ct);
            await RefreshSlideSetCacheAsync(ct);
            return entEntry.Entity;
        }

        public async Task<Slide> DeleteSlideAsync(Slide slide, CancellationToken ct = default)
        {
            var entEntry = _db.Slides.Remove(slide);
            await _db.SaveChangesAsync(ct);
            await RefreshSlideCacheAsync(ct);
            await RefreshSlideSetCacheAsync(ct);
            return entEntry.Entity;
        }
        #endregion

        private async Task<IEnumerable<Slide>> RefreshSlideCacheAsync(CancellationToken ct = default)
        {
            _cache.Remove(CACHE_KEY_SLIDES);
            var slides = await _db.Slides.ToListAsync(ct);
            _cache.Set(CACHE_KEY_SLIDES, slides);
            return slides;
        }

        private async Task<IEnumerable<SlideSet>> RefreshSlideSetCacheAsync(CancellationToken ct = default)
        {
            _cache.Remove(CACHE_KEY_SLIDESETS);
            var slideSets = await _db.SlideSets.Include(ss => ss.Slides)
                                               .ToListAsync(ct);
            _cache.Set(CACHE_KEY_SLIDESETS, slideSets);
            return slideSets;
        }
    }
}
