using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("NGUOI_DUNG")]
    public class NguoiDung
    {
        [Key]
        public int MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string MatKhauHash { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Vai trò")]
        public int MaVaiTro { get; set; } = 2; // Default: KhachHang

        public bool IsActive { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        // Navigation properties
        [ForeignKey("MaVaiTro")]
        public virtual VaiTro VaiTro { get; set; } = null!;

        public virtual KhachHang? KhachHang { get; set; }
        public virtual ICollection<HopDong> HopDongsTao { get; set; } = new List<HopDong>();
        public virtual ICollection<HopDong> HopDongsXacNhan { get; set; } = new List<HopDong>();
        public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
        public virtual ICollection<BaoDuong> BaoDuongs { get; set; } = new List<BaoDuong>();
        public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
    }
}
