namespace LTWeb_TBDT.Models
{
    public class NhanVien
    {
        private int maNhanVien;
        private string hoTen;
        private DateTime ngaySinh;
        private string email;
        private string soDienThoai;
        private int maTaiKhoan;

        public int MaNhanVien { get => maNhanVien; set => maNhanVien = value; }
        public string HoTen { get => hoTen; set => hoTen = value; }
        public DateTime NgaySinh { get => ngaySinh; set => ngaySinh = value; }
        public string Email { get => email; set => email = value; }
        public string SoDienThoai { get => soDienThoai; set => soDienThoai = value; }
        public int MaTaiKhoan { get => maTaiKhoan; set => maTaiKhoan = value; }

        public NhanVien() { }

        public NhanVien(int manhanvien, string hoten, DateTime ngaysinh, string email, string sdt, int mataikhoan)
        {
            MaNhanVien = manhanvien;
            HoTen = hoten;
            NgaySinh = ngaysinh;
            Email = email;
            SoDienThoai = sdt;
            MaTaiKhoan = mataikhoan;
        }
    }
}
