using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.QuickFsApi
{
    public class BalanceSheetApi : BaseApi
    {
        public async Task<string> GetBalanceSheets(string ticker, int numYears)
        {
            var request = await ComposeWebRequest();

            return await CallWebRequest(request, new
            {
                data =
                new
                {
                    cash_and_equiv = BuildQfs(ticker, "cash_and_equiv", numYears),
                    st_investments = BuildQfs(ticker, "st_investments", numYears),
                    receivables = BuildQfs(ticker, "receivables", numYears),
                    inventories = BuildQfs(ticker, "inventories", numYears),
                    other_current_assets = BuildQfs(ticker, "other_current_assets", numYears),
                    total_current_assets = BuildQfs(ticker, "total_current_assets", numYears),
                    equity_and_other_investments = BuildQfs(ticker, "equity_and_other_investments", numYears),
                    ppe_gross = BuildQfs(ticker, "ppe_gross", numYears),
                    accumulated_depreciation = BuildQfs(ticker, "accumulated_depreciation", numYears),
                    ppe_net = BuildQfs(ticker, "ppe_net", numYears),
                    intangible_assets = BuildQfs(ticker, "intangible_assets", numYears),
                    goodwill = BuildQfs(ticker, "goodwill", numYears),
                    other_lt_assets = BuildQfs(ticker, "other_lt_assets", numYears),
                    total_assets = BuildQfs(ticker, "total_assets", numYears),
                    accounts_payable = BuildQfs(ticker, "accounts_payable", numYears),
                    tax_payable = BuildQfs(ticker, "tax_payable", numYears),
                    current_accrued_liabilities = BuildQfs(ticker, "current_accrued_liabilities", numYears),
                    st_debt = BuildQfs(ticker, "st_debt", numYears),
                    current_deferred_revenue = BuildQfs(ticker, "current_deferred_revenue", numYears),
                    current_deferred_tax_liability = BuildQfs(ticker, "current_deferred_tax_liability", numYears),
                    other_current_liabilities = BuildQfs(ticker, "other_current_liabilities", numYears),
                    total_current_liabilities = BuildQfs(ticker, "total_current_liabilities", numYears),
                    lt_debt = BuildQfs(ticker, "lt_debt", numYears),
                    noncurrent_capital_leases = BuildQfs(ticker, "noncurrent_capital_leases", numYears),
                    pension_liabilities = BuildQfs(ticker, "pension_liabilities", numYears),
                    noncurrent_deferred_revenue = BuildQfs(ticker, "noncurrent_deferred_revenue", numYears),
                    other_lt_liabilities = BuildQfs(ticker, "other_lt_liabilities", numYears),
                    total_liabilities = BuildQfs(ticker, "total_liabilities", numYears),
                    common_stock = BuildQfs(ticker, "common_stock", numYears),
                    preferred_stock = BuildQfs(ticker, "preferred_stock", numYears),
                    retained_earnings = BuildQfs(ticker, "retained_earnings", numYears),
                    aoci = BuildQfs(ticker, "aoci", numYears),
                    apic = BuildQfs(ticker, "apic", numYears),
                    treasury_stock = BuildQfs(ticker, "treasury_stock", numYears),
                    other_equity = BuildQfs(ticker, "other_equity", numYears),
                    minority_interest_liability = BuildQfs(ticker, "minority_interest_liability", numYears),
                    total_equity = BuildQfs(ticker, "total_equity", numYears),
                    total_liabilities_and_equity = BuildQfs(ticker, "total_liabilities_and_equity", numYears),
                    long_term_debt_and_capital_lease_obligation = BuildQfs(ticker, "long_term_debt_and_capital_lease_obligation", numYears),

                    loans_gross = BuildQfs(ticker, "loans_gross", numYears),
                    allowance_for_loan_losses = BuildQfs(ticker, "allowance_for_loan_losses", numYears),
                    loans_net = BuildQfs(ticker, "loans_net", numYears),
                    total_investments = BuildQfs(ticker, "total_investments", numYears),
                    deposits_liability = BuildQfs(ticker, "deposits_liability", numYears),
                    deferred_policy_acquisition_cost = BuildQfs(ticker, "deferred_policy_acquisition_cost", numYears),
                    unearned_premiums = BuildQfs(ticker, "unearned_premiums", numYears),
                    future_policy_benefits = BuildQfs(ticker, "future_policy_benefits", numYears),
                }
            });
        }

        public class BalanceSheetJson
        {
            public List<decimal> cash_and_equiv { get; set; }
            public List<decimal> st_investments { get; set; }
            public List<decimal> receivables { get; set; }
            public List<decimal> inventories { get; set; }
            public List<decimal> other_current_assets { get; set; }
            public List<decimal> total_current_assets { get; set; }
            public List<decimal> equity_and_other_investments { get; set; }
            public List<decimal> ppe_gross { get; set; }
            public List<decimal> accumulated_depreciation { get; set; }
            public List<decimal> ppe_net { get; set; }
            public List<decimal> intangible_assets { get; set; }
            public List<decimal> goodwill { get; set; }
            public List<decimal> other_lt_assets { get; set; }
            public List<decimal> total_assets { get; set; }
            public List<decimal> accounts_payable { get; set; }
            public List<decimal> tax_payable { get; set; }
            public List<decimal> current_accrued_liabilities { get; set; }
            public List<decimal> st_debt { get; set; }
            public List<decimal> current_deferred_revenue { get; set; }
            public List<decimal> current_deferred_tax_liability { get; set; }
            public List<decimal> other_current_liabilities { get; set; }
            public List<decimal> total_current_liabilities { get; set; }
            public List<decimal> lt_debt { get; set; }
            public List<decimal> noncurrent_capital_leases { get; set; }
            public List<decimal> pension_liabilities { get; set; }
            public List<decimal> noncurrent_deferred_revenue { get; set; }
            public List<decimal> other_lt_liabilities { get; set; }
            public List<decimal> total_liabilities { get; set; }
            public List<decimal> common_stock { get; set; }
            public List<decimal> preferred_stock { get; set; }
            public List<decimal> retained_earnings { get; set; }
            public List<decimal> aoci { get; set; }
            public List<decimal> apic { get; set; }
            public List<decimal> treasury_stock { get; set; }
            public List<decimal> other_equity { get; set; }
            public List<decimal> minority_interest_liability { get; set; }
            public List<decimal> total_equity { get; set; }
            public List<decimal> total_liabilities_and_equity { get; set; }
            public List<decimal> long_term_debt_and_capital_lease_obligation { get; set; }
            public List<decimal> loans_gross { get; set; }
            public List<decimal> allowance_for_loan_losses { get; set; }
            public List<decimal> loans_net { get; set; }
            public List<decimal> total_investments { get; set; }
            public List<decimal> deposits_liability { get; set; }
            public List<decimal> deferred_policy_acquisition_cost { get; set; }
            public List<decimal> unearned_premiums { get; set; }
            public List<decimal> future_policy_benefits { get; set; }
        }
    }
}