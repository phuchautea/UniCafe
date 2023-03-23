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
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toan UniCafe #" + Session["orderCode"] +"";
            string redirectUrl = "https://localhost:44377/Pay/MomoProcessing";
            //string notifyurl = "https://5b56-116-108-86-76.ap.ngrok.io/Pay/ConfirmPaymentClient"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test
            string ipnUrl = "https://webhook.site/00642294-4421-43fd-8415-7e195ddcdad8";
            string requestType = "captureWallet";
            string amount = "1000";
            string orderId = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            //string rawHash = "partnerCode=" +
            //    partnerCode + "&accessKey=" +
            //    accessKey + "&requestId=" +
            //    requestId + "&amount=" +
            //    amount + "&orderId=" +
            //    orderid + "&orderInfo=" +
            //    orderInfo + "&returnUrl=" +
            //    returnUrl + "&notifyUrl=" +
            //    notifyUrl + "&extraData=" +
            //    extraData;

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
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            //JObject message = new JObject
            //{
            //    { "partnerCode", partnerCode },
            //    { "accessKey", accessKey },
            //    { "requestId", requestId },
            //    { "amount", amount },
            //    { "orderId", orderid },
            //    { "orderInfo", orderInfo },
            //    { "returnUrl", returnUrl },
            //    { "notifyUrl", notifyurl },
            //    { "extraData", extraData },
            //    { "requestType", "captureMoMoWallet" },
            //    { "signature", signature }

            //};
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
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }
        public ActionResult MomoProcessing(Result resultMomo)
        {
            //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
            if (resultMomo.resultCode == 0)
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
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string vnp_HashSecret = "NGXPMNYHDXTRHIKDMYSUWXEZHJJTAHMD";
            var vnp_Params = new Dictionary<string, string>();
            vnp_Params.Add("vnp_Version", "2.1.0");
            vnp_Params.Add("vnp_Command", "pay");
            vnp_Params.Add("vnp_TmnCode", "K651OP32"); // Mã website của merchant trên hệ thống của VNPAY
            string locale = "vn"; //en= English, vn=Tiếng Việt
            if (!string.IsNullOrEmpty(locale))
            {
                vnp_Params.Add("vnp_Locale", locale);
            }
            else
            {
                vnp_Params.Add("vnp_Locale", "vn");
            }
            vnp_Params.Add("vnp_CurrCode", "VND");
            vnp_Params.Add("vnp_TxnRef", Guid.NewGuid().ToString()); // Mã tham chiếu của giao dịch đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            vnp_Params.Add("vnp_OrderInfo", "Thanh toan UniCafe #" + Session["orderCode"] + "");
            vnp_Params.Add("vnp_OrderType", "topup"); //Mã danh mục hàng hóa.
            string amount = (10000 * 100).ToString();
            vnp_Params.Add("vnp_Amount", amount); // Số tiền thanh toán
            vnp_Params.Add("vnp_ReturnUrl", "https://localhost:44377/Pay/VNPayProcessing");
            vnp_Params.Add("vnp_IpAddr", GetIpAddress());
            vnp_Params.Add("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnp_Params.Add("vnp_BankCode", "NCB");
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

                return RedirectToAction("CompleteOrder", "Order");
            }
            return RedirectToAction("ErrorPayment", "Pay");
        }
    }
}