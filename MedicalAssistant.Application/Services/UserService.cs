using MedicalAssistant.Application.DTOs;
using MedicalAssistant.Application.Interfaces;
using MedicalAssistant.Domain.Constants;
using MedicalAssistant.Domain.Entities;
using MedicalAssistant.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;

        public UserService(ILogger<UserService> logger,
                           IPasswordHasher<User> passwordHasher,
                           ICurrentUserService currentUserService,
                           IRepository<User> userRepository,
                           IRepository<Role> roleRepository)
        {
            _logger = logger;
            _passwordHasher = passwordHasher;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync(x => x.Status == (int)CommonStatus.Active,
                                                         x => x.Include(u => u.IdRoleNavigation));
            var result = users.Select(x => new UserDto
            {
                IdUser = x.IdUser,
                IdRole = x.IdRole,
                RoleName = x.IdRoleNavigation?.Name,
                FullName = x.FullName,
                Username = x.Username,
            }).ToList();

            return result;
        }

        public async Task<UserDto> GetCurrentUserInfoAsync()
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            Guid idUser = Guid.Parse(_currentUserService.IdUser);
            var currentUser = await _userRepository.FirstOrDefaultAsync(
                                    x => x.IdUser == idUser && x.Status == (int)CommonStatus.Active,
                                    x => x.Include(u => u.IdRoleNavigation));

            if (currentUser == null)
            {
                throw new Exception("Not found current user info.");
            }

            var result = new UserDto();
            result.IdUser = idUser;
            result.IdRole = currentUser.IdRole;
            result.RoleName = currentUser.IdRoleNavigation?.Name;
            result.FullName = currentUser.FullName;
            result.Username = currentUser.Username;

            return result;
        }
    }
}
