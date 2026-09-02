namespace BlueCrown.Api.DTOs.HealthGoals
{
    public class DoctorHealthGoalPatientDto
    {
        public Guid PatientId { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime? LastAppointmentAt { get; set; }
        public int AppointmentCount { get; set; }
    }

    public class DoctorHealthGoalMetricTypeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }
}