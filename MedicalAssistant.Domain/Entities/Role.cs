using System;
using System.Collections.Generic;

namespace MedicalAssistant.Domain.Entities;

public partial class Role
{
    public Guid IdRole { get; set; }

    public string? Name { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
