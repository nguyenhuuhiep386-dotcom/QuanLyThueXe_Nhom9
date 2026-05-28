using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("THONG_BAO")]
    public class ThongBao
    {
        [Key]
        public int MaThongBao { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        [Display(Name = "Loại thông báo")]
        public string LoaiThongBao { get; set; } = string.Empty;

        public int? MaNguoiNhan { get; set; }

        [Display(Name = "Đã đọc")]
        public bool IsDoc { get; set; } = false;

        public int? MaHopDongLienQuan { get; set; }

        public int? MaXeLienQuan { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayHetHan { get; set; }

        // Navigation properties
        [ForeignKey("MaNguoiNhan")]
        public virtual NguoiDung? NguoiNhan { get; set; }

        [ForeignKey("MaHopDongLienQuan")]
        public virtual HopDong? HopDongLienQuan { get; set; }

        [ForeignKey("MaXeLienQuan")]
        public virtual Xe? XeLienQuan { get; set; }
    }
}
