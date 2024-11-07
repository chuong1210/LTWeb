using System;
using System.Collections.Generic;

namespace LTWeb_TBDT.Data
{
    public partial class TonKho
    {
        public int MaSanPham { get; set; }
        public int? SoLuongTon { get; set; }

        public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
    }
}
