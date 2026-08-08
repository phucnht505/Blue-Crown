using BlueCrown.Api.DTOs.Payments;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
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

        public async Task<IEnumerable<PaymentDto>> GetByAppointmentIdAsync(
            Guid appointmentId)
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

            if (dto.Amount <= 0)
                throw new Exception("Số tiền thanh toán phải lớn hơn 0.");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),

                AppointmentId = dto.AppointmentId,

                PatientId = dto.PatientId,

                Amount = dto.Amount,

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