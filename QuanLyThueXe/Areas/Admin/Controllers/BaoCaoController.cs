using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueXe.Data;

namespace QuanLyThueXe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BaoCaoController : Controller
    {
        private readonly QuanLyThueXeDbContext _context;

        public BaoCaoController(QuanLyThueXeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int nam = 0)
        {
            if (nam == 0) nam = DateTime.Now.Year;

            // ── 1. Doanh thu theo tháng (năm được chọn) ──────────────────────
            var doanhThuTheoThang = await _context.HopDongs
                .Where(h => h.TrangThai == "DaTra"
                         && h.NgayTraThucTe != null
                         && h.NgayTraThucTe.Value.Year == nam)
                .GroupBy(h => h.NgayTraThucTe!.Value.Month)
                .Select(g => new { Thang = g.Key, DoanhThu = g.Sum(h => h.TongTien + h.PhuPhiTreHan), SoHopDong = g.Count() })
                .OrderBy(x => x.Thang)
                .ToListAsync();

            // Fill đủ 12 tháng
            decimal[] doanhThuArr  = new decimal[12];
            int[]     soHopDongArr = new int[12];
            foreach (var d in doanhThuTheoThang)
            {
                doanhThuArr[d.Thang - 1]  = d.DoanhThu;
                soHopDongArr[d.Thang - 1] = d.SoHopDong;
            }

            // ── 2. Thống kê tổng quan ─────────────────────────────────────────
            decimal tongDoanhThuNam  = doanhThuArr.Sum();
            int     tongHopDongNam   = soHopDongArr.Sum();
            decimal tongDoanhThuThang = doanhThuArr[DateTime.Now.Month - 1];
            int     tongHopDongThang  = soHopDongArr[DateTime.Now.Month - 1];

            // Tháng trước để tính % tăng trưởng
            int     thangTruoc   = DateTime.Now.Month > 1 ? DateTime.Now.Month - 2 : 11;
            decimal dtThangTruoc = doanhThuArr[thangTruoc];
            double  tangTruong   = dtThangTruoc > 0
                ? (double)((tongDoanhThuThang - dtThangTruoc) / dtThangTruoc * 100)
                : 0;

            // ── 3. Top 5 xe doanh thu cao nhất ───────────────────────────────
            var topXe = await _context.HopDongs
                .Where(h => h.TrangThai == "DaTra" && h.NgayTraThucTe!.Value.Year == nam)
                .GroupBy(h => new { h.MaXe, h.Xe.TenXe, h.Xe.BienSo })
                .Select(g => new TopXeViewModel
                {
                    TenXe     = g.Key.TenXe,
                    BienSo    = g.Key.BienSo,
                    SoLanThue = g.Count(),
                    DoanhThu  = g.Sum(h => h.TongTien + h.PhuPhiTreHan)
                })
                .OrderByDescending(x => x.DoanhThu)
                .Take(5)
                .ToListAsync();

            // ── 4. Top 5 khách hàng chi tiêu nhiều nhất ──────────────────────
            var topKhach = await _context.HopDongs
                .Where(h => h.TrangThai == "DaTra" && h.NgayTraThucTe!.Value.Year == nam)
                .GroupBy(h => new { h.MaKhachHang, h.KhachHang.HoTen, h.KhachHang.SoDienThoai })
                .Select(g => new TopKhachViewModel
                {
                    HoTen       = g.Key.HoTen,
                    SoDienThoai = g.Key.SoDienThoai,
                    SoHopDong   = g.Count(),
                    TongChiTieu = g.Sum(h => h.TongTien + h.PhuPhiTreHan)
                })
                .OrderByDescending(x => x.TongChiTieu)
                .Take(5)
                .ToListAsync();

            // ── 5. Tỉ lệ trạng thái hợp đồng (năm được chọn) ────────────────
            var trangThaiHD = await _context.HopDongs
                .Where(h => h.NgayTao.Year == nam)
                .GroupBy(h => h.TrangThai)
                .Select(g => new TrangThaiHDViewModel
                {
                    TrangThai = g.Key,
                    SoLuong   = g.Count()
                })
                .ToListAsync();

            // ── 6. Doanh thu theo phong cách xe ──────────────────────────────
            var doanhThuPhongCach = await _context.HopDongs
                .Where(h => h.TrangThai == "DaTra" && h.NgayTraThucTe!.Value.Year == nam)
                .GroupBy(h => h.Xe.PhongCach.TenPhongCach)
                .Select(g => new DoanhThuPhongCachViewModel
                {
                    PhongCach = g.Key,
                    DoanhThu  = g.Sum(h => h.TongTien + h.PhuPhiTreHan)
                })
                .OrderByDescending(x => x.DoanhThu)
                .ToListAsync();

            // ── 7. Tổng quan nhanh ────────────────────────────────────────────
            var tongXe       = await _context.Xes.CountAsync();
            var xeConTrong   = await _context.Xes.CountAsync(x => x.TrangThai == "ConTrong");
            var xeDangThue   = await _context.Xes.CountAsync(x => x.TrangThai == "DangThue");
            var tongKhach    = await _context.KhachHangs.CountAsync();
            var hdChoXacNhan = await _context.HopDongs.CountAsync(h => h.TrangThai == "ChoXacNhan");

            // ── 8. Danh sách năm có dữ liệu ──────────────────────────────────
            var danhSachNam = await _context.HopDongs
                .Where(h => h.NgayTraThucTe != null)
                .Select(h => h.NgayTraThucTe!.Value.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            // Luôn thêm năm hiện tại vào dropdown dù chưa có data
            if (!danhSachNam.Contains(DateTime.Now.Year))
                danhSachNam.Insert(0, DateTime.Now.Year);

            // Nếu năm được chọn không có data, tự chuyển về năm gần nhất có data
            if (nam == DateTime.Now.Year && tongDoanhThuNam == 0 && danhSachNam.Count > 1)
            {
                var namCoData = danhSachNam.FirstOrDefault(y => y != DateTime.Now.Year);
                if (namCoData > 0) nam = namCoData;

                // Tính lại cho năm mới
                doanhThuTheoThang = await _context.HopDongs
                    .Where(h => h.TrangThai == "DaTra"
                             && h.NgayTraThucTe != null
                             && h.NgayTraThucTe.Value.Year == nam)
                    .GroupBy(h => h.NgayTraThucTe!.Value.Month)
                    .Select(g => new { Thang = g.Key, DoanhThu = g.Sum(h => h.TongTien + h.PhuPhiTreHan), SoHopDong = g.Count() })
                    .OrderBy(x => x.Thang)
                    .ToListAsync();

                doanhThuArr  = new decimal[12];
                soHopDongArr = new int[12];
                foreach (var d in doanhThuTheoThang)
                {
                    doanhThuArr[d.Thang - 1]  = d.DoanhThu;
                    soHopDongArr[d.Thang - 1] = d.SoHopDong;
                }

                tongDoanhThuNam  = doanhThuArr.Sum();
                tongHopDongNam   = soHopDongArr.Sum();
                tongDoanhThuThang = doanhThuArr[DateTime.Now.Month - 1];
                tongHopDongThang  = soHopDongArr[DateTime.Now.Month - 1];
                thangTruoc   = DateTime.Now.Month > 1 ? DateTime.Now.Month - 2 : 11;
                dtThangTruoc = doanhThuArr[thangTruoc];
                tangTruong   = dtThangTruoc > 0
                    ? (double)((tongDoanhThuThang - dtThangTruoc) / dtThangTruoc * 100)
                    : 0;

                topXe = await _context.HopDongs
                    .Where(h => h.TrangThai == "DaTra" && h.NgayTraThucTe!.Value.Year == nam)
                    .GroupBy(h => new { h.MaXe, h.Xe.TenXe, h.Xe.BienSo })
                    .Select(g => new TopXeViewModel { TenXe = g.Key.TenXe, BienSo = g.Key.BienSo, SoLanThue = g.Count(), DoanhThu = g.Sum(h => h.TongTien + h.PhuPhiTreHan) })
                    .OrderByDescending(x => x.DoanhThu).Take(5).ToListAsync();

                topKhach = await _context.HopDongs
                    .Where(h => h.TrangThai == "DaTra" && h.NgayTraThucTe!.Value.Year == nam)
                    .GroupBy(h => new { h.MaKhachHang, h.KhachHang.HoTen, h.KhachHang.SoDienThoai })
                    .Select(g => new TopKhachViewModel { HoTen = g.Key.HoTen, SoDienThoai = g.Key.SoDienThoai, SoHopDong = g.Count(), TongChiTieu = g.Sum(h => h.TongTien + h.PhuPhiTreHan) })
                    .OrderByDescending(x => x.TongChiTieu).Take(5).ToListAsync();

                trangThaiHD = await _context.HopDongs
                    .Where(h => h.NgayTao.Year == nam)
                    .GroupBy(h => h.TrangThai)
                    .Select(g => new TrangThaiHDViewModel { TrangThai = g.Key, SoLuong = g.Count() })
                    .ToListAsync();

                doanhThuPhongCach = await _context.HopDongs
                    .Where(h => h.TrangThai == "DaTra" && h.NgayTraThucTe!.Value.Year == nam)
                    .GroupBy(h => h.Xe.PhongCach.TenPhongCach)
                    .Select(g => new DoanhThuPhongCachViewModel { PhongCach = g.Key, DoanhThu = g.Sum(h => h.TongTien + h.PhuPhiTreHan) })
                    .OrderByDescending(x => x.DoanhThu).ToListAsync();
            }

            // ── Truyền ViewBag ────────────────────────────────────────────────
            ViewBag.Nam               = nam;
            ViewBag.DanhSachNam       = danhSachNam;

            ViewBag.DoanhThuArr       = doanhThuArr;
            ViewBag.SoHopDongArr      = soHopDongArr;

            ViewBag.TongDoanhThuNam   = tongDoanhThuNam;
            ViewBag.TongHopDongNam    = tongHopDongNam;
            ViewBag.TongDoanhThuThang = tongDoanhThuThang;
            ViewBag.TongHopDongThang  = tongHopDongThang;
            ViewBag.TangTruong        = Math.Round(tangTruong, 1);

            ViewBag.TopXe             = topXe;
            ViewBag.TopKhach          = topKhach;

            ViewBag.TrangThaiHD       = trangThaiHD;
            ViewBag.DoanhThuPhongCach = doanhThuPhongCach;

            ViewBag.TongXe            = tongXe;
            ViewBag.XeConTrong        = xeConTrong;
            ViewBag.XeDangThue        = xeDangThue;
            ViewBag.TongKhach         = tongKhach;
            ViewBag.HdChoXacNhan      = hdChoXacNhan;

            return View();
        }
    }

    // ── ViewModels nội bộ ─────────────────────────────────────────────────────
    public class TopXeViewModel
    {
        public string TenXe     { get; set; } = "";
        public string BienSo    { get; set; } = "";
        public int    SoLanThue { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class TopKhachViewModel
    {
        public string HoTen       { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public int    SoHopDong   { get; set; }
        public decimal TongChiTieu { get; set; }
    }

    public class TrangThaiHDViewModel
    {
        public string TrangThai { get; set; } = "";
        public int    SoLuong   { get; set; }
    }

    public class DoanhThuPhongCachViewModel
    {
        public string  PhongCach { get; set; } = "";
        public decimal DoanhThu  { get; set; }
    }
}
