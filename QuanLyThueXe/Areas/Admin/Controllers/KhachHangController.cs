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
                // Thêm k.SoGiayTo.Contains(search) để tìm luôn cả CMND/CCCD
                query = query.Where(k => k.HoTen.Contains(search) ||
                                         k.SoDienThoai.Contains(search) ||
                                         k.Email!.Contains(search) ||
                                         k.SoGiayTo.Contains(search));
            }

            // Lưu lại từ khóa để truyền ra ngoài giao diện
            ViewBag.CurrentSearch = search;

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
            // Cấp dữ liệu cho 2 Dropdown khi mở form trắng
            ViewData["MaLoaiGiayTo"] = new SelectList(_context.DmLoaiGiayTos.Where(l => l.IsActive), "MaLoai", "TenLoai");
            ViewData["MaTinh"] = new SelectList(_context.DmTinhThanhs, "MaTinh", "TenTinh");
            return View();
        }

        // POST: Admin/KhachHang/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang khachHang)
        {
            // Xóa kiểm tra khóa ngoại để form không bị "bấm lưu im re"
            ModelState.Remove("LoaiGiayTo");
            ModelState.Remove("DmLoaiGiayTo");
            ModelState.Remove("TinhThanh");
            ModelState.Remove("NguoiDung");
            ModelState.Remove("HopDongs");

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng Số GPLX hoặc Số Giấy tờ
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
                    // Thiết lập ngày tạo và lưu
                    khachHang.NgayTao = DateTime.Now;
                    _context.Add(khachHang);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Thêm khách hàng mới thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Nếu có lỗi, nạp lại dữ liệu cho Dropdown trước khi trả về View
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
            // THÊM CÁC DÒNG NÀY ĐỂ BỎ QUA KIỂM TRA CÁC BẢNG LIÊN KẾT
            ModelState.Remove("LoaiGiayTo");
            ModelState.Remove("DmLoaiGiayTo"); 
            ModelState.Remove("TinhThanh");
            ModelState.Remove("NguoiDung");
            ModelState.Remove("HopDongs");
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
        // POST: Admin/KhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                try
                {
                    _context.KhachHangs.Remove(khachHang);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đã xóa khách hàng thành công.";
                }
                catch (DbUpdateException) // Bắt đúng cái lỗi vướng khóa ngoại trong ảnh
                {
                    // Báo lỗi lịch sự thay vì sập web
                    TempData["ErrorMessage"] = "Không thể xóa! Khách hàng này đang có hợp đồng hoặc lịch sử giao dịch trong hệ thống.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
        // GET: Admin/KhachHang/LichSuThue/5
        public async Task<IActionResult> LichSuThue(int id)
        {
            var lichSu = await _context.HopDongs
                .Include(h => h.Xe)
                .Where(h => h.MaKhachHang == id)
                .OrderByDescending(h => h.NgayTao) // Mới nhất xếp trên
                .ToListAsync();

            // Thay vì trả về View to bự, ta trả về PartialView (View nhỏ) để nhét vào Modal
            return PartialView("_LichSuThue", lichSu);
        }
    }

}
