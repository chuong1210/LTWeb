using System;
using System.Collections.Generic;

namespace LTWeb_TBDT.Data
{
    public partial class NhanVien
    {
        public int MaNhanVien { get; set; }
        public string HoTen { get; set; } = null!;
        public DateTime? NgaySinh { get; set; }
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public int? MaTaiKhoan { get; set; }

        public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }
    }
}
