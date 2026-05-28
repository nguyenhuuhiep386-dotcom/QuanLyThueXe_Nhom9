using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("DM_HANG_XE")]
    public class DmHangXe
    {
        [Key]
        public int MaHangXe { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên hãng")]
        public string TenHang { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Nước sản xuất")]
        public string? NuocSanXuat { get; set; }

        [StringLength(300)]
        public string? Logo { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Xe> Xes { get; set; } = new List<Xe>();
    }
}
