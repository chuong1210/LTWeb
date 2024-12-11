using Microsoft.Data.SqlClient;

namespace LTWeb_TBDT.Data
{
    public class ConnectHoaDon
    {
        private readonly string connectionString;

        public ConnectHoaDon(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public void ThemHoaDon(HoaDon hoaDon)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO HoaDon (MaKhachHang, NgayDatHang, TongTien, TrangThai)
                        VALUES (@MaKhachHang, @NgayDatHang, @TongTien, @TrangThai);
                        SELECT SCOPE_IDENTITY();";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaKhachHang", hoaDon.MaKhachHang ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NgayDatHang", hoaDon.NgayDatHang);
                        cmd.Parameters.AddWithValue("@TongTien", hoaDon.TongTien ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TrangThai", hoaDon.TrangThai ?? (object)DBNull.Value);

                        // Lấy ID của hóa đơn vừa thêm
                        hoaDon.MaHoaDon = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm hóa đơn: " + ex.Message);
            }
        }

        // Lấy danh sách hóa đơn
        public List<HoaDon> LayDanhSachHoaDon()
        {
            List<HoaDon> danhSach = new List<HoaDon>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM HoaDon ORDER BY NgayDatHang DESC";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HoaDon hoaDon = new HoaDon
                                {
                                    MaHoaDon = (int)reader["MaHoaDon"],
                                    MaKhachHang = reader["MaKhachHang"] is DBNull ? (int?)null : (int)reader["MaKhachHang"],
                                    NgayDatHang = (DateTime)reader["NgayDatHang"],
                                    TongTien = reader["TongTien"] is DBNull ? (decimal?)null : (decimal)reader["TongTien"],
                                    TrangThai = reader["TrangThai"].ToString()
                                }; danhSach.Add(hoaDon);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách hóa đơn: " + ex.Message);
            }

            return danhSach;
        }

        // Sửa hóa đơn
        public void SuaHoaDon(HoaDon hoaDon)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        UPDATE HoaDon
                        SET MaKhachHang = @MaKhachHang,
                            NgayDatHang = @NgayDatHang,
                            TongTien = @TongTien,
                            TrangThai = @TrangThai
                        WHERE MaHoaDon = @MaHoaDon";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaKhachHang", hoaDon.MaKhachHang ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NgayDatHang", hoaDon.NgayDatHang);
                        cmd.Parameters.AddWithValue("@TongTien", hoaDon.TongTien ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TrangThai", hoaDon.TrangThai ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MaHoaDon", hoaDon.MaHoaDon);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi sửa hóa đơn: " + ex.Message);
            }
        }

        // Xóa hóa đơn
        public void XoaHoaDon(int maHoaDon)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM HoaDon WHERE MaHoaDon = @MaHoaDon";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa hóa đơn: " + ex.Message);
            }
        }

        // Lấy hóa đơn theo mã khách hàng
        public List<HoaDon> LayHoaDonTheoKhachHang(int maKhachHang)
        {
            List<HoaDon> danhSach = new List<HoaDon>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM HoaDon WHERE MaKhachHang = @MaKhachHang ORDER BY NgayDatHang DESC";
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HoaDon hoaDon = new HoaDon
                                {
                                    MaHoaDon = (int)reader["MaHoaDon"],
                                    MaKhachHang = reader["MaKhachHang"] is DBNull ? (int?)null : (int)reader["MaKhachHang"],
                                    NgayDatHang = (DateTime)reader["NgayDatHang"],
                                    TongTien = reader["TongTien"] is DBNull ? (decimal?)null : (decimal)reader["TongTien"],
                                    TrangThai = reader["TrangThai"].ToString()
                                };
                                danhSach.Add(hoaDon);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy hóa đơn theo khách hàng: " + ex.Message);
            }

            return danhSach;
        }
    }
}