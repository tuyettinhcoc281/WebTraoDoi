//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;

//public class BuyModel : PageModel
//{
//    private readonly VnPayService _vnPayService;

//    public BuyModel(VnPayService vnPayService)
//    {
//        _vnPayService = vnPayService;
//    }

//    public IActionResult OnGet()
//    {
//        var paymentUrl = _vnPayService.CreatePaymentUrl(User.Identity.Name, 100000, "VIP 1 month");
//        return Redirect(paymentUrl);
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class BuyModel : PageModel
{
    public IActionResult OnGet()
    {
        // FAKE VNPay: Simulate a successful payment for testing
        if (Request.Query.ContainsKey("fake"))
        {
            // Use absolute path to ensure correct routing
            return Redirect($"/Vip/PaymentReturn?vnp_ResponseCode=00&email={User.Identity.Name}");
        }

        // Otherwise, show the page
        return Page();
    }
}