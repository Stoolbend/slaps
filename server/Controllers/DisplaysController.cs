using AutoMapper;
using BCI.SLAPS.Server.Model.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
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
        private readonly IMapper _mapper;

        public DisplaysController(IMapper mapper)
        {
            _mapper = mapper;
        }

        #region Registration
        [HttpGet("Register"), AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> Register(CancellationToken ct)
        {
            return Ok(new List<object>());
        }
        #endregion

        #region Slide Sets & Slides
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
