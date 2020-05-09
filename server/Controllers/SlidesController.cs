using AutoMapper;
using BCI.SLAPS.Server.Model.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        #region Settings
        [HttpGet("Settings")]
        public async Task<ActionResult> GetSettings(CancellationToken ct)
        {
            return NoContent();
        }
        #endregion
    }
}
