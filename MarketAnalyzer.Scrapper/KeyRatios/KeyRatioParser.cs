using HtmlAgilityPack;
using MarketAnalyzer.Data;
using MarketAnalyzer.Scrapper.Common;
using System.Collections.Generic;

namespace MarketAnalyzer.Scrapper.KeyRatios
{
    public class KeyRatioParser : BaseParser<ExtractedKeyRatio>
    {
        public KeyRatioParser()
        {
            var keyRatio = new ExtractedKeyRatio();
            _propertyMappings.Add("", nameof(keyRatio.Year));

            _propertyMappings.Add("Returns" + "Return on Assets", nameof(keyRatio.ReturnOnAssets));
            _propertyMappings.Add("Returns" + "Return on Equity", nameof(keyRatio.ReturnOnEquity));
            _propertyMappings.Add("Returns" + "Return on Invested Capital", nameof(keyRatio.ReturnOnInvestedCapital));
            _propertyMappings.Add("Returns" + "Return on Capital Employed", nameof(keyRatio.ReturnOnCapitalEmployed));
            _propertyMappings.Add("Returns" + "Return on Tangible Capital Employed", nameof(keyRatio.ReturnOnTangibleCapitalEmployed));

            _propertyMappings.Add("Margins as % of Revenue" + "Gross Margin", nameof(keyRatio.GrossMargin));
            _propertyMappings.Add("Margins as % of Revenue" + "EBITDA Margin", nameof(keyRatio.EbitdaMargin));
            _propertyMappings.Add("Margins as % of Revenue" + "Operating Margin", nameof(keyRatio.OperatingMargin));
            _propertyMappings.Add("Margins as % of Revenue" + "Pretax Margin", nameof(keyRatio.PretaxMargin));
            _propertyMappings.Add("Margins as % of Revenue" + "Net Margin", nameof(keyRatio.NetMargin));
            _propertyMappings.Add("Margins as % of Revenue" + "Free Cash Margin", nameof(keyRatio.FreeCashMargin));

            _propertyMappings.Add("Capital Structure" + "Assets to Equity", nameof(keyRatio.AssetsToEquity));
            _propertyMappings.Add("Capital Structure" + "Equity to Assets", nameof(keyRatio.EquityToAssets));
            _propertyMappings.Add("Capital Structure" + "Debt to Equity", nameof(keyRatio.DebtToEquity));
            _propertyMappings.Add("Capital Structure" + "Debt to Assets", nameof(keyRatio.DebtToAssets));

            _propertyMappings.Add("Supplementary Items" + "Free Cash Flow", nameof(keyRatio.FreeCashFlow));
            _propertyMappings.Add("Supplementary Items" + "Book Value", nameof(keyRatio.BookValue));
            _propertyMappings.Add("Supplementary Items" + "Tangible Book Value", nameof(keyRatio.TangibleBookValue));

            _propertyMappings.Add("Valuation Metrics" + "Market Capitalization", nameof(keyRatio.MarketCapitalization));
            _propertyMappings.Add("Valuation Metrics" + "Price-to-Earnings", nameof(keyRatio.PriceToEarnings));
            _propertyMappings.Add("Valuation Metrics" + "Price-to-Book", nameof(keyRatio.PriceToBook));
            _propertyMappings.Add("Valuation Metrics" + "Price-to-Sales", nameof(keyRatio.PriceToSales));
            _propertyMappings.Add("Dividends" + "Dividends per share", nameof(keyRatio.DividendsPerShare));
            _propertyMappings.Add("Dividends" + "Payout Ratio", nameof(keyRatio.PayoutRatio));
        }

        public override List<ExtractedKeyRatio> ParseTable(HtmlNode table)
        {
            return ParseTable(table, 1, 0);
        }
    }
}