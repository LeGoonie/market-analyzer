using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.QuickFsApi
{
    public class CashFlowApi : BaseApi
    {
        public async Task<string> GetCashFlows(string ticker, int numYears)
        {
            var request = await ComposeWebRequest();

            return await CallWebRequest(request, new
            {
                data =
                new
                {
                    cfo_net_income = BuildQfs(ticker, "cfo_net_income", numYears),
                    cfo_da = BuildQfs(ticker, "cfo_da", numYears),
                    cfo_receivables = BuildQfs(ticker, "cfo_receivables", numYears),
                    cfo_inventory = BuildQfs(ticker, "cfo_inventory", numYears),
                    cfo_prepaid_expenses = BuildQfs(ticker, "cfo_prepaid_expenses", numYears),
                    cfo_other_working_capital = BuildQfs(ticker, "cfo_other_working_capital", numYears),
                    cfo_change_in_working_capital = BuildQfs(ticker, "cfo_change_in_working_capital", numYears),
                    cfo_deferred_tax = BuildQfs(ticker, "cfo_deferred_tax", numYears),
                    cfo_stock_comp = BuildQfs(ticker, "cfo_stock_comp", numYears),
                    cfo_other_noncash_items = BuildQfs(ticker, "cfo_other_noncash_items", numYears),
                    cf_cfo = BuildQfs(ticker, "cf_cfo", numYears),
                    cfi_ppe_net = BuildQfs(ticker, "cfi_ppe_net", numYears),
                    cfi_acquisitions_net = BuildQfs(ticker, "cfi_acquisitions_net", numYears),
                    cfi_investment_net = BuildQfs(ticker, "cfi_investment_net", numYears),
                    cfi_intangibles_net = BuildQfs(ticker, "cfi_intangibles_net", numYears),
                    cfi_other = BuildQfs(ticker, "cfi_other", numYears),
                    cf_cfi = BuildQfs(ticker, "cf_cfi", numYears),
                    cff_common_stock_net = BuildQfs(ticker, "cff_common_stock_net", numYears),
                    cff_pfd_net = BuildQfs(ticker, "cff_pfd_net", numYears),
                    cff_debt_net = BuildQfs(ticker, "cff_debt_net", numYears),
                    cff_dividend_paid = BuildQfs(ticker, "cff_dividend_paid", numYears),
                    cff_other = BuildQfs(ticker, "cff_other", numYears),
                    cf_cff = BuildQfs(ticker, "cf_cff", numYears),
                    cf_forex = BuildQfs(ticker, "cf_forex", numYears),
                    cf_net_change_in_cash = BuildQfs(ticker, "cf_net_change_in_cash", numYears),
                    capex = BuildQfs(ticker, "capex", numYears),
                }
            });
        }

        public class CashFlowJson
        {
            public List<decimal> cfo_net_income { get; set; }
            public List<decimal> cfo_da { get; set; }
            public List<decimal> cfo_receivables { get; set; }
            public List<decimal> cfo_inventory { get; set; }
            public List<decimal> cfo_prepaid_expenses { get; set; }
            public List<decimal> cfo_other_working_capital { get; set; }
            public List<decimal> cfo_change_in_working_capital { get; set; }
            public List<decimal> cfo_deferred_tax { get; set; }
            public List<decimal> cfo_stock_comp { get; set; }
            public List<decimal> cfo_other_noncash_items { get; set; }
            public List<decimal> cf_cfo { get; set; }
            public List<decimal> cfi_ppe_net { get; set; }
            public List<decimal> cfi_acquisitions_net { get; set; }
            public List<decimal> cfi_investment_net { get; set; }
            public List<decimal> cfi_intangibles_net { get; set; }
            public List<decimal> cfi_other { get; set; }
            public List<decimal> cf_cfi { get; set; }
            public List<decimal> cff_common_stock_net { get; set; }
            public List<decimal> cff_pfd_net { get; set; }
            public List<decimal> cff_debt_net { get; set; }
            public List<decimal> cff_dividend_paid { get; set; }
            public List<decimal> cff_other { get; set; }
            public List<decimal> cf_cff { get; set; }
            public List<decimal> cf_forex { get; set; }
            public List<decimal> cf_net_change_in_cash { get; set; }
            public List<decimal> capex { get; set; }
        }
    }
}