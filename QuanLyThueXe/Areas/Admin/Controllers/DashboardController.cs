using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueXe.Data;
using QuanLyThueXe.Models.ViewModels;

namespace QuanLyThueXe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly QuanLyThueXeDbContext _context;

        public DashboardController(QuanLyThueXeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TongSoXe = await _context.Xes.CountAsync(),
                SoXeConTrong = await _context.Xes.CountAsync(x => x.TrangThai == "ConTrong"),
                SoXeDangThue = await _context.Xes.CountAsync(x => x.TrangThai == "DangThue"),
                SoXeBaoDuong = await _context.Xes.CountAsync(x => x.TrangThai == "BaoDuong"),
                TongSoKhachHang = await _context.KhachHangs.CountAsync(),
                TongSoHopDong = await _context.HopDongs.CountAsync(),
                SoHopDongChoXacNhan = await _context.HopDongs.CountAsync(h => h.TrangThai == "ChoXacNhan"),
                SoHopDongDangThue = await _context.HopDongs.CountAsync(h => h.TrangThai == "DangThue")
            };

            // Doanh thu tháng hiện tại
            var thangHienTai = DateTime.Now.Month;
            var namHienTai = DateTime.Now.Year;
            viewModel.DoanhThuThang = await _context.HopDongs
                .Where(h => h.TrangThai == "DaTra" && 
                           h.NgayTraThucTe.HasValue &&
                           h.NgayTraThucTe.Value.Month == thangHienTai &&
                           h.NgayTraThucTe.Value.Year == namHienTai)
                .SumAsync(h => h.TongTien + h.PhuPhiTreHan);

            // Doanh thu năm hiện tại
            viewModel.DoanhThuNam = await _context.HopDongs
                .Where(h => h.TrangThai == "DaTra" && 
                           h.NgayTraThucTe.HasValue &&
                           h.NgayTraThucTe.Value.Year == namHienTai)
                .SumAsync(h => h.TongTien + h.PhuPhiTreHan);

            // Hợp đồng gần đây
            viewModel.HopDongGanDay = await _context.HopDongs
                .Include(h => h.Xe)
                .Include(h => h.KhachHang)
                .Include(h => h.NguoiTao)
                .OrderByDescending(h => h.NgayTao)
                .Take(10)
                .ToListAsync();

            // Thông báo mới
            viewModel.ThongBaoMoi = await _context.ThongBaos
                .Include(t => t.NguoiNhan)
                .Where(t => !t.IsDoc)
                .OrderByDescending(t => t.NgayTao)
                .Take(5)
                .ToListAsync();

            // Xe phổ biến
            viewModel.XePhoBien = await _context.Xes
                .Include(x => x.HangXe)
                .Include(x => x.PhongCach)
                .OrderByDescending(x => x.SoLanThue)
                .Take(5)
                .ToListAsync();

            return View(viewModel);
        }
    }
}
