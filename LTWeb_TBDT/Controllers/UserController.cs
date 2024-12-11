using LTWeb_TBDT.Data;
using LTWeb_TBDT.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LTWeb_TBDT.Models;
using Microsoft.AspNetCore.Authorization;

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

            //    if (User.IsInRole("KhachHang"))
            //    {
            //    }

            //    // Nếu không phải khách hàng, chuyển hướng đến trang quản lý cho manager
            //    return RedirectToAction("Home", "Manager"); //
            //}
        }
		public IActionResult Login()
		{
            if (User.Identity.IsAuthenticated)
            {
                // Nếu người dùng đã đăng nhập, kiểm tra vai trò
                if (User.IsInRole("KhachHang"))
                {
                    return RedirectToAction("Index", "Home"); // Điều hướng về trang Home nếu là khách hàng
                }
                else if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("DashBoard", "Manager"); // Điều hướng về trang quản lý nếu là admin
                }
            }

            // Nếu chưa đăng nhập, chuyển đến trang đăng nhập
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string tenDangNhap, string matKhau)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string ltk = "";
                await connection.OpenAsync();

                // Truy vấn tài khoản
                string query = "SELECT * FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap AND MatKhau = @MatKhau";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@TenDangNhap", tenDangNhap ?? string.Empty);
                command.Parameters.AddWithValue("@MatKhau", matKhau ?? string.Empty);

                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    string loaiTaiKhoan = reader["LoaiTaiKhoan"].ToString();
                    ltk = loaiTaiKhoan;
                    int maTaiKhoan = Convert.ToInt32(reader["MaTaiKhoan"]);
                    int? maKhachHang = null;

                    // Nếu là Khách hàng, truy vấn MaKhachHang từ bảng KhachHang
                    if (loaiTaiKhoan == "KhachHang")
                    {
                        reader.Close(); // Đóng reader trước khi thực hiện truy vấn mới

                        string khachHangQuery = "SELECT MaKhachHang FROM KhachHang WHERE MaTaiKhoan = @MaTaiKhoan";
                        SqlCommand khachHangCommand = new SqlCommand(khachHangQuery, connection);
                        khachHangCommand.Parameters.AddWithValue("@MaTaiKhoan", maTaiKhoan);

                        SqlDataReader khachHangReader = await khachHangCommand.ExecuteReaderAsync();
                        if (khachHangReader.HasRows && await khachHangReader.ReadAsync())
                        {
                            maKhachHang = Convert.ToInt32(khachHangReader["MaKhachHang"]);
                        }
                    }

                    // Tạo Claims cho người dùng
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, tenDangNhap),
                new Claim(ClaimTypes.Role, loaiTaiKhoan),
                new Claim("MaTaiKhoan", maTaiKhoan.ToString())
            };

                    if (maKhachHang.HasValue)
                    {
                        claims.Add(new Claim("MaKhachHang", maKhachHang.Value.ToString()));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true // Duy trì phiên đăng nhập
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    TempData["SuccessMessage"] = "Đăng nhập thành công!";

                    // Điều hướng dựa trên loại tài khoản
                    if (loaiTaiKhoan == "Admin")
                    {
                        return RedirectToAction("DashBoard", "Manager");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewData["ErrorMessage"] = "Sai tên đăng nhập hoặc mật khẩu.";
                    
                        return View();
                  
                }
            }
        }
        [Authorize]

        public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login", "User");
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


