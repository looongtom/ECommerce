using ECommerceMVC.Data;
using ECommerceMVC.Helpers;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context db;

        public HangHoaController(Hshop2023Context context) { db = context; }
        public async Task<IActionResult> Index(int? loai, string? querySearch, int? rangeInput, string? sortQuery, int? pageNumber)
        {
            string? querySearchSesion = HttpContext.Session.GetString("querySearch");
            int? rangeInputSesion = HttpContext.Session.GetInt32("rangeInput");
            string? sortQuerySesion = HttpContext.Session.GetString("sortQuery");
            int? loaiSession = HttpContext.Session.GetInt32("loai");

            ViewData["CurrentSort"] = sortQuery;
            ViewData["RangeInput"] = rangeInput;
            ViewData["QuerySearch"] = querySearch;
            ViewData["Loai"] = loai;

            var hangHoas = db.HangHoas.AsQueryable();
            if (querySearch != null)
            {
                pageNumber = 1;
            }

            if (loai != 0)
            {

                if (querySearch != null)
                {
                    HttpContext.Session.SetString("querySearch", querySearch);
                    hangHoas = hangHoas.Where(p => p.TenHh.Contains(querySearch));
                }
                else
                {
                    HttpContext.Session.Remove("querySearch");
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
                else if (!loai.HasValue && loaiSession != null && loaiSession != 0)
                {
                    hangHoas = hangHoas.Where(hh => hh.MaLoai == loaiSession.Value);
                }
            }


            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTa = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            if (loai.HasValue) HttpContext.Session.SetInt32("loai", loai ?? 0);
            int pageSize= 6;
            return View(await PaginatedList<HangHoaVM>.CreateAsync(result.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<ActionResult> Search(string sortOrder,string currentFilter,string searchString,int? pageNumber)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            if(searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var hangHoas = db.HangHoas.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                hangHoas = hangHoas.Where(s => s.TenHh.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    hangHoas = hangHoas.OrderByDescending(s => s.TenHh);
                    break;
                case "price_asc":
                    hangHoas = hangHoas.OrderBy(s => s.DonGia);
                    break;
                case "price_desc":
                    hangHoas = hangHoas.OrderByDescending(s => s.DonGia);
                    break;
                default:
                    hangHoas = hangHoas.OrderBy(s => s.TenHh);
                    break;
            }
            int pageSize = 6;
            return View(await PaginatedList<HangHoa>.CreateAsync(hangHoas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
       
        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);
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
