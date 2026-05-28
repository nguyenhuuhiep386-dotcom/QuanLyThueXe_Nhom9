using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("XE_HINH_ANH")]
    public class XeHinhAnh
    {
        [Key]
        public int MaHinhAnh { get; set; }

        public int MaXe { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Đường dẫn ảnh")]
        public string DuongDanAnh { get; set; } = string.Empty;

        [Display(Name = "Ảnh chính")]
        public bool IsAnhChinh { get; set; } = false;

        [Display(Name = "Thứ tự")]
        public int ThuTu { get; set; } = 0;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("MaXe")]
        public virtual Xe Xe { get; set; } = null!;
    }
}
