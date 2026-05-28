using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("DM_LOAI_GIAY_TO")]
    public class DmLoaiGiayTo
    {
        [Key]
        public int MaLoai { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Tên loại giấy tờ")]
        public string TenLoai { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
    }
}
