using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyThueXe.Migrations
{
    /// <inheritdoc />
    public partial class AddPhanHoiAdminToDanhGia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DM_HANG_XE",
                columns: table => new
                {
                    MaHangXe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenHang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NuocSanXuat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DM_HANG_XE", x => x.MaHangXe);
                });

            migrationBuilder.CreateTable(
                name: "DM_LOAI_GIAY_TO",
                columns: table => new
                {
                    MaLoai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DM_LOAI_GIAY_TO", x => x.MaLoai);
                });

            migrationBuilder.CreateTable(
                name: "DM_PHONG_CACH",
                columns: table => new
                {
                    MaPhongCach = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhongCach = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DM_PHONG_CACH", x => x.MaPhongCach);
                });

            migrationBuilder.CreateTable(
                name: "DM_TINH_THANH",
                columns: table => new
                {
                    MaTinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaVung = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DM_TINH_THANH", x => x.MaTinh);
                });

            migrationBuilder.CreateTable(
                name: "QUYEN",
                columns: table => new
                {
                    MaQuyen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenQuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NhomQuyen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QUYEN", x => x.MaQuyen);
                });

            migrationBuilder.CreateTable(
                name: "VAI_TRO",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VAI_TRO", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "XE",
                columns: table => new
                {
                    MaXe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenXe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BienSo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaHangXe = table.Column<int>(type: "int", nullable: false),
                    MaPhongCach = table.Column<int>(type: "int", nullable: false),
                    GiaThueNgay = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    NamSanXuat = table.Column<int>(type: "int", nullable: false),
                    DungTich = table.Column<int>(type: "int", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DanhGiaTrungBinh = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    SoLanThue = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XE", x => x.MaXe);
                    table.ForeignKey(
                        name: "FK_XE_DM_HANG_XE_MaHangXe",
                        column: x => x.MaHangXe,
                        principalTable: "DM_HANG_XE",
                        principalColumn: "MaHangXe",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_XE_DM_PHONG_CACH_MaPhongCach",
                        column: x => x.MaPhongCach,
                        principalTable: "DM_PHONG_CACH",
                        principalColumn: "MaPhongCach",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NGUOI_DUNG",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MatKhauHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NGUOI_DUNG", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_NGUOI_DUNG_VAI_TRO_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VAI_TRO",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VAI_TRO_QUYEN",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    MaQuyen = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VAI_TRO_QUYEN", x => new { x.MaVaiTro, x.MaQuyen });
                    table.ForeignKey(
                        name: "FK_VAI_TRO_QUYEN_QUYEN_MaQuyen",
                        column: x => x.MaQuyen,
                        principalTable: "QUYEN",
                        principalColumn: "MaQuyen",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VAI_TRO_QUYEN_VAI_TRO_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VAI_TRO",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XE_HINH_ANH",
                columns: table => new
                {
                    MaHinhAnh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaXe = table.Column<int>(type: "int", nullable: false),
                    DuongDanAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsAnhChinh = table.Column<bool>(type: "bit", nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XE_HINH_ANH", x => x.MaHinhAnh);
                    table.ForeignKey(
                        name: "FK_XE_HINH_ANH_XE_MaXe",
                        column: x => x.MaXe,
                        principalTable: "XE",
                        principalColumn: "MaXe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BAO_DUONG",
                columns: table => new
                {
                    MaBaoDuong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaXe = table.Column<int>(type: "int", nullable: false),
                    LoaiBaoDuong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgayVao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayRa = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChiPhi = table.Column<decimal>(type: "decimal(12,0)", nullable: true),
                    DonViThucHien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaNguoiPhuTrach = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BAO_DUONG", x => x.MaBaoDuong);
                    table.ForeignKey(
                        name: "FK_BAO_DUONG_NGUOI_DUNG_MaNguoiPhuTrach",
                        column: x => x.MaNguoiPhuTrach,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_BAO_DUONG_XE_MaXe",
                        column: x => x.MaXe,
                        principalTable: "XE",
                        principalColumn: "MaXe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CAU_HINH_HE_THONG",
                columns: table => new
                {
                    MaCauHinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiaTri = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KieuDuLieu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiCapNhat = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAU_HINH_HE_THONG", x => x.MaCauHinh);
                    table.ForeignKey(
                        name: "FK_CAU_HINH_HE_THONG_NGUOI_DUNG_NguoiCapNhat",
                        column: x => x.NguoiCapNhat,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "KHACH_HANG",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SoGPLX = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaLoaiGiayTo = table.Column<int>(type: "int", nullable: false),
                    SoGiayTo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayCap = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoiCap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MaTinh = table.Column<int>(type: "int", nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KHACH_HANG", x => x.MaKhachHang);
                    table.ForeignKey(
                        name: "FK_KHACH_HANG_DM_LOAI_GIAY_TO_MaLoaiGiayTo",
                        column: x => x.MaLoaiGiayTo,
                        principalTable: "DM_LOAI_GIAY_TO",
                        principalColumn: "MaLoai",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KHACH_HANG_DM_TINH_THANH_MaTinh",
                        column: x => x.MaTinh,
                        principalTable: "DM_TINH_THANH",
                        principalColumn: "MaTinh");
                    table.ForeignKey(
                        name: "FK_KHACH_HANG_NGUOI_DUNG_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KHUYEN_MAI",
                columns: table => new
                {
                    MaKhuyenMai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChuongTrinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoaiGiamGia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GiaTriGiam = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    GiaTriGiamToiDa = table.Column<decimal>(type: "decimal(15,0)", nullable: true),
                    SoLanSuDung = table.Column<int>(type: "int", nullable: false),
                    DaSuDung = table.Column<int>(type: "int", nullable: false),
                    TuNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DenNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DieuKienToiThieu = table.Column<decimal>(type: "decimal(15,0)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaNguoiTao = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KHUYEN_MAI", x => x.MaKhuyenMai);
                    table.ForeignKey(
                        name: "FK_KHUYEN_MAI_NGUOI_DUNG_MaNguoiTao",
                        column: x => x.MaNguoiTao,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "PHU_PHI_MUA",
                columns: table => new
                {
                    MaPhuPhi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TuNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DenNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HeSoNhan = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaNguoiTao = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHU_PHI_MUA", x => x.MaPhuPhi);
                    table.ForeignKey(
                        name: "FK_PHU_PHI_MUA_NGUOI_DUNG_MaNguoiTao",
                        column: x => x.MaNguoiTao,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "HOP_DONG",
                columns: table => new
                {
                    MaHopDong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaXe = table.Column<int>(type: "int", nullable: false),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaNguoiTao = table.Column<int>(type: "int", nullable: false),
                    MaNguoiXacNhan = table.Column<int>(type: "int", nullable: true),
                    NgayThue = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTraThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GiaThueGoc = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    HeSoMua = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(15,0)", nullable: false),
                    PhuPhiTreHan = table.Column<decimal>(type: "decimal(15,0)", nullable: false),
                    MaKhuyenMai = table.Column<int>(type: "int", nullable: true),
                    SoTienGiam = table.Column<decimal>(type: "decimal(15,0)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LyDoHuy = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HOP_DONG", x => x.MaHopDong);
                    table.ForeignKey(
                        name: "FK_HOP_DONG_KHACH_HANG_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KHACH_HANG",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HOP_DONG_KHUYEN_MAI_MaKhuyenMai",
                        column: x => x.MaKhuyenMai,
                        principalTable: "KHUYEN_MAI",
                        principalColumn: "MaKhuyenMai");
                    table.ForeignKey(
                        name: "FK_HOP_DONG_NGUOI_DUNG_MaNguoiTao",
                        column: x => x.MaNguoiTao,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HOP_DONG_NGUOI_DUNG_MaNguoiXacNhan",
                        column: x => x.MaNguoiXacNhan,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HOP_DONG_XE_MaXe",
                        column: x => x.MaXe,
                        principalTable: "XE",
                        principalColumn: "MaXe",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DANH_GIA",
                columns: table => new
                {
                    MaDanhGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHopDong = table.Column<int>(type: "int", nullable: false),
                    MaXe = table.Column<int>(type: "int", nullable: false),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    SoSao = table.Column<int>(type: "int", nullable: false),
                    NhanXet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHienThi = table.Column<bool>(type: "bit", nullable: false),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhanHoiAdmin = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayPhanHoi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_GIA", x => x.MaDanhGia);
                    table.ForeignKey(
                        name: "FK_DANH_GIA_HOP_DONG_MaHopDong",
                        column: x => x.MaHopDong,
                        principalTable: "HOP_DONG",
                        principalColumn: "MaHopDong",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DANH_GIA_KHACH_HANG_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KHACH_HANG",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DANH_GIA_XE_MaXe",
                        column: x => x.MaXe,
                        principalTable: "XE",
                        principalColumn: "MaXe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HOP_DONG_TAI_LIEU",
                columns: table => new
                {
                    MaTaiLieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHopDong = table.Column<int>(type: "int", nullable: false),
                    TenTaiLieu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DuongDan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LoaiFile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HOP_DONG_TAI_LIEU", x => x.MaTaiLieu);
                    table.ForeignKey(
                        name: "FK_HOP_DONG_TAI_LIEU_HOP_DONG_MaHopDong",
                        column: x => x.MaHopDong,
                        principalTable: "HOP_DONG",
                        principalColumn: "MaHopDong",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LICH_SU_KHUYEN_MAI",
                columns: table => new
                {
                    MaLichSu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhuyenMai = table.Column<int>(type: "int", nullable: false),
                    MaHopDong = table.Column<int>(type: "int", nullable: false),
                    SoTienDuocGiam = table.Column<decimal>(type: "decimal(15,0)", nullable: false),
                    ThoiGianApDung = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LICH_SU_KHUYEN_MAI", x => x.MaLichSu);
                    table.ForeignKey(
                        name: "FK_LICH_SU_KHUYEN_MAI_HOP_DONG_MaHopDong",
                        column: x => x.MaHopDong,
                        principalTable: "HOP_DONG",
                        principalColumn: "MaHopDong",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LICH_SU_KHUYEN_MAI_KHUYEN_MAI_MaKhuyenMai",
                        column: x => x.MaKhuyenMai,
                        principalTable: "KHUYEN_MAI",
                        principalColumn: "MaKhuyenMai",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "THANH_TOAN",
                columns: table => new
                {
                    MaThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHopDong = table.Column<int>(type: "int", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(15,0)", nullable: false),
                    PhuongThuc = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LoaiThanhToan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaNguoiNhanTien = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaGiaoDich = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THANH_TOAN", x => x.MaThanhToan);
                    table.ForeignKey(
                        name: "FK_THANH_TOAN_HOP_DONG_MaHopDong",
                        column: x => x.MaHopDong,
                        principalTable: "HOP_DONG",
                        principalColumn: "MaHopDong",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_THANH_TOAN_NGUOI_DUNG_MaNguoiNhanTien",
                        column: x => x.MaNguoiNhanTien,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "THONG_BAO",
                columns: table => new
                {
                    MaThongBao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiThongBao = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MaNguoiNhan = table.Column<int>(type: "int", nullable: true),
                    IsDoc = table.Column<bool>(type: "bit", nullable: false),
                    MaHopDongLienQuan = table.Column<int>(type: "int", nullable: true),
                    MaXeLienQuan = table.Column<int>(type: "int", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THONG_BAO", x => x.MaThongBao);
                    table.ForeignKey(
                        name: "FK_THONG_BAO_HOP_DONG_MaHopDongLienQuan",
                        column: x => x.MaHopDongLienQuan,
                        principalTable: "HOP_DONG",
                        principalColumn: "MaHopDong");
                    table.ForeignKey(
                        name: "FK_THONG_BAO_NGUOI_DUNG_MaNguoiNhan",
                        column: x => x.MaNguoiNhan,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_THONG_BAO_XE_MaXeLienQuan",
                        column: x => x.MaXeLienQuan,
                        principalTable: "XE",
                        principalColumn: "MaXe");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BAO_DUONG_MaNguoiPhuTrach",
                table: "BAO_DUONG",
                column: "MaNguoiPhuTrach");

            migrationBuilder.CreateIndex(
                name: "IX_BAO_DUONG_MaXe",
                table: "BAO_DUONG",
                column: "MaXe");

            migrationBuilder.CreateIndex(
                name: "IX_CAU_HINH_HE_THONG_NguoiCapNhat",
                table: "CAU_HINH_HE_THONG",
                column: "NguoiCapNhat");

            migrationBuilder.CreateIndex(
                name: "IX_CAU_HINH_HE_THONG_TenKhoa",
                table: "CAU_HINH_HE_THONG",
                column: "TenKhoa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_MaHopDong",
                table: "DANH_GIA",
                column: "MaHopDong",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_MaKhachHang",
                table: "DANH_GIA",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_MaXe",
                table: "DANH_GIA",
                column: "MaXe");

            migrationBuilder.CreateIndex(
                name: "IX_HOP_DONG_MaKhachHang",
                table: "HOP_DONG",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_HOP_DONG_MaKhuyenMai",
                table: "HOP_DONG",
                column: "MaKhuyenMai");

            migrationBuilder.CreateIndex(
                name: "IX_HOP_DONG_MaNguoiTao",
                table: "HOP_DONG",
                column: "MaNguoiTao");

            migrationBuilder.CreateIndex(
                name: "IX_HOP_DONG_MaNguoiXacNhan",
                table: "HOP_DONG",
                column: "MaNguoiXacNhan");

            migrationBuilder.CreateIndex(
                name: "IX_HOP_DONG_MaXe",
                table: "HOP_DONG",
                column: "MaXe");

            migrationBuilder.CreateIndex(
                name: "IX_HOP_DONG_TAI_LIEU_MaHopDong",
                table: "HOP_DONG_TAI_LIEU",
                column: "MaHopDong");

            migrationBuilder.CreateIndex(
                name: "IX_KHACH_HANG_MaLoaiGiayTo",
                table: "KHACH_HANG",
                column: "MaLoaiGiayTo");

            migrationBuilder.CreateIndex(
                name: "IX_KHACH_HANG_MaNguoiDung",
                table: "KHACH_HANG",
                column: "MaNguoiDung",
                unique: true,
                filter: "[MaNguoiDung] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KHACH_HANG_MaTinh",
                table: "KHACH_HANG",
                column: "MaTinh");

            migrationBuilder.CreateIndex(
                name: "IX_KHACH_HANG_SoGiayTo",
                table: "KHACH_HANG",
                column: "SoGiayTo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KHACH_HANG_SoGPLX",
                table: "KHACH_HANG",
                column: "SoGPLX",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KHUYEN_MAI_MaCode",
                table: "KHUYEN_MAI",
                column: "MaCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KHUYEN_MAI_MaNguoiTao",
                table: "KHUYEN_MAI",
                column: "MaNguoiTao");

            migrationBuilder.CreateIndex(
                name: "IX_LICH_SU_KHUYEN_MAI_MaHopDong",
                table: "LICH_SU_KHUYEN_MAI",
                column: "MaHopDong");

            migrationBuilder.CreateIndex(
                name: "IX_LICH_SU_KHUYEN_MAI_MaKhuyenMai",
                table: "LICH_SU_KHUYEN_MAI",
                column: "MaKhuyenMai");

            migrationBuilder.CreateIndex(
                name: "IX_NGUOI_DUNG_Email",
                table: "NGUOI_DUNG",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NGUOI_DUNG_MaVaiTro",
                table: "NGUOI_DUNG",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_PHU_PHI_MUA_MaNguoiTao",
                table: "PHU_PHI_MUA",
                column: "MaNguoiTao");

            migrationBuilder.CreateIndex(
                name: "IX_QUYEN_TenQuyen",
                table: "QUYEN",
                column: "TenQuyen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_THANH_TOAN_MaHopDong",
                table: "THANH_TOAN",
                column: "MaHopDong");

            migrationBuilder.CreateIndex(
                name: "IX_THANH_TOAN_MaNguoiNhanTien",
                table: "THANH_TOAN",
                column: "MaNguoiNhanTien");

            migrationBuilder.CreateIndex(
                name: "IX_THONG_BAO_MaHopDongLienQuan",
                table: "THONG_BAO",
                column: "MaHopDongLienQuan");

            migrationBuilder.CreateIndex(
                name: "IX_THONG_BAO_MaNguoiNhan",
                table: "THONG_BAO",
                column: "MaNguoiNhan");

            migrationBuilder.CreateIndex(
                name: "IX_THONG_BAO_MaXeLienQuan",
                table: "THONG_BAO",
                column: "MaXeLienQuan");

            migrationBuilder.CreateIndex(
                name: "IX_VAI_TRO_TenVaiTro",
                table: "VAI_TRO",
                column: "TenVaiTro",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VAI_TRO_QUYEN_MaQuyen",
                table: "VAI_TRO_QUYEN",
                column: "MaQuyen");

            migrationBuilder.CreateIndex(
                name: "IX_XE_BienSo",
                table: "XE",
                column: "BienSo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_XE_MaHangXe",
                table: "XE",
                column: "MaHangXe");

            migrationBuilder.CreateIndex(
                name: "IX_XE_MaPhongCach",
                table: "XE",
                column: "MaPhongCach");

            migrationBuilder.CreateIndex(
                name: "IX_XE_HINH_ANH_MaXe",
                table: "XE_HINH_ANH",
                column: "MaXe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BAO_DUONG");

            migrationBuilder.DropTable(
                name: "CAU_HINH_HE_THONG");

            migrationBuilder.DropTable(
                name: "DANH_GIA");

            migrationBuilder.DropTable(
                name: "HOP_DONG_TAI_LIEU");

            migrationBuilder.DropTable(
                name: "LICH_SU_KHUYEN_MAI");

            migrationBuilder.DropTable(
                name: "PHU_PHI_MUA");

            migrationBuilder.DropTable(
                name: "THANH_TOAN");

            migrationBuilder.DropTable(
                name: "THONG_BAO");

            migrationBuilder.DropTable(
                name: "VAI_TRO_QUYEN");

            migrationBuilder.DropTable(
                name: "XE_HINH_ANH");

            migrationBuilder.DropTable(
                name: "HOP_DONG");

            migrationBuilder.DropTable(
                name: "QUYEN");

            migrationBuilder.DropTable(
                name: "KHACH_HANG");

            migrationBuilder.DropTable(
                name: "KHUYEN_MAI");

            migrationBuilder.DropTable(
                name: "XE");

            migrationBuilder.DropTable(
                name: "DM_LOAI_GIAY_TO");

            migrationBuilder.DropTable(
                name: "DM_TINH_THANH");

            migrationBuilder.DropTable(
                name: "NGUOI_DUNG");

            migrationBuilder.DropTable(
                name: "DM_HANG_XE");

            migrationBuilder.DropTable(
                name: "DM_PHONG_CACH");

            migrationBuilder.DropTable(
                name: "VAI_TRO");
        }
    }
}
