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

namespace Nop.Plugin.Payments.LinePay.Services
{
    public class LinePay : ILinePay
    {
        private readonly HttpClient _httpClient;
        private readonly LinePaySettings _linePaySettings;


        public LinePay(HttpClient client, LinePaySettings linePaySettings)
        {
            //configure client
            client.Timeout = TimeSpan.FromSeconds(25);
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, $"nopCommerce-{NopVersion.CURRENT_VERSION}");

            _httpClient = client;
            _linePaySettings = linePaySettings;
        }

        public async Task<List<LinePayResponse>> RequestLinePayAsync()
        {
            var resp = new List<LinePayResponse>();

            using (var httpClient = new HttpClient())
            {
                //Settings
                var channelId = _linePaySettings.ChannelId;
                var channelSecret = _linePaySettings.ChannelSecretKey;
                //var channelId = "1657146343";
                //var channelSecret = "a8f55c90dfd0e5394ea0f63a4f601fe3";
                var baseUri = "https://sandbox-api-pay.line.me";
                var apiUrl = "/v3/payments/request";
                string orderId = Guid.NewGuid().ToString();

                //Body
                var requestBody = new LinePayRequest()
                {
                    amount = 100,
                    currency = "TWD",
                    orderId = Guid.NewGuid().ToString(),
                    packages = new List<Package>()
                    {
                        //Products
                        new Package()
                        {
                            id = "package-1",
                            name = "name-1",
                            amount = 100,
                            products = new List<Product>()
                            {
                                new Product()
                                {
                                    id = "prod1",
                                    name = "prod1",
                                    quantity = 1,
                                    price = 100
                                }
                            }
                        }
                    },
                    redirectUrls = new Redirecturls()
                    {
                        confirmUrl = "https://localhost:44369/",
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

                resp.Add(JsonSerializer.Deserialize<LinePayResponse>(result));
            }
            return resp;
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
