using System;
using System.Collections.Generic;

namespace LTWeb_TBDT.Data
{
    public partial class DanhMuc
    {
        public DanhMuc()
        {
            SanPhams = new HashSet<SanPham>();
        }

        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; } = null!;
        public string? MoTa { get; set; }

        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
