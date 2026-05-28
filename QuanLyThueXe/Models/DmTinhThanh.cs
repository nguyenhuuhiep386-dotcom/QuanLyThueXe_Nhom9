using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("DM_TINH_THANH")]
    public class DmTinhThanh
    {
        [Key]
        public int MaTinh { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên tỉnh/thành")]
        public string TenTinh { get; set; } = string.Empty;

        public int? MaVung { get; set; }

        // Navigation properties
        public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
    }
}
