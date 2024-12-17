namespace LTWeb_TBDT.Data
{
	using Microsoft.AspNetCore.Mvc;
	using VNPAY.NET;
	using VNPAY.NET.Enums;
	using VNPAY.NET.Models;
	using VNPAY.NET.Utilities;

	public class VnpayPayment
	{
		public string _tmnCode;
		public string _hashSecret;
		public string _baseUrl;
		public string _callbackUrl;

		public readonly IVnpay _vnpay;
        public VnpayPayment(IConfiguration configuration)
        {
            // Lấy thông tin từ appsettings.json hoặc các cấu hình khác
            _tmnCode = configuration["VnPay:TmnCode"];
            _hashSecret = configuration["VnPay:HashSecret"];
            _baseUrl = configuration["VnPay:BaseUrl"];
            _callbackUrl = configuration["VnPay:CallbackUrl"];

            _vnpay = new Vnpay();
            _vnpay.Initialize(_tmnCode, _hashSecret, _baseUrl, _callbackUrl);
        }

    }
}
