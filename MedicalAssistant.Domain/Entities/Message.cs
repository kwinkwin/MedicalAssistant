using System;
using System.Collections.Generic;

namespace MedicalAssistant.Domain.Entities;

public partial class Message
{
    public Guid IdMessage { get; set; }

    public Guid? IdConversation { get; set; }

    public string? MessageContent { get; set; }

    public int? IsAiResponse { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public virtual Conversation? IdConversationNavigation { get; set; }
}
