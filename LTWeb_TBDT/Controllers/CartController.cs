using LTWeb_TBDT.Data;
using LTWeb_TBDT.Helpers;
using LTWeb_TBDT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace LTWeb_TBDT.Controllers
{

    public class CartController : Controller
    {
        private readonly BanThietBiDienTuContext db;
        private readonly ConnectHoaDon _connectHoaDon;
        private readonly ConnnectChiTietHoaDon _connectChiTietHoaDon;


        private const string CartSessionKey = "CartItems";
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(StaticMethod.CART_KEY) ?? new List<CartItem>();
        public CartController(BanThietBiDienTuContext context, ConnectHoaDon connectHoaDon, ConnnectChiTietHoaDon connectChiTietHoaDon)
        {
            db = context;
            _connectHoaDon = connectHoaDon;
            _connectChiTietHoaDon = connectChiTietHoaDon;

        }
        public IActionResult Index()
        {
            var user = User;
            if (!user.IsInRole("KhachHang"))
            {
                // Trả về lỗi hoặc chuyển hướng đến trang không được phép
                return Unauthorized();
            }
            return View(Cart);
        }


        [Authorize]

        public IActionResult AddToCart(int id, int quantity = 1)

        {
            //if (!user.IsInRole("User"))
            //{
            //    // Trả về lỗi hoặc chuyển hướng đến trang không được phép
            //    return Unauthorized();
            //}
            if (quantity <= 0)
            {
                TempData["messageCart"] = "Số lượng không hợp lệ. Vui lòng chọn số lượng lớn hơn 0.";
                return RedirectToAction("Detail", "HangHoa", new { id });

            }
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(gh => gh.MaSanPham == id);
            if (item == null)
            {
                var hangHoa = db.SanPhams.SingleOrDefault(gh => gh.MaSanPham == id);
                if (hangHoa == null)
                {
                    TempData["message"] = $"Không tìm thấy mã {id} trong giỏ hàng";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaSanPham = hangHoa.MaSanPham,
                    TenSanPham = hangHoa.TenSanPham,
                    GiaBan = hangHoa.GiaBan,
                    HinhAnh = hangHoa.HinhAnh ?? string.Empty,
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                //   item.SoLuong += quantity;
                item.SoLuong = quantity;

            }

            HttpContext.Session.Set(StaticMethod.CART_KEY, gioHang);

            return RedirectToAction("Index");
        }
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaSanPham == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(StaticMethod.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CheckOut()
        {

            // Lấy giỏ hàng từ session
            List<CartItem> gioHangList = HttpContext.Session.Get<List<CartItem>>(StaticMethod.CART_KEY) ?? new List<CartItem>();

            // Kiểm tra xem giỏ hàng có sản phẩm không
            if (gioHangList == null || gioHangList.Count == 0)
            {
                TempData["Error"] = "Không thể thanh toán vì giỏ hàng trống!";
                return RedirectToAction("Index", "Home"); // Nếu giỏ hàng trống, chuyển về trang chủ
            }

            double tongTien = (double)gioHangList.Sum(sp => sp.GiaBan * sp.SoLuong);

            // Gửi giỏ hàng và tổng tiền sang view để hiển thị
            ViewBag.TongTien = tongTien;
            return View(gioHangList);
        }
        [Authorize]
        public IActionResult CheckOut(IFormCollection form)
        {
            List<CartItem> gioHangList = HttpContext.Session.Get<List<CartItem>>(StaticMethod.CART_KEY) ?? new List<CartItem>();

            double totalAmount = (double)gioHangList.Sum(sp => sp.ThanhTien);


            int? maTK = HttpContext.Session.GetInt32("MaTK");
            var maKhachHangClaim = User.Claims.FirstOrDefault(c => c.Type == "MaKhachHang").Value;
           HoaDon hoaDon = new HoaDon();

            if (maKhachHangClaim != null)
            {


                 hoaDon = new HoaDon
                {
                    NgayDatHang = DateTime.Now,
                    MaKhachHang = int.Parse(maKhachHangClaim),
                    TongTien = (decimal?)totalAmount,
                    TrangThai = "Chưa duyệt"
                 };

            }
            else

            {
                 hoaDon = new HoaDon
                {
                    NgayDatHang = DateTime.Now,
                    MaKhachHang = null,
                    TongTien = (decimal?)totalAmount,
                    TrangThai = "Chưa duyệt"
                 };

            }
            _connectHoaDon.ThemHoaDon(hoaDon);

            // Thêm chi tiết hóa đơn vào cơ sở dữ liệu
            foreach (var item in gioHangList)
            {
                ChiTietHoaDon chiTietHoaDon = new ChiTietHoaDon
                {
                    MaHoaDon = hoaDon.MaHoaDon,
                    MaSanPham = item.MaSanPham,
                    SoLuong = item.SoLuong,
                    GiaBan = item.GiaBan,
                };
                _connectChiTietHoaDon.ThemChiTietHoaDon(chiTietHoaDon);
            }



            HttpContext.Session.Remove(StaticMethod.CART_KEY);

            TempData["Message"] = "Thanh toán thành công! Đơn hàng đã được tạo.";
            return RedirectToAction("Index", "Home");
        }




         [Authorize(Roles = "KhachHang")]
    public async Task<IActionResult> History()
    {
        // Lấy MaKhachHang từ tài khoản người dùng đã đăng nhập
        var userId = User.Identity.Name;  // Lấy username của người dùng
                                          //var customer = await db.KhachHangs
                                          //                             .FirstOrDefaultAsync(kh => kh.MaTaiKhoan == (from tk in db.TaiKhoans
                                          //                                                                         where tk.TenDangNhap == userId
                                          //                                                                         select tk.MaTaiKhoan).FirstOrDefault());
            var taiKhoan = await db.TaiKhoans
                           .Where(tk => tk.TenDangNhap == userId)
                           .Select(tk => tk.MaTaiKhoan)
                           .FirstOrDefaultAsync();

            var customer = await db.KhachHangs
                                   .FirstOrDefaultAsync(kh => kh.MaTaiKhoan == taiKhoan);


            if (customer == null)
        {
            return NotFound();
        }

        // Lấy danh sách các hóa đơn của khách hàng
        var orders = await db.HoaDons
                                    .Where(hd => hd.MaKhachHang == customer.MaKhachHang)
                                    .OrderByDescending(hd => hd.NgayDatHang)  // Sắp xếp theo ngày đặt hàng giảm dần
                                    .ToListAsync();

        return View(orders);
    }

    // Chi tiết hóa đơn, có thể xem chi tiết sản phẩm đã mua
    [Authorize(Roles = "KhachHang")]
    public async Task<IActionResult> Detail(int id)
    {
        var order = await db.HoaDons
                                  .Include(hd => hd.ChiTietHoaDons)
                                  .ThenInclude(cthd => cthd.MaSanPhamNavigation)
                                  .FirstOrDefaultAsync(hd => hd.MaHoaDon == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}

    
}
