using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyThueXe.Data;
using QuanLyThueXe.Models;
using System.Security.Claims;

namespace QuanLyThueXe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HopDongController : Controller
    {
        private readonly QuanLyThueXeDbContext _context;

        public HopDongController(QuanLyThueXeDbContext context)
        {
            _context = context;
        }

        // GET: Admin/HopDong
        public async Task<IActionResult> Index(string? trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _context.HopDongs
                .Include(h => h.Xe)
                .Include(h => h.KhachHang)
                .Include(h => h.NguoiTao)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(h => h.TrangThai == trangThai);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(h => h.NgayThue >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(h => h.NgayTra <= denNgay.Value);
            }

            var hopDongs = await query.OrderByDescending(h => h.NgayTao).ToListAsync();
            return View(hopDongs);
        }

        // GET: Admin/HopDong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDong = await _context.HopDongs
                .Include(h => h.Xe)
                    .ThenInclude(x => x.HangXe)
                .Include(h => h.Xe)
                    .ThenInclude(x => x.PhongCach)
                .Include(h => h.KhachHang)
                .Include(h => h.NguoiTao)
                .Include(h => h.NguoiXacNhan)
                .Include(h => h.ThanhToans)
                .Include(h => h.TaiLieus)
                .Include(h => h.DanhGia)
                .FirstOrDefaultAsync(m => m.MaHopDong == id);

            if (hopDong == null)
            {
                return NotFound();
            }

            return View(hopDong);
        }

        // GET: Admin/HopDong/Create
        public IActionResult Create()
        {
            ViewData["MaXe"] = new SelectList(_context.Xes.Where(x => x.TrangThai == "ConTrong"), "MaXe", "TenXe");
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "HoTen");
            return View();
        }

        // POST: Admin/HopDong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaXe,MaKhachHang,NgayThue,NgayTra,GhiChu")] HopDong hopDong)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                
                // Get xe info
                var xe = await _context.Xes.FindAsync(hopDong.MaXe);
                if (xe == null || xe.TrangThai != "ConTrong")
                {
                    ModelState.AddModelError("", "Xe không khả dụng.");
                    ViewData["MaXe"] = new SelectList(_context.Xes.Where(x => x.TrangThai == "ConTrong"), "MaXe", "TenXe", hopDong.MaXe);
                    ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "HoTen", hopDong.MaKhachHang);
                    return View(hopDong);
                }

                // Calculate price
                var soNgay = (hopDong.NgayTra - hopDong.NgayThue).Days;
                if (soNgay < 1) soNgay = 1;

                hopDong.GiaThueGoc = xe.GiaThueNgay;
                hopDong.HeSoMua = await GetHeSoMua(hopDong.NgayThue, hopDong.NgayTra);
                hopDong.TongTien = hopDong.GiaThueGoc * soNgay * hopDong.HeSoMua;
                hopDong.MaNguoiTao = userId;
                hopDong.TrangThai = "ChoXacNhan";
                hopDong.NgayTao = DateTime.Now;

                _context.Add(hopDong);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Tạo hợp đồng thành công!";
                return RedirectToAction(nameof(Details), new { id = hopDong.MaHopDong });
            }

            ViewData["MaXe"] = new SelectList(_context.Xes.Where(x => x.TrangThai == "ConTrong"), "MaXe", "TenXe", hopDong.MaXe);
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "HoTen", hopDong.MaKhachHang);
            return View(hopDong);
        }

        // POST: Admin/HopDong/XacNhan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(int id)
        {
            var hopDong = await _context.HopDongs.Include(h => h.Xe).FirstOrDefaultAsync(h => h.MaHopDong == id);
            if (hopDong == null)
            {
                return NotFound();
            }

            if (hopDong.TrangThai != "ChoXacNhan")
            {
                TempData["ErrorMessage"] = "Hợp đồng không ở trạng thái chờ xác nhận.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            hopDong.TrangThai = "DangThue";
            hopDong.MaNguoiXacNhan = userId;
            hopDong.NgayCapNhat = DateTime.Now;

            // Update xe status
            hopDong.Xe.TrangThai = "DangThue";
            hopDong.Xe.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xác nhận hợp đồng thành công!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Admin/HopDong/Huy/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id, string lyDoHuy)
        {
            var hopDong = await _context.HopDongs.Include(h => h.Xe).FirstOrDefaultAsync(h => h.MaHopDong == id);
            if (hopDong == null)
            {
                return NotFound();
            }

            if (hopDong.TrangThai == "DaTra" || hopDong.TrangThai == "DaHuy")
            {
                TempData["ErrorMessage"] = "Không thể hủy hợp đồng này.";
                return RedirectToAction(nameof(Details), new { id });
            }

            hopDong.TrangThai = "DaHuy";
            hopDong.LyDoHuy = lyDoHuy;
            hopDong.NgayCapNhat = DateTime.Now;

            // Update xe status if needed
            if (hopDong.Xe.TrangThai == "DangThue")
            {
                hopDong.Xe.TrangThai = "ConTrong";
                hopDong.Xe.NgayCapNhat = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hủy hợp đồng thành công!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Admin/HopDong/TraXe/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraXe(int id, DateTime ngayTraThucTe)
        {
            var hopDong = await _context.HopDongs.Include(h => h.Xe).FirstOrDefaultAsync(h => h.MaHopDong == id);
            if (hopDong == null)
            {
                return NotFound();
            }

            if (hopDong.TrangThai != "DangThue")
            {
                TempData["ErrorMessage"] = "Hợp đồng không ở trạng thái đang thuê.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Calculate late fee
            if (ngayTraThucTe > hopDong.NgayTra)
            {
                var ngayTre = (ngayTraThucTe - hopDong.NgayTra).Days;
                var heSoTre = 1.5m; // Default late fee multiplier
                hopDong.PhuPhiTreHan = hopDong.GiaThueGoc * ngayTre * heSoTre;
            }

            hopDong.TrangThai = "DaTra";
            hopDong.NgayTraThucTe = ngayTraThucTe;
            hopDong.NgayCapNhat = DateTime.Now;

            // Update xe status
            hopDong.Xe.TrangThai = "ConTrong";
            hopDong.Xe.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Trả xe thành công! {(hopDong.PhuPhiTreHan > 0 ? $"Phụ phí trễ hạn: {hopDong.PhuPhiTreHan:N0} VNĐ" : "")}";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<decimal> GetHeSoMua(DateTime ngayThue, DateTime ngayTra)
        {
            var phuPhi = await _context.PhuPhiMuas
                .Where(p => p.TuNgay <= ngayTra && p.DenNgay >= ngayThue)
                .OrderByDescending(p => p.HeSoNhan)
                .FirstOrDefaultAsync();

            return phuPhi?.HeSoNhan ?? 1.00m;
        }
    }
}
