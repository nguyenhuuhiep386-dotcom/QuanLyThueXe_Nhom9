using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueXe.Models
{
    [Table("HOP_DONG")]
    public class HopDong
    {
        [Key]
        public int MaHopDong { get; set; }

        [Display(Name = "Xe")]
        public int MaXe { get; set; }

        [Display(Name = "Khách hàng")]
        public int MaKhachHang { get; set; }

        [Display(Name = "Người tạo")]
        public int MaNguoiTao { get; set; }

        [Display(Name = "Người xác nhận")]
        public int? MaNguoiXacNhan { get; set; }

        [Required(ErrorMessage = "Ngày thuê là bắt buộc")]
        [Display(Name = "Ngày thuê")]
        [DataType(DataType.DateTime)]
        public DateTime NgayThue { get; set; }

        [Required(ErrorMessage = "Ngày trả là bắt buộc")]
        [Display(Name = "Ngày trả")]
        [DataType(DataType.DateTime)]
        public DateTime NgayTra { get; set; }

        [Display(Name = "Ngày trả thực tế")]
        [DataType(DataType.DateTime)]
        public DateTime? NgayTraThucTe { get; set; }

        [Display(Name = "Giá thuê gốc")]
        [Column(TypeName = "decimal(12,0)")]
        public decimal GiaThueGoc { get; set; }

        [Display(Name = "Hệ số mùa")]
        [Column(TypeName = "decimal(4,2)")]
        public decimal HeSoMua { get; set; } = 1.00m;

        [Display(Name = "Tổng tiền")]
        [Column(TypeName = "decimal(15,0)")]
        public decimal TongTien { get; set; }

        [Display(Name = "Phụ phí trễ hạn")]
        [Column(TypeName = "decimal(15,0)")]
        public decimal PhuPhiTreHan { get; set; } = 0;

        [Display(Name = "Khuyến mãi")]
        public int? MaKhuyenMai { get; set; }

        [Display(Name = "Số tiền giảm")]
        [Column(TypeName = "decimal(15,0)")]
        public decimal SoTienGiam { get; set; } = 0;

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "ChoXacNhan";

        [StringLength(300)]
        [Display(Name = "Lý do hủy")]
        public string? LyDoHuy { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        // Navigation properties
        [ForeignKey("MaXe")]
        public virtual Xe Xe { get; set; } = null!;

        [ForeignKey("MaKhachHang")]
        public virtual KhachHang KhachHang { get; set; } = null!;

        [ForeignKey("MaNguoiTao")]
        public virtual NguoiDung NguoiTao { get; set; } = null!;

        [ForeignKey("MaNguoiXacNhan")]
        public virtual NguoiDung? NguoiXacNhan { get; set; }

        [ForeignKey("MaKhuyenMai")]
        public virtual KhuyenMai? KhuyenMai { get; set; }

        public virtual DanhGia? DanhGia { get; set; }
        public virtual ICollection<HopDongTaiLieu> TaiLieus { get; set; } = new List<HopDongTaiLieu>();
        public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
        public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
    }
}