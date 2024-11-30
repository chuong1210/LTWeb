using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using LTWeb_TBDT.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LTWeb_TBDT.Controllers
{
    public class ManagerController : Controller
    {
        private readonly string connectionString;

        public ManagerController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        // Action Index
        public IActionResult Index(string searchQuery)
        {
            // Kết nối tới CSDL và lấy dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Thêm điều kiện tìm kiếm nếu có
                string query = "SELECT * FROM SanPham WHERE TenSanPham LIKE @SearchQuery";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SearchQuery", "%" + searchQuery + "%");

                SqlDataReader reader = command.ExecuteReader();

                List<SanPham> sanPhams = new List<SanPham>();

                while (reader.Read())
                {
                    int maSP = Convert.ToInt32(reader["MaSanPham"]);
                    string tenSP = reader["TenSanPham"].ToString();
                    string moTa = reader["MoTa"].ToString();
                    double giaBan = Convert.ToDouble(reader["GiaBan"]);
                    int soLuongTon = Convert.ToInt32(reader["SoLuongTon"]);
                    string hinhAnh = reader["HinhAnh"].ToString();
                    int maNSX = Convert.ToInt32(reader["MaNhaSanXuat"]);
                    int maDanhMuc = Convert.ToInt32(reader["MaDanhMuc"]);

                    SanPham sanPham = new SanPham(maSP, tenSP, moTa, giaBan, soLuongTon, hinhAnh, maNSX, maDanhMuc);
                    sanPhams.Add(sanPham);
                }
                ViewData["SearchQuery"] = searchQuery;
                ViewData["SuccessMessage"] = TempData["SuccessMessage"]; // Lấy thông báo từ TempData

                return View(sanPhams);
            }
        }

        // Action Create (GET) - Hiển thị form thêm sản phẩm
        public IActionResult CreateProduct()
        {
            // Kết nối tới cơ sở dữ liệu và lấy danh sách Nhà Sản Xuất và Danh Mục
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Lấy danh sách Nhà Sản Xuất
                string queryNSX = "SELECT MaNhaSanXuat, TenNhaSanXuat FROM NhaSanXuat";
                SqlCommand commandNSX = new SqlCommand(queryNSX, connection);
                SqlDataReader readerNSX = commandNSX.ExecuteReader();
                List<SelectListItem> nhaSanXuatList = new List<SelectListItem>();

                // Đọc dữ liệu và tạo các SelectListItem
                while (readerNSX.Read())
                {
                    nhaSanXuatList.Add(new SelectListItem
                    {
                        Value = readerNSX["MaNhaSanXuat"].ToString(), // Giả sử MaNhaSanXuat là kiểu số
                        Text = readerNSX["TenNhaSanXuat"].ToString()
                    });
                }
                readerNSX.Close();

                // Lấy danh sách Danh Mục
                string queryDM = "SELECT MaDanhMuc, TenDanhMuc FROM DanhMuc";
                SqlCommand commandDM = new SqlCommand(queryDM, connection);
                SqlDataReader readerDM = commandDM.ExecuteReader();
                List<SelectListItem> danhMucList = new List<SelectListItem>();

                // Đọc dữ liệu và tạo các SelectListItem
                while (readerDM.Read())
                {
                    danhMucList.Add(new SelectListItem
                    {
                        Value = readerDM["MaDanhMuc"].ToString(), // Giả sử MaDanhMuc là kiểu số
                        Text = readerDM["TenDanhMuc"].ToString()
                    });
                }

                readerDM.Close();

                // Nếu không có dữ liệu, gán cho ViewBag danh sách trống
                ViewBag.NhaSanXuatList = nhaSanXuatList.Any() ? nhaSanXuatList : new List<SelectListItem>();
                ViewBag.DanhMucList = danhMucList.Any() ? danhMucList : new List<SelectListItem>();
            }

            return View();
        }




        // Action Create (POST) - Xử lý khi người dùng submit form
        [HttpPost]
        public IActionResult CreateProduct(SanPham sanPham, IFormFile hinhAnhFile)
        {
            if (!ModelState.IsValid)
            {
                string hinhAnhPath = string.Empty;

                if (hinhAnhFile != null && hinhAnhFile.Length > 0)
                {
                    // Đường dẫn lưu trữ ảnh (có thể thay đổi theo yêu cầu)
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", hinhAnhFile.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        hinhAnhFile.CopyTo(stream);
                    }

                    hinhAnhPath = hinhAnhFile.FileName;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Câu lệnh insert vào bảng SanPham
                    string query = "INSERT INTO SanPham (TenSanPham, MoTa, GiaBan, SoLuongTon, HinhAnh, MaNhaSanXuat, MaDanhMuc) " +
                                   "VALUES (@TenSanPham, @MoTa, @GiaBan, @SoLuongTon, @HinhAnh, @MaNhaSanXuat, @MaDanhMuc)";
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@TenSanPham", sanPham.TenSP);
                    command.Parameters.AddWithValue("@MoTa", sanPham.MoTa);
                    command.Parameters.AddWithValue("@GiaBan", sanPham.GiaBan);
                    command.Parameters.AddWithValue("@SoLuongTon", sanPham.SoLuongTon);
                    command.Parameters.AddWithValue("@HinhAnh", hinhAnhPath);
                    command.Parameters.AddWithValue("@MaNhaSanXuat", sanPham.MaNSX);
                    command.Parameters.AddWithValue("@MaDanhMuc", sanPham.MaDanhMuc);

                    command.ExecuteNonQuery();
                }

                // Thông báo thành công và chuyển hướng đến trang danh sách sản phẩm
                TempData["SuccessMessage"] = "Sản phẩm đã được lưu thành công!";
                    return RedirectToAction("Index");
            }

            // Nếu có lỗi, tiếp tục hiển thị form và dữ liệu danh mục, nhà sản xuất
            return View(sanPham);
        }

    }
}
