using MedicalAssistant.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ILogger<CurrentUserService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(ILogger<CurrentUserService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public string? IdUser =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? RoleName =>
            _httpContextAccessor.HttpContext?.User?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).FirstOrDefault();
    }
}
