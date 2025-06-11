using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CreateModel : PageModel
{
    private readonly VnPayService _vnPayService = new VnPayService();

    public IActionResult OnPostPay(decimal amount)
    {
        string orderId = Guid.NewGuid().ToString();
        string orderInfo = "Thanh toan don hang #" + orderId;
        string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        string paymentUrl = _vnPayService.CreatePaymentUrl(amount, orderInfo, orderId, clientIp);

        return Redirect(paymentUrl);
    }
}