using System;
using System.Collections.Generic;

namespace LTWeb_TBDT.Data
{
    public partial class Admin
    {
        public int MaAdmin { get; set; }
        public int? MaTaiKhoan { get; set; }

        public virtual TaiKhoan? MaTaiKhoanNavigation { get; set; }
    }
}
