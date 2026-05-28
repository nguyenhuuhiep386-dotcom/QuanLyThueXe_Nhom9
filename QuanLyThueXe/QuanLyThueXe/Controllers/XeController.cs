using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueXe.Data;

namespace QuanLyThueXe.Controllers
{
    public class XeController : Controller
    {
        private readonly QuanLyThueXeDbContext _context;

        public XeController(QuanLyThueXeDbContext context)
        {
            _context = context;
        }

        // GET: Xe/DanhSach
        public async Task<IActionResult> DanhSach(string? search, int? hangXe, int? phongCach, decimal? giaMin, decimal? giaMax, List<int> namSanXuat)
        {
            var query = _context.Xes
                .Include(x => x.HangXe)
                .Include(x => x.PhongCach)
                .Include(x => x.HinhAnhs)
                .Where(x => x.TrangThai == "ConTrong")
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.TenXe.Contains(search) || x.BienSo.Contains(search));
            }

            // Lọc theo hãng xe
            if (hangXe.HasValue)
            {
                query = query.Where(x => x.MaHangXe == hangXe.Value);
            }

            // Lọc theo phong cách
            if (phongCach.HasValue)
            {
                query = query.Where(x => x.MaPhongCach == phongCach.Value);
            }

            // Lọc theo giá
            if (giaMin.HasValue)
            {
                query = query.Where(x => x.GiaThueNgay >= giaMin.Value);
            }

            if (giaMax.HasValue && giaMax.Value > 0)
            {
                query = query.Where(x => x.GiaThueNgay <= giaMax.Value);
            }

            // Lọc theo năm sản xuất
            if (namSanXuat != null && namSanXuat.Count > 0)
            {
                query = query.Where(x => namSanXuat.Contains(x.NamSanXuat));
            }

            var xes = await query.OrderByDescending(x => x.DanhGiaTrungBinh).ToListAsync();

            // Lấy danh sách hãng xe và phong cách cho filter
            ViewBag.HangXes = await _context.DmHangXes.Where(h => h.IsActive).ToListAsync();
            ViewBag.PhongCachs = await _context.DmPhongCachs.Where(p => p.IsActive).ToListAsync();

            return View(xes);
        }

        // GET: Xe/ChiTiet/5
        public async Task<IActionResult> ChiTiet(int id)
        {
            var xe = await _context.Xes
                .Include(x => x.HangXe)
                .Include(x => x.PhongCach)
                .Include(x => x.HinhAnhs)
                .Include(x => x.DanhGias)
                    .ThenInclude(d => d.KhachHang)
                .FirstOrDefaultAsync(x => x.MaXe == id);

            if (xe == null)
            {
                return NotFound();
            }

            // Lấy các xe tương tự
            var xeTuongTu = await _context.Xes
                .Include(x => x.HangXe)
                .Include(x => x.PhongCach)
                .Include(x => x.HinhAnhs)
                .Where(x => x.MaXe != id && 
                           (x.MaPhongCach == xe.MaPhongCach || x.MaHangXe == xe.MaHangXe) &&
                           x.TrangThai == "ConTrong")
                .Take(4)
                .ToListAsync();

            ViewBag.XeTuongTu = xeTuongTu;

            return View(xe);
        }

        // GET: Xe/SearchApi
        [HttpGet]
        public async Task<IActionResult> SearchApi(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Json(new object[] { });
            var results = await _context.Xes
                .Where(x => x.TenXe.Contains(q) || x.HangXe.TenHang.Contains(q))
                .Select(x => new { x.MaXe, x.TenXe })
                .Take(5)
                .ToListAsync();
            return Json(results);
        }
    }
}
