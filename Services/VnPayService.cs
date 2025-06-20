using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

public class VnPayService
{
    private readonly string vnp_TmnCode = "XD2NSV6S";
    private readonly string vnp_HashSecret = "MD72V7LO7U7GUR39GB1KF6NHSYCXZ0GZ";
    private readonly string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
    private readonly string vnp_ReturnUrl = "https://localhost:7042/vnpay-return";

    public string CreatePaymentUrl(decimal amount, string orderInfo, string orderId, string clientIp)
    {
        var vnp_Params = new SortedDictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", vnp_TmnCode },
            { "vnp_Amount", ((int)(amount * 100)).ToString() },
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", orderId },
            { "vnp_OrderInfo", orderInfo },
            { "vnp_OrderType", "other" },
            { "vnp_Locale", "vn" },
            { "vnp_ReturnUrl", vnp_ReturnUrl },
            { "vnp_IpAddr", clientIp },
            { "vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss") }
        };

        // Build data for hash (do NOT URL encode, do NOT include vnp_SecureHash or vnp_SecureHashType)
        var hashData = new StringBuilder();
        foreach (var kv in vnp_Params)
        {
            if (hashData.Length > 0) hashData.Append('&');
            hashData.Append($"{kv.Key}={kv.Value}");
        }

        string signData = hashData.ToString();
        string vnp_SecureHash = CreateHmacSHA512(vnp_HashSecret, signData);
        vnp_Params.Add("vnp_SecureHashType", "SHA512");
        vnp_Params.Add("vnp_SecureHash", vnp_SecureHash);

        // Build final URL (URL encode values for the query string)
        var paymentUrl = QueryHelpers.AddQueryString(vnp_Url, vnp_Params);

        return paymentUrl;
    }

    private string CreateHmacSHA512(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(inputBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}