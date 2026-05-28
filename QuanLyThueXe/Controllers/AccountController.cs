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

        public AccountController(QuanLyThueXeDbContext context)
        {
            _context = context;
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

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
