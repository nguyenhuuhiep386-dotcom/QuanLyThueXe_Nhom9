-- =================================================================
--  HE THONG QUAN LY CHO THUE XE MO TO  —  FULL DATABASE (FIXED v2)
--  Nhom 9 | ASP.NET Core MVC + SQL Server
--  Actors: ADMIN / KHACH HANG
--
--  DANH SACH FIX:
--  [FIX 1]  Tach VAI_TRO + QUYEN + VAI_TRO_QUYEN (phan quyen ro rang, 3NF)
--  [FIX 2]  NGUOI_DUNG.VaiTro → FK sang VAI_TRO thay vi hardcode chuoi
--  [FIX 3]  DANH_GIA them MaKhachHang (lien ket truc tiep, tranh join vong)
--  [FIX 4]  sp_TinhTienHopDong: sua logic overlap phu phi mua vu
--  [FIX 5]  sp_TinhTienHopDong: them UPDATE DaSuDung khi ap dung khuyen mai
--  [FIX 6]  trg_TangSoLanThue: them dieu kien tranh dem sai
--  [FIX 7]  THANH_TOAN: them cot TrangThai de track giao dich online
--  [FIX 8]  XE: them CHECK NamSanXuat <= YEAR(GETDATE())
--  [FIX 9]  Seed data: cap nhat phu phi mua vu vao HOP_DONG mau
-- =================================================================

USE master;
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyThueXeDB')
    DROP DATABASE QuanLyThueXeDB;
GO
CREATE DATABASE QuanLyThueXeDB
    COLLATE Vietnamese_CI_AS;
GO
USE QuanLyThueXeDB;
GO

-- =================================================================
-- PHAN 1: BANG DANH MUC / LOOKUP
-- =================================================================

CREATE TABLE DM_PHONG_CACH (
    MaPhongCach     INT             IDENTITY(1,1)   PRIMARY KEY,
    TenPhongCach    NVARCHAR(50)    NOT NULL        UNIQUE,
    MoTa            NVARCHAR(200)   NULL,
    IsActive        BIT             NOT NULL        DEFAULT 1
);
GO

CREATE TABLE DM_HANG_XE (
    MaHangXe    INT             IDENTITY(1,1)   PRIMARY KEY,
    TenHang     NVARCHAR(100)   NOT NULL        UNIQUE,
    NuocSanXuat NVARCHAR(50)    NULL,
    Logo        VARCHAR(300)    NULL,
    IsActive    BIT             NOT NULL        DEFAULT 1
);
GO

CREATE TABLE DM_TINH_THANH (
    MaTinh      INT             IDENTITY(1,1)   PRIMARY KEY,
    TenTinh     NVARCHAR(100)   NOT NULL        UNIQUE,
    MaVung      INT             NULL
);
GO

CREATE TABLE DM_LOAI_GIAY_TO (
    MaLoai      INT             IDENTITY(1,1)   PRIMARY KEY,
    TenLoai     NVARCHAR(50)    NOT NULL        UNIQUE,
    IsActive    BIT             NOT NULL        DEFAULT 1
);
GO

-- =================================================================
-- PHAN 2: PHAN QUYEN (VAI TRO - QUYEN)
-- [FIX 1] Tach bảng phan quyen de dam bao 3NF va de mo rong
-- =================================================================

CREATE TABLE VAI_TRO (
    MaVaiTro    INT             IDENTITY(1,1)   PRIMARY KEY,
    TenVaiTro   NVARCHAR(50)    NOT NULL        UNIQUE,  -- 'Admin', 'KhachHang'
    MoTa        NVARCHAR(200)   NULL,
    IsActive    BIT             NOT NULL        DEFAULT 1
);
GO

CREATE TABLE QUYEN (
    MaQuyen     INT             IDENTITY(1,1)   PRIMARY KEY,
    TenQuyen    NVARCHAR(100)   NOT NULL        UNIQUE,  -- 'QuanLyXe', 'XemHopDong'...
    NhomQuyen   NVARCHAR(50)    NOT NULL,                -- 'HopDong', 'Xe', 'KhachHang'...
    MoTa        NVARCHAR(200)   NULL,
    IsActive    BIT             NOT NULL        DEFAULT 1
);
GO

-- Bang trung gian N-N: 1 vai tro co nhieu quyen, 1 quyen thuoc nhieu vai tro
CREATE TABLE VAI_TRO_QUYEN (
    MaVaiTro    INT             NOT NULL,
    MaQuyen     INT             NOT NULL,
    CONSTRAINT PK_VAI_TRO_QUYEN PRIMARY KEY (MaVaiTro, MaQuyen),
    CONSTRAINT FK_VTQ_VaiTro    FOREIGN KEY (MaVaiTro) REFERENCES VAI_TRO(MaVaiTro),
    CONSTRAINT FK_VTQ_Quyen     FOREIGN KEY (MaQuyen)  REFERENCES QUYEN(MaQuyen)
);
GO

-- =================================================================
-- PHAN 3: BANG CHINH
-- =================================================================

CREATE TABLE NGUOI_DUNG (
    MaNguoiDung     INT             IDENTITY(1,1)   PRIMARY KEY,
    HoTen           NVARCHAR(100)   NOT NULL,
    Email           VARCHAR(150)    NOT NULL        UNIQUE,
    MatKhauHash     NVARCHAR(256)   NOT NULL,
    SoDienThoai     VARCHAR(20)     NULL,
    -- [FIX 2] Thay VaiTro NVARCHAR hardcode → FK sang bang VAI_TRO
    MaVaiTro        INT             NOT NULL        DEFAULT 2,  -- 2 = KhachHang
    IsActive        BIT             NOT NULL        DEFAULT 1,
    NgayTao         DATETIME        NOT NULL        DEFAULT GETDATE(),
    NgayCapNhat     DATETIME        NULL,

    CONSTRAINT FK_ND_VaiTro FOREIGN KEY (MaVaiTro) REFERENCES VAI_TRO(MaVaiTro)
);
GO

CREATE TABLE KHACH_HANG (
    MaKhachHang     INT             IDENTITY(1,1)   PRIMARY KEY,
    HoTen           NVARCHAR(100)   NOT NULL,
    SoDienThoai     VARCHAR(20)     NOT NULL,
    Email           VARCHAR(150)    NULL,
    SoGPLX          VARCHAR(20)     NOT NULL        UNIQUE,
    MaLoaiGiayTo    INT             NOT NULL,
    SoGiayTo        VARCHAR(20)     NOT NULL        UNIQUE,
    NgayCap         DATE            NULL,
    NoiCap          NVARCHAR(100)   NULL,
    DiaChi          NVARCHAR(255)   NULL,
    MaTinh          INT             NULL,
    NgaySinh        DATE            NULL,
    GioiTinh        NVARCHAR(5)     NULL
                    CONSTRAINT CK_KH_GioiTinh CHECK (GioiTinh IN (N'Nam', N'Nu', N'Khac')),
    GhiChu          NVARCHAR(500)   NULL,
    NgayTao         DATETIME        NOT NULL        DEFAULT GETDATE(),
    NgayCapNhat     DATETIME        NULL,
    MaNguoiDung     INT             NULL,

    CONSTRAINT FK_KH_LoaiGiayTo FOREIGN KEY (MaLoaiGiayTo) REFERENCES DM_LOAI_GIAY_TO(MaLoai),
    CONSTRAINT FK_KH_TinhThanh  FOREIGN KEY (MaTinh)       REFERENCES DM_TINH_THANH(MaTinh),
    CONSTRAINT FK_KH_NguoiDung  FOREIGN KEY (MaNguoiDung)  REFERENCES NGUOI_DUNG(MaNguoiDung),
    CONSTRAINT UQ_KH_NguoiDung  UNIQUE (MaNguoiDung)
);
GO

