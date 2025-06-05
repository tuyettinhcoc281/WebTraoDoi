//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using ExchangeWebsite.Models;
//public class PaymentReturnModel : PageModel
//{
//    private readonly UserManager<User> _userManager;

//    public PaymentReturnModel(UserManager<User> userManager)
//    {
//        _userManager = userManager;
//    }

//    public async Task<IActionResult> OnGetAsync()
//    {
//        // Validate VN-Pay response here (omitted for brevity)
//        // If payment is successful:
//        var user = await _userManager.GetUserAsync(User);
//        if (user != null)
//        {
//            user.IsVip = true;
//            user.VipExpiration = DateTime.UtcNow.AddMonths(1);
//            await _userManager.UpdateAsync(user);
//        }
//        return Page();
//    }
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ExchangeWebsite.Models;

public class PaymentReturnModel : PageModel
{
    private readonly UserManager<User> _userManager;

    public PaymentReturnModel(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(string email = null)
    {
        var responseCode = Request.Query["vnp_ResponseCode"];
        if (responseCode == "00" && !string.IsNullOrEmpty(email))
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                user.IsVip = true;
                user.VipExpiration = DateTime.UtcNow.AddMonths(1);
                await _userManager.UpdateAsync(user);
            }
        }
        return Page();
    }
}