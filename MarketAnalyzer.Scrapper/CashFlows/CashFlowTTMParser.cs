using HtmlAgilityPack;
using MarketAnalyzer.Data;
using MarketAnalyzer.Scrapper.Common;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Scrapper.CashFlows
{
    public class CashFlowTTMParser : BaseParser<ExtractedCashFlowStatementTtm>
    {
        public CashFlowTTMParser()
        {
            var cashFlow = new ExtractedCashFlowStatementTtm();
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

        public override List<ExtractedCashFlowStatementTtm> ParseTable(HtmlNode table)
        {
            var rows = table.Descendants().Where(n => n.Name == "tr");

            var rowsCount = rows.Count();
            var columnsCount = rows.First().Descendants().Where(n => n.Name == "td").Count();
            return ParseTable(table, columnsCount - 1, 0);
        }
    }
}