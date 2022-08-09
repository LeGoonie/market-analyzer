using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper
{
    public class WebRequestHelper
    {
        public async Task<HttpWebRequest> ComposeWebRequestPost(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            return request;
        }

        public async Task<HttpWebRequest> ComposeWebRequestGet(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";

            return request;
        }

        public async Task<string> CallWebRequest(HttpWebRequest httpWebRequest, object jsonBody)
        {
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonBody);

                await streamWriter.WriteAsync(json);
            }

            var response = (HttpWebResponse)httpWebRequest.GetResponse();

            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        public async Task<string> CallWebRequest(HttpWebRequest httpWebRequest)
        {
            var response = (HttpWebResponse)httpWebRequest.GetResponse();
            return await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
        }
    }
}