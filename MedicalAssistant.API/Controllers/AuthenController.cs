using MedicalAssistant.Application.DTOs;
using MedicalAssistant.Application.Interfaces;
using MedicalAssistant.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MedicalAssistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly ILogger<AuthenController> _logger;
        private readonly IConfiguration _config;
        private readonly IAuthenService _authenService;

        public AuthenController(ILogger<AuthenController> logger,
                                IConfiguration config,
                                IAuthenService authenService)
        {
            _logger = logger;
            _config = config;
            _authenService = authenService;
        }
        [HttpPost("SignUp")]
        public async Task<ActionResult<string>> SignUpAsync(SignUpUserDto dto)
        {
            try
            {
                var result = await _authenService.SignUpAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message: {ex.Message} \n InnerException: {ex.InnerException}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto request)
        {
            try
            {
                var user = await _authenService.LoginAsync(request);
                string role = await _authenService.GetRoleByIdUser(user.IdUser);
                var token = GenerateToken(user.IdUser, role);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message: {ex.Message} \n InnerException: {ex.InnerException}");
                return BadRequest(ex.Message);
            }
        }
        private string GenerateToken(Guid userId, string role)
        {
            var jwt = _config.GetSection("Jwt");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwt["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