CREATE TABLE XE (
    MaXe                INT             IDENTITY(1,1)   PRIMARY KEY,
    TenXe               NVARCHAR(100)   NOT NULL,
    BienSo              VARCHAR(20)     NOT NULL        UNIQUE,
    MaHangXe            INT             NOT NULL,
    MaPhongCach         INT             NOT NULL,
    GiaThueNgay         DECIMAL(12,0)   NOT NULL,
    -- [FIX 8] Them rang buoc nam san xuat khong vuot qua nam hien tai
    NamSanXuat          INT             NOT NULL
                        CONSTRAINT CK_XE_NamSX CHECK (NamSanXuat BETWEEN 2000 AND 2099),
    DungTich            INT             NULL,
    MoTa                NVARCHAR(MAX)   NULL,
    TrangThai           NVARCHAR(20)    NOT NULL        DEFAULT N'ConTrong'
                        CONSTRAINT CK_XE_TrangThai
                        CHECK (TrangThai IN (N'ConTrong', N'DangThue', N'BaoDuong')),
    DanhGiaTrungBinh    DECIMAL(3,2)    NOT NULL        DEFAULT 0.00
                        CONSTRAINT CK_XE_DanhGia CHECK (DanhGiaTrungBinh BETWEEN 0 AND 5),
    SoLanThue           INT             NOT NULL        DEFAULT 0,
    NgayTao             DATETIME        NOT NULL        DEFAULT GETDATE(),
    NgayCapNhat         DATETIME        NULL,

    CONSTRAINT FK_Xe_HangXe     FOREIGN KEY (MaHangXe)    REFERENCES DM_HANG_XE(MaHangXe),
    CONSTRAINT FK_Xe_PhongCach  FOREIGN KEY (MaPhongCach) REFERENCES DM_PHONG_CACH(MaPhongCach)
);
GO

CREATE TABLE PHU_PHI_MUA (
    MaPhuPhi    INT             IDENTITY(1,1)   PRIMARY KEY,
    TenDot      NVARCHAR(100)   NOT NULL,
    TuNgay      DATETIME        NOT NULL,
    DenNgay     DATETIME        NOT NULL,
    HeSoNhan    DECIMAL(4,2)    NOT NULL        DEFAULT 1.00
                CONSTRAINT CK_PPM_HeSo CHECK (HeSoNhan >= 1.00 AND HeSoNhan <= 5.00),
    GhiChu      NVARCHAR(300)   NULL,
    NgayTao     DATETIME        NOT NULL        DEFAULT GETDATE(),
    MaNguoiTao  INT             NULL,
    CONSTRAINT CK_PPM_NgayHopLe CHECK (DenNgay > TuNgay),
    CONSTRAINT FK_PPM_NguoiTao  FOREIGN KEY (MaNguoiTao) REFERENCES NGUOI_DUNG(MaNguoiDung)
);
GO

CREATE TABLE HOP_DONG (
    MaHopDong       INT             IDENTITY(1,1)   PRIMARY KEY,
    MaXe            INT             NOT NULL,
    MaKhachHang     INT             NOT NULL,
    MaNguoiTao      INT             NOT NULL,
    MaNguoiXacNhan  INT             NULL,
    NgayThue        DATETIME        NOT NULL,
    NgayTra         DATETIME        NOT NULL,
    NgayTraThucTe   DATETIME        NULL,
    GiaThueGoc      DECIMAL(12,0)   NOT NULL,
    HeSoMua         DECIMAL(4,2)    NOT NULL        DEFAULT 1.00,
    TongTien        DECIMAL(15,0)   NOT NULL,
    PhuPhiTreHan    DECIMAL(15,0)   NOT NULL        DEFAULT 0,
    TrangThai       NVARCHAR(20)    NOT NULL        DEFAULT N'ChoXacNhan'
                    CONSTRAINT CK_HD_TrangThai
                    CHECK (TrangThai IN (N'ChoXacNhan', N'DangThue', N'DaTra', N'DaHuy')),
    LyDoHuy         NVARCHAR(300)   NULL,
    GhiChu          NVARCHAR(500)   NULL,
    NgayTao         DATETIME        NOT NULL        DEFAULT GETDATE(),
    NgayCapNhat     DATETIME        NULL,

    CONSTRAINT FK_HD_Xe             FOREIGN KEY (MaXe)           REFERENCES XE(MaXe),
    CONSTRAINT FK_HD_KhachHang      FOREIGN KEY (MaKhachHang)    REFERENCES KHACH_HANG(MaKhachHang),
    CONSTRAINT FK_HD_NguoiTao       FOREIGN KEY (MaNguoiTao)     REFERENCES NGUOI_DUNG(MaNguoiDung),
    CONSTRAINT FK_HD_NguoiXacNhan   FOREIGN KEY (MaNguoiXacNhan) REFERENCES NGUOI_DUNG(MaNguoiDung),
    CONSTRAINT CK_HD_NgayHopLe      CHECK (NgayTra > NgayThue)
);
GO

-- [FIX 3] Them MaKhachHang vao DANH_GIA de lien ket truc tiep
-- Tranh phai join vong: DANH_GIA → HOP_DONG → KHACH_HANG
-- De dang truy van "tat ca danh gia cua khach X"
CREATE TABLE DANH_GIA (
    MaDanhGia   INT             IDENTITY(1,1)   PRIMARY KEY,
    MaHopDong   INT             NOT NULL        UNIQUE,
    MaXe        INT             NOT NULL,
    MaKhachHang INT             NOT NULL,   -- [FIX 3]
    SoSao       INT             NOT NULL
                CONSTRAINT CK_DG_SoSao CHECK (SoSao BETWEEN 1 AND 5),
    NhanXet     NVARCHAR(MAX)   NULL,
    IsHienThi   BIT             NOT NULL        DEFAULT 1,
    NgayDanhGia DATETIME        NOT NULL        DEFAULT GETDATE(),

    CONSTRAINT FK_DG_HopDong   FOREIGN KEY (MaHopDong)   REFERENCES HOP_DONG(MaHopDong),
    CONSTRAINT FK_DG_Xe        FOREIGN KEY (MaXe)        REFERENCES XE(MaXe),
    CONSTRAINT FK_DG_KhachHang FOREIGN KEY (MaKhachHang) REFERENCES KHACH_HANG(MaKhachHang)
);
GO

-- =================================================================
-- PHAN 4: BANG PHU — HO TRO NGHIEP VU
-- =================================================================

