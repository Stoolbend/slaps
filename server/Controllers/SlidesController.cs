using AutoMapper;
using BCI.SLAPS.Server.Model.Media;
using BCI.SLAPS.Server.Model.User;
using BCI.SLAPS.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BCI.SLAPS.Server.Controllers
{
    [Authorize(Roles = Role.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class SlidesController : ControllerBase
    {
        private readonly IContentService _contentSvc;
        private readonly IMapper _mapper;

        public SlidesController(IContentService contentSvc, IMapper mapper)
        {
            _contentSvc = contentSvc;
            _mapper = mapper;
        }

        #region Slide get/update/delete
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SlideClientDTO>>> GetSlides(CancellationToken ct)
        {
            var obj = await _contentSvc.GetSlidesAsync(ct: ct);
            if (User.IsInRole(Role.Admin))
                return Ok(_mapper.Map<IEnumerable<SlideAdminDTO>>(obj));
            else
                return Ok(_mapper.Map<IEnumerable<SlideClientDTO>>(obj));
        }

        [HttpGet("{id}", Name = "GetSlide")]
        [AllowAnonymous]
        public async Task<ActionResult<SlideClientDTO>> GetSlide(Guid id, CancellationToken ct)
        {
            var obj = await _contentSvc.GetSlideAsync(id, ct: ct);
            if (obj == null) return NotFound();
            if (User.IsInRole(Role.Admin))
                return Ok(_mapper.Map<SlideAdminDTO>(obj));
            else
                return Ok(_mapper.Map<SlideClientDTO>(obj));
        }

        [HttpGet("{id}/Content", Name = "GetSlideContent")]
        [AllowAnonymous]
        public async Task<ActionResult<SlideClientContentDTO>> GetSlideContent(Guid id, CancellationToken ct)
        {
            var obj = await _contentSvc.GetSlideAsync(id, ct: ct);
            if (obj == null) return NotFound();
            return Ok(_mapper.Map<SlideClientContentDTO>(obj));
        }

        [HttpPut("{id}", Name = "UpdateSlide")]
        public async Task<ActionResult<SlideAdminDTO>> UpdateSlide(Guid id, [FromBody] SlideAdminCreateDTO dto, CancellationToken ct)
        {
            var obj = await _contentSvc.GetSlideAsync(id, ct: ct);
            if (obj == null) return NotFound();
            obj.Title = dto.Title;
            obj.Content = dto.Content;
            if (dto.Order.HasValue)
                obj.Order = dto.Order.Value;
            if (dto.DisplaySeconds.HasValue)
                obj.DisplaySeconds = dto.DisplaySeconds.Value;
            await _contentSvc.UpdateSlideAsync(obj, ct);
            return Ok(_mapper.Map<SlideAdminDTO>(obj));
        }

        [HttpDelete("{id}", Name = "DeleteSlide")]
        public async Task<ActionResult> DeleteSlide(Guid id, CancellationToken ct)
        {
            var obj = await _contentSvc.GetSlideAsync(id, ct: ct);
            if (obj == null) return NotFound();

            await _contentSvc.DeleteSlideAsync(obj, ct);
            return NoContent();
        }
        #endregion
    }
}