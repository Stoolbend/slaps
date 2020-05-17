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
    public class SlideSetsController : ControllerBase
    {
        private readonly IContentService _contentSvc;
        private readonly IMapper _mapper;

        public SlideSetsController(IContentService contentSvc, IMapper mapper)
        {
            _contentSvc = contentSvc;
            _mapper = mapper;
        }

        #region Slide Set CRUD
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SlideSetDTO>>> GetSlideSets(CancellationToken ct)
        {
            var obj = await _contentSvc.GetSlideSetsAsync(ct: ct);
            if (User.IsInRole(Role.Admin))
                return Ok(_mapper.Map<IEnumerable<SlideSetAdminDTO>>(obj));
            else
                return Ok(_mapper.Map<IEnumerable<SlideSetDTO>>(obj));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<SlideSetDTO>> GetSlideSet(Guid id, CancellationToken ct)
        {
            var obj = await _contentSvc.GetSlideSetAsync(id, ct: ct);
            if (obj == null) return NotFound();
            if (User.IsInRole(Role.Admin))
                return Ok(_mapper.Map<SlideSetAdminDTO>(obj));
            else
                return Ok(_mapper.Map<SlideSetDTO>(obj));
        }

        [HttpPost]
        public async Task<ActionResult<SlideSetAdminDTO>> CreateSlideSet([FromBody] SlideSetAdminCreateDTO dto, CancellationToken ct)
        {
            if (dto == null)
                return BadRequest();

            var obj = _mapper.Map<SlideSet>(dto);
            obj.Id = Guid.NewGuid();
            obj = await _contentSvc.AddSlideSetAsync(obj, ct);
            return CreatedAtAction(nameof(GetSlideSet), new { id = obj.Id }, _mapper.Map<SlideSetAdminDTO>(obj));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SlideSetAdminDTO>> UpdateSlideSet(Guid id, [FromBody] SlideSetAdminCreateDTO dto, CancellationToken ct)
        {
            var obj = await _contentSvc.GetSlideSetAsync(id, ct: ct);
            if (obj == null) return NotFound();
            obj.Title = dto.Title;
            await _contentSvc.UpdateSlideSetAsync(obj, ct);
            return Ok(_mapper.Map<SlideSetAdminDTO>(obj));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSlideSet(Guid id, CancellationToken ct)
        {
            var obj = await _contentSvc.GetSlideSetAsync(id, ct: ct);
            if (obj == null) return NotFound();

            await _contentSvc.DeleteSlideSetAsync(obj, ct);
            return NoContent();
        }
        #endregion

        #region Slide Creation & Redirects
        [HttpPost("{id}/Slides")]
        public async Task<ActionResult<SlideAdminDTO>> CreateSlide(Guid id, [FromBody] SlideAdminCreateDTO dto, CancellationToken ct)
        {
            if (dto == null)
                return BadRequest();

            var obj = _mapper.Map<Slide>(dto);
            obj.Id = Guid.NewGuid();
            obj.SlideSetId = id;

            // For lack of knowing how else to do this (with AutoMapper preferably)...
            if (obj.Order < 1)
                obj.Order = 999;
            if (obj.DisplaySeconds < 1)
                obj.DisplaySeconds = 20;
            // I hope one day I can find a better way for this :(

            obj = await _contentSvc.AddSlideAsync(obj, ct);
            return CreatedAtAction("GetSlide", "Slides", new { id = obj.Id }, _mapper.Map<SlideAdminDTO>(obj));
        }

        [HttpGet("{id}/Slides")]
        public async Task<ActionResult<IEnumerable<SlideClientDTO>>> GetSlides(Guid id, CancellationToken ct)
        {
            var obj = (await _contentSvc.GetSlideSetAsync(id, ct: ct)).Slides;
            if (User.IsInRole(Role.Admin))
                return Ok(_mapper.Map<IEnumerable<SlideAdminDTO>>(obj));
            else
                return Ok(_mapper.Map<IEnumerable<SlideClientDTO>>(obj));
        }
        #endregion
    }
}
