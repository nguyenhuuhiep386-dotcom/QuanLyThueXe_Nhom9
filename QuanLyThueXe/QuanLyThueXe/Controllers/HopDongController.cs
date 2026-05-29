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
    [Route("[controller]/[action]")]
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
        [HttpGet("{id}")]
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

            // Parse ngayNhan and ngayTra from query string (format yyyy-MM-dd from input type="date")
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            string[] formats = { "yyyy-MM-dd", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTH:mm", "yyyy-MM-ddTH:mm:ss", "MM/dd/yyyy", "dd/MM/yyyy" };
            if (!string.IsNullOrEmpty(ngayNhan) && DateTime.TryParseExact(ngayNhan, formats, culture, System.Globalization.DateTimeStyles.None, out var nt)) 
                dtNgayThue = nt;
            if (!string.IsNullOrEmpty(ngayTra) && DateTime.TryParseExact(ngayTra, formats, culture, System.Globalization.DateTimeStyles.None, out var ntr)) 
                dtNgayTra = ntr;

            var viewModel = new DatXeViewModel
            {
                Xe = xe,
                NgayThue = dtNgayThue,
                NgayTra = dtNgayTra
            };

            if (insurance == "caocap")
            {
                viewModel.PhiBaoHiem = 1000000;
                viewModel.GhiChu = "Gói bảo hiểm: Cao cấp";
            }

            // Calculate initial price
            await TinhTien(viewModel);

            // Fetch available promos for suggestions
            ViewBag.KhuyenMais = await _context.KhuyenMais
                .Where(k => k.IsActive && k.TuNgay <= DateTime.Now && k.DenNgay >= DateTime.Now && k.DaSuDung < k.SoLanSuDung)
                .ToListAsync();

            return View(viewModel);
        }

        // POST: HopDong/DatXe
        [HttpPost("{id?}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatXe(DatXeViewModel model, List<IFormFile>? taiLieus)
        {
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Xe.") || k == "Xe").ToList())
            {
                ModelState.Remove(key);
            }
            
            // Lấy giá trị ngày tháng thô từ form (nếu bị model binding lỗi do format)
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            string[] formats = { "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm", "yyyy-MM-ddTH:mm:ss", "yyyy-MM-ddTH:mm", "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy" };
            
            var reqNgayThue = Request.Form["NgayThue"].ToString();
            var reqNgayTra = Request.Form["NgayTra"].ToString();
            
            if (!string.IsNullOrEmpty(reqNgayThue) && DateTime.TryParseExact(reqNgayThue, formats, culture, System.Globalization.DateTimeStyles.None, out var nt)) model.NgayThue = nt;
            else if (!string.IsNullOrEmpty(reqNgayThue) && DateTime.TryParse(reqNgayThue, culture, System.Globalization.DateTimeStyles.None, out var nt2)) model.NgayThue = nt2;
            
            if (!string.IsNullOrEmpty(reqNgayTra) && DateTime.TryParseExact(reqNgayTra, formats, culture, System.Globalization.DateTimeStyles.None, out var ntr)) model.NgayTra = ntr;
            else if (!string.IsNullOrEmpty(reqNgayTra) && DateTime.TryParse(reqNgayTra, culture, System.Globalization.DateTimeStyles.None, out var ntr2)) model.NgayTra = ntr2;
            
            if (model.Xe == null || model.Xe.MaXe <= 0 || model.NgayThue == default || model.NgayTra == default)
            {
                int maXe = model.Xe?.MaXe ?? 0;
                model.Xe = (await _context.Xes
                    .Include(x => x.HangXe)
                    .Include(x => x.PhongCach)
                    .Include(x => x.HinhAnhs)
                    .FirstOrDefaultAsync(x => x.MaXe == maXe))!;
                return View(model);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Get or create KhachHang
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);
            if (khachHang == null)
            {
                TempData["ErrorMessage"] = "Vui lòng cập nhật thông tin cá nhân trước khi đặt xe.";
                return RedirectToAction("Profile", "Account");
            }

            // Calculate price
            await TinhTien(model);

            // Create contract
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

            // Upload documents if any
            if (taiLieus != null && taiLieus.Count > 0)
            {
                await UploadTaiLieus(hopDong.MaHopDong, taiLieus);
            }

            // Apply promotion if any
            if (!string.IsNullOrEmpty(model.MaKhuyenMai))
            {
                await ApDungKhuyenMai(hopDong.MaHopDong, model.MaKhuyenMai);
            }

            // Redirect to TaoHopDong page instead of ThanhToanThanhCong
            TempData["SuccessMessage"] = "Thanh toán cọc thành công! Vui lòng ký xác nhận hợp đồng.";
            return RedirectToAction("TaoHopDong", "HopDong", new { id = hopDong.MaHopDong });
        }

        // GET: HopDong/TaoHopDong/5
        [HttpGet("{id}")]
        public async Task<IActionResult> TaoHopDong(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hợp đồng.";
                return RedirectToAction(nameof(CuaToi));
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            if (khachHang == null)
            {
                TempData["ErrorMessage"] = "Vui lòng cập nhật thông tin cá nhân trước.";
                return RedirectToAction("Profile", "Account");
            }

            var hopDong = await _context.HopDongs
                .Include(h => h.Xe)
                .Include(h => h.KhachHang)
                .FirstOrDefaultAsync(h => h.MaHopDong == id && h.MaKhachHang == khachHang.MaKhachHang);

            if (hopDong == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hợp đồng hoặc bạn không có quyền truy cập.";
                return RedirectToAction(nameof(CuaToi));
            }

            return View(hopDong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanHopDong(int id, string signature)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            var hopDong = await _context.HopDongs
                .FirstOrDefaultAsync(h => h.MaHopDong == id && h.MaKhachHang == khachHang!.MaKhachHang);

            if (hopDong == null)
            {
                return NotFound();
            }

            // Mock saving signature as document/note for now
            hopDong.GhiChu = hopDong.GhiChu + "\n[Đã ký điện tử]";
            hopDong.TrangThai = "ChoXacNhan";
            
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ThanhToanThanhCong), new { id = hopDong.MaHopDong });
        }

        // GET: HopDong/ThanhToanThanhCong/5
        [HttpGet("{id}")]
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
        [HttpGet]
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
                .Include(h => h.DanhGia)
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
        [HttpGet("{id}")]
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

        // POST: HopDong/TraXeTruocHan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraXeTruocHan(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            var hopDong = await _context.HopDongs
                .FirstOrDefaultAsync(h => h.MaHopDong == id && h.MaKhachHang == khachHang!.MaKhachHang);

            if (hopDong == null) return NotFound();

            if (hopDong.TrangThai != "DangThue")
            {
                TempData["ErrorMessage"] = "Chỉ có thể trả xe đối với hợp đồng đang thuê.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            hopDong.TrangThai = "DaTra";
            // Ghi lại ngày trả thực tế
            hopDong.NgayTraThucTe = DateTime.Now;
            // Phải đảm bảo NgayTra > NgayThue (theo constraint DB)
            // Nếu trả sớm hơn dự kiến, giữ NgayTra gốc (không đổi)
            // Chỉ cập nhật NgayCapNhat để biết lúc nào trả
            hopDong.NgayCapNhat = DateTime.Now;

            // Tính tiền thực tế theo ngày dùng
            var ngayDung = (DateTime.Now - hopDong.NgayThue).Days;
            if (ngayDung < 1) ngayDung = 1;
            var tongTienThucTe = hopDong.GiaThueGoc * ngayDung * hopDong.HeSoMua;
            hopDong.TongTien = tongTienThucTe;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhGiaTraXe), new { id });
        }

        // POST: HopDong/GiaHan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GiaHan(int id, int soNgayGiaHan)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            var hopDong = await _context.HopDongs
                .FirstOrDefaultAsync(h => h.MaHopDong == id && h.MaKhachHang == khachHang!.MaKhachHang);

            if (hopDong == null) return NotFound();

            if (hopDong.TrangThai != "DangThue")
            {
                TempData["ErrorMessage"] = "Chỉ có thể gia hạn đối với hợp đồng đang thuê.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            if (soNgayGiaHan <= 0)
            {
                TempData["ErrorMessage"] = "Số ngày gia hạn không hợp lệ.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            var tienGiaHan = hopDong.GiaThueGoc * hopDong.HeSoMua * soNgayGiaHan;
            hopDong.NgayTra = hopDong.NgayTra.AddDays(soNgayGiaHan);
            hopDong.TongTien += tienGiaHan;
            hopDong.NgayCapNhat = DateTime.Now;

            // Tạo bản ghi thanh toán phần gia hạn
            var thanhToan = new ThanhToan
            {
                MaHopDong = id,
                SoTien = tienGiaHan,
                PhuongThuc = "TienMat",
                LoaiThanhToan = "ThanhToanCuoi",
                TrangThai = "ThanhCong",
                ThoiGian = DateTime.Now,
                GhiChu = $"Thanh toán gia hạn thêm {soNgayGiaHan} ngày"
            };
            _context.ThanhToans.Add(thanhToan);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Gia hạn thêm {soNgayGiaHan} ngày thành công! (+{tienGiaHan:N0}đ chi phí)";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> DanhGiaTraXe(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            var hopDong = await _context.HopDongs
                .Include(h => h.Xe)
                .Include(h => h.DanhGia)
                .FirstOrDefaultAsync(h => h.MaHopDong == id && h.MaKhachHang == khachHang!.MaKhachHang);

            if (hopDong == null) return NotFound();

            if (hopDong.TrangThai != "DaTra")
            {
                TempData["ErrorMessage"] = "Chỉ có thể đánh giá sau khi trả xe.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            if (hopDong.DanhGia != null)
            {
                TempData["ErrorMessage"] = "Bạn đã đánh giá hợp đồng này rồi.";
                return RedirectToAction(nameof(CuaToi));
            }

            return View(hopDong);
        }

        // POST: HopDong/DanhGia/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhGia(int id, int soSao, string? nhanXet, List<IFormFile> images)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            var hopDong = await _context.HopDongs
                .Include(h => h.DanhGia)
                .FirstOrDefaultAsync(h => h.MaHopDong == id && h.MaKhachHang == khachHang!.MaKhachHang);

            if (hopDong == null) return NotFound();

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

            int diemThuong = 50; // Sao = 50đ

            if (!string.IsNullOrWhiteSpace(nhanXet))
            {
                if (nhanXet.Length < 50) diemThuong += 25;
                else diemThuong += 80;
            }

            // Upload ảnh
            if (images != null && images.Count > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "danhgia");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                foreach (var file in images)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        nhanXet += $"\n[Ảnh đính kèm: /uploads/danhgia/{fileName}]";
                        diemThuong += 30; // 30đ mỗi ảnh
                    }
                }
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

            // Cập nhật điểm
            int currentPoints = 0;
            if (!string.IsNullOrEmpty(khachHang.GhiChu))
            {
                try {
                    var json = System.Text.Json.JsonDocument.Parse(khachHang.GhiChu);
                    if (json.RootElement.TryGetProperty("Diem", out var diemEl)) {
                        currentPoints = diemEl.GetInt32();
                    }
                } catch { }
            }
            currentPoints += diemThuong;
            khachHang.GhiChu = $"{{\"Diem\": {currentPoints}}}";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cảm ơn bạn đã đánh giá! Bạn được cộng {diemThuong} điểm thưởng.";
            return RedirectToAction(nameof(CuaToi));
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
            var tongTruocGiam = tamTinh + model.PhiBaoHiem;

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

                if (khuyenMai != null && (khuyenMai.DieuKienToiThieu == null || tongTruocGiam >= khuyenMai.DieuKienToiThieu))
                {
                    if (khuyenMai.LoaiGiamGia == "PhanTram")
                    {
                        model.TienGiam = tongTruocGiam * khuyenMai.GiaTriGiam / 100;
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

            model.TongTien = tongTruocGiam - model.TienGiam;
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

        [HttpGet]
        public async Task<IActionResult> CheckPromoCode(string code, decimal totalAmount)
        {
            if (string.IsNullOrEmpty(code)) return Json(new { success = false, message = "Vui lòng nhập mã khuyến mãi." });

            var khuyenMai = await _context.KhuyenMais
                .FirstOrDefaultAsync(k => k.MaCode == code &&
                                         k.IsActive &&
                                         k.TuNgay <= DateTime.Now &&
                                         k.DenNgay >= DateTime.Now &&
                                         k.DaSuDung < k.SoLanSuDung);

            if (khuyenMai == null) 
                return Json(new { success = false, message = "Mã khuyến mãi không hợp lệ hoặc đã hết hạn." });
            
            if (khuyenMai.DieuKienToiThieu.HasValue && totalAmount < khuyenMai.DieuKienToiThieu.Value)
                return Json(new { success = false, message = $"Đơn hàng tối thiểu để áp dụng là {khuyenMai.DieuKienToiThieu.Value.ToString("N0")}đ." });

            decimal discount = 0;
            if (khuyenMai.LoaiGiamGia == "PhanTram")
            {
                discount = totalAmount * khuyenMai.GiaTriGiam / 100;
                if (khuyenMai.GiaTriGiamToiDa.HasValue && discount > khuyenMai.GiaTriGiamToiDa.Value)
                    discount = khuyenMai.GiaTriGiamToiDa.Value;
            }
            else
            {
                discount = khuyenMai.GiaTriGiam;
            }

            return Json(new { success = true, discount = discount, code = khuyenMai.MaCode });
        }
    }
}
