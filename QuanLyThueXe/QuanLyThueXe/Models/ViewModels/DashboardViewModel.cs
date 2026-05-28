namespace QuanLyThueXe.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TongSoXe { get; set; }
        public int SoXeConTrong { get; set; }
        public int SoXeDangThue { get; set; }
        public int SoXeBaoDuong { get; set; }
        public int TongSoKhachHang { get; set; }
        public int TongSoHopDong { get; set; }
        public int SoHopDongChoXacNhan { get; set; }
        public int SoHopDongDangThue { get; set; }
        public decimal DoanhThuThang { get; set; }
        public decimal DoanhThuNam { get; set; }
        public List<HopDong> HopDongGanDay { get; set; } = new List<HopDong>();
        public List<ThongBao> ThongBaoMoi { get; set; } = new List<ThongBao>();
        public List<Xe> XePhoBien { get; set; } = new List<Xe>();
    }
}
