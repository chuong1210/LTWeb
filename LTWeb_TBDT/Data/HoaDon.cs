using System;
using System.Collections.Generic;

namespace LTWeb_TBDT.Data
{
    public partial class HoaDon
    {
        public HoaDon()
        {
            ChiTietHoaDons = new HashSet<ChiTietHoaDon>();
        }

        public int MaHoaDon { get; set; }
        public int? MaKhachHang { get; set; }
        public DateTime NgayDatHang { get; set; }
        public decimal? TongTien { get; set; }
        public string? TrangThai { get; set; }

        public virtual KhachHang? MaKhachHangNavigation { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}
