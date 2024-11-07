using System;
using System.Collections.Generic;

namespace LTWeb_TBDT.Data
{
    public partial class SanPham
    {
        public SanPham()
        {
            ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }

        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = null!;
        public string? MoTa { get; set; }
        public decimal GiaBan { get; set; }
        public int? SoLuongTon { get; set; }
        public string? HinhAnh { get; set; }
        public int? MaNhaSanXuat { get; set; }
        public int? MaDanhMuc { get; set; }

        public virtual DanhMuc? MaDanhMucNavigation { get; set; }
        public virtual NhaSanXuat? MaNhaSanXuatNavigation { get; set; }
        public virtual TonKho? TonKho { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}
