using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("VAI_TRO_QUYEN")]
    public class VaiTroQuyen
    {
        [Key, Column(Order = 0)]
        public int MaVaiTro { get; set; }

        [Key, Column(Order = 1)]
        public int MaQuyen { get; set; }

        // Navigation properties
        [ForeignKey("MaVaiTro")]
        public virtual VaiTro VaiTro { get; set; } = null!;

        [ForeignKey("MaQuyen")]
        public virtual Quyen Quyen { get; set; } = null!;
    }
}
