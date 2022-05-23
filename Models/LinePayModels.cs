using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.LinePay.Models
{
    public record LinePayRequest  
    {
        public int amount { get; set; }

        public string currency { get; set; }

        public string orderId { get; set; }

        public List<Package> packages { get; set; }

        public Options options { get; set; }

        public Redirecturls redirectUrls { get; set; }
    }

    public record Package  
    {
        public string id { get; set; }

        public int amount { get; set; }

        public string name { get; set; }

        public List<Product> products { get; set; }
    }

    public record Product  
    {
        public string id { get; set; }

        public string name { get; set; }

        public string imageUrl { get; set; }

        public int quantity { get; set; }

        public int price { get; set; }
    }

    public record Options  
    {
        public Payment payment { get; set; }
    }

    public record Payment  
    {
        public bool capture { get; set; }
    }

    public record Redirecturls  
    {
        public string confirmUrl { get; set; }

        public string cancelUrl { get; set; }
    }

    public record LinePayResponse  
    {
        public string returnCode { get; set; }

        public string returnMessage { get; set; }

        public Info info { get; set; }
    }
    public record Info  
    {
        public Paymenturl paymentUrl { get; set; }

        public long transactionId { get; set; }

        public string paymentAccessToken { get; set; }
    }

    public record Paymenturl  
    {
        public string web { get; set; }

        public string app { get; set; }
    }

}
