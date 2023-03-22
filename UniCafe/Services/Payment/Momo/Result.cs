using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniCafe.Services.Payment.Momo
{
    public class Result
    {
        public string partnerCode { get; set; }
        public string orderId { get; set; }
        public string requestId { get; set; }
        public string amount { get; set; }
        public string orderInfo { get; set; }
        public string orderType { get; set; }
        public string transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public string payType { get; set; }
        public string responseTime { get; set; }
        public string extraData { get; set; }
        public string signature { get; set; }
    }
}