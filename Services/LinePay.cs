using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper.Internal;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Payments.LinePay.Models;
using Nop.Plugin.Payments.LinePay.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.LinePay.Services
{
    public class LinePay : ILinePay
    {
        private readonly HttpClient _httpClient;
        private readonly LinePaySettings _linePaySettings;
        private readonly IOrderService _orderService;

        public LinePay(HttpClient client, 
            LinePaySettings linePaySettings,
            IOrderService orderService)
        {
            //configure client
            client.Timeout = TimeSpan.FromSeconds(25);
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CURRENT_VERSION}");

            _httpClient = client;
            _linePaySettings = linePaySettings;
            _orderService = orderService;
        }

        public async Task<LinePayResponse> RequestLinePayAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            using (var httpClient = new HttpClient())
            {
                //Settings
                var channelId = _linePaySettings.ChannelId;
                var channelSecret = _linePaySettings.ChannelSecretKey;
                //var channelId = "1657146343";
                //var channelSecret = "a8f55c90dfd0e5394ea0f63a4f601fe3";
                var baseUri = "https://sandbox-api-pay.line.me";
                var apiUrl = "/v3/payments/request";
                var orderId = postProcessPaymentRequest.Order.OrderGuid.ToString();
                var amount = (int)postProcessPaymentRequest.Order.OrderTotal;

                //Body
                var requestBody = new LinePayRequest()
                {
                    options = new Options() { 
                        payment = new Payment() { 
                            capture = true
                        }
                    },
                    amount = amount,
                    currency = "TWD",
                    orderId = orderId,
                    packages = new List<Package>()
                    {
                        //Products
                        new Package()
                        {
                            id = "package-1",
                            name = "name-1",
                            amount = amount,
                            products = new List<Product>()
                            {
                                new Product()
                                {
                                    id = "nop-" + postProcessPaymentRequest.Order.Id.ToString(),
                                    name = "台酒購物網",
                                    quantity = 1,
                                    price = amount
                                }
                            }
                        }
                    },
                    redirectUrls = new Redirecturls()
                    {
                        confirmUrl = "https://localhost:44369/Admin/LinePayPlugin/Confirm",
                        cancelUrl = "https://localhost:44369/"
                    }
                };


                var body = JsonSerializer.SerializeToElement(requestBody);

                string Signature = HashLinePayRequest(channelSecret, apiUrl, body.ToString(), orderId, channelSecret);

                httpClient.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
                httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", orderId);
                httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization", Signature);

                var content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(baseUri + apiUrl, content);
                var result = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<LinePayResponse>(result);
            }
        }

        public async Task<ConfirmResponse> ConfirmLinePayAsync(string orderId, string transactionId)
        {
            using (var httpClient = new HttpClient())
            {
                //Settings
                var channelId = _linePaySettings.ChannelId;
                var channelSecret = _linePaySettings.ChannelSecretKey;
                //var channelId = "1657146343";
                //var channelSecret = "a8f55c90dfd0e5394ea0f63a4f601fe3";
                var baseUri = "https://sandbox-api-pay.line.me";
                var apiUrl = "/v3/payments/" + transactionId + "/confirm";

                var order = await _orderService.GetOrderByGuidAsync(Guid.Parse(orderId));
                var amount = (int)order.OrderTotal;
                var currnecy = "TWD";


                //Body
                var requestBody = new LinePayRequest()
                {
                    amount = amount,
                    currency = "TWD"
                };

                var body = JsonSerializer.SerializeToElement(requestBody);

                string Signature = HashLinePayRequest(channelSecret, apiUrl, body.ToString(), orderId, channelSecret);

                httpClient.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
                httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", orderId);
                httpClient.DefaultRequestHeaders.Add("X-LINE-Authorization", Signature);

                var content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(baseUri + apiUrl, content);
                var result = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<ConfirmResponse>(result);
            }
        }


        internal static string HashLinePayRequest(string channelSecret, string apiUrl, string body, string orderId, string key)
        {
            var request = channelSecret + apiUrl + body + orderId;
            key = key ?? "";

            var encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(request);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

    }
}
