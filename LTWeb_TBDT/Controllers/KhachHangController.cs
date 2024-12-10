using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using LTWeb_TBDT.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LTWeb_TBDT.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly string connectionString;

        public KhachHangController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IActionResult Index(string searchQuery)
        {
            // Kết nối tới CSDL và lấy dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Thêm điều kiện tìm kiếm nếu có
                string query = "SELECT * FROM KhachHang WHERE HoTen LIKE @SearchQuery";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + (searchQuery ?? string.Empty) + "%");


                SqlDataReader reader = command.ExecuteReader();

                List<KhachHang> khachHangs = new List<KhachHang>();

                while (reader.Read())
                {
                    int maKhachHang = Convert.ToInt32(reader["MaKhachHang"]);
                    string hoTen = reader["HoTen"].ToString();
                    string diaChi = reader["DiaChi"].ToString();
                    string soDienThoai = reader["SoDienThoai"].ToString();
                    string email = reader["Email"].ToString();
                    int maTaiKhoan = Convert.ToInt32(reader["MaTaiKhoan"]);

                    KhachHang khachhang = new KhachHang();
                    khachHangs.Add(khachhang);
                }
                ViewData["SearchQuery"] = searchQuery;
                ViewData["SuccessMessage"] = TempData["SuccessMessage"]; // Lấy thông báo từ TempData

                return View(khachHangs);
            }
        }
    }
}
