using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Application.DTOs
{
    public class MessageDto
    {
        public Guid IdMessage { get; set; }
        public Guid IdConversation { get; set; }
        public string Content { get; set; }
        public int IsAiResponse { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class ConversationDto
    {
        public Guid IdConversation { get; set; }
        public string Title { get; set; }
        public DateTime? LastMessageAt { get; set; }
    }
    public class SendMessageRequest
    {
        public Guid? IdConversation { get; set; } // Nếu null nghĩa là tạo cuộc hội thoại mới
        public string MessageContent { get; set; }
    }
}
