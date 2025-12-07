using MedicalAssistant.Application.DTOs;
using MedicalAssistant.Application.Interfaces;
using MedicalAssistant.Domain.Constants;
using MedicalAssistant.Domain.Entities;
using MedicalAssistant.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IRepository<Conversation> _conversationRepo;
        private readonly IRepository<Message> _messageRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAiService _aiService;

        public ChatService(
            IRepository<Conversation> conversationRepo,
            IRepository<Message> messageRepo,
            ICurrentUserService currentUserService,
            IAiService aiService)
        {
            _conversationRepo = conversationRepo;
            _messageRepo = messageRepo;
            _currentUserService = currentUserService;
            _aiService = aiService;
        }

        public async Task<List<ConversationDto>> GetConversationsAsync()
        {
            var userId = Guid.Parse(_currentUserService.IdUser);

            var query = _conversationRepo.GetQueryable(
                x => x.IdUser == userId && x.Status == (int)CommonStatus.Active
            );

            var conversations = await query
                                .OrderByDescending(c => c.LastMessageAt)
                                .ToListAsync();

            return conversations.Select(x => new ConversationDto
            {
                IdConversation = x.IdConversation,
                Title = x.Title,
                LastMessageAt = x.LastMessageAt
            }).ToList();
        }

        public async Task<List<MessageDto>> GetMessagesByConversationIdAsync(Guid idConversation)
        {
            var userId = Guid.Parse(_currentUserService.IdUser);

            // Kiểm tra quyền sở hữu conversation
            var conversation = await _conversationRepo.FirstOrDefaultAsync(x => x.IdConversation == idConversation
                                                                                && x.IdUser == userId
                                                                                && x.Status == (int)CommonStatus.Active);
            if (conversation == null)
                throw new Exception("Conversation not found.");

            // Dùng GetQueryable để Sort tin nhắn cũ lên trước
            var query = _messageRepo.GetQueryable(
                x => x.IdConversation == idConversation && x.Status == (int)CommonStatus.Active
            );

            // Sort và Execute
            var messages = await query
                                .OrderBy(m => m.CreatedDate)
                                .ToListAsync();

            return messages.Select(x => new MessageDto
            {
                IdMessage = x.IdMessage,
                IdConversation = (Guid)x.IdConversation,
                Content = x.MessageContent,
                IsAiResponse = (int)x.IsAiResponse,
                CreatedDate = (DateTime)x.CreatedDate
            }).ToList();
        }

        public async Task<MessageDto> SendMessageAsync(SendMessageRequest request)
        {
            var userId = Guid.Parse(_currentUserService.IdUser);
            if (request.MessageContent == null)
            {
                throw new Exception("Message content cannot be empty.");
            }
            Conversation conversation;

            // Xử lý conservation
            if (request.IdConversation == null || request.IdConversation == Guid.Empty)
            {
                conversation = new Conversation
                {
                    IdConversation = Guid.NewGuid(),
                    IdUser = userId,
                    Title = request.MessageContent.Length > 30
                            ? request.MessageContent.Substring(0, 30) + "..."
                            : request.MessageContent,
                    LastMessageAt = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = userId.ToString(),
                    Status = (int)CommonStatus.Active
                };
                await _conversationRepo.AddAsync(conversation);
            }
            else
            {
                conversation = await _conversationRepo.FirstOrDefaultAsync(x => x.IdConversation == request.IdConversation
                                                                                && x.IdUser == userId
                                                                                && x.Status == (int)CommonStatus.Active);
                if (conversation == null)
                    throw new Exception("Conversation not found.");

                conversation.LastMessageAt = DateTime.UtcNow;
                conversation.UpdatedDate = DateTime.UtcNow;
                conversation.UpdatedBy = userId.ToString();

                _conversationRepo.Update(conversation);
            }

            // Tạo user message
            var userMessage = new Message
            {
                IdMessage = Guid.NewGuid(),
                IdConversation = conversation.IdConversation,
                MessageContent = request.MessageContent,
                IsAiResponse = (int)IsAiResponse.False,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = userId.ToString(),
                Status = (int)CommonStatus.Active
            };
            await _messageRepo.AddAsync(userMessage);

            List<MessageDto> historyDtos = new List<MessageDto>();

            // Chỉ lấy lịch sử nếu đây là cuộc trò chuyện cũ
            if (request.IdConversation != null && request.IdConversation != Guid.Empty)
            {
                var historyQuery = _messageRepo.GetQueryable(x => x.IdConversation == conversation.IdConversation
                                                               && x.Status == (int)CommonStatus.Active);

                var historyMessages = await historyQuery
                                            .OrderByDescending(x => x.CreatedDate) // Lấy cái mới nhất trước
                                            .Take(6) // Lấy 3 cuộc đối thoại gần nhất
                                            .ToListAsync();

                // Đảo ngược lại để đúng thứ tự thời gian cũ -> mới
                historyDtos = historyMessages.OrderBy(x => x.CreatedDate)
                                             .Select(x => new MessageDto
                                             {
                                                 Content = x.MessageContent,
                                                 IsAiResponse = (int)x.IsAiResponse,
                                                 CreatedDate = (DateTime)x.CreatedDate,
                                             }).ToList();
            }

            // Gọi AI
            string aiResponseText = await _aiService.GetAnswerFromAiAsync(request.MessageContent, historyDtos);
            
            // Tạo AI message
            var aiMessage = new Message
            {
                IdMessage = Guid.NewGuid(),
                IdConversation = conversation.IdConversation,
                MessageContent = aiResponseText,
                IsAiResponse = (int)IsAiResponse.True,
                CreatedDate = DateTime.UtcNow, 
                CreatedBy = "System_AI",
                Status = (int)CommonStatus.Active
            };
            await _messageRepo.AddAsync(aiMessage);

            // Save changes
            await _messageRepo.SaveChangesAsync();

            return new MessageDto
            {
                IdMessage = aiMessage.IdMessage,
                IdConversation = (Guid)aiMessage.IdConversation,
                Content = aiMessage.MessageContent,
                IsAiResponse = (int)IsAiResponse.True,
                CreatedDate = (DateTime)aiMessage.CreatedDate
            };
        }

        public async Task<string> DeleteConversationAsync(Guid idConversation)
        {
            var userId = Guid.Parse(_currentUserService.IdUser);
            var conversation = await _conversationRepo.FirstOrDefaultAsync(x => x.IdConversation == idConversation && x.IdUser == userId && x.Status == (int)CommonStatus.Active);

            if (conversation == null)
            {
                throw new Exception("Conversation not found.");
            }

            conversation.Status = (int)CommonStatus.Inactive;
            conversation.UpdatedDate = DateTime.UtcNow;
            conversation.UpdatedBy = userId.ToString();

            _conversationRepo.Update(conversation);

            await _conversationRepo.SaveChangesAsync();

            return "Deleted conversation successfully.";
        }
    }
}
