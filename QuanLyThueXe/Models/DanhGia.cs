using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("DANH_GIA")]
    public class DanhGia
    {
        [Key]
        public int MaDanhGia { get; set; }

        public int MaHopDong { get; set; }

        public int MaXe { get; set; }

        public int MaKhachHang { get; set; }

        [Required(ErrorMessage = "Số sao là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5")]
        [Display(Name = "Số sao")]
        public int SoSao { get; set; }

        [Display(Name = "Nhận xét")]
        public string? NhanXet { get; set; }

        [Display(Name = "Hiển thị")]
        public bool IsHienThi { get; set; } = true;

        [Display(Name = "Ngày đánh giá")]
        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MaHopDong")]
        public virtual HopDong HopDong { get; set; } = null!;

        [ForeignKey("MaXe")]
        public virtual Xe Xe { get; set; } = null!;

        [ForeignKey("MaKhachHang")]
        public virtual KhachHang KhachHang { get; set; } = null!;
    }
}
