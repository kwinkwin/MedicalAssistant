using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Domain.Constants
{
    public enum CommonStatus
    {
        Inactive = 0,
        Active = 1
    }
    public static class RoleName
    {
        public const string NormalUser = "NormalUser";
    }
    public enum IsAiResponse
    {
        True = 1,
        False = 0
    }
}
