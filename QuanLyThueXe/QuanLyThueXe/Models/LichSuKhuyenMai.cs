using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("LICH_SU_KHUYEN_MAI")]
    public class LichSuKhuyenMai
    {
        [Key]
        public int MaLichSu { get; set; }

        public int MaKhuyenMai { get; set; }

        public int MaHopDong { get; set; }

        [Column(TypeName = "decimal(15,0)")]
        [Display(Name = "Số tiền được giảm")]
        public decimal SoTienDuocGiam { get; set; }

        [Display(Name = "Thời gian áp dụng")]
        public DateTime ThoiGianApDung { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MaKhuyenMai")]
        public virtual KhuyenMai KhuyenMai { get; set; } = null!;

        [ForeignKey("MaHopDong")]
        public virtual HopDong HopDong { get; set; } = null!;
    }
}
