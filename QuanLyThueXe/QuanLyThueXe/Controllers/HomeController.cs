using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueXe.Data;
using QuanLyThueXe.Models;

namespace QuanLyThueXe.Controllers;

public class HomeController : Controller
{
    private readonly QuanLyThueXeDbContext _context;

    public HomeController(QuanLyThueXeDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Lấy danh sách xe phổ biến (top 8)
        var xePhoBien = await _context.Xes
            .Include(x => x.HangXe)
            .Include(x => x.PhongCach)
            .Include(x => x.HinhAnhs)
            .Where(x => x.TrangThai == "ConTrong")
            .OrderByDescending(x => x.SoLanThue)
            .ThenByDescending(x => x.DanhGiaTrungBinh)
            .Take(8)
            .ToListAsync();

        // Lấy khuyến mãi đang hoạt động
        var khuyenMais = await _context.KhuyenMais
            .Where(k => k.IsActive && k.TuNgay <= DateTime.Now && k.DenNgay >= DateTime.Now)
            .OrderByDescending(k => k.GiaTriGiam)
            .Take(3)
            .ToListAsync();

        ViewBag.KhuyenMais = khuyenMais;
        return View(xePhoBien);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult ThongBao()
    {
        return View();
    }

    public IActionResult UuDai()
    {
        return View();
    }

    public IActionResult ChiTietThongBao(int id = 1)
    {
        ViewBag.Id = id;
        return View();
    }

    public IActionResult BaoHiem()
    {
        return View();
    }

    public IActionResult HoTro()
    {
        return View();
    }

    public IActionResult DieuKhoanDichVu()
    {
        return View();
    }

    public IActionResult ChinhSachBaoMat()
    {
        return View();
    }

    public IActionResult HopDongThueXe()
    {
        return View();
    }

    public IActionResult LienHe()
    {
        return View();
    }

    public IActionResult GioiThieu()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
