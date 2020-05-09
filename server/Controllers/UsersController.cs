using AutoMapper;
using BCI.SLAPS.Server.Helpers;
using BCI.SLAPS.Server.Model.User;
using BCI.SLAPS.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BCI.SLAPS.Server.Controllers
{
    [Authorize(Roles = Role.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userSvc;

        public UsersController(IMapper mapper, IUserService userSvc)
        {
            _mapper = mapper;
            _userSvc = userSvc;
        }

        #region CRUD
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers(CancellationToken ct)
        {
            return Ok(_mapper.Map<IEnumerable<UserDTO>>(await _userSvc.GetUsersAsync(ct: ct)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id, CancellationToken ct)
        {
            var obj = await _userSvc.GetUserAsync(id, ct: ct);
            if (obj == null) return NotFound();
            return Ok(_mapper.Map<UserDTO>(obj));
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserLoginRequestDTO dto, CancellationToken ct)
        {
            if (dto == null)
                return BadRequest();

            if (!await _userSvc.CheckUsernameAvailableAsync(dto.Username, ct))
            {
                var modelErrors = new ModelStateDictionary();
                modelErrors.AddModelError("details", "That username is already in use.");
                return Conflict(modelErrors);
            }

            if (Validators.CheckForbiddenName(dto.Username))
            {
                var modelErrors = new ModelStateDictionary();
                modelErrors.AddModelError("details", "That username is forbidden or reserved for system use.");
                return Conflict(modelErrors);
            }

            var obj = _mapper.Map<User>(dto);
            obj.Id = Guid.NewGuid();
            obj.Salt = PasswordUtils.GenerateSalt();
            obj.Hash = PasswordUtils.GenerateHash(dto.Password, obj.Salt);
            obj = await _userSvc.AddUserAsync(obj, ct);
            return CreatedAtAction(nameof(GetUser), new { id = obj.Id }, _mapper.Map<UserDTO>(obj));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> UpdateUser(Guid id, [FromBody] UserUpdateDTO dto, CancellationToken ct)
        {
            var obj = await _userSvc.GetUserAsync(id, ct: ct);
            if (obj == null) return NotFound();
            obj.Salt = PasswordUtils.GenerateSalt();
            obj.Hash = PasswordUtils.GenerateHash(dto.Password, obj.Salt);
            obj = await _userSvc.UpdateUserAsync(obj, ct);
            return Ok(_mapper.Map<UserDTO>(obj));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(Guid id, CancellationToken ct)
        {
            var obj = await _userSvc.GetUserAsync(id, ct: ct);
            if (obj == null) return NotFound();

            await _userSvc.DeleteUserAsync(obj, ct);
            return NoContent();
        }
        #endregion

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<ActionResult<UserLoginResponseDTO>> AuthenticateUser([FromBody]UserLoginRequestDTO dto, CancellationToken ct)
        {
            // Find user
            var user = await _userSvc.GetUserAsync(dto.Username, ct: ct);
            if (user == null)
            {
                var modelErrors = new ModelStateDictionary();
                modelErrors.AddModelError("details", "Bad username or password.");
                return Unauthorized(modelErrors);
            }

            // Confirm password
            if (!user.CheckPassword(dto.Password))
            {
                var modelErrors = new ModelStateDictionary();
                modelErrors.AddModelError("details", "Bad username or password.");
                return Unauthorized(modelErrors);
            }

            // Generate token, Return result
            var result = _mapper.Map<UserLoginResponseDTO>(user);
            result.Token = await _userSvc.GenerateAdminTokenAsync(user, ct);
            return Ok(result);
        }
    }
}
