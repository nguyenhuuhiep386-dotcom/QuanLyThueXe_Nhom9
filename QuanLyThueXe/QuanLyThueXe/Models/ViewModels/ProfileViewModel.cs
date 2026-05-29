using System.ComponentModel.DataAnnotations;

namespace QuanLyThueXe.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Số GPLX là bắt buộc")]
        [StringLength(20)]
        [Display(Name = "Số giấy phép lái xe")]
        public string SoGPLX { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số giấy tờ là bắt buộc")]
        [StringLength(20)]
        [Display(Name = "Số CMND/CCCD")]
        public string SoGiayTo { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }
    }
}

    public class BankInfoViewModel
    {
        public string BankName { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;
        public string BankOwner { get; set; } = string.Empty;
    }

    public class ChangePasswordViewModel
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class GPLXVerificationViewModel
    {
        public string Number { get; set; } = string.Empty;
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Class { get; set; } = string.Empty;
    }