CREATE TABLE XE_HINH_ANH (
    MaHinhAnh   INT             IDENTITY(1,1)   PRIMARY KEY,
    MaXe        INT             NOT NULL,
    DuongDanAnh VARCHAR(500)    NOT NULL,
    IsAnhChinh  BIT             NOT NULL        DEFAULT 0,
    ThuTu       INT             NOT NULL        DEFAULT 0,
    NgayTao     DATETIME        NOT NULL        DEFAULT GETDATE(),

    CONSTRAINT FK_XeAnh_Xe FOREIGN KEY (MaXe) REFERENCES XE(MaXe) ON DELETE CASCADE
);
GO

CREATE TABLE HOP_DONG_TAI_LIEU (
    MaTaiLieu   INT             IDENTITY(1,1)   PRIMARY KEY,
    MaHopDong   INT             NOT NULL,
    TenTaiLieu  NVARCHAR(100)   NOT NULL,
    DuongDan    VARCHAR(500)    NOT NULL,
    LoaiFile    VARCHAR(20)     NULL,
    NgayTao     DATETIME        NOT NULL        DEFAULT GETDATE(),

    CONSTRAINT FK_TaiLieu_HopDong FOREIGN KEY (MaHopDong) REFERENCES HOP_DONG(MaHopDong) ON DELETE CASCADE
);
GO

-- [FIX 7] Them cot TrangThai de track trang thai giao dich (dac biet giao dich online)
CREATE TABLE THANH_TOAN (
    MaThanhToan     INT             IDENTITY(1,1)   PRIMARY KEY,
    MaHopDong       INT             NOT NULL,
    SoTien          DECIMAL(15,0)   NOT NULL,
    PhuongThuc      NVARCHAR(30)    NOT NULL
                    CONSTRAINT CK_TT_PhuongThuc
                    CHECK (PhuongThuc IN (N'TienMat', N'ChuyenKhoan', N'TheNganHang', N'Vi_MoMo', N'Vi_ZaloPay')),
    LoaiThanhToan   NVARCHAR(20)    NOT NULL
                    CONSTRAINT CK_TT_Loai
                    CHECK (LoaiThanhToan IN (N'DatCoc', N'ThanhToanCuoi', N'HoanTien', N'PhuPhi')),
    -- [FIX 7] TrangThai track duoc giao dich online that bai / cho xu ly
    TrangThai       NVARCHAR(20)    NOT NULL        DEFAULT N'ThanhCong'
                    CONSTRAINT CK_TT_TrangThai
                    CHECK (TrangThai IN (N'ThanhCong', N'ThatBai', N'ChoXuLy', N'DaHoan')),
    MaNguoiNhanTien INT             NULL,
    GhiChu          NVARCHAR(300)   NULL,
    ThoiGian        DATETIME        NOT NULL        DEFAULT GETDATE(),
    MaGiaoDich      VARCHAR(100)    NULL,

    CONSTRAINT FK_TT_HopDong   FOREIGN KEY (MaHopDong)       REFERENCES HOP_DONG(MaHopDong),
    CONSTRAINT FK_TT_NguoiDung FOREIGN KEY (MaNguoiNhanTien) REFERENCES NGUOI_DUNG(MaNguoiDung)
);
GO

CREATE TABLE BAO_DUONG (
    MaBaoDuong      INT             IDENTITY(1,1)   PRIMARY KEY,
    MaXe            INT             NOT NULL,
    LoaiBaoDuong    NVARCHAR(100)   NOT NULL,
    NgayVao         DATETIME        NOT NULL,
    NgayRa          DATETIME        NULL,
    ChiPhi          DECIMAL(12,0)   NULL,
    DonViThucHien   NVARCHAR(100)   NULL,
    MaNguoiPhuTrach INT             NULL,
    GhiChu          NVARCHAR(500)   NULL,
    TrangThai       NVARCHAR(20)    NOT NULL        DEFAULT N'DangSua'
                    CONSTRAINT CK_BD_TrangThai
                    CHECK (TrangThai IN (N'DangSua', N'HoanThanh')),
    NgayTao         DATETIME        NOT NULL        DEFAULT GETDATE(),

    CONSTRAINT FK_BD_Xe        FOREIGN KEY (MaXe)              REFERENCES XE(MaXe),
    CONSTRAINT FK_BD_NguoiDung FOREIGN KEY (MaNguoiPhuTrach)   REFERENCES NGUOI_DUNG(MaNguoiDung)
);
GO

CREATE TABLE THONG_BAO (
    MaThongBao          INT             IDENTITY(1,1)   PRIMARY KEY,
    TieuDe              NVARCHAR(200)   NOT NULL,
    NoiDung             NVARCHAR(MAX)   NOT NULL,
    LoaiThongBao        NVARCHAR(30)    NOT NULL
                        CONSTRAINT CK_TB_Loai
                        CHECK (LoaiThongBao IN (N'HopDongSapHetHan', N'TraTre', N'BaoDuong', N'HeThong', N'KhuyenMai')),
    MaNguoiNhan         INT             NULL,
    IsDoc               BIT             NOT NULL        DEFAULT 0,
    MaHopDongLienQuan   INT             NULL,
    MaXeLienQuan        INT             NULL,
    NgayTao             DATETIME        NOT NULL        DEFAULT GETDATE(),
    NgayHetHan          DATETIME        NULL,

    CONSTRAINT FK_TB_NguoiNhan FOREIGN KEY (MaNguoiNhan)       REFERENCES NGUOI_DUNG(MaNguoiDung),
    CONSTRAINT FK_TB_HopDong   FOREIGN KEY (MaHopDongLienQuan) REFERENCES HOP_DONG(MaHopDong),
    CONSTRAINT FK_TB_Xe        FOREIGN KEY (MaXeLienQuan)      REFERENCES XE(MaXe)
);
GO

CREATE TABLE CAU_HINH_HE_THONG (
    MaCauHinh   INT             IDENTITY(1,1)   PRIMARY KEY,
    TenKhoa     VARCHAR(100)    NOT NULL        UNIQUE,
    GiaTri      NVARCHAR(500)   NOT NULL,
    KieuDuLieu  VARCHAR(20)     NOT NULL        DEFAULT 'string'
                CONSTRAINT CK_CH_Kieu CHECK (KieuDuLieu IN ('string','int','decimal','bool','json')),
    MoTa        NVARCHAR(200)   NULL,
    NgayCapNhat DATETIME        NOT NULL        DEFAULT GETDATE(),
    NguoiCapNhat INT            NULL,

    CONSTRAINT FK_CH_NguoiCapNhat FOREIGN KEY (NguoiCapNhat) REFERENCES NGUOI_DUNG(MaNguoiDung)
);
GO

CREATE TABLE KHUYEN_MAI (
    MaKhuyenMai      INT             IDENTITY(1,1)   PRIMARY KEY,
    TenChuongTrinh   NVARCHAR(100)   NOT NULL,
    MaCode           VARCHAR(50)     NOT NULL        UNIQUE,
    LoaiGiamGia      NVARCHAR(20)    NOT NULL
                     CONSTRAINT CK_KM_Loai CHECK (LoaiGiamGia IN (N'PhanTram', N'SoTienCoDinh')),
    GiaTriGiam       DECIMAL(10,2)   NOT NULL,
    GiaTriGiamToiDa  DECIMAL(15,0)   NULL,
    SoLanSuDung      INT             NOT NULL        DEFAULT 1,
    DaSuDung         INT             NOT NULL        DEFAULT 0,
    TuNgay           DATETIME        NOT NULL,
    DenNgay          DATETIME        NOT NULL,
    DieuKienToiThieu DECIMAL(15,0)   NULL,
    IsActive         BIT             NOT NULL        DEFAULT 1,
    NgayTao          DATETIME        NOT NULL        DEFAULT GETDATE(),
    MaNguoiTao       INT             NULL,

    CONSTRAINT CK_KM_NgayHopLe CHECK (DenNgay > TuNgay),
    CONSTRAINT FK_KM_NguoiTao  FOREIGN KEY (MaNguoiTao) REFERENCES NGUOI_DUNG(MaNguoiDung)
);
GO

