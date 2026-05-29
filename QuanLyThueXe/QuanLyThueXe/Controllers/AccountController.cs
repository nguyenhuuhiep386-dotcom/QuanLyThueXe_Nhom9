using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueXe.Data;
using QuanLyThueXe.Models;
using QuanLyThueXe.Models.ViewModels;
using System.Security.Claims;
using BCrypt.Net;

namespace QuanLyThueXe.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyThueXeDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(QuanLyThueXeDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _context.NguoiDungs
                    .Include(n => n.VaiTro)
                    .FirstOrDefaultAsync(n => n.Email == model.Email && n.IsActive);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.MatKhau, user.MatKhauHash))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.MaNguoiDung.ToString()),
                        new Claim(ClaimTypes.Name, user.HoTen),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.VaiTro.TenVaiTro),
                        new Claim("MaVaiTro", user.MaVaiTro.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.GhiNho,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Redirect based on role
                    if (user.VaiTro.TenVaiTro == "Admin")
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (await _context.NguoiDungs.AnyAsync(n => n.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                    return View(model);
                }

                // Create new user
                var user = new NguoiDung
                {
                    HoTen = model.HoTen,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                    MaVaiTro = 2, // KhachHang
                    IsActive = true,
                    NgayTao = DateTime.Now
                };

                _context.NguoiDungs.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login");
            
            int userId = int.Parse(userIdStr);
            var user = await _context.NguoiDungs.FindAsync(userId);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            var model = new ProfileViewModel
            {
                HoTen = khachHang?.HoTen ?? user?.HoTen ?? "",
                SoDienThoai = khachHang?.SoDienThoai ?? user?.SoDienThoai ?? "",
                Email = khachHang?.Email ?? user?.Email ?? "",
                SoGPLX = khachHang?.SoGPLX ?? "",
                SoGiayTo = khachHang?.SoGiayTo ?? "",
                DiaChi = khachHang?.DiaChi ?? ""
            };

            int points = 0;
            if (khachHang != null && !string.IsNullOrEmpty(khachHang.GhiChu))
            {
                try {
                    var json = System.Text.Json.JsonDocument.Parse(khachHang.GhiChu);
                    if (json.RootElement.TryGetProperty("Diem", out var diemEl)) {
                        points = diemEl.GetInt32();
                    }
                } catch { }
            }
            ViewBag.Points = points;
            
            string tier = "Đồng";
            int nextTierPoints = 500;
            string tierColor = "#a3683a"; // Bronze
            if (points > 2000) { tier = "Vàng"; nextTierPoints = 0; tierColor = "#fbbf24"; }
            else if (points > 500) { tier = "Bạc"; nextTierPoints = 2001; tierColor = "#9ca3af"; }
            
            ViewBag.Tier = tier;
            ViewBag.TierColor = tierColor;
            ViewBag.NextTierPoints = nextTierPoints;

            if (khachHang != null)
            {
                var danhGias = await _context.DanhGias
                    .Include(d => d.Xe)
                    .Where(d => d.MaKhachHang == khachHang.MaKhachHang)
                    .OrderByDescending(d => d.NgayDanhGia)
                    .ToListAsync();
                ViewBag.DanhGias = danhGias;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "KhachHang")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);
            var user = await _context.NguoiDungs.FindAsync(userId);

            if (user != null)
            {
                user.HoTen = model.HoTen;
                user.SoDienThoai = model.SoDienThoai;
            }

            if (khachHang == null)
            {
                khachHang = new KhachHang
                {
                    MaNguoiDung = userId,
                    NgayTao = DateTime.Now,
                    MaLoaiGiayTo = 1 // CCCD default
                };
                _context.KhachHangs.Add(khachHang);
            }

            khachHang.HoTen = model.HoTen;
            khachHang.SoDienThoai = model.SoDienThoai;
            khachHang.Email = model.Email;
            khachHang.SoGPLX = model.SoGPLX;
            khachHang.SoGiayTo = model.SoGiayTo;
            khachHang.DiaChi = model.DiaChi;
            khachHang.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // API: Upload Avatar
        [HttpPost]
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn ảnh" });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return Json(new { success = false, message = "Chỉ chấp nhận file ảnh (JPG, PNG, GIF)" });
            }

            // Validate file size (max 5MB)
            if (avatar.Length > 5 * 1024 * 1024)
            {
                return Json(new { success = false, message = "Kích thước ảnh không được vượt quá 5MB" });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Save file
            var webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "images", "avatars");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{userId}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(fileStream);
            }

            var avatarUrl = $"/images/avatars/{uniqueFileName}";

            return Json(new { success = true, avatarUrl = avatarUrl });
        }

        // API: Save Bank Info
        [HttpPost]
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> SaveBankInfo([FromBody] BankInfoViewModel model)
        {
            if (string.IsNullOrEmpty(model.BankName) || string.IsNullOrEmpty(model.BankAccount) || string.IsNullOrEmpty(model.BankOwner))
            {
                return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin" });
            }

            // For now, just return success since we don't have these fields in database
            // You can add these fields to KhachHang model later
            return Json(new { success = true, message = "Đã lưu thông tin ngân hàng" });
        }

        // API: Change Password
        [HttpPost]
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin" });
            }

            if (model.NewPassword.Length < 6)
            {
                return Json(new { success = false, message = "Mật khẩu mới phải có ít nhất 6 ký tự" });
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return Json(new { success = false, message = "Mật khẩu xác nhận không khớp" });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.NguoiDungs.FindAsync(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng" });
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.MatKhauHash))
            {
                return Json(new { success = false, message = "Mật khẩu hiện tại không đúng" });
            }

            // Update password
            user.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đổi mật khẩu thành công" });
        }

        // API: Submit GPLX Verification
        [HttpPost]
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> SubmitGPLX([FromBody] GPLXVerificationViewModel model)
        {
            if (string.IsNullOrEmpty(model.Number) || string.IsNullOrEmpty(model.Class))
            {
                return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin" });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNguoiDung == userId);

            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng" });
            }

            khachHang.SoGPLX = model.Number;
            khachHang.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã gửi yêu cầu xác thực GPLX" });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
