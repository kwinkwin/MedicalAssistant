using System;
using System.Collections.Generic;

namespace MedicalAssistant.Domain.Entities;

public partial class Conversation
{
    public Guid IdConversation { get; set; }

    public Guid? IdUser { get; set; }

    public string? Title { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public virtual User? IdUserNavigation { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
