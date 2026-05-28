using System.ComponentModel.DataAnnotations;

namespace QuanLyThueXe.Models.ViewModels
{
    public class DatXeViewModel
    {
        public Xe Xe { get; set; } = null!;
        
        [Required(ErrorMessage = "Ngày thuê là bắt buộc")]
        [Display(Name = "Ngày thuê")]
        [DataType(DataType.DateTime)]
        public DateTime NgayThue { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "Ngày trả là bắt buộc")]
        [Display(Name = "Ngày trả")]
        [DataType(DataType.DateTime)]
        public DateTime NgayTra { get; set; } = DateTime.Now.AddDays(4);

        [Display(Name = "Mã khuyến mãi")]
        public string? MaKhuyenMai { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // Thông tin tính toán
        public int SoNgay { get; set; }
        public decimal GiaThueGoc { get; set; }
        public decimal HeSoMua { get; set; } = 1.00m;
        public decimal TienGiam { get; set; }
        public decimal TongTien { get; set; }
        public string? TenDotPhuPhi { get; set; }
    }
}
