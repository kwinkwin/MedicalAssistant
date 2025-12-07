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
    public class AuthenService : IAuthenService
    {
        private readonly ILogger<AuthenService> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;

        public AuthenService(ILogger<AuthenService> logger,
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
        public async Task<string> SignUpAsync(SignUpUserDto dto)
        {
            if (dto.FullName == null || dto.Email == null || dto.Username == null || dto.Password == null)
            {
                throw new Exception("Please fill all information.");
            }

            var isExistUsername = await _userRepository.AnyAsync(x => x.Username == dto.Username);
            if (isExistUsername)
            {
                throw new Exception("Username has been already used. ");
            }

            var isExistEmail = await _userRepository.AnyAsync(x => x.Email == dto.Email);
            if (isExistEmail)
            {
                throw new Exception("Email has been already used. ");
            }

            var role = await _roleRepository.FirstOrDefaultAsync(x => x.Name == RoleName.NormalUser && x.Status == (int)CommonStatus.Active);
            if (role == null)
            {
                throw new Exception("Role is invalid.");
            }

            User user = new User();
            var idUser = Guid.NewGuid();
            user.IdUser = idUser;
            user.IdRole = role.IdRole;
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Username = dto.Username;
            user.Password = _passwordHasher.HashPassword(user, dto.Password);
            user.Status = (int)CommonStatus.Active;
            user.CreatedDate = DateTime.UtcNow;
            user.CreatedBy = idUser.ToString();

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return "Sign up successfully.";
        }

        public async Task<User> LoginAsync(LoginDto request)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Username == request.Username && x.Status == (int)CommonStatus.Active);
            if (user == null)
            {
                throw new Exception("User was not found.");
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                throw new Exception("Password is incorrect.");
            }

            var utcNow = DateTime.UtcNow;

            // update user
            user.UpdatedDate = utcNow;
            user.UpdatedBy = user.IdUser.ToString();

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }
        public async Task<string> GetRoleByIdUser(Guid idUser)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.IdUser == idUser && x.Status == (int)CommonStatus.Active,
                                                                 x => x.Include(u => u.IdRoleNavigation));
            if (user == null || user.IdRoleNavigation == null)
            {
                throw new Exception("User was not found.");
            }

            return user.IdRoleNavigation.Name;
        }
    }
}
