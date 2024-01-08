using AutoMapper;
using ECommerceMVC.Data;
using ECommerceMVC.Helpers;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceMVC.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context _context;
        private readonly IMapper _mapper;

        public KhachHangController(Hshop2023Context context,IMapper mapper)
        {
            _context= context;
            _mapper = mapper;
        }
        #region Register

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(RegisterVM model,IFormFile Hinh)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRandomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);

                    khachHang.HieuLuc = true;//xử lí khi dùng mail để active
                    khachHang.VaiTro = 0;

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHangs");
                    }
                    _context.Add(khachHang);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "HangHoa");
                }
            }catch(Exception ex)
            {

            }
            return View();
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model,string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachhang=_context.KhachHangs.SingleOrDefault(kh=>kh.MaKh==model.Username);
                if (khachhang == null)
                {
                    ModelState.AddModelError("Error", "Không tồn tại khách hàng");
                }
                else
                {
                    if (!khachhang.HieuLuc)
                    {
                        ModelState.AddModelError("Error", "Tài khoản bị khoá. Liên hệ admin để mở lại");
                    }
                    else
                    {
                        if(khachhang.MatKhau != model.Password.ToMd5Hash(khachhang.RandomKey))
                        {
                            ModelState.AddModelError("Error", "Sai mật khẩu");
                        }
                        else
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email,khachhang.Email),
                                new Claim(ClaimTypes.Name,khachhang.HoTen),
                                new Claim("CustomerID",khachhang.MaKh),

                                //claim - role 
                                new Claim(ClaimTypes.Role,"Customer")   
                            };
                            var claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimsPrincipal);

                            if(Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                    }
                }
            }
            return View();
        }
        #endregion
        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
