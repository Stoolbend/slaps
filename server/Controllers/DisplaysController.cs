using AutoMapper;
using BCI.SLAPS.Server.Model.Displays;
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
    public class DisplaysController : ControllerBase
    {
        private readonly IContentService _content;
        private readonly IDisplayService _displays;
        private readonly IMapper _mapper;

        public DisplaysController(IContentService content, IDisplayService displays, IMapper mapper)
        {
            _content = content;
            _displays = displays;
            _mapper = mapper;
        }

        #region CRUD
        [HttpGet("Register"), AllowAnonymous]
        public async Task<ActionResult<DisplayDTO>> Register(CancellationToken ct)
        {
            var display = await _displays.AddDisplayAsync(new Display
            {
                Id = Guid.NewGuid()
            }, ct: ct);
            return Ok(_mapper.Map<DisplayDTO>(display));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisplayDTO>>> GetAll(CancellationToken ct)
        {
            var displays = await _displays.GetDisplaysAsync(ct: ct);
            if (displays == null || displays.Count < 1)
                return NoContent();
            else
                return Ok(_mapper.Map<List<DisplayDTO>>(displays));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DisplayDTO>> GetSingle(Guid id, CancellationToken ct)
        {
            var display = await _displays.GetDisplayAsync(id, ct: ct);
            if (display == null)
                return NotFound();
            else
                return Ok(_mapper.Map<DisplayDTO>(display));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> Update(Guid id, [FromBody] DisplayDTO dto, CancellationToken ct)
        {
            var obj = await _displays.GetDisplayAsync(id, ct: ct);
            if (obj == null) return NotFound();

            obj = await _displays.UpdateDisplayAsync(obj, ct);
            return Ok(_mapper.Map<UserDTO>(obj));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
        {
            var obj = await _displays.GetDisplayAsync(id, ct: ct);
            if (obj == null) return NotFound();

            await _displays.DeleteDisplayAsync(obj, ct);
            return NoContent();
        }
        #endregion

        #region Slide Sets & Slides
        [HttpGet("{id}/SlideSet"), AllowAnonymous]
        public async Task<ActionResult<SlideSetDTO>> GetSlideSet(Guid id, CancellationToken ct)
        {
            var display = await _displays.GetDisplayAsync(id, ct: ct);
            if (display == null)
                return NotFound();
            else
            {
                if (display.SlideSetId.HasValue)
                {
                    var slideSet = await _content.GetSlideSetAsync(display.SlideSetId.Value, ct: ct);
                    if (slideSet == null)
                        return NoContent();
                    else
                        return Ok(_mapper.Map<SlideSetDTO>(slideSet));
                }
                else
                    return NoContent();
            }
        }
        #endregion

        #region Settings
        [HttpGet("Settings"), AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetSettings(CancellationToken ct)
        {
            return Ok(new List<object>());
        }
        #endregion
    }
}
