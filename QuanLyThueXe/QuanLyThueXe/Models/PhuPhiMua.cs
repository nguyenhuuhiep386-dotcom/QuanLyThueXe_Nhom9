using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("PHU_PHI_MUA")]
    public class PhuPhiMua
    {
        [Key]
        public int MaPhuPhi { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên đợt")]
        public string TenDot { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Từ ngày")]
        public DateTime TuNgay { get; set; }

        [Required]
        [Display(Name = "Đến ngày")]
        public DateTime DenNgay { get; set; }

        [Required]
        [Column(TypeName = "decimal(4,2)")]
        [Display(Name = "Hệ số nhân")]
        [Range(1.00, 5.00, ErrorMessage = "Hệ số phải từ 1.00 đến 5.00")]
        public decimal HeSoNhan { get; set; } = 1.00m;

        [StringLength(300)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public int? MaNguoiTao { get; set; }

        // Navigation properties
        [ForeignKey("MaNguoiTao")]
        public virtual NguoiDung? NguoiTao { get; set; }
    }
}
