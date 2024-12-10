using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using LTWeb_TBDT.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

                    KhachHang khachhang = new KhachHang(maKhachHang, hoTen, diaChi, soDienThoai, email, maTaiKhoan);
                    khachHangs.Add(khachhang);
                }
                ViewData["SearchQuery"] = searchQuery;
                ViewData["SuccessMessage"] = TempData["SuccessMessage"]; 

                return View(khachHangs);
            }
        }

        // GET: KhachHang/CreateKhachHang
        public IActionResult CreateKhachHang()
        {
            return View();
        }

        // POST: KhachHang/CreateKhachHang
        [HttpPost]
        public IActionResult CreateKhachHang(KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO KhachHang (HoTen, DiaChi, SoDienThoai, Email, MaTaiKhoan) VALUES( @HoTen, @DiaChi, @SoDienThoai, @Email, @MaTaiKhoan)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@HoTen", khachHang.HoTen);
                    command.Parameters.AddWithValue("@DiaChi", khachHang.DiaChi);
                    command.Parameters.AddWithValue("@SoDienThoai", khachHang.SoDienThoai);
                    command.Parameters.AddWithValue("@Email", khachHang.Email);
                    command.Parameters.AddWithValue("@MaTaiKhoan", khachHang.MaTaiKhoan);

                    command.ExecuteNonQuery();

                    TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
                }
                return RedirectToAction("Index");
            }

            return View(khachHang);
        }

        public IActionResult EditKhachHang(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Lấy thông tin khách hàng theo ID
                string queryKH = "SELECT * FROM KhachHang WHERE MaKhachHang = @Id";
                SqlCommand commandKH = new SqlCommand(queryKH, connection);
                commandKH.Parameters.AddWithValue("@Id", id);

                SqlDataReader readerKH = commandKH.ExecuteReader();
                KhachHang khachHang = null;

                if (readerKH.Read())
                {
                    khachHang = new KhachHang
                    {
                        MaKhachHang = Convert.ToInt32(readerKH["MaKhachHang"]),
                        HoTen = readerKH["HoTen"].ToString(),
                        Email = readerKH["Email"].ToString(),
                        SoDienThoai = readerKH["SoDienThoai"].ToString(),
                        DiaChi = readerKH["DiaChi"].ToString()
                    };
                }
                readerKH.Close();

                // Kiểm tra khách hàng có tồn tại
                if (khachHang == null)
                {
                    return NotFound("Khách hàng không tồn tại.");
                }

                return View(khachHang);
            }
        }


        [HttpPost]
        public IActionResult EditKhachHang(KhachHang khachHang)
        {
            if (khachHang == null || khachHang.MaKhachHang <= 0)
            {
                TempData["ErrorMessage"] = "Thông tin khách hàng không hợp lệ!";
                return RedirectToAction("Index");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Cập nhật thông tin khách hàng
                string query = "UPDATE KhachHang SET HoTen = @HoTen, Email = @Email, " +
                               "SoDienThoai = @SoDienThoai, DiaChi = @DiaChi WHERE MaKhachHang = @MaKhachHang";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@HoTen", khachHang.HoTen);
                command.Parameters.AddWithValue("@Email", khachHang.Email);
                command.Parameters.AddWithValue("@SoDienThoai", khachHang.SoDienThoai);
                command.Parameters.AddWithValue("@DiaChi", khachHang.DiaChi);
                command.Parameters.AddWithValue("@MaKhachHang", khachHang.MaKhachHang);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    TempData["SuccessMessage"] = "Thông tin khách hàng đã được cập nhật thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin khách hàng!";
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteKhachHang(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Câu lệnh SQL để xóa sản phẩm theo ID
                    string query = "DELETE FROM KhachHang WHERE MaKhachHang = @Id";
                    SqlCommand command = new SqlCommand(query, connection);

                    // Thêm tham số ID vào câu lệnh SQL
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = command.ExecuteNonQuery();

                    // Kiểm tra xem có bản ghi nào bị ảnh hưởng
                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Xóa khách hàng thành công!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy khách hàn để xóa.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
                }
            }

            return RedirectToAction("Index");
        }
    }
}
