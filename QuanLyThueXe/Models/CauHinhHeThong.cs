using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("CAU_HINH_HE_THONG")]
    public class CauHinhHeThong
    {
        [Key]
        public int MaCauHinh { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên khóa")]
        public string TenKhoa { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Giá trị")]
        public string GiaTri { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Kiểu dữ liệu")]
        public string KieuDuLieu { get; set; } = "string";

        [StringLength(200)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        public int? NguoiCapNhat { get; set; }

        // Navigation properties
        [ForeignKey("NguoiCapNhat")]
        public virtual NguoiDung? NguoiCapNhatNavigation { get; set; }
    }
}
