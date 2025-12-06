using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace MedicalAssistant.Domain.Entities;

public partial class User
{
    public Guid IdUser { get; set; }

    public Guid? IdRole { get; set; }

    public string? FullName { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }
    
    public string? Email { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual Role? IdRoleNavigation { get; set; }
}
