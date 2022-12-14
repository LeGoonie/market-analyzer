using HtmlAgilityPack;
using MarketAnalyzer.Data;
using MarketAnalyzer.Scrapper.Common;
using System.Collections.Generic;
using System.Linq;

namespace MarketAnalyzer.Scrapper.IncomeStatements
{
    public class IncomeStatementTTMParser : BaseParser<ExtractedIncomeStatementTtm>
    {
        public IncomeStatementTTMParser()
        {
            var incomeStatement = new ExtractedIncomeStatementTtm();
            _propertyMappings.Add("Revenue", nameof(incomeStatement.Revenue));
            _propertyMappings.Add("Cost of Goods Sold", nameof(incomeStatement.CostOfGoodsSold));
            _propertyMappings.Add("Gross Profit", nameof(incomeStatement.GrossProfit));
            _propertyMappings.Add("Operating Expenses" + "Sales, General, & Administrative", nameof(incomeStatement.Sales));
            _propertyMappings.Add("Operating Expenses" + "Research & Development", nameof(incomeStatement.Rd));
            _propertyMappings.Add("Operating Expenses" + "Special Charges", nameof(incomeStatement.SpecialCharges));
            _propertyMappings.Add("Operating Expenses" + "Other Operating Expense", nameof(incomeStatement.OtherExpenses));
            _propertyMappings.Add("Operating Expenses" + "Total Operating Expenses", nameof(incomeStatement.TotalOperatingExpenses));

            _propertyMappings.Add("Operating Expenses" + "Operating Profit", nameof(incomeStatement.OperatingProfit));

            _propertyMappings.Add("Operating Expenses" + "Net Interest Income", nameof(incomeStatement.NetInterestIncome));
            _propertyMappings.Add("Operating Expenses" + "Other Non-Operating Income", nameof(incomeStatement.OtherNonOperatingIncome));

            _propertyMappings.Add("Operating Expenses" + "Pre-Tax Income", nameof(incomeStatement.PreTaxIncome));

            _propertyMappings.Add("Operating Expenses" + "Income Tax", nameof(incomeStatement.IncomeTax));

            _propertyMappings.Add("Operating Expenses" + "Net Income", nameof(incomeStatement.NetIncome));

            _propertyMappings.Add("Operating Expenses" + "EPS (Basic)", nameof(incomeStatement.EpsBasic));
            _propertyMappings.Add("Operating Expenses" + "EPS (Diluted)", nameof(incomeStatement.EpsDiluted));
            _propertyMappings.Add("Operating Expenses" + "Shares (Basic)", nameof(incomeStatement.SharesBasic));
            _propertyMappings.Add("Operating Expenses" + "Shares (Diluted)", nameof(incomeStatement.SharesDiluted));
        }

        public override List<ExtractedIncomeStatementTtm> ParseTable(HtmlNode table)
        {
            var rows = table.Descendants().Where(n => n.Name == "tr");

            var rowsCount = rows.Count();
            var columnsCount = rows.First().Descendants().Where(n => n.Name == "td").Count();
            return ParseTable(table, columnsCount - 1, 0);
        }
    }
}