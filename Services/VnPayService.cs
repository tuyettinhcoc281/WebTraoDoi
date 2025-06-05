using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

public class VnPayService
{
    private readonly string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
    private readonly string vnp_Returnurl = "https://yourdomain.com/Vip/PaymentReturn";
    private readonly string vnp_TmnCode = "YOUR_TMN_CODE";
    private readonly string vnp_HashSecret = "YOUR_HASH_SECRET";

    public string CreatePaymentUrl(string userId, decimal amount, string orderInfo)
    {
        var vnp_Params = new SortedDictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", vnp_TmnCode },
            { "vnp_Amount", ((int)amount * 100).ToString() }, // VND * 100
            { "vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", "127.0.0.1" },
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", orderInfo },
            { "vnp_OrderType", "other" },
            { "vnp_ReturnUrl", vnp_Returnurl },
            { "vnp_TxnRef", Guid.NewGuid().ToString() }
        };

        var signData = string.Join('&', vnp_Params.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        var hash = HmacSHA512(vnp_HashSecret, signData);
        vnp_Params.Add("vnp_SecureHash", hash);

        return QueryHelpers.AddQueryString(vnp_Url, vnp_Params);
    }

    private string HmacSHA512(string key, string inputData)
    {
        var hash = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(inputData));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}