using ECommerceMVC.Data;

namespace ECommerceMVC.ViewModels
{
    public class HoaDonVM
    {
        public int MaHd { get; set; }
        public List<ChiTietHangHoaVM> ChiTietHd { get; set; }
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; }
        public double TongTien { get; set; }
    }
}
