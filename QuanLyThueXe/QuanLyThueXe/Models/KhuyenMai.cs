using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("KHUYEN_MAI")]
    public class KhuyenMai
    {
        [Key]
        public int MaKhuyenMai { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên chương trình")]
        public string TenChuongTrinh { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Mã code")]
        public string MaCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Loại giảm giá")]
        public string LoaiGiamGia { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Giá trị giảm")]
        public decimal GiaTriGiam { get; set; }

        [Column(TypeName = "decimal(15,0)")]
        [Display(Name = "Giá trị giảm tối đa")]
        public decimal? GiaTriGiamToiDa { get; set; }

        [Display(Name = "Số lần sử dụng")]
        public int SoLanSuDung { get; set; } = 1;

        [Display(Name = "Đã sử dụng")]
        public int DaSuDung { get; set; } = 0;

        [Required]
        [Display(Name = "Từ ngày")]
        public DateTime TuNgay { get; set; }

        [Required]
        [Display(Name = "Đến ngày")]
        public DateTime DenNgay { get; set; }

        [Column(TypeName = "decimal(15,0)")]
        [Display(Name = "Điều kiện tối thiểu")]
        public decimal? DieuKienToiThieu { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public int? MaNguoiTao { get; set; }

        // Navigation properties
        [ForeignKey("MaNguoiTao")]
        public virtual NguoiDung? NguoiTao { get; set; }

        public virtual ICollection<LichSuKhuyenMai> LichSuKhuyenMais { get; set; } = new List<LichSuKhuyenMai>();
    }
}
