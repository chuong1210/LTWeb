using Microsoft.Data.SqlClient;

namespace LTWeb_TBDT.Data
{
    public class ConnnectChiTietHoaDon
    {
        private readonly string connectionString;


        public ConnnectChiTietHoaDon(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        // Thêm chi tiết hóa đơn
        public void ThemChiTietHoaDon(ChiTietHoaDon chiTietHoaDon)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO ChiTietHoaDon (MaHoaDon, MaSanPham, SoLuong, GiaBan)
                        VALUES (@MaHoaDon, @MaSanPham, @SoLuong, @GiaBan)";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaHoaDon", chiTietHoaDon.MaHoaDon);
                        cmd.Parameters.AddWithValue("@MaSanPham", chiTietHoaDon.MaSanPham);
                        cmd.Parameters.AddWithValue("@SoLuong", chiTietHoaDon.SoLuong);
                        cmd.Parameters.AddWithValue("@GiaBan", chiTietHoaDon.GiaBan);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm chi tiết hóa đơn: " + ex.Message);
            }
        }

        // Lấy danh sách chi tiết hóa đơn theo MaHoaDon
        public List<ChiTietHoaDon> LayDanhSachChiTietHoaDon(int maHoaDon)
        {
            List<ChiTietHoaDon> ds = new List<ChiTietHoaDon>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM ChiTietHoaDon WHERE MaHoaDon = @MaHoaDon";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ChiTietHoaDon chiTiet = new ChiTietHoaDon
                                {
                                    MaHoaDon = (int)reader["MaHoaDon"],
                                    MaSanPham = (int)reader["MaSanPham"],
                                    SoLuong = (int)reader["SoLuong"],
                                    GiaBan = (decimal)reader["GiaBan"],
                                    ThanhTien = reader["ThanhTien"] is DBNull ? (decimal?)null : (decimal)reader["ThanhTien"]
                                };
                                ds.Add(chiTiet);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách chi tiết hóa đơn: " + ex.Message);
            }

            return ds;
        }

        // Sửa chi tiết hóa đơn
        public void SuaChiTietHoaDon(ChiTietHoaDon chiTietHoaDon)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        UPDATE ChiTietHoaDon
                        SET SoLuong = @SoLuong, 
                            GiaBan = @GiaBan, 
                            ThanhTien = @ThanhTien
                        WHERE MaHoaDon = @MaHoaDon AND MaSanPham = @MaSanPham";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaHoaDon", chiTietHoaDon.MaHoaDon);
                        cmd.Parameters.AddWithValue("@MaSanPham", chiTietHoaDon.MaSanPham);
                        cmd.Parameters.AddWithValue("@SoLuong", chiTietHoaDon.SoLuong);
                        cmd.Parameters.AddWithValue("@GiaBan", chiTietHoaDon.GiaBan);
                        cmd.Parameters.AddWithValue("@ThanhTien", chiTietHoaDon.ThanhTien ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi sửa chi tiết hóa đơn: " + ex.Message);
            }
        }

        // Xóa chi tiết hóa đơn
        public void XoaChiTietHoaDon(int maHoaDon, int maSanPham)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM ChiTietHoaDon WHERE MaHoaDon = @MaHoaDon AND MaSanPham = @MaSanPham";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                        cmd.Parameters.AddWithValue("@MaSanPham", maSanPham);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa chi tiết hóa đơn: " + ex.Message);
            }
        }
    }
}
