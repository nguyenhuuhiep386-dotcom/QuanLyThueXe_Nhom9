using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueXe.Data;
using QuanLyThueXe.Models;
using System.Security.Claims;

namespace QuanLyThueXe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class KhuyenMaiController : Controller
    {
        private readonly QuanLyThueXeDbContext _context;

        public KhuyenMaiController(QuanLyThueXeDbContext context)
        {
            _context = context;
        }

        // GET: Admin/KhuyenMai
        public async Task<IActionResult> Index(string? trangThai, string? loaiGiam, string? keyword)
        {
            var query = _context.KhuyenMais
                .Include(k => k.NguoiTao)
                .AsQueryable();

            if (!string.IsNullOrEmpty(loaiGiam))
                query = query.Where(k => k.LoaiGiamGia == loaiGiam);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(k => k.MaCode.Contains(keyword) || k.TenChuongTrinh.Contains(keyword));

            var list = await query.OrderByDescending(k => k.NgayTao).ToListAsync();

            // Filter trạng thái sau khi load (vì cần so sánh DateTime)
            if (trangThai == "active")
                list = list.Where(k => k.IsActive && k.DenNgay >= DateTime.Now && k.DaSuDung < k.SoLanSuDung).ToList();
            else if (trangThai == "expired")
                list = list.Where(k => k.DenNgay < DateTime.Now).ToList();
            else if (trangThai == "inactive")
                list = list.Where(k => !k.IsActive).ToList();

            ViewBag.TrangThai = trangThai;
            ViewBag.LoaiGiam  = loaiGiam;
            ViewBag.Keyword   = keyword;

            return View(list);
        }

        // GET: Admin/KhuyenMai/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var km = await _context.KhuyenMais
                .Include(k => k.NguoiTao)
                .Include(k => k.LichSuKhuyenMais)
                    .ThenInclude(ls => ls.HopDong)
                        .ThenInclude(h => h.KhachHang)
                .FirstOrDefaultAsync(k => k.MaKhuyenMai == id);

            if (km == null) return NotFound();
            return View(km);
        }

        // GET: Admin/KhuyenMai/Create
        public IActionResult Create()
        {
            return View(new KhuyenMai
            {
                IsActive     = true,
                SoLanSuDung  = 1,
                TuNgay       = DateTime.Today,
                DenNgay      = DateTime.Today.AddMonths(1),
                LoaiGiamGia  = "PhanTram"
            });
        }

        // POST: Admin/KhuyenMai/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("TenChuongTrinh,MaCode,LoaiGiamGia,GiaTriGiam,GiaTriGiamToiDa,SoLanSuDung,TuNgay,DenNgay,DieuKienToiThieu,IsActive")]
            KhuyenMai km)
        {
            // Kiểm tra mã code trùng
            if (await _context.KhuyenMais.AnyAsync(k => k.MaCode == km.MaCode))
            {
                ModelState.AddModelError("MaCode", "Mã code này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                km.MaCode     = km.MaCode.ToUpper().Trim();
                km.DaSuDung   = 0;
                km.NgayTao    = DateTime.Now;
                km.MaNguoiTao = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                _context.Add(km);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Tạo khuyến mãi '{km.TenChuongTrinh}' thành công!";
                return RedirectToAction(nameof(Details), new { id = km.MaKhuyenMai });
            }

            return View(km);
        }

        // GET: Admin/KhuyenMai/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var km = await _context.KhuyenMais.FindAsync(id);
            if (km == null) return NotFound();
            return View("Create", km); // Dùng lại view Create
        }

        // POST: Admin/KhuyenMai/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("MaKhuyenMai,TenChuongTrinh,MaCode,LoaiGiamGia,GiaTriGiam,GiaTriGiamToiDa,SoLanSuDung,DaSuDung,TuNgay,DenNgay,DieuKienToiThieu,IsActive,NgayTao")]
            KhuyenMai km)
        {
            if (id != km.MaKhuyenMai) return NotFound();

            // Kiểm tra trùng code (trừ chính nó)
            if (await _context.KhuyenMais.AnyAsync(k => k.MaCode == km.MaCode && k.MaKhuyenMai != id))
            {
                ModelState.AddModelError("MaCode", "Mã code này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                km.MaCode = km.MaCode.ToUpper().Trim();
                _context.Update(km);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật khuyến mãi thành công!";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View("Create", km);
        }

        // POST: Admin/KhuyenMai/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var km = await _context.KhuyenMais.FindAsync(id);
            if (km == null) return NotFound();

            km.IsActive = !km.IsActive;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = km.IsActive ? "Đã bật khuyến mãi!" : "Đã tắt khuyến mãi!";
            return RedirectToAction(nameof(Index));
        }
    }
}
