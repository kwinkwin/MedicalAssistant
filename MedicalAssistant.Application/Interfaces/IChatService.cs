using MedicalAssistant.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Application.Interfaces
{
    public interface IChatService
    {
        // Lấy danh sách hội thoại của User hiện tại
        Task<List<ConversationDto>> GetConversationsAsync();

        // Lấy chi tiết tin nhắn của 1 hội thoại
        Task<List<MessageDto>> GetMessagesByConversationIdAsync(Guid idConversation);

        // Gửi tin nhắn
        Task<MessageDto> SendMessageAsync(SendMessageRequest request);

        // Xóa hội thoại
        Task<string> DeleteConversationAsync(Guid idConversation);
    }
}
