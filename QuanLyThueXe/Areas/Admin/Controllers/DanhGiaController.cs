using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueXe.Data;

namespace QuanLyThueXe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DanhGiaController : Controller
    {
        private readonly QuanLyThueXeDbContext _context;

        public DanhGiaController(QuanLyThueXeDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DanhGia
        public async Task<IActionResult> Index(int? soSao, string? trangThai, string? keyword, int trang = 1)
        {
            int pageSize = 10;

            var query = _context.DanhGias
                .Include(d => d.KhachHang)
                .Include(d => d.Xe)
                .Include(d => d.HopDong)
                .AsQueryable();

            // Filter
            if (soSao.HasValue)
                query = query.Where(d => d.SoSao == soSao.Value);

            if (trangThai == "hien")
                query = query.Where(d => d.IsHienThi);
            else if (trangThai == "an")
                query = query.Where(d => !d.IsHienThi);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(d =>
                    d.KhachHang.HoTen.Contains(keyword) ||
                    d.Xe.TenXe.Contains(keyword) ||
                    (d.NhanXet != null && d.NhanXet.Contains(keyword)));

            var tongSo = await query.CountAsync();

            var list = await query
                .OrderByDescending(d => d.NgayDanhGia)
                .Skip((trang - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Thống kê
            var tatCa = await _context.DanhGias.ToListAsync();
            ViewBag.TongDanhGia    = tatCa.Count;
            ViewBag.TrungBinhSao   = tatCa.Any() ? Math.Round(tatCa.Average(d => d.SoSao), 1) : 0.0;
            ViewBag.DangHienThi    = tatCa.Count(d => d.IsHienThi);
            ViewBag.DangAn         = tatCa.Count(d => !d.IsHienThi);
            ViewBag.PhanBoSao      = Enumerable.Range(1, 5)
                .Select(s => new { Sao = s, SoLuong = tatCa.Count(d => d.SoSao == s) })
                .ToList();

            // Paging
            ViewBag.TrangHienTai   = trang;
            ViewBag.TongTrang      = (int)Math.Ceiling((double)tongSo / pageSize);
            ViewBag.TongKetQua     = tongSo;

            // Filter values
            ViewBag.SoSao          = soSao;
            ViewBag.TrangThai      = trangThai;
            ViewBag.Keyword        = keyword;

            return View(list);
        }

        // POST: Admin/DanhGia/ToggleHienThi/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHienThi(int id, string? returnUrl)
        {
            var dg = await _context.DanhGias.FindAsync(id);
            if (dg == null) return NotFound();

            dg.IsHienThi = !dg.IsHienThi;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = dg.IsHienThi
                ? "Đã hiện đánh giá này."
                : "Đã ẩn đánh giá này.";

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/DanhGia/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var dg = await _context.DanhGias.FindAsync(id);
            if (dg == null) return NotFound();

            _context.DanhGias.Remove(dg);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa đánh giá.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PhanHoi(int id, string phanHoiAdmin)
        {
            var danhGia = await _context.DanhGias.FindAsync(id);
            if (danhGia == null) return NotFound();

            danhGia.PhanHoiAdmin = phanHoiAdmin;
            danhGia.NgayPhanHoi = DateTime.Now;

            // Thêm dòng này để tránh lỗi trigger
            _context.Database.ExecuteSqlRaw(
                "UPDATE DANH_GIA SET PhanHoiAdmin = {0}, NgayPhanHoi = {1} WHERE MaDanhGia = {2}",
                phanHoiAdmin, DateTime.Now, id);

            TempData["SuccessMessage"] = "Đã gửi phản hồi thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
