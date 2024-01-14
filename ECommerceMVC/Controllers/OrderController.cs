using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly Hshop2023Context _context;

        public OrderController(Hshop2023Context context)
        {
            _context = context;
        }
        public IActionResult History(string id)
        {
            var listOrder = _context.HoaDons.Include(i=>i.MaTrangThaiNavigation).Where(p => p.MaKh == id).ToList();

            List<HoaDonVM> listOrderVM = new List<HoaDonVM>();
            foreach (var hd in listOrder)
            {
                var hdVM = new HoaDonVM
                {
                    MaHd = hd.MaHd,
                    NgayDat = hd.NgayDat,
                    TrangThai = hd.MaTrangThaiNavigation.TenTrangThai,

                };
                var listHh = _context.ChiTietHds.Include(i=>i.MaHhNavigation).Where(p => p.MaHd == hd.MaHd).ToList();
                var chiTietHdVM = new List<ChiTietHangHoaVM>();
                double tongTien = 0;
                foreach (var hh in listHh)
                {
                    ChiTietHangHoaVM chiTietHangHoaVM = new ChiTietHangHoaVM
                    {
                        MaHh = hh.MaHh,
                        TenHh = hh.MaHhNavigation.TenHh,
                        Hinh = hh.MaHhNavigation.Hinh,
                        DonGia = hh.DonGia,
                    };
                    chiTietHdVM.Add(chiTietHangHoaVM);
                    tongTien += hh.DonGia;
                }
                hdVM.ChiTietHd = chiTietHdVM;
                hdVM.TongTien = tongTien;
                listOrderVM.Add(hdVM);
            }
            return View(listOrderVM);
        }
    }
}
