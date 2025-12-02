using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Application.Common
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(int statusCode, T data, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? (statusCode == 200 ? "Success" : "Error");
            Data = data;
        }
    }
}
