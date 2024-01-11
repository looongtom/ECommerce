using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Helpers;
using Microsoft.AspNetCore.Authorization;
using ECommerceMVC.Services;

namespace ECommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IVnPayService _vnPayService;

        public CartController(Hshop2023Context context, IVnPayService vnPayService)
        {
            db = context;
            _vnPayService = vnPayService;
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        [Authorize]
        public IActionResult Index()
        {
            return View(Cart);
        }

        [Authorize]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item == null)
            {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaHh = hangHoa.MaHh,
                    TenHh = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? string.Empty,
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return RedirectToAction("/");
            }
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model, string payment = "COD")
        {
            if (ModelState.IsValid)
            {
                if (payment == "Thanh toán VNPay")
                {
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = Cart.Sum(p => p.ThanhTien),
                        CreatedDate = DateTime.Now,
                        Description = $"{model.HoTen} {model.DienThoai} ",
                        FullName = model.HoTen,
                        OrderId = new Random().Next(1000, 10000)
                    };
                    return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
                }

                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(p => p.MaKh == customerId);
                }
                var hoadon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "Standard",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu,
                };
                db.Database.BeginTransaction();

                try
                {
                    db.Database.CommitTransaction();
                    db.Add(hoadon);
                    db.SaveChanges();

                    var chitiethoadon = new List<ChiTietHd>();
                    foreach (var item in Cart)
                    {
                        chitiethoadon.Add(new ChiTietHd
                        {
                            MaHd = hoadon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            GiamGia = 0
                        });
                    }
                    db.AddRange(chitiethoadon);
                    db.SaveChanges();
                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                    return View("Success");
                }
                catch
                {
                    db.Database.RollbackTransaction();
                }
            }
            return View(Cart);
        }

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }
		[Authorize]
		public IActionResult PaymentSuccess()
		{
			return View("Success");
		}
		[Authorize]

        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán thất bại: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            //save db

            HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }
    }
}