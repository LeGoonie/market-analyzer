using HtmlAgilityPack;
using MarketAnalyzer.Data;
using MarketAnalyzer.Scrapper.Common;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Scrapper.CashFlows
{
    public class CashFlowParser : BaseParser<ExtractedCashFlowStatement>
    {
        public CashFlowParser()
        {
            var cashFlow = new ExtractedCashFlowStatement();
            _propertyMappings.Add("", nameof(cashFlow.Year));
            _propertyMappings.Add("Depreciation & Amortization", nameof(cashFlow.DepreciationAmortization));
            _propertyMappings.Add("Change in Working Capital", nameof(cashFlow.ChangeInWorkCapital));
            _propertyMappings.Add("Change in Deferred Tax", nameof(cashFlow.ChangeInDeferredTax));
            _propertyMappings.Add("Stock-Based Compensation", nameof(cashFlow.StockBasedCompensation));

            //_propertyMappings.Add("Other", nameof(cashFlow.OperationsOther));

            _propertyMappings.Add("Cash From Operations", nameof(cashFlow.CashFromOperations));

            _propertyMappings.Add("Property, Plant, & Equipment", nameof(cashFlow.PropertyPlanEquipment));
            _propertyMappings.Add("Acquisitions", nameof(cashFlow.Acquisitions));
            _propertyMappings.Add("Investments", nameof(cashFlow.Investments));

            _propertyMappings.Add("Intangibles", nameof(cashFlow.Intangibles));

            _propertyMappings.Add("Cash From Investing", nameof(cashFlow.CashFromInvesting));

            _propertyMappings.Add("Net Issuance of Common Stock", nameof(cashFlow.IssuanceCommonStockNet));
            _propertyMappings.Add("Net Issuance of Debt", nameof(cashFlow.IssuanceDebtNet));

            _propertyMappings.Add("Cash Paid for Dividends", nameof(cashFlow.CashPaidForDividends));

            //_propertyMappings.Add("Other", nameof(cashFlow.Other));

            _propertyMappings.Add("Cash From Financing", nameof(cashFlow.CashFromFinancing));
        }

        public override List<ExtractedCashFlowStatement> ParseTable(HtmlNode table)
        {
            return ParseTable(table, 1, 1);
        }

        private string GetCellValue(HtmlNode cell)
        {
            var dataValue = cell.Attributes.Where(a => a.Name == "data-value").SingleOrDefault();
            if (dataValue != null) return dataValue.Value;
            else return cell.InnerHtml;
        }
    }
}