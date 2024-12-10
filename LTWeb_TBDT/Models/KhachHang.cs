using LTWeb_TBDT.Data;
using System.Diagnostics.SymbolStore;

namespace LTWeb_TBDT.Models
{
    public class KhachHang
    {
        private int maKhachHang;
        private string hoTen;
        private string diaChi;
        private string soDienThoai;
        private string email;
        private int maTaiKhoan;

        public int MaKhachHang { get => maKhachHang; set => maKhachHang = value; }
        public string HoTen { get => hoTen; set => hoTen = value; }
        public string DiaChi { get => diaChi; set => diaChi = value; }
        public string SoDienThoai { get => soDienThoai; set => soDienThoai = value; }
        public string Email { get => email; set => email = value; }
        public int MaTaiKhoan { get => maTaiKhoan; set => maTaiKhoan = value; }

        public KhachHang() { }
        public KhachHang(int makhachhang, string hoten, string diachi, string sdt, string email, int mataikhoan)
        {
            MaKhachHang = makhachhang;
            HoTen = hoten;
            DiaChi = diachi;
            SoDienThoai = sdt;
            Email = email;
            MaTaiKhoan = mataikhoan;
        }
    }
}
