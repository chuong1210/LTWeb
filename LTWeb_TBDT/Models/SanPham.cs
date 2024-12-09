using LTWeb_TBDT.Data;
using System.Diagnostics.SymbolStore;

namespace LTWeb_TBDT.Models
{
    public class SanPham
    {
        private int maSP;
        private string tenSP;
        private string moTa;
        private double giaBan;
        private int soLuongTon;
        private string hinhAnh;
        private int maNSX;
        private int maDanhMuc;

        public int MaSP { get => maSP; set => maSP = value; }
        public string TenSP { get => tenSP; set => tenSP = value; }
        public string MoTa { get => moTa; set => moTa = value; }
        public double GiaBan { get => giaBan; set => giaBan = value; }
        public int SoLuongTon { get => soLuongTon; set => soLuongTon = value; }
        public string HinhAnh { get => hinhAnh; set => hinhAnh = value; }
        public int MaNSX { get => maNSX; set => maNSX = value; }
        public int MaDanhMuc { get => maDanhMuc; set => maDanhMuc = value; }
		public DanhMuc DanhMuc { get; set; }

		public SanPham() { }


        public SanPham(int masp, string tensp, string mota, double giaban, int slt, string hinhanh, int mansx, int madanhmuc)
        {
            MaSP = masp;
            TenSP = tensp;
            MoTa = mota;
            GiaBan = giaban;
            SoLuongTon = slt;
            HinhAnh = hinhanh;
            MaNSX = mansx;
            MaDanhMuc = madanhmuc;
        }
    }
}
