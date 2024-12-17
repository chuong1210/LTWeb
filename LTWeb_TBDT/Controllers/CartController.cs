using LTWeb_TBDT.Data;
using LTWeb_TBDT.Helpers;
using LTWeb_TBDT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace LTWeb_TBDT.Controllers
{

    public class CartController : Controller
    {
        private readonly BanThietBiDienTuContext db;
        private readonly ConnectHoaDon _connectHoaDon;
        private readonly ConnnectChiTietHoaDon _connectChiTietHoaDon;
        private readonly VnpayPayment _vnpay;


        private const string CartSessionKey = "CartItems";
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(StaticMethod.CART_KEY) ?? new List<CartItem>();
        public CartController(BanThietBiDienTuContext context, ConnectHoaDon connectHoaDon, ConnnectChiTietHoaDon connectChiTietHoaDon, VnpayPayment vnpay)
        {
            db = context;
            _connectHoaDon = connectHoaDon;
            _connectChiTietHoaDon = connectChiTietHoaDon;

            _vnpay = vnpay;
            
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
        public IActionResult AddToCart(int id, int quantity = 1, bool update=false)
        {
            // Kiểm tra số lượng không hợp lệ
            if (quantity <= 0)
            {
                TempData["messageCart"] = "Số lượng không hợp lệ. Vui lòng chọn số lượng lớn hơn 0.";
                return RedirectToAction("Detail", "HangHoa", new { id });
            }

            // Lấy giỏ hàng từ session
            var gioHang = Cart;

            // Tìm sản phẩm trong giỏ
            var item = gioHang.SingleOrDefault(gh => gh.MaSanPham == id);

            // Nếu sản phẩm chưa có trong giỏ, lấy thông tin sản phẩm từ cơ sở dữ liệu
            if (item == null)
            {
                var hangHoa = db.SanPhams.SingleOrDefault(gh => gh.MaSanPham == id);
                if (hangHoa == null)
                {
                    TempData["message"] = $"Không tìm thấy mã {id} trong giỏ hàng";
                    return Redirect("/404");
                }

                // Kiểm tra xem số lượng người dùng yêu cầu có vượt quá tồn kho không
                if (quantity > hangHoa.SoLuongTon)
                {
                    TempData["messageCart"] = $"Số lượng yêu cầu vượt quá tồn kho. Chỉ còn {hangHoa.SoLuongTon} sản phẩm.";
                    return RedirectToAction("Detail", "HangHoa", new { id });
                }

                // Thêm sản phẩm vào giỏ
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
                // Kiểm tra số lượng trong giỏ có vượt quá tồn kho không
                var hangHoa = db.SanPhams.SingleOrDefault(gh => gh.MaSanPham == id);
                if (hangHoa == null)
                {
                    TempData["message"] = $"Không tìm thấy sản phẩm trong giỏ hàng.";
                    return Redirect("/404");
                }

                if (item.SoLuong + quantity > hangHoa.SoLuongTon)
                {
                    TempData["messageCart"] = $"Số lượng yêu cầu vượt quá tồn kho. Chỉ còn {hangHoa.SoLuongTon - item.SoLuong} sản phẩm.";
                    return RedirectToAction("Detail", "HangHoa", new { id });
                }

                // Cập nhật số lượng sản phẩm trong giỏ
                //item.SoLuong += quantity;
                if (update)
                {
                    item.SoLuong = quantity;  // Cập nhật số lượng nếu update = true
                }
                else
                {
                    item.SoLuong += quantity;  // Thêm số lượng mới nếu không phải update
                }
            }

            // Lưu giỏ hàng vào session
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
            ViewBag.TongTien = tongTien ;
            return View(gioHangList);
        }


		[Authorize]

        [HttpPost("CreatePaymentUrl")]
        public ActionResult CreatePaymentUrl(double moneyToPay, string description)
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch
                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = moneyToPay,
                    Description = description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = DateTime.Now,
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese
                };

                var paymentUrl = _vnpay._vnpay.GetPaymentUrl(request);
                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tạo URL thanh toán: " + ex.Message;
                return RedirectToAction("CheckOut"); // Quay lại trang checkout với thông báo lỗi
            }
        }




        [HttpGet("IpnAction")]
		public IActionResult IpnAction()
		{
			if (Request.QueryString.HasValue)
			{
				try
				{
					var paymentResult = _vnpay._vnpay.GetPaymentResult(Request.Query);
					if (paymentResult.IsSuccess)
					{
						// Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
						return Ok();
					}

					// Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
					return BadRequest("Thanh toán thất bại");
				}
				catch (Exception ex)
				{
					return BadRequest(ex.Message);
				}
			}

			return NotFound("Không tìm thấy thông tin thanh toán.");
		}


        public IActionResult OrderSuccess()
        {
            ViewData["OrderId"] = "12345";  // Thay bằng mã đơn hàng thực tế
            ViewData["TotalAmount"] = "500000";  // Tổng tiền đơn hàng

            return View();
        }

        public IActionResult OrderFailed()
        {
            ViewData["OrderId"] = "12345";  // Thay bằng mã đơn hàng thực tế
            ViewData["TotalAmount"] = "500000";  // Tổng tiền đơn hàng

            return View();
        }

        [HttpGet("Callback")]
        public ActionResult<string> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay._vnpay.GetPaymentResult(Request.Query);
                    var resultDescription = $"{paymentResult.Description}. {paymentResult.TransactionStatusCode}.";

                    if (paymentResult.IsSuccess)
                    {
                        // Cập nhật trạng thái đơn hàng thành công
                        TempData["Message"] = "Thanh toán thành công!";
                        return RedirectToAction("OrderSuccess", "Cart");
                    }
                    else
                    {
                        // Cập nhật trạng thái đơn hàng thất bại
                        TempData["Message"] = "Thanh toán thất bại!";
                        return RedirectToAction("OrderFailed", "Cart");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Đã xảy ra lỗi khi xử lý thanh toán.";
                    return BadRequest(ex.Message);
                }
            }
            return NotFound("Không tìm thấy thông tin thanh toán.");
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
