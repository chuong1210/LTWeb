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
    HoTen NVARCHAR(100) NOT NULL,
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
    HoTen NVARCHAR(100) NOT NULL,
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

INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES (N'Điện Thoại', N'Các loại điện thoại di động');
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES (N'Laptop', N'Các loại máy tính xách tay');
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES (N'Máy Tính Bảng', N'Các loại máy tính bảng');
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES (N'Tivi', N'Các loại tivi các kích cỡ khác nhau');
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES (N'Âm Thanh', N'Thiết bị âm thanh như loa, tai nghe');
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES (N'Đồng Hồ Thông Minh', N'Các loại đồng hồ thông minh');
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES (N'Phụ Kiện', N'Các loại phụ kiện điện tử như sạc, cáp, bao da');
INSERT INTO DanhMuc (TenDanhMuc, MoTa) VALUES (N'Thiết Bị Gia Dụng', N'Các loại thiết bị điện tử gia dụng');


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


-- Thêm dữ liệu vào bảng NhaSanXuat
INSERT INTO NhaSanXuat (TenNhaSanXuat, DiaChi, SoDienThoai, Email) VALUES
(N'Apple', N'Cupertino, California, USA', N'18002752273', N'contact@apple.com'),
(N'Samsung', N'Suwon, South Korea', N'18007267864', N'support@samsung.com'),
(N'Sony', N'Tokyo, Japan', N'810362722211', N'info@sony.com'),
(N'LG', N'Seoul, South Korea', N'18008550386', N'lg.support@lg.com'),
(N'Dell', N'Round Rock, Texas, USA', N'18006249896', N'dell.support@dell.com');

-- Thêm dữ liệu vào bảng SanPham
INSERT INTO SanPham (TenSanPham, MoTa, GiaBan, SoLuongTon, HinhAnh, MaNhaSanXuat, MaDanhMuc) VALUES
(N'iPhone 15', N'Dòng điện thoại mới nhất của Apple', 29999000, 100, 'SP001.jpg', 1, 1),
(N'Điện Thoại Samsung Galaxy A05s 4GB/128GB', N'Dòng flagship mới của Samsung', 20999000, 150, 'SP002.jpg', 2, 1),
(N'Xperia 1 IV', N'Smartphone cao cấp từ Sony', 25999000, 50, 'SP003.jpg', 3, 1),
(N'Macbook Pro 14 inch 2021', N'Máy tính xách tay chuyên nghiệp từ Apple', 52999000, 30, 'SP004.jpg', 1, 2),
(N'Dell XPS 13', N'Máy tính xách tay siêu mỏng từ Dell', 32999000, 40, 'SP005.jpg', 5, 2),
(N'LG OLED CX', N'Tivi OLED cao cấp từ LG', 34999000, 20, 'SP006.jpg', 4, 4),
(N'Bravia XR', N'Tivi 4K HDR từ Sony', 39999000, 25, 'SP007.jpg', 3, 4),
(N'Galaxy Tab S8', N'Máy tính bảng mạnh mẽ từ Samsung', 18999000, 60, 'SP008.jpg', 2, 3),
(N'Apple Watch Series 8', N'Đồng hồ thông minh mới từ Apple', 11999000, 200, 'SP009.jpg', 1, 6),
(N'AirPods Pro 2', N'Tai nghe không dây cao cấp từ Apple', 5499000, 300, 'SP0010.jpg', 1, 5);
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, LoaiTaiKhoan)
VALUES 
('admin1', '123456', 'Admin'),
('nhanvien1', '123456', 'NhanVien'),
('khachhang1', '123456', 'KhachHang'),
('nhanvien2', 'abcdef', 'NhanVien'),
('khachhang2', 'abcdef', 'KhachHang');
INSERT INTO NhanVien (HoTen, NgaySinh, Email, SoDienThoai, MaTaiKhoan)
VALUES 
('Nguyen Van A', '1990-05-15', 'nva@example.com', '0123456789', 2),
('Tran Thi B', '1992-08-22', 'ttb@example.com', '0987654321', 4),
('Le Van C', '1988-03-12', 'lvc@example.com', '0123987654', NULL),
('Pham Thi D', '1995-07-19', NULL, '0912345678', NULL),
('Hoang Van E', '1985-01-10', 'hve@example.com', '0123456780', NULL);
INSERT INTO Admin (MaTaiKhoan)
VALUES 
(1)
INSERT INTO KhachHang (HoTen, DiaChi, SoDienThoai, Email, MaTaiKhoan)
VALUES 
('Nguyen Minh A', 'Hanoi', '0123456789', 'nma@gmail.com', 3),
('Tran Van B', 'HCM', '0987654321', 'tvb@gmail.com', NULL),
('Le Hanh C', 'Hue', '0912345678', 'lhc@gmail.com', NULL),
('Pham Thi E', 'Vinh', NULL , 'no-mail@example.com',NULL),
('Pham Thi G', 'Vung Tau', '0909345678', 'ptg@gmail.com', NULL);

