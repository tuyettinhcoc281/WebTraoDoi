using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class BuyModel : PageModel
{
    public IActionResult OnGet()
    {
        var vnPayService = new VnPayService();
        string orderId = Guid.NewGuid().ToString();
        string orderInfo = "Subscribe VIP";
        decimal amount = 100000; // Set your VIP price here
        string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        string paymentUrl = vnPayService.CreatePaymentUrl(amount, orderInfo, orderId, clientIp);

        return Redirect(paymentUrl);
    }
}