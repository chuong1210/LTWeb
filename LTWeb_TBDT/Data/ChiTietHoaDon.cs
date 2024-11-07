using System;
using System.Collections.Generic;

namespace LTWeb_TBDT.Data
{
    public partial class ChiTietHoaDon
    {
        public int MaHoaDon { get; set; }
        public int MaSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
        public decimal? ThanhTien { get; set; }

        public virtual HoaDon MaHoaDonNavigation { get; set; } = null!;
        public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
    }
}
