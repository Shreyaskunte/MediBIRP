using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Models;
using UserService.Services;
using Shared.DTO;

namespace UserService.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IUserService _svc;
        private readonly ITokenService _tokenService;
        public AuthController(IUserService svc)
        {
            _svc = svc;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var user = await _svc.RegisterAsync(dto.Email, dto.Password);
                return Created("", new { user.Email, user.ClinicianId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _svc.AuthenticateAsync(dto.Email, dto.Password);
            if (user == null) return Unauthorized();
            var token = _tokenService.GenerateToken(user);
            return Ok(new { token });
        }
    }
}
