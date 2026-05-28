using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("BAO_DUONG")]
    public class BaoDuong
    {
        [Key]
        public int MaBaoDuong { get; set; }

        public int MaXe { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Loại bảo dưỡng")]
        public string LoaiBaoDuong { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Ngày vào")]
        public DateTime NgayVao { get; set; }

        [Display(Name = "Ngày ra")]
        public DateTime? NgayRa { get; set; }

        [Column(TypeName = "decimal(12,0)")]
        [Display(Name = "Chi phí")]
        public decimal? ChiPhi { get; set; }

        [StringLength(100)]
        [Display(Name = "Đơn vị thực hiện")]
        public string? DonViThucHien { get; set; }

        public int? MaNguoiPhuTrach { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "DangSua";

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MaXe")]
        public virtual Xe Xe { get; set; } = null!;

        [ForeignKey("MaNguoiPhuTrach")]
        public virtual NguoiDung? NguoiPhuTrach { get; set; }
    }
}