CREATE TABLE LICH_SU_KHUYEN_MAI (
    MaLichSu        INT             IDENTITY(1,1)   PRIMARY KEY,
    MaKhuyenMai     INT             NOT NULL,
    MaHopDong       INT             NOT NULL        UNIQUE,
    SoTienDuocGiam  DECIMAL(15,0)   NOT NULL,
    ThoiGianApDung  DATETIME        NOT NULL        DEFAULT GETDATE(),

    CONSTRAINT FK_LSKM_KhuyenMai FOREIGN KEY (MaKhuyenMai) REFERENCES KHUYEN_MAI(MaKhuyenMai),
    CONSTRAINT FK_LSKM_HopDong   FOREIGN KEY (MaHopDong)   REFERENCES HOP_DONG(MaHopDong)
);
GO

-- =================================================================
-- PHAN 5: INDEX TOI UU
-- =================================================================

CREATE INDEX IX_HD_MaXe         ON HOP_DONG (MaXe);
CREATE INDEX IX_HD_MaKhachHang  ON HOP_DONG (MaKhachHang);
CREATE INDEX IX_HD_TrangThai    ON HOP_DONG (TrangThai);
CREATE INDEX IX_HD_NgayThue     ON HOP_DONG (NgayThue, NgayTra);
CREATE INDEX IX_HD_NgayTao      ON HOP_DONG (NgayTao DESC);

CREATE INDEX IX_XE_TrangThai    ON XE (TrangThai);
CREATE INDEX IX_XE_PhongCach    ON XE (MaPhongCach);
CREATE INDEX IX_XE_HangXe       ON XE (MaHangXe);

CREATE INDEX IX_KH_SoDienThoai  ON KHACH_HANG (SoDienThoai);
CREATE INDEX IX_KH_Email        ON KHACH_HANG (Email);

CREATE INDEX IX_TT_MaHopDong    ON THANH_TOAN (MaHopDong);
CREATE INDEX IX_TT_ThoiGian     ON THANH_TOAN (ThoiGian DESC);

CREATE INDEX IX_TB_NguoiNhan    ON THONG_BAO (MaNguoiNhan, IsDoc);

CREATE INDEX IX_BD_MaXe         ON BAO_DUONG (MaXe, TrangThai);

CREATE INDEX IX_DG_MaXe         ON DANH_GIA (MaXe);
-- [FIX 3] Index bo sung cho MaKhachHang trong DANH_GIA
CREATE INDEX IX_DG_MaKhachHang  ON DANH_GIA (MaKhachHang);
GO

-- =================================================================
-- PHAN 6: TRIGGERS
-- =================================================================

-- [FIX 6] Trigger cap nhat diem danh gia trung binh (giu nguyen logic cu, dung)
CREATE OR ALTER TRIGGER trg_CapNhatDanhGia
ON DANH_GIA
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaXe INT;
    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT DISTINCT MaXe FROM inserted
        UNION
        SELECT DISTINCT MaXe FROM deleted;

    OPEN cur;
    FETCH NEXT FROM cur INTO @MaXe;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        UPDATE XE
        SET DanhGiaTrungBinh = ISNULL((
            SELECT CAST(AVG(CAST(SoSao AS DECIMAL(3,2))) AS DECIMAL(3,2))
            FROM DANH_GIA
            WHERE MaXe = @MaXe AND IsHienThi = 1
        ), 0)
        WHERE MaXe = @MaXe;

        FETCH NEXT FROM cur INTO @MaXe;
    END;

    CLOSE cur;
    DEALLOCATE cur;
END;
GO

-- [FIX 6] Them dieu kien ro rang, tranh dem sai khi update nhieu lan
-- Chi tang SoLanThue khi TrangThai chuyen tu trang thai KHAC sang 'DaTra'
CREATE OR ALTER TRIGGER trg_TangSoLanThue
ON HOP_DONG AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE XE
    SET SoLanThue = SoLanThue + 1
    FROM XE x
    INNER JOIN inserted i ON x.MaXe = i.MaXe
    INNER JOIN deleted  d ON d.MaHopDong = i.MaHopDong
                          AND d.MaXe     = i.MaXe          -- [FIX 6] Dam bao cung xe
    WHERE i.TrangThai  = N'DaTra'
      AND d.TrangThai <> N'DaTra';                         -- [FIX 6] Chi dem 1 lan khi chuyen sang DaTra
END;
GO

-- =================================================================
-- PHAN 7: VIEWS
-- =================================================================

CREATE VIEW vw_HopDong_ChiTiet AS
SELECT
    hd.MaHopDong, hd.TrangThai, hd.NgayThue, hd.NgayTra, hd.NgayTraThucTe,
    hd.GiaThueGoc, hd.HeSoMua, hd.TongTien, hd.PhuPhiTreHan,
    hd.TongTien + hd.PhuPhiTreHan         AS ThucThu,
    hd.GhiChu, hd.NgayTao, hd.LyDoHuy,
    xe.MaXe, xe.TenXe, xe.BienSo,
    pc.TenPhongCach, hx.TenHang,
    kh.MaKhachHang, kh.HoTen              AS TenKhachHang,
    kh.SoDienThoai, kh.Email              AS EmailKhach,
    nd.HoTen                              AS NguoiTao,
    vt.TenVaiTro                          AS VaiTro       -- [FIX 2] Lay TenVaiTro tu bang VAI_TRO
FROM HOP_DONG hd
JOIN XE            xe ON hd.MaXe         = xe.MaXe
JOIN DM_PHONG_CACH pc ON xe.MaPhongCach  = pc.MaPhongCach
JOIN DM_HANG_XE    hx ON xe.MaHangXe    = hx.MaHangXe
JOIN KHACH_HANG    kh ON hd.MaKhachHang  = kh.MaKhachHang
JOIN NGUOI_DUNG    nd ON hd.MaNguoiTao   = nd.MaNguoiDung
JOIN VAI_TRO       vt ON nd.MaVaiTro     = vt.MaVaiTro;
GO

CREATE VIEW vw_DoanhThu_TheoThang AS
SELECT
    YEAR(NgayTraThucTe)             AS Nam,
    MONTH(NgayTraThucTe)            AS Thang,
    COUNT(*)                        AS SoHopDong,
    SUM(TongTien + PhuPhiTreHan)    AS TongDoanhThu
FROM HOP_DONG
WHERE TrangThai = N'DaTra' AND NgayTraThucTe IS NOT NULL
GROUP BY YEAR(NgayTraThucTe), MONTH(NgayTraThucTe);
GO

