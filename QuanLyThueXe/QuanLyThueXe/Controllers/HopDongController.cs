using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueXe.Data;
using QuanLyThueXe.Models;
using QuanLyThueXe.Models.ViewModels;
using System.Security.Claims;

namespace QuanLyThueXe.Controllers
{
    [Authorize(Roles = "KhachHang")]
    public class HopDongController : Controller
    {
        private readonly QuanLyThueXeDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HopDongController(QuanLyThueXeDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: HopDong/DatXe/5
        public async Task<IActionResult> DatXe(int id, string? ngayNhan, string? ngayTra, string? insurance)
        {
            var xe = await _context.Xes
                .Include(x => x.HangXe)
                .Include(x => x.PhongCach)
                .Include(x => x.HinhAnhs)
                .FirstOrDefaultAsync(x => x.MaXe == id);

            if (xe == null || xe.TrangThai != "ConTrong")
            {
                TempData["ErrorMessage"] = "Xe không khả dụng để đặt.";
                return RedirectToAction("DanhSach", "Xe");
            }

            DateTime dtNgayThue = DateTime.Now.AddDays(1);
            DateTime dtNgayTra = DateTime.Now.AddDays(4);

            // Parse ngayNhan and ngayTra from query string
            if (!string.IsNullOrEmpty(ngayNhan) && DateTime.TryParse(ngayNhan, out var nt)) 
                dtNgayThue = nt;
            if (!string.IsNullOrEmpty(ngayTra) && DateTime.TryParse(ngayTra, out var ntr)) 
                dtNgayTra = ntr;

            var viewModel = new DatXeViewModel
            {
                Xe = xe,
                NgayThue = dtNgayThue,
                NgayTra = dtNgayTra
            };

            if (insurance == "caocap")
            {
                viewModel.GhiChu = "Gói bảo hiểm: Cao cấp";
            }

            // Calculate initial price
            await TinhTien(viewModel);

            return View(viewModel);
        }

        // POST: HopDong/DatXe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatXe(DatXeViewModel model, List<IFormFile>? taiLieus)
        {
            ModelState.Remove("Xe");
            ModelState.Remove("Xe.TenXe");
            ModelState.Remove("Xe.BienSo");
            ModelState.Remove("Xe.SoKhung");
            ModelState.Remove("Xe.SoMay");
            ModelState.Remove("Xe.TrangThai");
            ModelState.Remove("Xe.HangXe");
            ModelState.Remove("Xe.PhongCach");

            if (!ModelState.IsValid)
            {
                model.Xe = (await _context.Xes
                    .Include(x => x.HangXe)
                    .Include(x => x.PhongCach)
                    .Include(x => x.HinhAnhs)
                    .FirstOrDefaultAsync(x => x.MaXe == model.Xe.MaXe))!;
                await TinhTien(model);
                return View(model);
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);
                if (khachHang == null)
                {
                    TempData["ErrorMessage"] = "Vui lòng cập nhật thông tin cá nhân trước khi đặt xe.";
                    return RedirectToAction("Profile", "Account");
                }

                // Reload xe từ DB thay vì dùng model.Xe
                var xe = await _context.Xes
                    .Include(x => x.HangXe)
                    .Include(x => x.PhongCach)
                    .Include(x => x.HinhAnhs)
                    .FirstOrDefaultAsync(x => x.MaXe == model.Xe.MaXe);

                if (xe == null)
                {
                    TempData["ErrorMessage"] = "Xe không tồn tại.";
                    return RedirectToAction("DanhSach", "Xe");
                }

                model.Xe = xe;
                await TinhTien(model);

                var hopDong = new HopDong
                {
                    MaXe = model.Xe.MaXe,
                    MaKhachHang = khachHang.MaKhachHang,
                    MaNguoiTao = userId,
                    NgayThue = model.NgayThue,
                    NgayTra = model.NgayTra,
                    GiaThueGoc = model.GiaThueGoc,
                    HeSoMua = model.HeSoMua,
                    TongTien = model.TongTien,
                    TrangThai = "ChoXacNhan",
                    GhiChu = model.GhiChu,
                    NgayTao = DateTime.Now
                };

                _context.HopDongs.Add(hopDong);
                await _context.SaveChangesAsync();

                if (taiLieus != null && taiLieus.Count > 0)
                    await UploadTaiLieus(hopDong.MaHopDong, taiLieus);

                if (!string.IsNullOrEmpty(model.MaKhuyenMai))
                    await ApDungKhuyenMai(hopDong.MaHopDong, model.MaKhuyenMai);

                return RedirectToAction(nameof(ThanhToanThanhCong), new { id = hopDong.MaHopDong });
            }
            catch (Exception ex)
            {
                // Tạm thời hiển thị lỗi để debug
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message} | Inner: {ex.InnerException?.Message}";

                model.Xe = (await _context.Xes
                    .Include(x => x.HangXe)
                    .Include(x => x.PhongCach)
                    .Include(x => x.HinhAnhs)
                    .FirstOrDefaultAsync(x => x.MaXe == model.Xe.MaXe))!;
                await TinhTien(model);
                return View(model);
            }
        }
        // GET: HopDong/ThanhToanThanhCong/5
        public async Task<IActionResult> ThanhToanThanhCong(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            var hopDong = await _context.HopDongs
                .Include(h => h.Xe)
                .FirstOrDefaultAsync(h => h.MaHopDong == id && h.MaKhachHang == khachHang!.MaKhachHang);

            if (hopDong == null)
            {
                return NotFound();
            }

            return View(hopDong);
        }

