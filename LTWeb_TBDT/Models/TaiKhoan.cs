using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LTWeb_TBDT.Models
{
	public class TaiKhoan
	{
		private int maTaiKhoan;
		private string tenDangNhap;
		private string matKhau;
		private string loaiTaiKhoan;

		public global::System.Int32 MaTaiKhoan { get => maTaiKhoan; set => maTaiKhoan = value; }
		public global::System.String TenDangNhap { get => tenDangNhap; set => tenDangNhap = value; }
		public global::System.String MatKhau { get => matKhau; set => matKhau = value; }
		public global::System.String LoaiTaiKhoan { get => loaiTaiKhoan; set => loaiTaiKhoan = value; }
		public TaiKhoan()
		{}
	}
}