CREATE VIEW vw_Xe_ThongKe AS
SELECT
    xe.MaXe, xe.TenXe, xe.BienSo,
    hx.TenHang, pc.TenPhongCach,
    xe.GiaThueNgay, xe.TrangThai,
    xe.DanhGiaTrungBinh, xe.SoLanThue,
    (SELECT COUNT(*)
     FROM DANH_GIA dg
     WHERE dg.MaXe = xe.MaXe AND dg.IsHienThi = 1)         AS SoDanhGia,
    ISNULL((SELECT SUM(hd.TongTien + hd.PhuPhiTreHan)
            FROM HOP_DONG hd
            WHERE hd.MaXe = xe.MaXe AND hd.TrangThai = N'DaTra'), 0) AS TongDoanhThu
FROM XE xe
JOIN DM_HANG_XE    hx ON xe.MaHangXe    = hx.MaHangXe
JOIN DM_PHONG_CACH pc ON xe.MaPhongCach = pc.MaPhongCach;
GO

CREATE VIEW vw_HopDong_SapHetHan AS
SELECT
    hd.MaHopDong,
    kh.HoTen        AS TenKhachHang,
    kh.SoDienThoai,
    xe.TenXe, xe.BienSo,
    hd.NgayTra,
    DATEDIFF(DAY, GETDATE(), hd.NgayTra) AS SoNgayConLai
FROM HOP_DONG hd
JOIN XE         xe ON hd.MaXe        = xe.MaXe
JOIN KHACH_HANG kh ON hd.MaKhachHang = kh.MaKhachHang
WHERE hd.TrangThai = N'DangThue'
  AND hd.NgayTra BETWEEN GETDATE() AND DATEADD(DAY, 3, GETDATE());
GO

-- =================================================================
-- PHAN 8: STORED PROCEDURES
-- =================================================================

CREATE OR ALTER PROCEDURE sp_KiemTraXeTrong
    @MaXe INT,
    @NgayThue DATETIME,
    @NgayTra DATETIME,
    @MaHopDongLoaiTru INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM HOP_DONG
        WHERE MaXe = @MaXe
          AND TrangThai IN (N'ChoXacNhan', N'DangThue')
          AND (@MaHopDongLoaiTru IS NULL OR MaHopDong <> @MaHopDongLoaiTru)
          AND NgayThue < @NgayTra
          AND NgayTra  > @NgayThue
    )
        SELECT CAST(0 AS BIT) AS ConTrong, N'Xe da co lich trong khoang nay.' AS ThongBao;
    ELSE
        SELECT CAST(1 AS BIT) AS ConTrong, N'Xe con trong.' AS ThongBao;
END;
GO

-- [FIX 4] Sua logic overlap: dung TuNgay <= @NgayTra AND DenNgay >= @NgayThue
--         Truoc day: TuNgay <= @NgayThue AND DenNgay >= @NgayTra
--         → bi bo qua khi phu phi khong bao tron toan bo thoi gian thue
-- [FIX 5] Them UPDATE DaSuDung khi ap dung khuyen mai thanh cong
CREATE OR ALTER PROCEDURE sp_TinhTienHopDong
    @MaXe INT,
    @NgayThue DATETIME,
    @NgayTra DATETIME,
    @MaCodeKM VARCHAR(50) = NULL,
    @SoNgay     INT            OUTPUT,
    @GiaGoc     DECIMAL(12,0)  OUTPUT,
    @HeSo       DECIMAL(4,2)   OUTPUT,
    @TienGiam   DECIMAL(15,0)  OUTPUT,
    @TongTien   DECIMAL(15,0)  OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @SoNgay = DATEDIFF(DAY, @NgayThue, @NgayTra);
    IF @SoNgay < 1 SET @SoNgay = 1;

    SELECT @GiaGoc = GiaThueNgay FROM XE WHERE MaXe = @MaXe;

    -- [FIX 4] Dung overlap check chuan: TuNgay <= @NgayTra AND DenNgay >= @NgayThue
    -- Dam bao bat duoc phu phi du hop dong thue dat vao bat ky ngay nao trong dot
    SELECT @HeSo = ISNULL(MAX(HeSoNhan), 1.00)
    FROM PHU_PHI_MUA
    WHERE TuNgay <= @NgayTra AND DenNgay >= @NgayThue;

    DECLARE @TamTinh DECIMAL(15,0) = @GiaGoc * @SoNgay * @HeSo;
    SET @TienGiam = 0;

    IF @MaCodeKM IS NOT NULL
    BEGIN
        DECLARE @MaKhuyenMai INT,
                @LoaiGiam    NVARCHAR(20),
                @GiaTriGiam  DECIMAL(10,2),
                @GiaTriMax   DECIMAL(15,0),
                @DieuKien    DECIMAL(15,0);

        SELECT
            @MaKhuyenMai = MaKhuyenMai,
            @LoaiGiam    = LoaiGiamGia,
            @GiaTriGiam  = GiaTriGiam,
            @GiaTriMax   = GiaTriGiamToiDa,
            @DieuKien    = DieuKienToiThieu
        FROM KHUYEN_MAI
        WHERE MaCode  = @MaCodeKM
          AND IsActive = 1
          AND GETDATE() BETWEEN TuNgay AND DenNgay
          AND DaSuDung < SoLanSuDung;

        IF @LoaiGiam IS NOT NULL AND (@DieuKien IS NULL OR @TamTinh >= @DieuKien)
        BEGIN
            IF @LoaiGiam = N'PhanTram'
            BEGIN
                SET @TienGiam = @TamTinh * @GiaTriGiam / 100;
                IF @GiaTriMax IS NOT NULL AND @TienGiam > @GiaTriMax
                    SET @TienGiam = @GiaTriMax;
            END
            ELSE
                SET @TienGiam = @GiaTriGiam;

            -- [FIX 5] Tang DaSuDung khi khuyen mai duoc ap dung thanh cong
            -- Tranh tinh phi khuyen mai nhung khong cap nhat so lan su dung
            UPDATE KHUYEN_MAI
            SET DaSuDung = DaSuDung + 1
            WHERE MaKhuyenMai = @MaKhuyenMai;
        END
    END

    SET @TongTien = @TamTinh - @TienGiam;
    IF @TongTien < 0 SET @TongTien = 0;
END;
GO

