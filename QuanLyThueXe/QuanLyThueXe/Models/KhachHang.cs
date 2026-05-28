using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("KHACH_HANG")]
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Số GPLX là bắt buộc")]
        [StringLength(20)]
        [Display(Name = "Số giấy phép lái xe")]
        public string SoGPLX { get; set; } = string.Empty;

        [Display(Name = "Loại giấy tờ")]
        public int MaLoaiGiayTo { get; set; }

        [Required(ErrorMessage = "Số giấy tờ là bắt buộc")]
        [StringLength(20)]
        [Display(Name = "Số giấy tờ")]
        public string SoGiayTo { get; set; } = string.Empty;

        [Display(Name = "Ngày cấp")]
        [DataType(DataType.Date)]
        public DateTime? NgayCap { get; set; }

        [StringLength(100)]
        [Display(Name = "Nơi cấp")]
        public string? NoiCap { get; set; }

        [StringLength(255)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Tỉnh/Thành")]
        public int? MaTinh { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [StringLength(5)]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        public int? MaNguoiDung { get; set; }

        // Navigation properties
        [ForeignKey("MaLoaiGiayTo")]
        public virtual DmLoaiGiayTo LoaiGiayTo { get; set; } = null!;

        [ForeignKey("MaTinh")]
        public virtual DmTinhThanh? TinhThanh { get; set; }

        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung? NguoiDung { get; set; }

        public virtual ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}
