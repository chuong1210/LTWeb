CREATE DATABASE BanThietBiDienTu
go
use BanThietBiDienTu
go
CREATE TABLE TaiKhoan (
    MaTaiKhoan INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) NOT NULL,
    MatKhau NVARCHAR(255) NOT NULL,
    LoaiTaiKhoan NVARCHAR(50) NOT NULL
);

CREATE TABLE NhanVien (
    MaNhanVien INT IDENTITY(1,1) PRIMARY KEY,
    TenNhanVien NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    Email NVARCHAR(100),
    SoDienThoai NVARCHAR(15),
    MaTaiKhoan INT,
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

CREATE TABLE Admin (
    MaAdmin INT IDENTITY(1,1) PRIMARY KEY,
    MaTaiKhoan INT,
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

CREATE TABLE KhachHang (
    MaKhachHang INT IDENTITY(1,1) PRIMARY KEY,
    TenKhachHang NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(255),
    SoDienThoai NVARCHAR(15),
    Email NVARCHAR(100),
    MaTaiKhoan INT,
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);


CREATE TABLE NhaSanXuat (
    MaNhaSanXuat INT IDENTITY(1,1) PRIMARY KEY,
    TenNhaSanXuat NVARCHAR(255) NOT NULL,
    DiaChi NVARCHAR(255),
    SoDienThoai NVARCHAR(15),
    Email NVARCHAR(100)
);

CREATE TABLE DanhMuc (
    MaDanhMuc INT IDENTITY(1,1) PRIMARY KEY,
    TenDanhMuc NVARCHAR(255) NOT NULL,
    MoTa NVARCHAR(MAX)
);

CREATE TABLE SanPham (
    MaSanPham INT IDENTITY(1,1) PRIMARY KEY,
    TenSanPham NVARCHAR(255) NOT NULL,
    MoTa NVARCHAR(MAX),
    GiaBan DECIMAL(18, 2) NOT NULL,
    SoLuongTon INT,
    HinhAnh NVARCHAR(MAX),
    MaNhaSanXuat INT,
	MaDanhMuc INT,
    FOREIGN KEY (MaNhaSanXuat) REFERENCES NhaSanXuat(MaNhaSanXuat),
    FOREIGN KEY (MaDanhMuc) REFERENCES DanhMuc(MaDanhMuc)

);

CREATE TABLE HoaDon (
    MaHoaDon INT IDENTITY(1,1) ,
    MaKhachHang INT,
    NgayDatHang DATE NOT NULL,
    TongTien DECIMAL(18, 2),
    TrangThai NVARCHAR(50),
	CONSTRAINT PK_HOADON  PRIMARY KEY (MaHoaDon),
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang)
);
CREATE TABLE ChiTietHoaDon (
    MaHoaDon INT,
    MaSanPham INT,
    SoLuong INT NOT NULL,
    GiaBan DECIMAL(18, 2) NOT NULL,
    ThanhTien AS (SoLuong * GiaBan), 
    PRIMARY KEY (MaHoaDon, MaSanPham),
    FOREIGN KEY (MaHoaDon) REFERENCES HoaDon(MaHoaDon),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);

CREATE TABLE TonKho
(
MaSanPham INT PRIMARY KEY,
SoLuongTon INT,
FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)

)