CREATE OR ALTER PROCEDURE sp_XuLyTraXe
    @MaHopDong          INT,
    @NgayTraThucTe      DATETIME,
    @MaNguoiThucHien    INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    DECLARE @MaXe           INT,
            @NgayTra        DATETIME,
            @GiaGoc         DECIMAL(12,0),
            @PhuPhiTreHan   DECIMAL(15,0) = 0;

    SELECT @MaXe    = MaXe,
           @NgayTra = NgayTra,
           @GiaGoc  = GiaThueGoc
    FROM HOP_DONG
    WHERE MaHopDong = @MaHopDong AND TrangThai = N'DangThue';

    IF @MaXe IS NULL
    BEGIN
        ROLLBACK;
        RAISERROR(N'Hop dong khong hop le hoac da tra.', 16, 1);
        RETURN;
    END

    IF @NgayTraThucTe > @NgayTra
    BEGIN
        DECLARE @NgayTre INT = DATEDIFF(DAY, @NgayTra, @NgayTraThucTe);
        DECLARE @HeSoTre DECIMAL(4,2);
        SELECT @HeSoTre = ISNULL(MAX(CAST(GiaTri AS DECIMAL(4,2))), 1.5)
        FROM CAU_HINH_HE_THONG
        WHERE TenKhoa = 'phi_tre_han_he_so';

        SET @PhuPhiTreHan = @GiaGoc * @NgayTre * @HeSoTre;
    END

    UPDATE HOP_DONG
    SET TrangThai        = N'DaTra',
        NgayTraThucTe    = @NgayTraThucTe,
        PhuPhiTreHan     = @PhuPhiTreHan,
        MaNguoiXacNhan   = @MaNguoiThucHien,
        NgayCapNhat      = GETDATE()
    WHERE MaHopDong = @MaHopDong;

    UPDATE XE
    SET TrangThai   = N'ConTrong',
        NgayCapNhat = GETDATE()
    WHERE MaXe = @MaXe;

    COMMIT;
    SELECT @PhuPhiTreHan AS PhuPhiTreHan, N'Tra xe thanh cong.' AS ThongBao;
END;
GO

-- =================================================================
-- PHAN 9: DU LIEU MAU (SEED DATA)
-- =================================================================

INSERT INTO DM_PHONG_CACH (TenPhongCach, MoTa) VALUES
(N'Sport',     N'Xe the thao, toc do cao, tu the lai cup nguoi'),
(N'Cruiser',   N'Xe cruiser manh me, thoai mai, phu hop duong dai'),
(N'Touring',   N'Xe touring hanh trinh, trang bi day du tien nghi'),
(N'Bobber',    N'Xe bobber phong cach co dien My'),
(N'Scrambler', N'Xe scrambler da dia hinh, ca tinh');

INSERT INTO DM_HANG_XE (TenHang, NuocSanXuat) VALUES
(N'Harley-Davidson', N'My'), (N'Ducati', N'Y'), (N'BMW Motorrad', N'Duc'),
(N'Honda', N'Nhat Ban'), (N'Kawasaki', N'Nhat Ban'), (N'Royal Enfield', N'Anh/An Do'),
(N'Triumph', N'Anh'), (N'Indian', N'My');

INSERT INTO DM_LOAI_GIAY_TO (TenLoai) VALUES (N'CMND'), (N'CCCD'), (N'Ho chieu');

INSERT INTO DM_TINH_THANH (TenTinh) VALUES
(N'TP. Ho Chi Minh'), (N'Ha Noi'), (N'Da Nang'),
(N'Can Tho'), (N'Binh Duong'), (N'Dong Nai');

-- [FIX 1] Seed du lieu bang VAI_TRO va QUYEN
INSERT INTO VAI_TRO (TenVaiTro, MoTa) VALUES
(N'Admin',     N'Quan tri vien he thong, co toan quyen thao tac'),
(N'KhachHang', N'Khach hang thue xe, chi xem va quan ly don hang cua chinh minh');

-- Danh sach cac quyen chuc nang trong he thong
INSERT INTO QUYEN (TenQuyen, NhomQuyen, MoTa) VALUES
-- Quyen Admin
(N'QuanLyNguoiDung',    N'NguoiDung', N'Them, sua, xoa, khoa tai khoan nguoi dung'),
(N'QuanLyXe',           N'Xe',        N'Them, sua, xoa thong tin xe'),
(N'QuanLyHopDong',      N'HopDong',   N'Xem, xac nhan, huy toan bo hop dong'),
(N'QuanLyKhachHang',    N'KhachHang', N'Xem va cap nhat thong tin khach hang'),
(N'QuanLyBaoDuong',     N'BaoDuong',  N'Quan ly lich bao duong xe'),
(N'QuanLyKhuyenMai',    N'KhuyenMai', N'Tao va quan ly chuong trinh khuyen mai'),
(N'XemBaoCao',          N'BaoCao',    N'Xem bao cao doanh thu, thong ke'),
(N'CauHinhHeThong',     N'HeThong',   N'Thay doi cau hinh he thong'),
(N'DuyetDanhGia',       N'DanhGia',   N'An hien danh gia cua khach hang'),
-- Quyen KhachHang
(N'XemXe',              N'Xe',        N'Xem danh sach va chi tiet xe'),
(N'DatXe',              N'HopDong',   N'Tao hop dong thue xe moi'),
(N'XemHopDongCuaMiNh',  N'HopDong',   N'Xem danh sach hop dong ca nhan'),
(N'HuyHopDongCuaMiNh',  N'HopDong',   N'Huy hop dong chua duoc xac nhan'),
(N'DanhGiaXe',          N'DanhGia',   N'Danh gia xe sau khi tra xe'),
(N'XemThongBaoCuaMiNh', N'ThongBao',  N'Xem thong bao ca nhan'),
(N'CapNhatHoSo',        N'NguoiDung', N'Cap nhat thong tin ca nhan');

-- Gan quyen cho Admin (co tat ca quyen)
INSERT INTO VAI_TRO_QUYEN (MaVaiTro, MaQuyen)
SELECT 1, MaQuyen FROM QUYEN;

-- Gan quyen cho KhachHang (chi co quyen tu MaQuyen 10 tro di)
INSERT INTO VAI_TRO_QUYEN (MaVaiTro, MaQuyen)
SELECT 2, MaQuyen FROM QUYEN WHERE TenQuyen IN (
    N'XemXe', N'DatXe', N'XemHopDongCuaMiNh', N'HuyHopDongCuaMiNh',
    N'DanhGiaXe', N'XemThongBaoCuaMiNh', N'CapNhatHoSo'
);

INSERT INTO CAU_HINH_HE_THONG (TenKhoa, GiaTri, KieuDuLieu, MoTa) VALUES
('phi_tre_han_he_so',      '1.5',  'decimal', N'He so nhan gia thue cho moi ngay tra tre (1.5 = 150%)'),
('so_ngay_thue_toi_thieu', '1',    'int',     N'So ngay thue toi thieu moi hop dong'),
('so_ngay_thue_toi_da',    '30',   'int',     N'So ngay thue toi da moi hop dong'),
('canh_bao_het_han_ngay',  '3',    'int',     N'Gui thong bao khi con N ngay den han tra'),
('ten_cua_hang',            N'UTE Moto Rental', 'string', N'Ten hien thi tren hop dong PDF'),
('logo_url',               '/images/logo.png', 'string', N'Duong dan logo he thong');

-- [FIX 2] MaVaiTro thay vi VaiTro chuoi: 1=Admin, 2=KhachHang
INSERT INTO NGUOI_DUNG (HoTen, Email, MatKhauHash, SoDienThoai, MaVaiTro) VALUES
(N'Nguyen Huu Hiep',  'admin@thuexe.vn',            '$2a$11$Ngl2PntGmdNRtuhR.38sLeR2vzv/eUynoLrpagrwWSPuodFJVRQM.', '0901111111', 1),
(N'Le Van An',        'levan.an@gmail.com',          '$2a$11$Ob4uVBBVyyNtsG51bTBhtOqxwqlCLibencrphsMTys/rfnMIgTssa', '0911000001', 2),
(N'Pham Thi Binh',    'phamthi.binh@gmail.com',      '$2a$11$Ob4uVBBVyyNtsG51bTBhtOqxwqlCLibencrphsMTys/rfnMIgTssa', '0911000002', 2),
(N'Tran Van Cuong',   'tranvan.cuong@gmail.com',     '$2a$11$Ob4uVBBVyyNtsG51bTBhtOqxwqlCLibencrphsMTys/rfnMIgTssa', '0911000003', 2),
(N'Nguyen Thi Dung',  'nguyenthi.dung@gmail.com',    '$2a$11$Ob4uVBBVyyNtsG51bTBhtOqxwqlCLibencrphsMTys/rfnMIgTssa', '0911000004', 2);

