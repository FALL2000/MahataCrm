using iText.Commons.Bouncycastle.Asn1.X509;
using MahataCrm.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace MahataCrm.Service
{
    public class APIService
    {
        public static async Task<JObject> GetAccessToken(string clientId, string clientSecret)
        {
            var url = "https://webshop.co.tz/api/public/index.php/api/v1/login";
           // var clientId = "demo";
            //var clientSecret = "rehema";

            using (var client = new HttpClient())
            {
                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                });

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                var response = await client.PostAsync(url, requestContent);
                var responseString = await response.Content.ReadAsStringAsync();
                JObject objetJson = new JObject();
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JObject.Parse(responseString);
                    var accessToken = jsonResponse["access_token"]?.ToString();
                    objetJson["accessToken"] = accessToken;
                    objetJson["isError"] = false;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine(responseString);
                    objetJson["ErrorStatus"] = response.StatusCode.ToString();
                    objetJson["Error"] = responseString;
                    objetJson["isError"] = true;
                }
                return objetJson;
            }
        }

        public static async Task<JObject> PostGenerateReceipt(string token, Receipt receipt)
        {
            var url = "https://webshop.co.tz/api/public/index.php/api/v1/generatereceipt";
            var items = new List<object>();
            foreach(var item in receipt.ReceiptItems)
            {
                items.Add(
                    new
                    {
                        itemcode = "ZTL2000",
                        itemdesc = item.Description,
                        itemqty = item.Quantity,
                        net = item.Price,
                        tax = item.Quantity * item.Price * item.Tax,
                        amount = (item.Quantity * item.Price) + (item.Quantity * item.Price * item.Tax),
                        discountamout = item.DiscountAmount,
                        itemtaxcode = item.TaxCode
                    }

                 );
            }
            // Définir les variables dynamiques

            var jsonBody = new
            {
                dbrecord = new
                {
                    invoice_id = receipt.RctNum,
                    invoice_date = receipt.RctDate.ToString()
                },
                customer = new
                {
                    idtype = "1",
                    idnumber = receipt.CustId,
                    mobile = "0699458952",
                    name = receipt.CustName
                },
                items = items,
                payment = new
                {
                    paymenttype = "INVOICE"
                }
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(jsonBody), System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JObject.Parse(responseString);
                    return jsonResponse;
                }
                else
                {
                    JObject objetJson = new JObject();
                    objetJson["ErrorStatus"] = response.StatusCode.ToString();
                    objetJson["Error"] = responseString;
                    objetJson["isError"] = true;
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine(responseString);
                    return objetJson;
                }
            }
        }

        public static async Task<JObject> GetZReports(string token)
        {
            var url = "https://webshop.co.tz/api/public/index.php/api/v1/zreport";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JObject.Parse(responseString);
                    return jsonResponse;
                }
                else
                {
                    JObject objetJson = new JObject();
                    objetJson["ErrorStatus"] = response.StatusCode.ToString();
                    objetJson["Error"] = responseString;
                    objetJson["isError"] = true;
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine(responseString);
                    return objetJson;
                }
            }
        }

        public static async Task<JObject> GetDashboardData(string token)
        {
            var url = "https://webshop.co.tz/api/public/index.php/api/v1/dashboard";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JObject.Parse(responseString);
                    return jsonResponse;
                   /* var dashboard = jsonResponse["dashboard"];

                    if (dashboard != null)
                    {
                        Console.WriteLine($"Total Receipts: {dashboard["total_rct"]}");
                        Console.WriteLine($"Total Amount: {dashboard["total_amount"]}");
                        Console.WriteLine($"Date: {dashboard["date"]}");
                        Console.WriteLine($"Total Month Amount: {dashboard["total_month_amount"]}");
                    }
                    else
                    {
                        Console.WriteLine("No dashboard data found.");
                    }*/
                }
                else
                {
                    JObject objetJson = new JObject();
                    objetJson["ErrorStatus"] = response.StatusCode.ToString();
                    objetJson["Error"] = responseString;
                    objetJson["isError"] = true;
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine(responseString);
                    return objetJson;
                }
            }
        }
    }
}
