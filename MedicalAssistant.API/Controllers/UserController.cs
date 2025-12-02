using MedicalAssistant.Application.DTOs;
using MedicalAssistant.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAssistant.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger,
                              IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("GetCurrentUserInfo")]
        public async Task<ActionResult<UserDto>> GetCurrentUserInfoAsync()
        {
            try
            {
                var result = await _userService.GetCurrentUserInfoAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message: {ex.Message} \n InnerException: {ex.InnerException}");
                return BadRequest(ex.Message);
            }
        }
    }
}