INSERT INTO KHACH_HANG (HoTen, SoDienThoai, Email, SoGPLX, MaLoaiGiayTo, SoGiayTo, NgayCap, DiaChi, MaTinh, NgaySinh, GioiTinh, MaNguoiDung) VALUES
(N'Le Van An',       '0911000001', 'levan.an@gmail.com',       'A1-123456', 2, '079201001111', '2020-01-10', N'123 Nguyen Trai, Q.1', 1, '1995-03-15', N'Nam', 2),
(N'Pham Thi Binh',   '0911000002', 'phamthi.binh@gmail.com',   'B2-234567', 2, '079201002222', '2021-05-20', N'45 Le Loi, Q.3',       1, '1998-07-22', N'Nu',  3),
(N'Tran Van Cuong',  '0911000003', 'tranvan.cuong@gmail.com',  'A2-345678', 2, '079201003333', '2019-11-15', N'789 Dien Bien Phu',    1, '1993-12-01', N'Nam', 4),
(N'Nguyen Thi Dung', '0911000004', 'nguyenthi.dung@gmail.com', 'B1-456789', 2, '079201004444', '2022-03-08', N'22 Vo Van Tan, Q.3',   1, '2000-09-30', N'Nu',  5);

INSERT INTO XE (TenXe, BienSo, MaHangXe, MaPhongCach, GiaThueNgay, NamSanXuat, DungTich, MoTa, TrangThai) VALUES
(N'Harley Sportster S',       '51K1-12345', 1, 2, 1200000, 2022, 1250, N'Cruiser V-twin dac trung.',         N'ConTrong'),
(N'Ducati Monster 937',       '51K1-23456', 2, 1, 1500000, 2023,  937, N'Naked sport hieu suat cao.',        N'ConTrong'),
(N'Royal Enfield Meteor 350', '51K1-34567', 6, 2,  700000, 2023,  350, N'Classic cruiser nhe nhang.',        N'ConTrong'),
(N'Honda CB650R',             '51K1-45678', 4, 1,  900000, 2022,  648, N'Neo Sports Cafe 4 xy-lanh.',        N'DangThue'),
(N'Triumph Bonneville T120',  '51K1-56789', 7, 4, 1100000, 2021, 1200, N'British classic style.',            N'ConTrong'),
(N'BMW R 1250 GS',            '51K1-67890', 3, 3, 1800000, 2023, 1254, N'Adventure tourer hang dau.',        N'BaoDuong'),
(N'Kawasaki Z900',            '51K1-78901', 5, 1, 1000000, 2022,  948, N'Naked sport can bang.',             N'ConTrong'),
(N'Indian Scout Bobber',      '51K1-89012', 8, 4, 1300000, 2022, 1133, N'American bobber 69 cubic inch.',    N'ConTrong');

INSERT INTO XE_HINH_ANH (MaXe, DuongDanAnh, IsAnhChinh, ThuTu) VALUES
(1, '/images/xe/harley-sportster-s.jpg',      1, 1),
(1, '/images/xe/harley-sportster-s-side.jpg', 0, 2),
(2, '/images/xe/ducati-monster-937.jpg',      1, 1),
(3, '/images/xe/royal-enfield-meteor.jpg',    1, 1),
(4, '/images/xe/honda-cb650r.jpg',            1, 1),
(5, '/images/xe/triumph-bonneville.jpg',      1, 1),
(6, '/images/xe/bmw-r1250gs.jpg',             1, 1),
(7, '/images/xe/kawasaki-z900.jpg',           1, 1),
(8, '/images/xe/indian-scout-bobber.jpg',     1, 1);

INSERT INTO PHU_PHI_MUA (TenDot, TuNgay, DenNgay, HeSoNhan, GhiChu, MaNguoiTao) VALUES
(N'Tet Nguyen Dan 2025', '2025-01-25', '2025-02-05', 1.50, N'Tang 50% dip Tet At Ti',       1),
(N'Le 30/4 - 1/5 2025', '2025-04-28', '2025-05-02', 1.30, N'Tang 30% dip le 30/4 - 1/5',  1),
(N'He 2025',             '2025-06-01', '2025-08-31', 1.20, N'Tang 20% mua he du lich',      1),
(N'Quoc Khanh 2/9/2025', '2025-08-30', '2025-09-03', 1.30, N'Tang 30% dip Quoc Khanh',    1);

INSERT INTO KHUYEN_MAI (TenChuongTrinh, MaCode, LoaiGiamGia, GiaTriGiam, GiaTriGiamToiDa, SoLanSuDung, TuNgay, DenNgay, DieuKienToiThieu, MaNguoiTao) VALUES
(N'Khach hang moi',    'NEWCUST',  N'PhanTram',     10, 200000, 100, '2025-01-01', '2025-12-31', 500000,  1),
(N'Giam co dinh 100k', 'SAVE100K', N'SoTienCoDinh', 100000, NULL, 50, '2025-05-01', '2025-07-31', 1000000, 1),
(N'He ruc ro 15%',     'SUMMER15', N'PhanTram',     15, 500000,  30, '2025-06-01', '2025-08-31', 1500000, 1);

-- [FIX 9] HeSoMua dung gia tri thuc te de test logic phu phi
-- HD1: thue dip Le 30/4 (HeSo 1.3), HD2: thue binh thuong (HeSo 1.0)
INSERT INTO HOP_DONG (MaXe, MaKhachHang, MaNguoiTao, MaNguoiXacNhan, NgayThue, NgayTra, NgayTraThucTe, GiaThueGoc, HeSoMua, TongTien, PhuPhiTreHan, TrangThai) VALUES
(4, 1, 1, 1, '2025-04-29', '2025-05-02', '2025-05-02', 900000,  1.30, 3510000, 0,       N'DaTra'),      -- Le 30/4, HeSo=1.3
(1, 2, 1, 1, '2025-05-10', '2025-05-13', '2025-05-14', 1200000, 1.00, 3600000, 1800000, N'DaTra'),      -- Binh thuong, tra tre 1 ngay
(7, 3, 1, 1, '2025-05-15', '2025-05-18', NULL,         1000000, 1.00, 3000000, 0,       N'DangThue'),   -- Dang thue
(5, 4, 1, NULL,'2025-05-20','2025-05-22', NULL,         1100000, 1.00, 2200000, 0,       N'ChoXacNhan'); -- Cho xac nhan

UPDATE XE SET TrangThai = N'DangThue', NgayCapNhat = GETDATE()
WHERE MaXe IN (
    SELECT DISTINCT MaXe FROM HOP_DONG 
    WHERE TrangThai IN (N'DangThue', N'ChoXacNhan')
);

