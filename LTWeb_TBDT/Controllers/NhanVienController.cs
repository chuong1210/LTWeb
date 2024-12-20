﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using LTWeb_TBDT.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LTWeb_TBDT.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly string connectionString;

        public NhanVienController(IConfiguration configuration)
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
                string query = "SELECT * FROM NhanVien WHERE HoTen LIKE @SearchQuery";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + (searchQuery ?? string.Empty) + "%");


                SqlDataReader reader = command.ExecuteReader();

                List<NhanVien> nhanViens = new List<NhanVien>();

                while (reader.Read())
                {
                    int maNhanVien = Convert.ToInt32(reader["MaNhanVien"]);
                    string hoTen = reader["HoTen"].ToString();
                    DateTime ngaySinh = Convert.ToDateTime(reader["ngaySinh"]);
                    string soDienThoai = reader["SoDienThoai"].ToString();
                    string email = reader["Email"].ToString();
                    int maTaiKhoan = Convert.ToInt32(reader["MaTaiKhoan"]);

                    NhanVien nhanVien = new NhanVien(maNhanVien, hoTen, ngaySinh, soDienThoai, email, maTaiKhoan);
                    nhanViens.Add(nhanVien);
                }
                ViewData["SearchQuery"] = searchQuery;
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                return View(nhanViens);
            }
        }

        public IActionResult CreateNhanVien()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT MaTaiKhoan FROM TaiKhoan WHERE LoaiTaiKhoan = 'NhanVien'";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                List<SelectListItem> taiKhoanList = new List<SelectListItem>();

                while (reader.Read())
                {
                    taiKhoanList.Add(new SelectListItem
                    {
                        Value = reader["MaTaiKhoan"].ToString(),
                        Text = reader["MaTaiKhoan"].ToString()
                    });
                }

                ViewBag.TaiKhoanList = taiKhoanList;
            }

            return View();
        }

        // POST: KhachHang/CreateKhachHang
        [HttpPost]
        public IActionResult CreateNhanVien(NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO NhanVien (HoTen, NgaySinh, SoDienThoai, Email, MaTaiKhoan) VALUES( @HoTen, @NgaySinh, @SoDienThoai, @Email, @MaTaiKhoan)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@HoTen", nhanVien.HoTen);
                    command.Parameters.AddWithValue("@NgaySinh", nhanVien.NgaySinh);
                    command.Parameters.AddWithValue("@SoDienThoai", nhanVien.SoDienThoai);
                    command.Parameters.AddWithValue("@Email", nhanVien.Email);
                    command.Parameters.AddWithValue("@MaTaiKhoan", nhanVien.MaTaiKhoan);

                    command.ExecuteNonQuery();

                    TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
                }
                return RedirectToAction("Index");
            }

            return View(nhanVien);
        }

        public IActionResult EditNhanVien(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string queryKH = "SELECT * FROM NhanVien WHERE MaNhanVien = @Id";
                SqlCommand commandKH = new SqlCommand(queryKH, connection);
                commandKH.Parameters.AddWithValue("@Id", id);

                SqlDataReader readerKH = commandKH.ExecuteReader();
                NhanVien nhanVien = null;

                if (readerKH.Read())
                {
                    nhanVien = new NhanVien
                    {
                        MaNhanVien = Convert.ToInt32(readerKH["MaNhanVien"]),
                        HoTen = readerKH["HoTen"].ToString(),
                        Email = readerKH["Email"].ToString(),
                        SoDienThoai = readerKH["SoDienThoai"].ToString(),
                        NgaySinh = Convert.ToDateTime(readerKH["NgaySinh"].ToString()),
                        MaTaiKhoan = Convert.ToInt32(readerKH["MaTaiKhoan"])
                    };
                }
                readerKH.Close();

                if (nhanVien == null)
                {
                    return NotFound("Nhân viên không tồn tại.");
                }

                string queryTaiKhoan = "SELECT MaTaiKhoan FROM TaiKhoan WHERE LoaiTaiKhoan = 'NhanVien'";
                SqlCommand commandTaiKhoan = new SqlCommand(queryTaiKhoan, connection);
                SqlDataReader readerTaiKhoan = commandTaiKhoan.ExecuteReader();

                List<SelectListItem> taiKhoanList = new List<SelectListItem>();
                while (readerTaiKhoan.Read())
                {
                    taiKhoanList.Add(new SelectListItem
                    {
                        Value = readerTaiKhoan["MaTaiKhoan"].ToString(),
                        Text = readerTaiKhoan["MaTaiKhoan"].ToString()
                    });
                }
                readerTaiKhoan.Close();

                // Truyền danh sách mã tài khoản vào ViewBag
                ViewBag.TaiKhoanList = taiKhoanList;

                return View(nhanVien);
            }
        }

        [HttpPost]
        public IActionResult EditNhanVien(NhanVien nhanVien)
        {
            if (nhanVien == null || nhanVien.MaNhanVien <= 0)
            {
                TempData["ErrorMessage"] = "Thông tin nhân viên không hợp lệ!";
                return RedirectToAction("Index");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE NhanVien SET HoTen = @HoTen, Email = @Email, " +
                               "SoDienThoai = @SoDienThoai, NgaySinh = @NgaySinh, MaTaiKhoan = @MaTaiKhoan WHERE MaNhanVien = @MaNhanVien";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@HoTen", nhanVien.HoTen);
                command.Parameters.AddWithValue("@Email", nhanVien.Email);
                command.Parameters.AddWithValue("@SoDienThoai", nhanVien.SoDienThoai);
                command.Parameters.AddWithValue("@NgaySinh", nhanVien.NgaySinh);
                command.Parameters.AddWithValue("@MaTaiKhoan", nhanVien.MaTaiKhoan);
                command.Parameters.AddWithValue("@MaNhanVien", nhanVien.MaNhanVien);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    TempData["SuccessMessage"] = "Thông tin nhân viên đã được cập nhật thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin nhân viên!";
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Câu lệnh SQL để xóa sản phẩm theo ID
                    string query = "DELETE FROM NhanVien WHERE MaNhanVien = @Id";
                    SqlCommand command = new SqlCommand(query, connection);

                    // Thêm tham số ID vào câu lệnh SQL
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = command.ExecuteNonQuery();

                    // Kiểm tra xem có bản ghi nào bị ảnh hưởng
                    if (rowsAffected > 0)
                    {
                        TempData["SuccessMessage"] = "Xóa nhân viên thành công!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy nhân viên để xóa.";
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
