namespace ECommerceMVC.ViewModels
{
    public class ChiTietHoaDonVM
    {
        public int MaHd { get; set; }
        public List<ChiTietHangHoaVM> ChiTietHd { get; set; }
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; }
        public double TongTien { get; set; }
        public string CachThanhToan { get; set; }
        public string CachVanChuyen { get; set; }
        public double PhiVanChuyen { get; set; }
        public string DiaChi { get; set; }
    }
}
