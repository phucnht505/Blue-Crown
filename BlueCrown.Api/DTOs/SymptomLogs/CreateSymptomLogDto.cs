using System.ComponentModel.DataAnnotations;

namespace BlueCrown.Api.DTOs.SymptomLogs
{
    public class CreateSymptomLogDto
    {
        [Required(ErrorMessage = "Vui lòng nhập triệu chứng.")]
        [StringLength(2000, MinimumLength = 5, ErrorMessage = "Mô tả triệu chứng phải từ 5 đến 2000 ký tự.")]
        public string SymptomsDescription { get; set; } = null!;
    }
}