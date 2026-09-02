using BlueCrown.Api.DTOs.Prescriptions;
using BlueCrown.Api.Models;
using BlueCrown.Api.Repositories.Interfaces;
using BlueCrown.Api.Services.Interfaces;

namespace BlueCrown.Api.Services.Implementations
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly IDoctorProfileRepository _doctorProfileRepository;
        private readonly IMedicationRepository _medicationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPrescriptionDispenseRepository _dispenseRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository, IMedicalRecordRepository medicalRecordRepository, IPatientProfileRepository patientProfileRepository, IDoctorProfileRepository doctorProfileRepository, IMedicationRepository medicationRepository, IProductRepository productRepository, IPrescriptionDispenseRepository dispenseRepository, IAppointmentRepository appointmentRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _patientProfileRepository = patientProfileRepository;
            _doctorProfileRepository = doctorProfileRepository;
            _medicationRepository = medicationRepository;
            _productRepository = productRepository;
            _dispenseRepository = dispenseRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<PrescriptionDto>> GetPatientPrescriptionsAsync(Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);
            var prescriptions = await _prescriptionRepository.GetByPatientIdAsync(patientProfile.Id);
            return prescriptions.Select(MapToDto).ToList();
        }

        public async Task<PrescriptionDto?> GetPatientPrescriptionByIdAsync(Guid id, Guid userId)
        {
            var patientProfile = await GetPatientProfileAsync(userId);
            var prescription = await _prescriptionRepository.GetByIdAsync(id);

            // BR-PRE-013: Patient chỉ được xem đơn thuốc của chính mình.
            if (prescription == null || prescription.PatientId != patientProfile.Id)
                return null;

            return MapToDto(prescription);
        }

        public async Task<List<PrescriptionDto>> GetDoctorPrescriptionsAsync(Guid userId)
        {
            var doctorProfile = await GetDoctorProfileAsync(userId);
            var prescriptions = await _prescriptionRepository.GetByDoctorIdAsync(doctorProfile.Id);
            return prescriptions.Select(MapToDto).ToList();
        }

        public async Task<PrescriptionDto?> GetDoctorPrescriptionByIdAsync(Guid id, Guid userId)
        {
            var doctorProfile = await GetDoctorProfileAsync(userId);
            var prescription = await _prescriptionRepository.GetByIdAsync(id);

            // BR-PRE-014: Doctor chỉ được xem đơn thuốc do chính mình kê.
            if (prescription == null || prescription.DoctorId != doctorProfile.Id)
                return null;

            return MapToDto(prescription);
        }

        public async Task<PrescriptionDto?> GetDoctorPrescriptionByMedicalRecordAsync(Guid medicalRecordId, Guid userId)
        {
            var doctorProfile = await GetDoctorProfileAsync(userId);
            var prescription = await _prescriptionRepository.GetByMedicalRecordIdAsync(medicalRecordId);

            if (prescription == null || prescription.DoctorId != doctorProfile.Id)
                return null;

            return MapToDto(prescription);
        }

        public async Task<PrescriptionDto> CreateAsync(Guid userId, CreatePrescriptionDto dto)
        {
            var doctorProfile = await GetDoctorProfileAsync(userId);

            // BR-PRE-007: Đơn thuốc phải có ít nhất một Medication.
            if (dto.Items == null || dto.Items.Count == 0)
                throw new ArgumentException("Đơn thuốc phải có ít nhất một loại thuốc.");

            if (dto.Items.Any(item => item.MedicationId == Guid.Empty))
                throw new ArgumentException("Thuốc trong đơn không hợp lệ.");

            var medicationIds = dto.Items.Select(item => item.MedicationId).ToList();

            // BR-PRE-009: Không được kê trùng Medication.
            if (medicationIds.Distinct().Count() != medicationIds.Count)
                throw new InvalidOperationException("Không được thêm cùng một loại thuốc nhiều lần trong đơn.");

            // BR-PRE-008: Medication phải tồn tại.
            var medications = await _medicationRepository.GetByIdsAsync(medicationIds);

            if (medications.Count != medicationIds.Count)
                throw new ArgumentException("Có thuốc trong đơn không tồn tại.");

            foreach (var item in dto.Items)
            {
                // BR-PRE-010: Liều dùng không được để trống.
                if (string.IsNullOrWhiteSpace(item.Dosage))
                    throw new ArgumentException("Vui lòng nhập liều dùng cho tất cả thuốc.");

                // BR-PRE-011: Số lần dùng mỗi ngày phải lớn hơn 0.
                if (item.FrequencyPerDay.HasValue && item.FrequencyPerDay.Value <= 0)
                    throw new ArgumentException("Số lần dùng mỗi ngày phải lớn hơn 0.");

                // BR-PRE-012: Số ngày sử dụng phải lớn hơn 0.
                if (item.DurationDays.HasValue && item.DurationDays.Value <= 0)
                    throw new ArgumentException("Số ngày sử dụng phải lớn hơn 0.");
            }

            var hasMedicalRecord = dto.MedicalRecordId.HasValue && dto.MedicalRecordId.Value != Guid.Empty;
            var hasAppointment = dto.AppointmentId.HasValue && dto.AppointmentId.Value != Guid.Empty;

            if (!hasMedicalRecord && !hasAppointment)
                throw new ArgumentException("Vui lòng chọn lịch khám hoặc hồ sơ bệnh án.");

            Appointment appointment;
            MedicalRecord? medicalRecord = null;
            string diagnosis;
            string status;

            if (hasMedicalRecord)
            {
                medicalRecord = await _medicalRecordRepository.GetByIdAsync(dto.MedicalRecordId!.Value);

                // BR-PRE-003: MedicalRecord phải tồn tại.
                if (medicalRecord == null)
                    throw new ArgumentException("Không tìm thấy hồ sơ bệnh án.");

                // BR-PRE-004: Doctor chỉ được kê đơn cho MedicalRecord của chính mình.
                if (medicalRecord.DoctorId != doctorProfile.Id)
                    throw new InvalidOperationException("Bạn không có quyền kê đơn cho hồ sơ bệnh án này.");

                if (medicalRecord.Appointment == null)
                    throw new InvalidOperationException("Hồ sơ bệnh án không có lịch khám hợp lệ.");

                appointment = medicalRecord.Appointment;

                // BR-PRE-005: MedicalRecord chỉ thuộc khám trực tiếp đã hoàn thành.
                if (!string.Equals(appointment.Type, "clinic_visit", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Hồ sơ bệnh án chỉ được sử dụng cho lịch khám trực tiếp.");

                if (!string.Equals(appointment.Status, "completed", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Chỉ hồ sơ từ lịch khám đã hoàn thành mới được kê đơn thuốc.");

                if (hasAppointment && dto.AppointmentId!.Value != appointment.Id)
                    throw new ArgumentException("Lịch khám không khớp với hồ sơ bệnh án.");

                diagnosis = medicalRecord.Diagnosis;
                status = "pending";
            }
            else
            {
                appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId!.Value)
                    ?? throw new ArgumentException("Không tìm thấy lịch tư vấn.");

                // BR-PRE-015: Doctor chỉ được kê đơn cho Appointment của chính mình.
                if (appointment.DoctorId != doctorProfile.Id)
                    throw new InvalidOperationException("Bạn không có quyền kê đơn cho lịch tư vấn này.");

                // BR-PRE-016: Kê đơn không có MedicalRecord chỉ áp dụng cho online_consult.
                if (!string.Equals(appointment.Type, "online_consult", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Khám trực tiếp phải có hồ sơ bệnh án trước khi kê đơn thuốc.");

                // BR-PRE-017: Tư vấn trực tuyến phải hoàn thành trước khi kê đơn.
                if (!string.Equals(appointment.Status, "completed", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Chỉ tư vấn trực tuyến đã hoàn thành mới được kê đơn thuốc.");

                // BR-PRE-018: Đơn thuốc online phải có chẩn đoán.
                if (string.IsNullOrWhiteSpace(dto.Diagnosis))
                    throw new ArgumentException("Vui lòng nhập chẩn đoán cho đơn thuốc tư vấn trực tuyến.");

                diagnosis = dto.Diagnosis.Trim();
                status = "issued";
            }

            // BR-PRE-006: Mỗi Appointment chỉ được có một Prescription.
            var existingPrescription = await _prescriptionRepository.GetByAppointmentIdAsync(appointment.Id);

            if (existingPrescription != null)
                throw new InvalidOperationException("Lịch khám này đã có đơn thuốc.");

            var prescription = new Prescription
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointment.Id,
                MedicalRecordId = medicalRecord?.Id,
                PatientId = appointment.PatientId,
                DoctorId = doctorProfile.Id,
                Diagnosis = diagnosis,
                Status = status,
                CreatedAt = DateTime.UtcNow
            };

            prescription.PrescriptionItems = dto.Items.Select(item => new PrescriptionItem
            {
                Id = Guid.NewGuid(),
                PrescriptionId = prescription.Id,
                MedicationId = item.MedicationId,
                Dosage = item.Dosage.Trim(),
                FrequencyPerDay = item.FrequencyPerDay,
                DurationDays = item.DurationDays,
                Instructions = string.IsNullOrWhiteSpace(item.Instructions) ? null : item.Instructions.Trim()
            }).ToList();

            await _prescriptionRepository.AddAsync(prescription);
            await _prescriptionRepository.SaveChangesAsync();

            var created = await _prescriptionRepository.GetByIdAsync(prescription.Id);

            if (created == null)
                throw new Exception("Không thể lấy đơn thuốc vừa tạo.");

            return MapToDto(created);
        }

        public async Task<List<PrescriptionDto>> GetPharmacistPrescriptionsAsync()
        {
            var prescriptions = await _prescriptionRepository.GetAllAsync();

            // BR-PHA-PRE-004: Đơn tư vấn online không tự động chuyển cho Pharmacist cấp thuốc.
            return prescriptions
                .Where(p => !string.Equals(p.Appointment.Type, "online_consult", StringComparison.OrdinalIgnoreCase))
                .Select(MapToDto)
                .ToList();
        }

        public async Task<PrescriptionDto?> GetPharmacistPrescriptionByIdAsync(Guid id)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);

            if (prescription == null)
                return null;

            return MapToDto(prescription);
        }

        public async Task<PrescriptionDto?> UpdatePharmacistStatusAsync(Guid id, UpdatePrescriptionStatusDto dto)
        {
            var prescription = await _prescriptionRepository.GetByIdForUpdateAsync(id);

            if (prescription == null)
                return null;

            // BR-PHA-PRE-004: Đơn online không được xử lý theo luồng cấp thuốc trực tiếp.
            if (string.Equals(prescription.Appointment.Type, "online_consult", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Đơn thuốc tư vấn trực tuyến không thuộc luồng cấp thuốc trực tiếp tại nhà thuốc.");

            var currentStatus = prescription.Status?.Trim().ToLowerInvariant() ?? "pending";
            var newStatus = dto.Status.Trim().ToLowerInvariant();

            // BR-PHA-PRE-001: pending chỉ được chuyển sang approved hoặc cancelled.
            if (currentStatus == "pending")
            {
                if (newStatus != "approved" && newStatus != "cancelled")
                    throw new InvalidOperationException("Đơn thuốc đang chờ chỉ có thể được duyệt hoặc hủy.");
            }
            // BR-PHA-PRE-002: approved chỉ được hủy, cấp thuốc phải qua DispenseAsync.
            else if (currentStatus == "approved")
            {
                if (newStatus != "cancelled")
                    throw new InvalidOperationException("Đơn thuốc đã duyệt chỉ có thể hủy hoặc thực hiện cấp thuốc.");
            }
            // BR-PHA-PRE-003: dispensed và cancelled là trạng thái cuối.
            else
            {
                throw new InvalidOperationException("Đơn thuốc ở trạng thái hiện tại không thể thay đổi.");
            }

            prescription.Status = newStatus;
            await _prescriptionRepository.SaveChangesAsync();

            var updated = await _prescriptionRepository.GetByIdAsync(id);

            if (updated == null)
                throw new Exception("Không thể lấy đơn thuốc sau khi cập nhật.");

            return MapToDto(updated);
        }

        public async Task<PrescriptionDto?> DispenseAsync(Guid id, Guid pharmacistUserId, DispensePrescriptionDto dto)
        {
            var prescription = await _prescriptionRepository.GetByIdForUpdateAsync(id);

            if (prescription == null)
                return null;

            // BR-DISP-009: Online consultation không được cấp thuốc trực tiếp qua Prescription.
            if (string.Equals(prescription.Appointment.Type, "online_consult", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Đơn thuốc tư vấn trực tuyến chỉ được sử dụng để bệnh nhân tự mua hoặc đặt hàng.");

            // BR-DISP-001: Chỉ approved mới được cấp thuốc.
            if (!string.Equals(prescription.Status, "approved", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Chỉ đơn thuốc đã được duyệt mới có thể cấp thuốc.");

            // BR-DISP-002: Phải có danh sách thuốc.
            if (dto.Items == null || dto.Items.Count == 0)
                throw new ArgumentException("Vui lòng nhập danh sách thuốc được cấp.");

            if (dto.Items.Count != prescription.PrescriptionItems.Count)
                throw new InvalidOperationException("Phải xử lý đầy đủ tất cả thuốc trong đơn.");

            if (dto.Items.Any(item => item.PrescriptionItemId == Guid.Empty || item.ProductId == Guid.Empty))
                throw new ArgumentException("Thông tin thuốc được cấp không hợp lệ.");

            var prescriptionItemIds = dto.Items.Select(item => item.PrescriptionItemId).ToList();

            // BR-DISP-003: Không gửi trùng PrescriptionItem.
            if (prescriptionItemIds.Distinct().Count() != prescriptionItemIds.Count)
                throw new InvalidOperationException("Không được xử lý cùng một thuốc trong đơn nhiều lần.");

            var actualItemIds = prescription.PrescriptionItems.Select(item => item.Id).ToHashSet();

            // BR-DISP-004: Item request phải thuộc Prescription hiện tại.
            if (prescriptionItemIds.Any(itemId => !actualItemIds.Contains(itemId)))
                throw new InvalidOperationException("Có thuốc không thuộc đơn thuốc đang xử lý.");

            // BR-DISP-005: Không được cấp lại đơn đã có dữ liệu cấp.
            if (prescription.PrescriptionItems.Any(item => item.PrescriptionDispenseItem != null))
                throw new InvalidOperationException("Đơn thuốc này đã có dữ liệu cấp thuốc.");

            var productIds = dto.Items.Select(item => item.ProductId).Distinct().ToList();
            var products = await _productRepository.GetByIdsForUpdateAsync(productIds);

            if (products.Count != productIds.Count)
                throw new ArgumentException("Có Product được chọn không tồn tại.");

            var dispenseItems = new List<PrescriptionDispenseItem>();
            var dispensedAt = DateTime.UtcNow;

            foreach (var requestItem in dto.Items)
            {
                if (requestItem.QuantityDispensed <= 0)
                    throw new ArgumentException("Số lượng cấp phải lớn hơn 0.");

                var prescriptionItem = prescription.PrescriptionItems.First(item => item.Id == requestItem.PrescriptionItemId);
                var product = products.First(item => item.Id == requestItem.ProductId);

                // BR-DISP-006: Product phải thuộc đúng Medication bác sĩ kê.
                if (!product.MedicationId.HasValue || product.MedicationId.Value != prescriptionItem.MedicationId)
                    throw new InvalidOperationException($"Product '{product.Name}' không thuộc Medication '{prescriptionItem.Medication.Name}'.");

                var currentStock = product.StockQuantity ?? 0;

                // BR-DISP-007: Không cấp vượt tồn kho.
                if (currentStock < requestItem.QuantityDispensed)
                    throw new InvalidOperationException($"Product '{product.Name}' không đủ tồn kho. Tồn hiện tại: {currentStock}.");

                product.StockQuantity = currentStock - requestItem.QuantityDispensed;

                dispenseItems.Add(new PrescriptionDispenseItem
                {
                    Id = Guid.NewGuid(),
                    PrescriptionItemId = prescriptionItem.Id,
                    ProductId = product.Id,
                    QuantityDispensed = requestItem.QuantityDispensed,
                    DispensedBy = pharmacistUserId,
                    DispensedAt = dispensedAt
                });
            }

            // BR-DISP-008: Chỉ sau khi tất cả item hợp lệ mới chuyển sang dispensed.
            prescription.Status = "dispensed";

            await _dispenseRepository.AddRangeAsync(dispenseItems);
            await _prescriptionRepository.SaveChangesAsync();

            var updated = await _prescriptionRepository.GetByIdAsync(id);

            if (updated == null)
                throw new Exception("Không thể lấy đơn thuốc sau khi cấp thuốc.");

            return MapToDto(updated);
        }

        private async Task<PatientProfile> GetPatientProfileAsync(Guid userId)
        {
            var patientProfile = await _patientProfileRepository.GetByUserIdAsync(userId);

            // BR-PRE-002: Patient phải có PatientProfile.
            if (patientProfile == null)
                throw new InvalidOperationException("Bạn chưa có hồ sơ sức khỏe.");

            return patientProfile;
        }

        private async Task<DoctorProfile> GetDoctorProfileAsync(Guid userId)
        {
            var doctorProfile = await _doctorProfileRepository.GetByUserIdAsync(userId);

            // BR-PRE-001: Doctor phải có DoctorProfile.
            if (doctorProfile == null)
                throw new InvalidOperationException("Bạn chưa có hồ sơ bác sĩ.");

            return doctorProfile;
        }

        private static DateTime AsUtc(DateTime value)
        {
            return value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        private static DateTime? AsUtc(DateTime? value)
        {
            if (!value.HasValue)
                return null;

            return AsUtc(value.Value);
        }

        private static PrescriptionDto MapToDto(Prescription prescription)
        {
            var diagnosis = prescription.Diagnosis ?? prescription.MedicalRecord?.Diagnosis ?? string.Empty;

            return new PrescriptionDto
            {
                Id = prescription.Id,
                AppointmentId = prescription.AppointmentId,
                MedicalRecordId = prescription.MedicalRecordId,
                MedicalRecordDiagnosis = diagnosis,
                Diagnosis = diagnosis,
                AppointmentScheduledAt = AsUtc(prescription.Appointment.ScheduledAt),
                AppointmentType = prescription.Appointment.Type,
                PatientId = prescription.PatientId,
                PatientName = prescription.Patient.User.FullName,
                DoctorId = prescription.DoctorId,
                DoctorName = prescription.Doctor.User.FullName,
                DoctorSpecialty = prescription.Doctor.Specialty,
                Status = prescription.Status,
                CreatedAt = AsUtc(prescription.CreatedAt),
                Items = prescription.PrescriptionItems.Select(MapItemToDto).ToList()
            };
        }

        private static PrescriptionItemDto MapItemToDto(PrescriptionItem item)
        {
            return new PrescriptionItemDto
            {
                Id = item.Id,
                PrescriptionId = item.PrescriptionId,
                MedicationId = item.MedicationId,
                MedicationName = item.Medication.Name,
                GenericName = item.Medication.GenericName,
                Category = item.Medication.Category,
                Dosage = item.Dosage,
                FrequencyPerDay = item.FrequencyPerDay,
                DurationDays = item.DurationDays,
                Instructions = item.Instructions,
                Dispense = MapDispenseToDto(item.PrescriptionDispenseItem)
            };
        }

        private static PrescriptionDispenseItemDto? MapDispenseToDto(PrescriptionDispenseItem? dispense)
        {
            if (dispense == null)
                return null;

            return new PrescriptionDispenseItemDto
            {
                Id = dispense.Id,
                PrescriptionItemId = dispense.PrescriptionItemId,
                ProductId = dispense.ProductId,
                ProductName = dispense.Product?.Name ?? "Không xác định",
                QuantityDispensed = dispense.QuantityDispensed,
                DispensedBy = dispense.DispensedBy,
                DispensedByName = dispense.DispensedByNavigation?.FullName,
                DispensedAt = AsUtc(dispense.DispensedAt)
            };
        }
    }
}