using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyThueXe.Data;
using QuanLyThueXe.Models;

namespace QuanLyThueXe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class XeController : Controller
    {
        private readonly QuanLyThueXeDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public XeController(QuanLyThueXeDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Xe
        // Thêm tham số string? phongCach vào hàm Index
        public async Task<IActionResult> Index(string? search, string? trangThai, string? phongCach)
        {
            var query = _context.Xes
                .Include(x => x.HangXe)
                .Include(x => x.PhongCach)
                .Include(x => x.HinhAnhs)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.TenXe.Contains(search) || x.BienSo.Contains(search));
            }

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(x => x.TrangThai == trangThai);
            }

            // Logic xử lý lọc theo phong cách xe
            if (!string.IsNullOrEmpty(phongCach) && phongCach != "all")
            {
                query = query.Where(x => x.PhongCach.TenPhongCach == phongCach);
            }

            var xes = await query.OrderByDescending(x => x.NgayTao).ToListAsync();
            return View(xes);
        }
        // GET: Admin/Xe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xe = await _context.Xes
                .Include(x => x.HangXe)
                .Include(x => x.PhongCach)
                .Include(x => x.HinhAnhs)
                .Include(x => x.DanhGias)
                    .ThenInclude(d => d.KhachHang)
                .Include(x => x.HopDongs)
                    .ThenInclude(h => h.KhachHang)
                .FirstOrDefaultAsync(m => m.MaXe == id);

            if (xe == null)
            {
                return NotFound();
            }

            return View(xe);
        }

        // GET: Admin/Xe/Create
        public IActionResult Create()
        {
            ViewData["MaHangXe"] = new SelectList(_context.DmHangXes.Where(h => h.IsActive), "MaHangXe", "TenHang");
            ViewData["MaPhongCach"] = new SelectList(_context.DmPhongCachs.Where(p => p.IsActive), "MaPhongCach", "TenPhongCach");
            return View();
        }

        // POST: Admin/Xe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenXe,BienSo,MaHangXe,MaPhongCach,GiaThueNgay,NamSanXuat,DungTich,MoTa,TrangThai")] Xe xe, List<IFormFile>? hinhAnhs)
        {
            ModelState.Remove("HangXe");
            ModelState.Remove("PhongCach");
            ModelState.Remove("HinhAnhs");
            ModelState.Remove("DanhGias");
            ModelState.Remove("HopDongs");
            if (ModelState.IsValid)
            {
                // Check if BienSo already exists
                if (await _context.Xes.AnyAsync(x => x.BienSo == xe.BienSo))
                {
                    ModelState.AddModelError("BienSo", "Biển số này đã tồn tại.");
                    ViewData["MaHangXe"] = new SelectList(_context.DmHangXes.Where(h => h.IsActive), "MaHangXe", "TenHang", xe.MaHangXe);
                    ViewData["MaPhongCach"] = new SelectList(_context.DmPhongCachs.Where(p => p.IsActive), "MaPhongCach", "TenPhongCach", xe.MaPhongCach);
                    return View(xe);
                }

                xe.NgayTao = DateTime.Now;
                _context.Add(xe);
                await _context.SaveChangesAsync();

                // Upload hình ảnh
                if (hinhAnhs != null && hinhAnhs.Count > 0)
                {
                    await UploadHinhAnhs(xe.MaXe, hinhAnhs);
                }

                TempData["SuccessMessage"] = "Thêm xe mới thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaHangXe"] = new SelectList(_context.DmHangXes.Where(h => h.IsActive), "MaHangXe", "TenHang", xe.MaHangXe);
            ViewData["MaPhongCach"] = new SelectList(_context.DmPhongCachs.Where(p => p.IsActive), "MaPhongCach", "TenPhongCach", xe.MaPhongCach);
            return View(xe);
        }

        // GET: Admin/Xe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xe = await _context.Xes
                .Include(x => x.HinhAnhs)
                .FirstOrDefaultAsync(x => x.MaXe == id);

            if (xe == null)
            {
                return NotFound();
            }

            ViewData["MaHangXe"] = new SelectList(_context.DmHangXes.Where(h => h.IsActive), "MaHangXe", "TenHang", xe.MaHangXe);
            ViewData["MaPhongCach"] = new SelectList(_context.DmPhongCachs.Where(p => p.IsActive), "MaPhongCach", "TenPhongCach", xe.MaPhongCach);
            return View(xe);
        }

        // POST: Admin/Xe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaXe,TenXe,BienSo,MaHangXe,MaPhongCach,GiaThueNgay,NamSanXuat,DungTich,MoTa,TrangThai,NgayTao")] Xe xe, List<IFormFile>? hinhAnhs)
        {
            if (id != xe.MaXe)
            {
                return NotFound();
            }
            ModelState.Remove("HangXe");
            ModelState.Remove("PhongCach");
            ModelState.Remove("HinhAnhs");
            ModelState.Remove("DanhGias"); // (Thêm nếu có)
            ModelState.Remove("HopDongs");
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if BienSo already exists (excluding current xe)
                    if (await _context.Xes.AnyAsync(x => x.BienSo == xe.BienSo && x.MaXe != id))
                    {
                        ModelState.AddModelError("BienSo", "Biển số này đã tồn tại.");
                        ViewData["MaHangXe"] = new SelectList(_context.DmHangXes.Where(h => h.IsActive), "MaHangXe", "TenHang", xe.MaHangXe);
                        ViewData["MaPhongCach"] = new SelectList(_context.DmPhongCachs.Where(p => p.IsActive), "MaPhongCach", "TenPhongCach", xe.MaPhongCach);
                        return View(xe);
                    }

                    xe.NgayCapNhat = DateTime.Now;
                    _context.Update(xe);
                    await _context.SaveChangesAsync();

                    // Upload hình ảnh mới nếu có
                    if (hinhAnhs != null && hinhAnhs.Count > 0)
                    {
                        await UploadHinhAnhs(xe.MaXe, hinhAnhs);
                    }

                    TempData["SuccessMessage"] = "Cập nhật thông tin xe thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XeExists(xe.MaXe))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaHangXe"] = new SelectList(_context.DmHangXes.Where(h => h.IsActive), "MaHangXe", "TenHang", xe.MaHangXe);
            ViewData["MaPhongCach"] = new SelectList(_context.DmPhongCachs.Where(p => p.IsActive), "MaPhongCach", "TenPhongCach", xe.MaPhongCach);
            return View(xe);
        }

        // GET: Admin/Xe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xe = await _context.Xes
                .Include(x => x.HangXe)
                .Include(x => x.PhongCach)
                .FirstOrDefaultAsync(m => m.MaXe == id);

            if (xe == null)
            {
                return NotFound();
            }

            return View(xe);
        }
        // POST: Admin/Xe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var xe = await _context.Xes
                .Include(x => x.HinhAnhs)
                .Include(x => x.DanhGias)
                .FirstOrDefaultAsync(m => m.MaXe == id);

            if (xe != null)
            {
                // 1. Kiểm tra ưu tiên số 1: Hợp đồng
                if (await _context.HopDongs.AnyAsync(h => h.MaXe == id))
                {
                    TempData["ErrorMessage"] = "Không thể xóa! Xe này đã có lịch sử hợp đồng. Vui lòng ẩn đi thay vì xóa.";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    // 2. Xóa các dữ liệu con đã biết (Hình ảnh, Đánh giá)
                    if (xe.HinhAnhs != null && xe.HinhAnhs.Any())
                    {
                        _context.XeHinhAnhs.RemoveRange(xe.HinhAnhs);
                    }
                    if (xe.DanhGias != null && xe.DanhGias.Any())
                    {
                        _context.DanhGias.RemoveRange(xe.DanhGias);
                    }

                    // 3. Tiến hành xóa xe
                    _context.Xes.Remove(xe);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đã xóa xe thành công khỏi hệ thống!";
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    // 4. NẾU SQL SERVER CHẶN XÓA (Do dính Lịch bảo dưỡng, Giỏ hàng...)
                    // Bẫy lỗi tại đây để phần mềm không bị sập (Crash)
                    TempData["ErrorMessage"] = "Không thể xóa xe này vì đang có dữ liệu liên kết (VD: Lịch bảo dưỡng). Vui lòng hoàn tất hoặc xóa lịch bảo dưỡng trước.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
        private bool XeExists(int id)
        {
            return _context.Xes.Any(e => e.MaXe == id);
        }

        private async Task UploadHinhAnhs(int maXe, List<IFormFile> files)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "xe");
            Directory.CreateDirectory(uploadsFolder);

            int thuTu = await _context.XeHinhAnhs.Where(x => x.MaXe == maXe).CountAsync();
            bool isFirst = thuTu == 0;

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var uniqueFileName = $"{maXe}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var hinhAnh = new XeHinhAnh
                    {
                        MaXe = maXe,
                        DuongDanAnh = $"/images/xe/{uniqueFileName}",
                        IsAnhChinh = isFirst,
                        ThuTu = thuTu++,
                        NgayTao = DateTime.Now
                    };

                    _context.XeHinhAnhs.Add(hinhAnh);
                    isFirst = false;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
