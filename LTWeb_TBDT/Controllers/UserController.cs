using LTWeb_TBDT.Data;
using LTWeb_TBDT.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LTWeb_TBDT.Controllers
{
	public class UserController : Controller
	{
		private readonly string connectionString;

		public UserController(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(string tenDangNhap, string matKhau)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				await connection.OpenAsync();

				// Câu lệnh SQL với tham số
				string query = "SELECT * FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap AND MatKhau = @MatKhau";
				SqlCommand command = new SqlCommand(query, connection);

				// Thêm giá trị cho tham số
				command.Parameters.AddWithValue("@TenDangNhap", tenDangNhap ?? string.Empty); // Kiểm tra null
				command.Parameters.AddWithValue("@MatKhau", matKhau ?? string.Empty);

				SqlDataReader reader = await command.ExecuteReaderAsync();

				if (reader.HasRows)
				{
					// Xử lý khi tìm thấy tài khoản
					TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    return RedirectToAction("Index", "Home");
                }
				else
				{
					// Xử lý khi không tìm thấy tài khoản
					ViewData["ErrorMessage"] = "Sai tên đăng nhập hoặc mật khẩu.";
					return View();
				}
			}

		}

        public IActionResult Signup()
        {
            return View(); // Hiển thị form đăng ký
        }

        [HttpPost]
        public async Task<IActionResult> Signup(string tenDangNhap, string matKhau, string hoTen, string email, string soDienThoai, string diaChi)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // 1. Thêm tài khoản mới vào bảng TaiKhoan
                    string insertTaiKhoanQuery = @"
                INSERT INTO TaiKhoan (TenDangNhap, MatKhau, LoaiTaiKhoan) 
                OUTPUT INSERTED.MaTaiKhoan 
                VALUES (@TenDangNhap, @MatKhau, 'KhachHang')";
                    SqlCommand taiKhoanCommand = new SqlCommand(insertTaiKhoanQuery, connection);
                    taiKhoanCommand.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                    taiKhoanCommand.Parameters.AddWithValue("@MatKhau", matKhau);

                    // Lấy MaTaiKhoan vừa được tạo
                    int maTaiKhoan = (int)await taiKhoanCommand.ExecuteScalarAsync();

                    // 2. Thêm thông tin khách hàng vào bảng KhachHang
                    string insertKhachHangQuery = @"
                INSERT INTO KhachHang (HoTen, DiaChi, SoDienThoai, Email, MaTaiKhoan) 
                OUTPUT INSERTED.MaKhachHang
                VALUES (@HoTen, @DiaChi, @SoDienThoai, @Email, @MaTaiKhoan)";
                    SqlCommand khachHangCommand = new SqlCommand(insertKhachHangQuery, connection);
                    khachHangCommand.Parameters.AddWithValue("@HoTen", hoTen);
                    khachHangCommand.Parameters.AddWithValue("@DiaChi", diaChi);
                    khachHangCommand.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                    khachHangCommand.Parameters.AddWithValue("@Email", email);
                    khachHangCommand.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

                    await khachHangCommand.ExecuteNonQueryAsync();

                    TempData["SuccessMessage"] = "Đăng ký tài khoản khách hàng thành công!";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return View();
            }
        }

    }

}


