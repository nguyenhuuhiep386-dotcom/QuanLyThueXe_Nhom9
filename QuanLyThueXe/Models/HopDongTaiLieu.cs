using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("HOP_DONG_TAI_LIEU")]
    public class HopDongTaiLieu
    {
        [Key]
        public int MaTaiLieu { get; set; }

        public int MaHopDong { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên tài liệu")]
        public string TenTaiLieu { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Đường dẫn")]
        public string DuongDan { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Loại file")]
        public string? LoaiFile { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MaHopDong")]
        public virtual HopDong HopDong { get; set; } = null!;
    }
}
