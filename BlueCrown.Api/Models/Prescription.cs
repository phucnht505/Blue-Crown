using System;
using System.Collections.Generic;

namespace BlueCrown.Api.Models;

public partial class Prescription
{
    public Guid Id { get; set; }

    public Guid MedicalRecordId { get; set; }

    public Guid PatientId { get; set; }

    public Guid DoctorId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual DoctorProfile Doctor { get; set; } = null!;

    public virtual ICollection<EcommerceOrder> EcommerceOrders { get; set; } = new List<EcommerceOrder>();

    public virtual MedicalRecord MedicalRecord { get; set; } = null!;

    public virtual PatientProfile Patient { get; set; } = null!;

    public virtual ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
}
