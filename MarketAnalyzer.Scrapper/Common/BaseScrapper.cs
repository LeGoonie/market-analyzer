using HtmlAgilityPack;
using MarketAnalyzer.Data.Interfaces;
using MarketAnalyzer.Data.JsonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.Common
{
    public abstract class BaseScrapper
    {
        private WebRequestHelper _webRequestHelper = new WebRequestHelper();

        public async Task<Login> GetLogin(string loginToken)
        {
            var url = "https://api.quickfs.net/auth/login";
            var webRequest = await _webRequestHelper.ComposeWebRequestPost(url);
            var result = await _webRequestHelper.CallWebRequest(webRequest, new { token = loginToken });

            var login = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(result);

            return login;
        }

        protected async Task<List<TOut>> GetFinancialInfo<TOut, TParser>(string requestToken, string ticker, string infoType) where TParser : BaseParser<TOut> where TOut : ICreatedTime, IUpdatedTime
        {
            var url = $"https://api.quickfs.net/stocks/{ticker}/{infoType}/Annual/{requestToken}";
            var webRequest = await _webRequestHelper.ComposeWebRequestGet(url);
            var result = await _webRequestHelper.CallWebRequest(webRequest);

            result = result.Replace("<\\/td>", "</td>");

            var document = new HtmlDocument();

            document.LoadHtml(result);

            var table = document.DocumentNode.Descendants().Where(n => n.Name == "tbody").Single();

            BaseParser<TOut> parser = Activator.CreateInstance<TParser>();

            return parser.ParseTable(table);
        }
    }
}