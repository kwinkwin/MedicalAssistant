using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Application.Interfaces
{
    public interface IAiService
    {
        Task<string> GetAnswerFromAiAsync(string question);
    }
}
