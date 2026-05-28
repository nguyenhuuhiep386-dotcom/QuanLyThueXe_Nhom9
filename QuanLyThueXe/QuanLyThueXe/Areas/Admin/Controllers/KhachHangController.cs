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
    public class KhachHangController : Controller
    {
        private readonly QuanLyThueXeDbContext _context;

        public KhachHangController(QuanLyThueXeDbContext context)
        {
            _context = context;
        }

        // GET: Admin/KhachHang
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.KhachHangs
                .Include(k => k.LoaiGiayTo)
                .Include(k => k.TinhThanh)
                .Include(k => k.NguoiDung)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(k => k.HoTen.Contains(search) || 
                                        k.SoDienThoai.Contains(search) || 
                                        k.Email!.Contains(search));
            }

            var khachHangs = await query.OrderByDescending(k => k.NgayTao).ToListAsync();
            return View(khachHangs);
        }

        // GET: Admin/KhachHang/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .Include(k => k.LoaiGiayTo)
                .Include(k => k.TinhThanh)
                .Include(k => k.NguoiDung)
                .Include(k => k.HopDongs)
                    .ThenInclude(h => h.Xe)
                .FirstOrDefaultAsync(m => m.MaKhachHang == id);

            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // GET: Admin/KhachHang/Create
        public IActionResult Create()
        {
            ViewData["MaLoaiGiayTo"] = new SelectList(_context.DmLoaiGiayTos.Where(l => l.IsActive), "MaLoai", "TenLoai");
            ViewData["MaTinh"] = new SelectList(_context.DmTinhThanhs, "MaTinh", "TenTinh");
            return View();
        }

        // POST: Admin/KhachHang/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoTen,SoDienThoai,Email,SoGPLX,MaLoaiGiayTo,SoGiayTo,NgayCap,NoiCap,DiaChi,MaTinh,NgaySinh,GioiTinh,GhiChu")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                // Check duplicates
                if (await _context.KhachHangs.AnyAsync(k => k.SoGPLX == khachHang.SoGPLX))
                {
                    ModelState.AddModelError("SoGPLX", "Số GPLX này đã tồn tại.");
                }
                else if (await _context.KhachHangs.AnyAsync(k => k.SoGiayTo == khachHang.SoGiayTo))
                {
                    ModelState.AddModelError("SoGiayTo", "Số giấy tờ này đã tồn tại.");
                }
                else
                {
                    khachHang.NgayTao = DateTime.Now;
                    _context.Add(khachHang);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["MaLoaiGiayTo"] = new SelectList(_context.DmLoaiGiayTos.Where(l => l.IsActive), "MaLoai", "TenLoai", khachHang.MaLoaiGiayTo);
            ViewData["MaTinh"] = new SelectList(_context.DmTinhThanhs, "MaTinh", "TenTinh", khachHang.MaTinh);
            return View(khachHang);
        }

        // GET: Admin/KhachHang/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }

            ViewData["MaLoaiGiayTo"] = new SelectList(_context.DmLoaiGiayTos.Where(l => l.IsActive), "MaLoai", "TenLoai", khachHang.MaLoaiGiayTo);
            ViewData["MaTinh"] = new SelectList(_context.DmTinhThanhs, "MaTinh", "TenTinh", khachHang.MaTinh);
            return View(khachHang);
        }

        // POST: Admin/KhachHang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKhachHang,HoTen,SoDienThoai,Email,SoGPLX,MaLoaiGiayTo,SoGiayTo,NgayCap,NoiCap,DiaChi,MaTinh,NgaySinh,GioiTinh,GhiChu,NgayTao,MaNguoiDung")] KhachHang khachHang)
        {
            if (id != khachHang.MaKhachHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check duplicates (excluding current)
                    if (await _context.KhachHangs.AnyAsync(k => k.SoGPLX == khachHang.SoGPLX && k.MaKhachHang != id))
                    {
                        ModelState.AddModelError("SoGPLX", "Số GPLX này đã tồn tại.");
                    }
                    else if (await _context.KhachHangs.AnyAsync(k => k.SoGiayTo == khachHang.SoGiayTo && k.MaKhachHang != id))
                    {
                        ModelState.AddModelError("SoGiayTo", "Số giấy tờ này đã tồn tại.");
                    }
                    else
                    {
                        khachHang.NgayCapNhat = DateTime.Now;
                        _context.Update(khachHang);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.MaKhachHang))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["MaLoaiGiayTo"] = new SelectList(_context.DmLoaiGiayTos.Where(l => l.IsActive), "MaLoai", "TenLoai", khachHang.MaLoaiGiayTo);
            ViewData["MaTinh"] = new SelectList(_context.DmTinhThanhs, "MaTinh", "TenTinh", khachHang.MaTinh);
            return View(khachHang);
        }

        private bool KhachHangExists(int id)
        {
            return _context.KhachHangs.Any(e => e.MaKhachHang == id);
        }
    }
}
