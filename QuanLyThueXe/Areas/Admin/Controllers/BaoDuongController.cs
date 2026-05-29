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
    public class BaoDuongController : Controller
    {
        private readonly QuanLyThueXeDbContext _context;

        public BaoDuongController(QuanLyThueXeDbContext context)
        {
            _context = context;
        }

        // GET: Admin/BaoDuong
        public async Task<IActionResult> Index(string? trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _context.BaoDuongs
                .Include(b => b.Xe)
                    .ThenInclude(x => x.HangXe)
                .Include(b => b.NguoiPhuTrach)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(b => b.TrangThai == trangThai);

            if (tuNgay.HasValue)
                query = query.Where(b => b.NgayVao >= tuNgay.Value);

            if (denNgay.HasValue)
                query = query.Where(b => b.NgayVao <= denNgay.Value);

            // Truyền lại giá trị filter cho view
            ViewBag.TrangThai = trangThai;
            ViewBag.TuNgay    = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay   = denNgay?.ToString("yyyy-MM-dd");

            var list = await query.OrderByDescending(b => b.NgayTao).ToListAsync();
            return View(list);
        }

        // GET: Admin/BaoDuong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var baoDuong = await _context.BaoDuongs
                .Include(b => b.Xe)
                    .ThenInclude(x => x.HangXe)
                .Include(b => b.Xe)
                    .ThenInclude(x => x.HinhAnhs)
                .Include(b => b.NguoiPhuTrach)
                .FirstOrDefaultAsync(b => b.MaBaoDuong == id);

            if (baoDuong == null) return NotFound();
            return View(baoDuong);
        }

        // GET: Admin/BaoDuong/Create
        public IActionResult Create()
        {
            // Chỉ lấy xe ConTrong
            ViewBag.DanhSachXe = new SelectList(
                _context.Xes.Where(x => x.TrangThai == "ConTrong").OrderBy(x => x.TenXe),
                "MaXe", "TenXe"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
     [Bind("MaXe,LoaiBaoDuong,NgayVao,NgayRa,ChiPhi,DonViThucHien,GhiChu")]
    BaoDuong baoDuong)
        {
            // Xóa lỗi validation của navigation properties
            ModelState.Remove("Xe");
            ModelState.Remove("NguoiPhuTrach");
            ModelState.Remove("TrangThai");

            if (ModelState.IsValid)
            {
                baoDuong.TrangThai = "DangSua";
                baoDuong.NgayTao = DateTime.Now;

                var xe = await _context.Xes.FindAsync(baoDuong.MaXe);
                if (xe != null)
                {
                    xe.TrangThai = "BaoDuong";
                    xe.NgayCapNhat = DateTime.Now;
                }

                _context.Add(baoDuong);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Tạo phiếu bảo dưỡng thành công!";
                return RedirectToAction(nameof(Details), new { id = baoDuong.MaBaoDuong });
            }

            ViewBag.DanhSachXe = new SelectList(
                _context.Xes.Where(x => x.TrangThai == "ConTrong").OrderBy(x => x.TenXe),
                "MaXe", "TenXe", baoDuong.MaXe
            );
            return View(baoDuong);
        }
        // GET: Admin/BaoDuong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var baoDuong = await _context.BaoDuongs.FindAsync(id);
            if (baoDuong == null) return NotFound();

            // Lấy xe ConTrong + chính chiếc xe của phiếu này
            ViewBag.DanhSachXe = new SelectList(
                _context.Xes
                    .Where(x => x.TrangThai == "ConTrong" || x.MaXe == baoDuong.MaXe)
                    .OrderBy(x => x.TenXe),
                "MaXe", "TenXe", baoDuong.MaXe
            );
            return View(baoDuong);
        }

        // POST: Admin/BaoDuong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("MaBaoDuong,MaXe,LoaiBaoDuong,NgayVao,NgayRa,ChiPhi,DonViThucHien,GhiChu,TrangThai,NgayTao")]
    BaoDuong baoDuong)
        {
            if (id != baoDuong.MaBaoDuong) return NotFound();

            ModelState.Remove("Xe");
            ModelState.Remove("NguoiPhuTrach");

            if (ModelState.IsValid)
            {
                _context.Update(baoDuong);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật phiếu bảo dưỡng thành công!";
                return RedirectToAction(nameof(Details), new { id });
            }

            ViewBag.DanhSachXe = new SelectList(
                _context.Xes
                    .Where(x => x.TrangThai == "ConTrong" || x.MaXe == baoDuong.MaXe)
                    .OrderBy(x => x.TenXe),
                "MaXe", "TenXe", baoDuong.MaXe
            );
            return View(baoDuong);
        }
        // POST: Admin/BaoDuong/HoanThanh/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoanThanh(int id)
        {
            var baoDuong = await _context.BaoDuongs
                .Include(b => b.Xe)
                .FirstOrDefaultAsync(b => b.MaBaoDuong == id);

            if (baoDuong == null) return NotFound();

            baoDuong.TrangThai   = "HoanThanh";
            baoDuong.NgayRa      = baoDuong.NgayRa ?? DateTime.Now;

            // Trả xe về trạng thái ConTrong
            if (baoDuong.Xe != null && baoDuong.Xe.TrangThai == "BaoDuong")
            {
                baoDuong.Xe.TrangThai   = "ConTrong";
                baoDuong.Xe.NgayCapNhat = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã đánh dấu hoàn thành bảo dưỡng!";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
