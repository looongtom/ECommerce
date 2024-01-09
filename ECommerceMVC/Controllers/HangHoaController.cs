using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ECommerceMVC.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context db;

        public HangHoaController(Hshop2023Context context) { db = context; }
        public IActionResult Index(int? loai, string? querySearch, int? rangeInput, string? sortQuery)
        {
            string? querySearchSesion = HttpContext.Session.GetString("querySearch");
            int? rangeInputSesion = HttpContext.Session.GetInt32("rangeInput");
            string? sortQuerySesion = HttpContext.Session.GetString("sortQuery");
            int? loaiSession=HttpContext.Session.GetInt32("loai");

            var hangHoas=db.HangHoas.AsQueryable();

            if (loai != 0)
            {

                if (querySearch != null)
                {
                    HttpContext.Session.SetString("querySearch", querySearch);
                    hangHoas = hangHoas.Where(p => p.TenHh.Contains(querySearch));
                }
                if (rangeInput.HasValue)
                {
                    HttpContext.Session.SetInt32("rangeInput", rangeInput ?? 0);
                    hangHoas = hangHoas.Where(p => p.DonGia >= rangeInput);
                }
                else if (rangeInputSesion.HasValue && rangeInput == null)
                {
                    hangHoas = hangHoas.Where(p => p.DonGia >= rangeInputSesion);
                }
                if (sortQuery != null)
                {
                    HttpContext.Session.SetString("sortQuery", sortQuery);
                    if (sortQuery == "price-asc")
                    {
                        hangHoas = hangHoas.OrderBy(p => p.DonGia);
                    }
                    else if (sortQuery == "price-desc")
                    {
                        hangHoas = hangHoas.OrderByDescending(p => p.DonGia);
                    }
                }
                else if (sortQuerySesion != null && sortQuery == null)
                {
                    if (sortQuerySesion == "price-asc")
                    {
                        hangHoas = hangHoas.OrderBy(p => p.DonGia);
                    }
                    else if (sortQuerySesion == "price-desc")
                    {
                        hangHoas = hangHoas.OrderByDescending(p => p.DonGia);
                    }
                }

                if (loai.HasValue)
                {
                    HttpContext.Session.SetInt32("loai", loai ?? 0);
                    hangHoas = hangHoas.Where(hh => hh.MaLoai == loai.Value);
                }
                else if (!loai.HasValue && loaiSession != null && loaiSession!=0)
                {
                    hangHoas = hangHoas.Where(hh => hh.MaLoai == loaiSession.Value);
                }
            }


            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh=p.MaHh,
                TenHh=p.TenHh,
                DonGia=p.DonGia ?? 0,
                Hinh=p.Hinh ?? "",
                MoTa=p.MoTaDonVi ?? "",
                TenLoai=p.MaLoaiNavigation.TenLoai
            });
            if(loai.HasValue) HttpContext.Session.SetInt32("loai", loai?? 0);
            return View(result);
        }

  //      public IActionResult Search(string? query)
  //      {
  //          int? rangeInput = HttpContext.Session.GetInt32("rangeInput");
  //          string? sortQuery = HttpContext.Session.GetString("sortQuery");
  //          int? loai = HttpContext.Session.GetInt32("loai");

  //          var hangHoas = db.HangHoas.AsQueryable();

  //          if (loai.HasValue)
  //          {
  //                 hangHoas = hangHoas.Where(hh => hh.MaLoai == loai.Value);
  //          }
  //          if (rangeInput.HasValue)
  //          {
  //              hangHoas = hangHoas.Where(p => p.DonGia >= rangeInput);
  //          }
  //          if (sortQuery != null)
  //          {
  //              if (sortQuery == "price-asc")
  //              {
  //                  hangHoas = hangHoas.OrderBy(p => p.DonGia);
  //              }
  //              else if (sortQuery == "price-desc")
  //              {
  //                  hangHoas = hangHoas.OrderByDescending(p => p.DonGia);
  //              }
  //          }

  //          if (query != null)
  //          {
  //              hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
  //          }

  //          var result = hangHoas.Select(p => new HangHoaVM
  //          {
  //              MaHh = p.MaHh,
  //              TenHh = p.TenHh,
  //              DonGia = p.DonGia ?? 0,
  //              Hinh = p.Hinh ?? "",
  //              MoTa = p.MoTaDonVi ?? "",
  //              TenLoai = p.MaLoaiNavigation.TenLoai
  //          });
  //          HttpContext.Session.SetString("querySearch", query);
  //          return View(result);
  //      }

  //      public IActionResult FilterByPrice(int rangeInput)
  //      {
  //          string? querySearch = HttpContext.Session.GetString("querySearch");
  //          string? sortQuery = HttpContext.Session.GetString("sortQuery");
  //          int? loai = HttpContext.Session.GetInt32("loai");

  //          var hangHoas =db.HangHoas.Where(p=>p.DonGia >= rangeInput);

  //          if (loai.HasValue)
  //          {
  //              hangHoas = hangHoas.Where(hh => hh.MaLoai == loai.Value);
  //          }
  //          if (sortQuery != null)
  //          {
  //              if (sortQuery == "price-asc")
  //              {
  //                  hangHoas = hangHoas.OrderBy(p => p.DonGia);
  //              }
  //              else if (sortQuery == "price-desc")
  //              {
  //                  hangHoas = hangHoas.OrderByDescending(p => p.DonGia);
  //              }
  //          }
  //          if (querySearch != null)
  //          {
  //              hangHoas = hangHoas.Where(p => p.TenHh.Contains(querySearch));
  //          }

  //          var result=hangHoas.Select(p=>new HangHoaVM
  //          {
		//		MaHh=p.MaHh,
		//		TenHh=p.TenHh,
		//		DonGia=p.DonGia ?? 0,
		//		Hinh=p.Hinh ?? "",
		//		MoTa=p.MoTaDonVi ?? "",
		//		TenLoai=p.MaLoaiNavigation.TenLoai
		//	});
  //          HttpContext.Session.SetInt32("rangeInput", rangeInput);
  //           return View("Search",result);
  //      }

  //      public IActionResult Sort(string? sortQuery)
  //      {
  //          string? querySearch = HttpContext.Session.GetString("querySearch");
  //          int? loai = HttpContext.Session.GetInt32("loai");
  //          int? rangeInput = HttpContext.Session.GetInt32("rangeInput");

  //          var hangHoas = db.HangHoas.AsQueryable();
  //          if(loai.HasValue)
  //          {
  //              hangHoas = hangHoas.Where(hh => hh.MaLoai == loai.Value);
  //          }
  //          if(rangeInput.HasValue)
  //          {
  //              hangHoas = hangHoas.Where(p => p.DonGia >= rangeInput);
  //          }
  //          if(querySearch != null)
  //          {
  //              hangHoas = hangHoas.Where(p => p.TenHh.Contains(querySearch));
  //          }

  //          if (sortQuery != null)
  //          {
  //              if(sortQuery == "price-asc")
  //              {
  //                  hangHoas = hangHoas.OrderBy(p => p.DonGia);
  //              }
  //              else if(sortQuery == "price-desc")
  //              {
  //                  hangHoas = hangHoas.OrderByDescending(p => p.DonGia);
  //              }
  //          }
  //          var result = hangHoas.Select(p => new HangHoaVM
  //          {
  //              MaHh = p.MaHh,
  //              TenHh = p.TenHh,
  //              DonGia = p.DonGia ?? 0,
  //              Hinh = p.Hinh ?? "",
  //              MoTa = p.MoTaDonVi ?? "",
  //              TenLoai = p.MaLoaiNavigation.TenLoai
  //          });
  //          HttpContext.Session.SetString("sortQuery", sortQuery);
  //          return View("Search", result);
		//}

		public IActionResult Detail(int id)
        {
            var data=db.HangHoas
                .Include(p =>p.MaLoaiNavigation)
                .SingleOrDefault(p=>p.MaHh==id);
            if (data == null)
            {
                TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            var result = new ChiTietHangHoaVM
            {
                MaHh = data.MaHh,
                TenHh = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                Mota = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,

                SoLuongTon = 10,
                DiemDanhGia = 5
            };

            return View(result);
        }

    }
}
