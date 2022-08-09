using System.Net;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.QuickFsApi
{
    public class BaseApi
    {
        //protected const string API_KEY = "d4089a95fc589f2d804c241f4f23b9732ff9ab6e";
        protected const string API_KEY = "e7653bd5ed2a0250ad1b94f02117fe0fe016cb66";

        protected async Task<HttpWebRequest> ComposeWebRequest()
        {
            var webRequestHelper = new WebRequestHelper();
            var request = await webRequestHelper.ComposeWebRequestPost("https://public-api.quickfs.net/v1/data/batch");

            request.Headers.Add("X-QFS-API-Key", API_KEY);
            return request;
        }

        protected async Task<string> CallWebRequest(HttpWebRequest httpWebRequest, object jsonBody)
        {
            var webRequestHelper = new WebRequestHelper();
            return await webRequestHelper.CallWebRequest(httpWebRequest, jsonBody);
        }

        protected string BuildQfs(string ticker, string parameter, int numYears)
        {
            return $"QFS({ticker},{parameter},FY-{numYears - 1}:FY)";
        }
    }
}