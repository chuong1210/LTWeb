using LTWeb_TBDT.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LTWeb_TBDT.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly BanThietBiDienTuContext db;
        private readonly ConnectHoaDon _connectHoaDon;
        private readonly ConnnectChiTietHoaDon _connectChiTietHoaDon;

        public HoaDonController(BanThietBiDienTuContext context, ConnectHoaDon connectHoaDon, ConnnectChiTietHoaDon connectChiTietHoaDon)
        {
            db = context;
            _connectHoaDon = connectHoaDon;
            _connectChiTietHoaDon = connectChiTietHoaDon;

        }
        public IActionResult Index(int? id)
        {
            if (id.HasValue)
            {
                var hoadon = db.HoaDons
                               .Include(hd => hd.ChiTietHoaDons) // Bao gồm chi tiết hóa đơn
                               .ThenInclude(cthd => cthd.MaSanPhamNavigation) // Bao gồm thông tin sản phẩm
                               .FirstOrDefault(hd => hd.MaHoaDon == id);

                if (hoadon != null && (hoadon.TrangThai == null || hoadon.TrangThai.ToLower() != "đã duyệt"))
                {
                    // Cập nhật trạng thái hóa đơn
                    hoadon.TrangThai = "Đã duyệt";

                    // Duyệt qua các chi tiết hóa đơn để trừ số lượng tồn kho
                    foreach (var chiTiet in hoadon.ChiTietHoaDons)
                    {
                        var sanPham = chiTiet.MaSanPhamNavigation;
                        if (sanPham != null)
                        {
                            // Trừ số lượng tồn kho
                            sanPham.SoLuongTon -= chiTiet.SoLuong;

                            // Đảm bảo số lượng tồn không âm
                            if (sanPham.SoLuongTon < 0)
                            {
                                sanPham.SoLuongTon = 0;
                            }

                            // Cập nhật thông tin sản phẩm
                            db.Update(sanPham);
                        }
                    }

                    // Cập nhật hóa đơn
                    db.Update(hoadon);
                    db.SaveChanges();
                }
            }

            return View(db.HoaDons.ToList());
        }

        public IActionResult Detail(int id)
        {

            {
                var order = db.HoaDons
                                          .Include(hd => hd.ChiTietHoaDons)
                                          .ThenInclude(cthd => cthd.MaSanPhamNavigation)
                                          .FirstOrDefault(hd => hd.MaHoaDon == id);

                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }


        }
    }
}
