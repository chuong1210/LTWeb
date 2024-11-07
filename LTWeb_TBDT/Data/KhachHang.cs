using System;
using System.Collections.Generic;

namespace LTWeb_TBDT.Data
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            HoaDons = new HashSet<HoaDon>();
        }

        public int MaKhachHang { get; set; }
        public string TenKhachHang { get; set; } = null!;
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public int? MaTaiKhoan { get; set; }

        public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}
