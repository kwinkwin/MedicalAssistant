using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Application.DTOs
{
    public class UserDto
    {
        public Guid IdUser { get; set; }
        public Guid? IdRole { get; set; }
        public string? RoleName { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
    }
    public class SignUpUserDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
