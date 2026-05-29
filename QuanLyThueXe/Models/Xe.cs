using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("XE")]
    public class Xe
    {
        [Key]
        public int MaXe { get; set; }

        [Required(ErrorMessage = "Tên xe là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Tên xe")]
        public string TenXe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Biển số là bắt buộc")]
        [StringLength(20)]
        [Display(Name = "Biển số")]
        public string BienSo { get; set; } = string.Empty;

        [Display(Name = "Hãng xe")]
        public int MaHangXe { get; set; }

        [Display(Name = "Phong cách")]
        public int MaPhongCach { get; set; }

        [Required(ErrorMessage = "Giá thuê là bắt buộc")]
        [Display(Name = "Giá thuê/ngày")]
        [Column(TypeName = "decimal(12,0)")]
        public decimal GiaThueNgay { get; set; }

        [Required(ErrorMessage = "Năm sản xuất là bắt buộc")]
        [Display(Name = "Năm sản xuất")]
        [Range(2000, 2099, ErrorMessage = "Năm sản xuất phải từ 2000 đến 2099")]
        public int NamSanXuat { get; set; }

        [Display(Name = "Dung tích (cc)")]
        public int? DungTich { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "ConTrong";

        [Display(Name = "Đánh giá TB")]
        [Column(TypeName = "decimal(3,2)")]
        public decimal DanhGiaTrungBinh { get; set; } = 0;

        [Display(Name = "Số lần thuê")]
        public int SoLanThue { get; set; } = 0;

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        // Navigation properties
        [ForeignKey("MaHangXe")]
        public virtual DmHangXe HangXe { get; set; } = null!;

        [ForeignKey("MaPhongCach")]
        public virtual DmPhongCach PhongCach { get; set; } = null!;

        public virtual ICollection<XeHinhAnh> HinhAnhs { get; set; } = new List<XeHinhAnh>();
        [NotMapped]
        public string? AnhDaiDien =>
        HinhAnhs.FirstOrDefault(a => a.IsAnhChinh)?.DuongDanAnh
        ?? HinhAnhs.FirstOrDefault()?.DuongDanAnh;
        public virtual ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
        public virtual ICollection<BaoDuong> BaoDuongs { get; set; } = new List<BaoDuong>();
        public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
    }
}
