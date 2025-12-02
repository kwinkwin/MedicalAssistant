using MedicalAssistant.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MedicalAssistant.API.Filters
{
    public class ApiResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Không cần xử lý trước khi action chạy
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Nếu kết quả là ObjectResult và chưa phải ApiResponse thì bọc lại
            if (context.Result is ObjectResult objectResult)
            {
                var response = objectResult.Value;

                // Nếu đã là ApiResponse thì bỏ qua
                if (response != null && response.GetType().Name.StartsWith("ApiResponse"))
                    return;

                var wrappedResponse = new ApiResponse<object>(
                    objectResult.StatusCode ?? 200,
                    response
                );

                context.Result = new ObjectResult(wrappedResponse)
                {
                    StatusCode = objectResult.StatusCode ?? 200
                };
            }
        }
    }
}
