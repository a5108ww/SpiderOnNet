using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpiderOnNet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

using System.Text.Json;

namespace SpiderOnNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        HttpClient httpClient = new HttpClient();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(string queryString)
        {
            List<string> urlList = await GetUrlList(queryString);
            if (urlList != null && urlList.Count > 0)
            {
                string responseResult = await GetResponseString(urlList[0]); //發送請求
                ViewData["temp"] = responseResult;
            }
            return View();
        }

        //去google下搜尋特定字串
        public async Task<List<string>> GetUrlList(string queryString)
        {
            List<string> urlList = new List<string>();

            //因google 每個搜尋頁面僅保留10筆資料，故start 是資料的index，0代表第一頁，10代表第2頁
            string url = "https://www.google.com/search?q=" + queryString + "&start=0";

            url = HttpUtility.UrlDecode(url);

            string responseResult = await GetResponseString(url);//取得內容

            if (!string.IsNullOrWhiteSpace(responseResult))
            {
                responseResult = HttpUtility.HtmlDecode(responseResult);
                string splitString = @"<a href=""/url?q=";

                List<string> newList = new List<string>();

                List<string> tempArr = responseResult.Split(splitString).ToList();
                for (int i = 1; i < tempArr.Count; i++)
                {
                    string tempString = tempArr[i];
                    int doubleQuoteIndex = tempString.IndexOf(@"""");
                    if (doubleQuoteIndex > 0)
                    {
                        newList.Add(tempString.Substring(0, doubleQuoteIndex));
                    }
                }

                urlList = newList;
            }


            return urlList;
        }

        public async Task<string> GetResponseString(string url)
        {
            string responseResult = "";

            var responseMessage = await httpClient.GetAsync(url); //發送請求
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                responseResult = responseMessage.Content.ReadAsStringAsync().Result;//取得內容
            }

            return responseResult;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string GetDecodeStringByUrlString(string urlString)
        {
            return HttpUtility.UrlDecode(urlString);
        }
        public string GetDecodeStringByHtmlString(string htmlString)
        {
            return HttpUtility.HtmlDecode(htmlString);
        }
    }
}
