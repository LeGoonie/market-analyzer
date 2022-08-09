using HtmlAgilityPack;
using MarketAnalyzer.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.Fool
{
    public class FoolRatingsScrapper
    {
        private readonly WebRequestHelper _webRequestHelper = new WebRequestHelper();

        public async Task<FoolRating> GetFoolRating(string ticker)
        {
            var url = $"https://www.fool.com/quote/{ticker}";
            var webRequest = await _webRequestHelper.ComposeWebRequestGet(url);
            try
            {
                var result = await _webRequestHelper.CallWebRequest(webRequest);

                var document = new HtmlDocument();

                document.LoadHtml(result);

                var foolRating = new FoolRating();

                //CAPS Rating
                HtmlNode image = document.DocumentNode.SelectSingleNode("//img[@class='number-of-stars']");

                if (image != null)
                {
                    var starString = image.GetAttributeValue("alt", "alt").Substring(0, 1);
                    foolRating.CapsRating = Convert.ToDouble(starString);
                }


                //OutPerform Ratio

                HtmlNodeCollection nodeCollection = document.DocumentNode.SelectNodes("//div[@class='jointSentimentGroup']");

                if (nodeCollection != null)
                {
                    HtmlNode outPerformNode = nodeCollection.First();


                    double outPerform = Convert.ToDouble(outPerformNode.SelectSingleNode("//span[@id='all-out-picks']").InnerHtml);

                    double underPerform = Convert.ToDouble(outPerformNode.SelectSingleNode("//span[@id='all-under-picks']").InnerHtml);

                    double outPerformRatio;
                    if (outPerform == 0.0)
                    {
                        outPerformRatio = 0.0;
                    }
                    else if (underPerform == 0)
                    {
                        outPerformRatio = outPerform;
                    }
                    else
                    {
                        outPerformRatio = Math.Round(outPerform / underPerform, 3);
                    }


                    foolRating.OutPerformRatio = outPerformRatio;
                    //All-Star OutPerform Ratio

                    HtmlNode allStarPerformNode = nodeCollection.LastOrDefault();

                    double allStarOutPerform = Convert.ToDouble(allStarPerformNode.SelectNodes("//span[@id='all-out-picks']").LastOrDefault().InnerHtml);

                    double allStarUnderPerform = Convert.ToDouble(allStarPerformNode.SelectNodes("//span[@id='all-under-picks']").LastOrDefault().InnerHtml);

                    double allStarOutPerformRatio;
                    if (allStarOutPerform == 0.0)
                    {
                        allStarOutPerformRatio = 0.0;
                    }
                    else if (allStarUnderPerform == 0)
                    {
                        allStarOutPerformRatio = allStarOutPerform;
                    }
                    else
                    {
                        allStarOutPerformRatio = Math.Round(allStarOutPerform / allStarUnderPerform, 3);
                    }

                    foolRating.AllStarRatio = allStarOutPerformRatio;

                }



                return foolRating;
            } catch
            {
                return null;
            }
        }
    }
}
