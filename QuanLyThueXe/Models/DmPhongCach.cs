using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("DM_PHONG_CACH")]
    public class DmPhongCach
    {
        [Key]
        public int MaPhongCach { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Tên phong cách")]
        public string TenPhongCach { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Xe> Xes { get; set; } = new List<Xe>();
    }
}
