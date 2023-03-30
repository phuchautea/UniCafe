using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Windows.Input;
using UniCafe.Data;
using UniCafe.Models;
using UniCafe.Services;
using UniCafe.Services.Payment.Momo;

namespace UniCafe.Controllers
{
    public class PayController : BaseController<Payment>
    {
        private readonly IRepository<Order> _orderRepository;
        public PayController()
        {
            _orderRepository = new Repository<Order>(Context);
        }
        public ActionResult ErrorPayment()
        {
            return View();
        }
        // GET: Pay
        public ActionResult MomoPay()
        {
            if (String.IsNullOrEmpty(Session["orderCode"].ToString()))
            {
                // Lỗi không tìm thấy order
                return RedirectToAction("CheckOut", "Order");
            }
            var orderCode = Session["orderCode"].ToString();
            var order = Context.Orders.FirstOrDefault(x => x.Code == orderCode);
            if(order == null)
            {
                // Lỗi không tìm thấy order
                return RedirectToAction("CheckOut", "Order");
            }
            string endPoint = System.Configuration.ConfigurationManager.AppSettings["endPoint_Momo"];
            string partnerCode = System.Configuration.ConfigurationManager.AppSettings["partnerCode_Momo"];
            string accessKey = System.Configuration.ConfigurationManager.AppSettings["accessKey_Momo"];
            string secretKey = System.Configuration.ConfigurationManager.AppSettings["secretKey_Momo"];
            string redirectUrl = System.Configuration.ConfigurationManager.AppSettings["redirectUrl_Momo"];
            string ipnUrl = System.Configuration.ConfigurationManager.AppSettings["ipnUrl_Momo"];
            string requestType = "captureWallet";
            string orderInfo = "Thanh toan UniCafe #" + orderCode + "";
            string amount = string.Join("", order.Total.ToString("N0").Where(char.IsDigit)); // Xóa dấu phẩy
            string orderId = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "accessKey=" + accessKey +
                "&amount=" + amount +
                "&extraData=" + extraData +
                "&ipnUrl=" + ipnUrl +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + redirectUrl +
                "&requestId=" + requestId +
                "&requestType=" + requestType
                ;

            MomoSecurity crypto = new MomoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, secretKey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }
            };
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endPoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);
            return Redirect(jmessage.GetValue("payUrl").ToString());
        }
        public ActionResult MomoProcessing(MomoResult result)
        {
            //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
            if (result.resultCode == 0)
            {
                if (!String.IsNullOrEmpty(Session["orderCode"].ToString()))
                {
                    // Cập nhật lại status paid
                    string orderCode = Session["orderCode"].ToString();
                    var order = Context.Orders.FirstOrDefault(x => x.Code == orderCode);
                    if (order != null)
                    {
                        order.Paid = 1;
                        Context.SaveChanges();
                    }
                    // Xóa OrderCode
                    Session["orderCode"] = null;
                    Session["payment"] = null;
                }
                return RedirectToAction("CompleteOrder", "Order");
            }
            return RedirectToAction("ErrorPayment", "Pay");
        }
        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
        public string GetIpAddress()
        {
            string ipAddress;
            try
            {
                ipAddress = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ipAddress) || (ipAddress.ToLower() == "unknown") || ipAddress.Length > 45)
                    ipAddress = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch (Exception ex)
            {
                ipAddress = "Invalid IP:" + ex.Message;
            }

            return ipAddress;
        }
        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret, Dictionary<string, string> _requestData)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            string queryString = data.ToString();

            baseUrl += "?" + queryString;
            String signData = queryString;
            if (signData.Length > 0)
            {

                signData = signData.Remove(data.Length - 1, 1);
            }
            string vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

            return baseUrl;
        }
        public ActionResult VNPay()
        {
            if (String.IsNullOrEmpty(Session["orderCode"].ToString()))
            {
                // Lỗi không tìm thấy order
                return RedirectToAction("CheckOut", "Order");
            }
            var orderCode = Session["orderCode"].ToString();
            var order = Context.Orders.FirstOrDefault(x => x.Code == orderCode);
            if (order == null)
            {
                // Lỗi không tìm thấy order
                return RedirectToAction("CheckOut", "Order");
            }
            string vnp_Url = System.Configuration.ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["vnp_ReturnUrl"];
            string vnp_HashSecret = System.Configuration.ConfigurationManager.AppSettings["vnp_HashSecret"];
            string vnp_TmnCode = System.Configuration.ConfigurationManager.AppSettings["vnp_TmnCode"]; // Mã website của merchant trên hệ thống của VNPAY
            var vnp_Params = new Dictionary<string, string>();
            vnp_Params.Add("vnp_Version", "2.1.0");
            vnp_Params.Add("vnp_Command", "pay");
            vnp_Params.Add("vnp_TmnCode", vnp_TmnCode);
            vnp_Params.Add("vnp_Locale", "vn"); //en= English, vn=Tiếng Việt
            vnp_Params.Add("vnp_CurrCode", "VND");
            vnp_Params.Add("vnp_TxnRef", Guid.NewGuid().ToString()); // Mã tham chiếu của giao dịch đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            vnp_Params.Add("vnp_OrderInfo", "Thanh toan UniCafe #" + orderCode + "");
            vnp_Params.Add("vnp_OrderType", "topup"); //Mã danh mục hàng hóa.
            string amount = string.Join("", order.Total.ToString("N0").Where(char.IsDigit)); // Xóa dấu phẩy
            amount = (Int32.Parse(amount) * 100).ToString();
            vnp_Params.Add("vnp_Amount", amount); // Số tiền thanh toán
            vnp_Params.Add("vnp_ReturnUrl", vnp_ReturnUrl);
            vnp_Params.Add("vnp_IpAddr", GetIpAddress());
            vnp_Params.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            //vnp_Params.Add("vnp_BankCode", "NCB");
            vnp_Params = vnp_Params.OrderBy(o => o.Key).ToDictionary(k => k.Key, v => v.Value);
            String signData = string.Join("&",
            vnp_Params.Where(x => !string.IsNullOrEmpty(x.Value))
            .Select(k => k.Key + "=" + k.Value));
            string paymentUrl = CreateRequestUrl(vnp_Url, vnp_HashSecret, vnp_Params);
            return Redirect(paymentUrl);
        }
        public ActionResult VNPayProcessing(int vnp_ResponseCode)
        {
            //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
            if (vnp_ResponseCode == 00)
            {
                if (!String.IsNullOrEmpty(Session["orderCode"].ToString()))
                {
                    // Cập nhật lại status paid
                    string orderCode = Session["orderCode"].ToString();
                    var order = Context.Orders.FirstOrDefault(x => x.Code == orderCode);
                    if (order != null)
                    {
                        order.Paid = 1;
                        Context.SaveChanges();
                    }
                    // Xóa OrderCode
                    Session["orderCode"] = null;
                    Session["payment"] = null;
                }
                

                return RedirectToAction("CompleteOrder", "Order");
            }
            return RedirectToAction("ErrorPayment", "Pay");
        }
    }
}