        // GET: HopDong/CuaToi
        public async Task<IActionResult> CuaToi(string? trangThai, string? thoiGian, string? search)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            if (khachHang == null)
            {
                return View(new List<HopDong>());
            }

            var query = _context.HopDongs
                .Include(h => h.Xe)
                    .ThenInclude(x => x.HangXe)
                .Include(h => h.Xe)
                    .ThenInclude(x => x.HinhAnhs)
                .Where(h => h.MaKhachHang == khachHang.MaKhachHang)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // Extract number from search string like "#HD-12" or "12"
                var searchNum = new string(search.Where(char.IsDigit).ToArray());
                if (int.TryParse(searchNum, out int hdId))
                {
                    query = query.Where(h => h.MaHopDong == hdId);
                }
            }

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(h => h.TrangThai == trangThai);
            }

            if (!string.IsNullOrEmpty(thoiGian))
            {
                if (thoiGian == "30")
                {
                    var date30 = DateTime.Now.AddDays(-30);
                    query = query.Where(h => h.NgayTao >= date30);
                }
                else if (thoiGian == "90")
                {
                    var date90 = DateTime.Now.AddDays(-90);
                    query = query.Where(h => h.NgayTao >= date90);
                }
            }

            var hopDongs = await query.OrderByDescending(h => h.NgayTao).ToListAsync();
            return View(hopDongs);
        }

        // GET: HopDong/ChiTiet/5
        public async Task<IActionResult> ChiTiet(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            var hopDong = await _context.HopDongs
                .Include(h => h.Xe)
                    .ThenInclude(x => x.HangXe)
                .Include(h => h.Xe)
                    .ThenInclude(x => x.PhongCach)
                .Include(h => h.Xe)
                    .ThenInclude(x => x.HinhAnhs)
                .Include(h => h.KhachHang)
                .Include(h => h.ThanhToans)
                .Include(h => h.TaiLieus)
                .Include(h => h.DanhGia)
                .FirstOrDefaultAsync(h => h.MaHopDong == id && h.MaKhachHang == khachHang!.MaKhachHang);

            if (hopDong == null)
            {
                return NotFound();
            }

            return View(hopDong);
        }

        // POST: HopDong/Huy/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id, string lyDoHuy)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            var hopDong = await _context.HopDongs
                .FirstOrDefaultAsync(h => h.MaHopDong == id && h.MaKhachHang == khachHang!.MaKhachHang);

            if (hopDong == null)
            {
                return NotFound();
            }

            if (hopDong.TrangThai != "ChoXacNhan")
            {
                TempData["ErrorMessage"] = "Chỉ có thể hủy hợp đồng đang chờ xác nhận.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            hopDong.TrangThai = "DaHuy";
            hopDong.LyDoHuy = lyDoHuy;
            hopDong.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hủy hợp đồng thành công.";
            return RedirectToAction(nameof(CuaToi));
        }

        // POST: HopDong/DanhGia/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhGia(int id, int soSao, string? nhanXet)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            var hopDong = await _context.HopDongs
                .Include(h => h.DanhGia)
                .FirstOrDefaultAsync(h => h.MaHopDong == id && h.MaKhachHang == khachHang!.MaKhachHang);

            if (hopDong == null)
            {
                return NotFound();
            }

            if (hopDong.TrangThai != "DaTra")
            {
                TempData["ErrorMessage"] = "Chỉ có thể đánh giá sau khi trả xe.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            if (hopDong.DanhGia != null)
            {
                TempData["ErrorMessage"] = "Bạn đã đánh giá hợp đồng này rồi.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            var danhGia = new DanhGia
            {
                MaHopDong = id,
                MaXe = hopDong.MaXe,
                MaKhachHang = khachHang!.MaKhachHang,
                SoSao = soSao,
                NhanXet = nhanXet,
                IsHienThi = true,
                NgayDanhGia = DateTime.Now
            };

            _context.DanhGias.Add(danhGia);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá!";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }

        private async Task TinhTien(DatXeViewModel model)
        {
            var xe = await _context.Xes.FindAsync(model.Xe.MaXe);
            if (xe == null) return;

            model.SoNgay = (model.NgayTra - model.NgayThue).Days;
            if (model.SoNgay < 1) model.SoNgay = 1;

            model.GiaThueGoc = xe.GiaThueNgay;

            // Get seasonal surcharge
            var phuPhi = await _context.PhuPhiMuas
                .Where(p => p.TuNgay <= model.NgayTra && p.DenNgay >= model.NgayThue)
                .OrderByDescending(p => p.HeSoNhan)
                .FirstOrDefaultAsync();

            model.HeSoMua = phuPhi?.HeSoNhan ?? 1.00m;
            model.TenDotPhuPhi = phuPhi?.TenDot;

            var tamTinh = model.GiaThueGoc * model.SoNgay * model.HeSoMua;

            // Apply promotion if any
            model.TienGiam = 0;
            if (!string.IsNullOrEmpty(model.MaKhuyenMai))
            {
                var khuyenMai = await _context.KhuyenMais
                    .FirstOrDefaultAsync(k => k.MaCode == model.MaKhuyenMai &&
                                             k.IsActive &&
                                             k.TuNgay <= DateTime.Now &&
                                             k.DenNgay >= DateTime.Now &&
                                             k.DaSuDung < k.SoLanSuDung);

                if (khuyenMai != null && (khuyenMai.DieuKienToiThieu == null || tamTinh >= khuyenMai.DieuKienToiThieu))
                {
                    if (khuyenMai.LoaiGiamGia == "PhanTram")
                    {
                        model.TienGiam = tamTinh * khuyenMai.GiaTriGiam / 100;
                        if (khuyenMai.GiaTriGiamToiDa.HasValue && model.TienGiam > khuyenMai.GiaTriGiamToiDa.Value)
                        {
                            model.TienGiam = khuyenMai.GiaTriGiamToiDa.Value;
                        }
                    }
                    else
                    {
                        model.TienGiam = khuyenMai.GiaTriGiam;
                    }
                }
            }

            model.TongTien = tamTinh - model.TienGiam;
            if (model.TongTien < 0) model.TongTien = 0;
        }

        private async Task UploadTaiLieus(int maHopDong, List<IFormFile> files)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "contracts", maHopDong.ToString());
            Directory.CreateDirectory(uploadsFolder);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    var taiLieu = new HopDongTaiLieu
                    {
                        MaHopDong = maHopDong,
                        TenTaiLieu = file.FileName,
                        DuongDan = $"/uploads/contracts/{maHopDong}/{uniqueFileName}",
                        LoaiFile = Path.GetExtension(file.FileName),
                        NgayTao = DateTime.Now
                    };

                    _context.HopDongTaiLieus.Add(taiLieu);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task ApDungKhuyenMai(int maHopDong, string maCode)
        {
            var khuyenMai = await _context.KhuyenMais
                .FirstOrDefaultAsync(k => k.MaCode == maCode &&
                                         k.IsActive &&
                                         k.TuNgay <= DateTime.Now &&
                                         k.DenNgay >= DateTime.Now &&
                                         k.DaSuDung < k.SoLanSuDung);

            if (khuyenMai != null)
            {
                var hopDong = await _context.HopDongs.FindAsync(maHopDong);
                if (hopDong != null)
                {
                    var lichSu = new LichSuKhuyenMai
                    {
                        MaKhuyenMai = khuyenMai.MaKhuyenMai,
                        MaHopDong = maHopDong,
                        SoTienDuocGiam = hopDong.TongTien - (hopDong.GiaThueGoc * (hopDong.NgayTra - hopDong.NgayThue).Days * hopDong.HeSoMua),
                        ThoiGianApDung = DateTime.Now
                    };

                    _context.LichSuKhuyenMais.Add(lichSu);
                    
                    khuyenMai.DaSuDung++;
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
