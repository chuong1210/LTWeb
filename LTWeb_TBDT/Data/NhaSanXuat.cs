using System;
using System.Collections.Generic;

namespace LTWeb_TBDT.Data
{
    public partial class NhaSanXuat
    {
        public NhaSanXuat()
        {
            SanPhams = new HashSet<SanPham>();
        }

        public int MaNhaSanXuat { get; set; }
        public string TenNhaSanXuat { get; set; } = null!;
        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
