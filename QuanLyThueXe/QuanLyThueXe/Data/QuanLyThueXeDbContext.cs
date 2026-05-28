using Microsoft.EntityFrameworkCore;
using QuanLyThueXe.Models;

namespace QuanLyThueXe.Data
{
    public class QuanLyThueXeDbContext : DbContext
    {
        public QuanLyThueXeDbContext(DbContextOptions<QuanLyThueXeDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<Quyen> Quyens { get; set; }
        public DbSet<VaiTroQuyen> VaiTroQuyens { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<Xe> Xes { get; set; }
        public DbSet<XeHinhAnh> XeHinhAnhs { get; set; }
        public DbSet<DmHangXe> DmHangXes { get; set; }
        public DbSet<DmPhongCach> DmPhongCachs { get; set; }
        public DbSet<DmTinhThanh> DmTinhThanhs { get; set; }
        public DbSet<DmLoaiGiayTo> DmLoaiGiayTos { get; set; }
        public DbSet<HopDong> HopDongs { get; set; }
        public DbSet<HopDongTaiLieu> HopDongTaiLieus { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<BaoDuong> BaoDuongs { get; set; }
        public DbSet<ThongBao> ThongBaos { get; set; }
        public DbSet<KhuyenMai> KhuyenMais { get; set; }
        public DbSet<LichSuKhuyenMai> LichSuKhuyenMais { get; set; }
        public DbSet<PhuPhiMua> PhuPhiMuas { get; set; }
        public DbSet<CauHinhHeThong> CauHinhHeThongs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite key for VaiTroQuyen
            modelBuilder.Entity<VaiTroQuyen>()
                .HasKey(vq => new { vq.MaVaiTro, vq.MaQuyen });

            // Configure relationships
            modelBuilder.Entity<NguoiDung>()
                .HasOne(n => n.VaiTro)
                .WithMany(v => v.NguoiDungs)
                .HasForeignKey(n => n.MaVaiTro)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NguoiDung>()
                .HasOne(n => n.KhachHang)
                .WithOne(k => k.NguoiDung)
                .HasForeignKey<KhachHang>(k => k.MaNguoiDung)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.NguoiTao)
                .WithMany(n => n.HopDongsTao)
                .HasForeignKey(h => h.MaNguoiTao)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.NguoiXacNhan)
                .WithMany(n => n.HopDongsXacNhan)
                .HasForeignKey(h => h.MaNguoiXacNhan)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.Xe)
                .WithMany(x => x.HopDongs)
                .HasForeignKey(h => h.MaXe)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.KhachHang)
                .WithMany(k => k.HopDongs)
                .HasForeignKey(h => h.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DanhGia>()
                .HasOne(d => d.HopDong)
                .WithOne(h => h.DanhGia)
                .HasForeignKey<DanhGia>(d => d.MaHopDong)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure unique indexes
            modelBuilder.Entity<NguoiDung>()
                .HasIndex(n => n.Email)
                .IsUnique();

            modelBuilder.Entity<Xe>()
                .HasIndex(x => x.BienSo)
                .IsUnique();

            modelBuilder.Entity<KhachHang>()
                .HasIndex(k => k.SoGPLX)
                .IsUnique();

            modelBuilder.Entity<KhachHang>()
                .HasIndex(k => k.SoGiayTo)
                .IsUnique();

            modelBuilder.Entity<VaiTro>()
                .HasIndex(v => v.TenVaiTro)
                .IsUnique();

            modelBuilder.Entity<Quyen>()
                .HasIndex(q => q.TenQuyen)
                .IsUnique();

            modelBuilder.Entity<KhuyenMai>()
                .HasIndex(k => k.MaCode)
                .IsUnique();

            modelBuilder.Entity<CauHinhHeThong>()
                .HasIndex(c => c.TenKhoa)
                .IsUnique();
        }
    }
}
