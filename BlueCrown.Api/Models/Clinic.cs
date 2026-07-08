using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class Clinic
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<DoctorProfile> DoctorProfiles { get; set; } = new List<DoctorProfile>();
}
