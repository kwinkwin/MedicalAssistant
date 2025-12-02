using MedicalAssistant.Application.DTOs;
using MedicalAssistant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Application.Interfaces
{
    public interface IAuthenService
    {
        Task<string> SignUpAsync(SignUpUserDto dto);
        Task<User> LoginAsync(LoginDto request);
        Task<string> GetRoleByIdUser(Guid idUser);
    }
}
