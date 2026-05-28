using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("QUYEN")]
    public class Quyen
    {
        [Key]
        public int MaQuyen { get; set; }

        [Required]
        [StringLength(100)]
        public string TenQuyen { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string NhomQuyen { get; set; } = string.Empty;

        [StringLength(200)]
        public string? MoTa { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<VaiTroQuyen> VaiTroQuyens { get; set; } = new List<VaiTroQuyen>();
    }
}
