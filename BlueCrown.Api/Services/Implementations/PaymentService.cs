using BlueCrown.Api.DTOs.Payments;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public PaymentService(IPaymentRepository paymentRepository, IAppointmentRepository appointmentRepository)
        {
            _paymentRepository = paymentRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments.Select(MapToDto);
        }

        public async Task<PaymentDto?> GetByIdAsync(Guid id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
                return null;

            return MapToDto(payment);
        }

        public async Task<IEnumerable<PaymentDto>> GetByAppointmentIdAsync(Guid appointmentId)
        {
            var payments = await _paymentRepository.GetByAppointmentIdAsync(appointmentId);
            return payments.Select(MapToDto);
        }

        public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
        {
            if (dto.AppointmentId == Guid.Empty)
                throw new Exception("AppointmentId không hợp lệ.");

            if (dto.PatientId == Guid.Empty)
                throw new Exception("PatientId không hợp lệ.");

            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId);

            if (appointment == null)
                throw new Exception("Không tìm thấy lịch khám.");

            // BR-PAY-001: Tư vấn trực tuyến được miễn phí và không phát sinh Payment.
            if (string.Equals(appointment.Type, "online_consult", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Tư vấn trực tuyến được miễn phí và không cần thanh toán.");

            // BR-PAY-002: Payment phải thuộc đúng Patient của lịch khám.
            if (appointment.PatientId != dto.PatientId)
                throw new InvalidOperationException("Patient không thuộc lịch khám này.");

            // BR-PAY-003: Chỉ lịch khám trực tiếp mới phát sinh phí khám.
            if (!string.Equals(appointment.Type, "clinic_visit", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Hình thức khám này không hỗ trợ thanh toán.");

            var consultationFee = appointment.Doctor.ConsultationFee;

            if (!consultationFee.HasValue || consultationFee.Value <= 0)
                throw new InvalidOperationException("Bác sĩ chưa được cấu hình phí khám.");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                Amount = consultationFee.Value,
                PlatformFee = dto.PlatformFee,
                Status = "pending",
                PaymentMethod = dto.PaymentMethod,
                TransactionRef = dto.TransactionRef,
                CreatedAt = DateTime.Now
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return MapToDto(payment);
        }

        public async Task<bool> UpdateStatusAsync(Guid id, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new Exception("Trạng thái thanh toán không được để trống.");

            var payment = await _paymentRepository.GetByIdAsync(id);

            if (payment == null)
                return false;

            payment.Status = status;

            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return true;
        }

        private static PaymentDto MapToDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                AppointmentId = payment.AppointmentId,
                PatientId = payment.PatientId,
                Amount = payment.Amount,
                PlatformFee = payment.PlatformFee,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                TransactionRef = payment.TransactionRef,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}