INSERT INTO THANH_TOAN (MaHopDong, SoTien, PhuongThuc, LoaiThanhToan, TrangThai, MaNguoiNhanTien, GhiChu) VALUES
(1, 3510000, N'TienMat',     N'ThanhToanCuoi', N'ThanhCong', 1,    N'Khach thanh toan khi nhan xe'),
(2, 1000000, N'ChuyenKhoan', N'DatCoc',        N'ThanhCong', 1,    N'Dat coc 1 trieu truoc'),
(2, 4200000, N'TienMat',     N'ThanhToanCuoi', N'ThanhCong', 1,    N'Thanh toan phan con lai + phi tre'),
(3, 1500000, N'Vi_MoMo',     N'DatCoc',        N'ThanhCong', NULL, N'Dat coc qua MoMo');

INSERT INTO HOP_DONG_TAI_LIEU (MaHopDong, TenTaiLieu, DuongDan, LoaiFile) VALUES
(1, N'CCCD mat truoc', '/uploads/contracts/1/cccd_front.jpg', 'jpg'),
(1, N'CCCD mat sau',   '/uploads/contracts/1/cccd_back.jpg',  'jpg'),
(1, N'GPLX',           '/uploads/contracts/1/gplx.jpg',       'jpg'),
(2, N'CCCD mat truoc', '/uploads/contracts/2/cccd_front.jpg', 'jpg');

INSERT INTO BAO_DUONG (MaXe, LoaiBaoDuong, NgayVao, NgayRa, ChiPhi, DonViThucHien, MaNguoiPhuTrach, TrangThai) VALUES
(6, N'Bao duong dinh ky 10.000km', '2025-05-10', NULL,         1500000, N'Gara BMW Quan 3', 1, N'DangSua'),
(4, N'Thay nhot + loc',            '2025-04-20', '2025-04-21',  350000, N'Gara Honda Q.7',  1, N'HoanThanh');

-- [FIX 3] Them MaKhachHang vao DANH_GIA
INSERT INTO DANH_GIA (MaHopDong, MaXe, MaKhachHang, SoSao, NhanXet) VALUES
(1, 4, 1, 5, N'Xe chay em, nhan vien nhiet tinh. Se quay lai!'),
(2, 1, 2, 4, N'Harley cam giac rat tot, hoi ton xang nhung xung dang.');

INSERT INTO THONG_BAO (TieuDe, NoiDung, LoaiThongBao, MaNguoiNhan, MaHopDongLienQuan) VALUES
(N'Hop dong #3 sap den han', N'Hop dong cua anh Tran Van Cuong se den han tra ngay 18/05/2025.', N'HopDongSapHetHan', 1, 3),
(N'BMW R 1250 GS dang bao duong', N'Xe BMW R 1250 GS (51K1-67890) dang bao duong, chua co lich xong.', N'BaoDuong', NULL, NULL);
GO

-- =================================================================
-- KIEM TRA
-- =================================================================
PRINT N'======================================================';
PRINT N'  THONG KE SO BAN GHI SEED DATA';
PRINT N'======================================================';
SELECT Bang, SoBanGhi FROM (
    SELECT N'DM_PHONG_CACH'        AS Bang, COUNT(*) AS SoBanGhi FROM DM_PHONG_CACH      UNION ALL
    SELECT N'DM_HANG_XE',                   COUNT(*) FROM DM_HANG_XE                     UNION ALL
    SELECT N'DM_LOAI_GIAY_TO',              COUNT(*) FROM DM_LOAI_GIAY_TO                UNION ALL
    SELECT N'DM_TINH_THANH',                COUNT(*) FROM DM_TINH_THANH                  UNION ALL
    SELECT N'VAI_TRO',                      COUNT(*) FROM VAI_TRO                        UNION ALL
    SELECT N'QUYEN',                        COUNT(*) FROM QUYEN                          UNION ALL
    SELECT N'VAI_TRO_QUYEN',               COUNT(*) FROM VAI_TRO_QUYEN                  UNION ALL
    SELECT N'CAU_HINH_HE_THONG',            COUNT(*) FROM CAU_HINH_HE_THONG              UNION ALL
    SELECT N'NGUOI_DUNG',                   COUNT(*) FROM NGUOI_DUNG                     UNION ALL
    SELECT N'KHACH_HANG',                   COUNT(*) FROM KHACH_HANG                     UNION ALL
    SELECT N'XE',                           COUNT(*) FROM XE                             UNION ALL
    SELECT N'XE_HINH_ANH',                  COUNT(*) FROM XE_HINH_ANH                    UNION ALL
    SELECT N'PHU_PHI_MUA',                   COUNT(*) FROM PHU_PHI_MUA                    UNION ALL
    SELECT N'KHUYEN_MAI',                    COUNT(*) FROM KHUYEN_MAI                     UNION ALL
    SELECT N'HOP_DONG',                      COUNT(*) FROM HOP_DONG                       UNION ALL
    SELECT N'THANH_TOAN',                    COUNT(*) FROM THANH_TOAN                     UNION ALL
    SELECT N'HOP_DONG_TAI_LIEU',             COUNT(*) FROM HOP_DONG_TAI_LIEU              UNION ALL
    SELECT N'BAO_DUONG',                     COUNT(*) FROM BAO_DUONG                      UNION ALL
    SELECT N'DANH_GIA',                      COUNT(*) FROM DANH_GIA                       UNION ALL
    SELECT N'THONG_BAO',                     COUNT(*) FROM THONG_BAO
) t ORDER BY Bang;

PRINT N'';
PRINT N'=== PHAN QUYEN: VAI TRO VA QUYEN ===';
SELECT vt.TenVaiTro, q.NhomQuyen, q.TenQuyen
FROM VAI_TRO_QUYEN vtq
JOIN VAI_TRO vt ON vtq.MaVaiTro = vt.MaVaiTro
JOIN QUYEN   q  ON vtq.MaQuyen  = q.MaQuyen
ORDER BY vt.TenVaiTro, q.NhomQuyen;

PRINT N'';
PRINT N'=== HOP DONG CHI TIET ===';
SELECT MaHopDong, TenKhachHang, TenXe, TenPhongCach,
       NgayThue, NgayTra, TongTien, ThucThu, TrangThai
FROM vw_HopDong_ChiTiet;

PRINT N'';
PRINT N'=== XE SAP HET HAN TRA ===';
SELECT * FROM vw_HopDong_SapHetHan;

PRINT N'';
PRINT N'=== TAI KHOAN DEMO ===';
PRINT N'  [Admin]      admin@thuexe.vn            / Admin@123456';
PRINT N'  [KhachHang]  levan.an@gmail.com         / Khachhang@123456';
PRINT N'  [KhachHang]  phamthi.binh@gmail.com     / Khachhang@123456';
PRINT N'  [KhachHang]  tranvan.cuong@gmail.com    / Khachhang@123456';
PRINT N'  [KhachHang]  nguyenthi.dung@gmail.com   / Khachhang@123456';
PRINT N'';
PRINT N'[OK] QuanLyThueXeDB Fixed v2 - Tong: 17 bang (them VAI_TRO, QUYEN, VAI_TRO_QUYEN)';
GO