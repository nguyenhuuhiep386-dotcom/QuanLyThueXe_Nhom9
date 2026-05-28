using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("THANH_TOAN")]
    public class ThanhToan
    {
        [Key]
        public int MaThanhToan { get; set; }

        public int MaHopDong { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,0)")]
        [Display(Name = "Số tiền")]
        public decimal SoTien { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Phương thức")]
        public string PhuongThuc { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Loại thanh toán")]
        public string LoaiThanhToan { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "ThanhCong";

        public int? MaNguoiNhanTien { get; set; }

        [StringLength(300)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Thời gian")]
        public DateTime ThoiGian { get; set; } = DateTime.Now;

        [StringLength(100)]
        [Display(Name = "Mã giao dịch")]
        public string? MaGiaoDich { get; set; }

        // Navigation properties
        [ForeignKey("MaHopDong")]
        public virtual HopDong HopDong { get; set; } = null!;

        [ForeignKey("MaNguoiNhanTien")]
        public virtual NguoiDung? NguoiNhanTien { get; set; }
    }
}
