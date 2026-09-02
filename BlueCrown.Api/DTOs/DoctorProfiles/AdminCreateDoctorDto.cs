using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.DoctorProfiles
{
    public class AdminCreateDoctorDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 50 ký tự.")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Họ tên chỉ được chứa chữ cái và khoảng trắng.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Mật khẩu phải có chữ hoa, chữ thường và chữ số.")]
        public string Password { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        [RegularExpression(@"^(male|female|other)$", ErrorMessage = "Giới tính không hợp lệ.")]
        public string? Gender { get; set; }

        public string? AvatarUrl { get; set; }

        [Required(ErrorMessage = "Chuyên khoa không được để trống.")]
        [StringLength(100, ErrorMessage = "Chuyên khoa không được vượt quá 100 ký tự.")]
        public string Specialty { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số giấy phép hành nghề không được để trống.")]
        [StringLength(100, ErrorMessage = "Số giấy phép không được vượt quá 100 ký tự.")]
        public string LicenseNumber { get; set; } = string.Empty;

        public bool LicenseVerified { get; set; }

        [StringLength(2000, ErrorMessage = "Giới thiệu không được vượt quá 2000 ký tự.")]
        public string? Bio { get; set; }

        [Range(0, 80, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 80.")]
        public int? YearsExperience { get; set; }

        public Guid? ClinicId { get; set; }

        [Range(typeof(decimal), "0", "10000000", ErrorMessage = "Phí khám không hợp lệ.")]
        public decimal? ConsultationFee { get; set; }

        [Required]
        [RegularExpression(@"^(active|suspended|pending)$", ErrorMessage = "Trạng thái không hợp lệ.")]
        public string Status { get; set; } = "active";
    }
}