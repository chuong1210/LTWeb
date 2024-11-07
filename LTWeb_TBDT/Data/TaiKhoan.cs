using System;
using System.Collections.Generic;

namespace LTWeb_TBDT.Data
{
    public partial class TaiKhoan
    {
        public TaiKhoan()
        {
            Admins = new HashSet<Admin>();
            KhachHangs = new HashSet<KhachHang>();
            NhanViens = new HashSet<NhanVien>();
        }

        public int MaTaiKhoan { get; set; }
        public string TenDangNhap { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
        public string LoaiTaiKhoan { get; set; } = null!;

        public virtual ICollection<Admin> Admins { get; set; }
        public virtual ICollection<KhachHang> KhachHangs { get; set; }
        public virtual ICollection<NhanVien> NhanViens { get; set; }
    }
}
