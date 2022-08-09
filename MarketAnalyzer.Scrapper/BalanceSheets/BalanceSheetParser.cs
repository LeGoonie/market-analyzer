using HtmlAgilityPack;
using MarketAnalyzer.Data;
using MarketAnalyzer.Scrapper.Common;
using System.Collections.Generic;

namespace MarketAnalyzer.Scrapper.BalanceSheets
{
    public class BalanceSheetParser : BaseParser<ExtractedBalanceSheet>
    {
        public BalanceSheetParser()
        {
            var balanceSheet = new ExtractedBalanceSheet();
            _propertyMappings.Add("", nameof(balanceSheet.Year));
            _propertyMappings.Add("Assets" + "Cash & Equivalents", nameof(balanceSheet.CashEquivalents));
            _propertyMappings.Add("Assets" + "Short-Term Investments", nameof(balanceSheet.ShortTermInvestments));
            _propertyMappings.Add("Assets" + "Accounts Receivable", nameof(balanceSheet.AccountsReceivable));
            _propertyMappings.Add("Assets" + "Other Current Assets", nameof(balanceSheet.OtherCurrentAssets));
            _propertyMappings.Add("Assets" + "Total Current Assets", nameof(balanceSheet.TotalCurrentAssets));
            _propertyMappings.Add("Assets" + "Investments", nameof(balanceSheet.Investments));
            _propertyMappings.Add("Assets" + "Property, Plant, & Equipment (Net)", nameof(balanceSheet.PropertyPlanEquipmentNet));
            _propertyMappings.Add("Assets" + "Goodwill", nameof(balanceSheet.Goodwill));
            _propertyMappings.Add("Assets" + "Other Intangible Assets", nameof(balanceSheet.OtherIntangibleAssets));
            _propertyMappings.Add("Assets" + "Other Assets", nameof(balanceSheet.OtherAssets));

            _propertyMappings.Add("Assets" + "Total Assets", nameof(balanceSheet.TotalAssets));

            _propertyMappings.Add("Liabilities & Equity" + "Accounts Payable", nameof(balanceSheet.AccountsPayable));
            _propertyMappings.Add("Liabilities & Equity" + "Tax Payable", nameof(balanceSheet.TaxPayable));
            _propertyMappings.Add("Liabilities & Equity" + "Accrued Liabilities", nameof(balanceSheet.AccruedLiabilities));
            _propertyMappings.Add("Liabilities & Equity" + "Short-Term Debt", nameof(balanceSheet.ShortTermDebt));
            _propertyMappings.Add("Liabilities & Equity" + "Deferred Revenue", nameof(balanceSheet.DeferedRevenue1));
            _propertyMappings.Add("Liabilities & Equity" + "Other Current Liabilities", nameof(balanceSheet.OtherCurrentLiabilities));

            _propertyMappings.Add("Liabilities & Equity" + "Total Current Liabilities", nameof(balanceSheet.TotalCurrentLiabilities));

            _propertyMappings.Add("Liabilities & Equity" + "Long-Term Debt", nameof(balanceSheet.LongTermDebt));
            _propertyMappings.Add("Liabilities & Equity" + "Capital Leases", nameof(balanceSheet.CapitalLeases));
            //_propertyMappings.Add("", nameof(incomeStatement.Deffered));
            _propertyMappings.Add("Liabilities & Equity" + "Other Liabilities", nameof(balanceSheet.OtherLiabilities));

            _propertyMappings.Add("Liabilities & Equity" + "Total Liabilities", nameof(balanceSheet.TotalLiabilities));

            _propertyMappings.Add("Liabilities & Equity" + "Retained Earnings", nameof(balanceSheet.RetainedEarnings));
            _propertyMappings.Add("Liabilities & Equity" + "Paid-in Capital", nameof(balanceSheet.PaidInCapital));
            _propertyMappings.Add("Liabilities & Equity" + "Common Stock", nameof(balanceSheet.CommonStock));
            _propertyMappings.Add("Liabilities & Equity" + "AOCI", nameof(balanceSheet.Aoci));

            _propertyMappings.Add("Liabilities & Equity" + "Treasury Stock", nameof(balanceSheet.TreasuryStock));

            _propertyMappings.Add("Liabilities & Equity" + "Other", nameof(balanceSheet.Other));
            _propertyMappings.Add("Liabilities & Equity" + "Shareholders' Equity", nameof(balanceSheet.ShareholdersEquity));

            _propertyMappings.Add("Liabilities & Equity" + "Liabilities & Equity", nameof(balanceSheet.LiabilitiesAndEquity));
        }

        public override List<ExtractedBalanceSheet> ParseTable(HtmlNode table)
        {
            return ParseTable(table, 1, 0);
        }
    }
}