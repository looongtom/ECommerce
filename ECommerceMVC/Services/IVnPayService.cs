using ECommerceMVC.ViewModels;

namespace ECommerceMVC.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext content, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }

}
