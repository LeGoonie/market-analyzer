using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.QuickFsApi
{
    public class IncomeStatementApi : BaseApi
    {
        public async Task<string> GetIncomeStatements(string ticker, int numYears)
        {
            var request = await ComposeWebRequest();

            return await CallWebRequest(request, new
            {
                data =
                new
                {
                    revenue = BuildQfs(ticker, "revenue", numYears),
                    cogs = BuildQfs(ticker, "cogs", numYears),
                    gross_profit = BuildQfs(ticker, "gross_profit", numYears),
                    sga = BuildQfs(ticker, "sga", numYears),
                    rnd = BuildQfs(ticker, "rnd", numYears),
                    special_charges = BuildQfs(ticker, "special_charges", numYears),
                    other_opex = BuildQfs(ticker, "other_opex", numYears),
                    total_opex = BuildQfs(ticker, "total_opex", numYears),
                    operating_income = BuildQfs(ticker, "operating_income", numYears),
                    net_interest_income_normal = BuildQfs(ticker, "net_interest_income_normal", numYears),
                    other_nonoperating_income = BuildQfs(ticker, "other_nonoperating_income", numYears),
                    pretax_income = BuildQfs(ticker, "pretax_income", numYears),
                    income_tax = BuildQfs(ticker, "income_tax", numYears),
                    net_income_continuing = BuildQfs(ticker, "net_income_continuing", numYears),
                    interest_income = BuildQfs(ticker, "interest_income", numYears),
                    interest_expense = BuildQfs(ticker, "interest_expense", numYears),
                    net_income_discontinued = BuildQfs(ticker, "net_income_discontinued", numYears),
                    income_allocated_to_minority_interest = BuildQfs(ticker, "income_allocated_to_minority_interest", numYears),
                    other_income_statement_items = BuildQfs(ticker, "other_income_statement_items", numYears),
                    net_income = BuildQfs(ticker, "net_income", numYears),
                    preferred_dividends = BuildQfs(ticker, "preferred_dividends", numYears),
                    net_income_available_to_shareholders = BuildQfs(ticker, "net_income_available_to_shareholders", numYears),
                    eps_basic = BuildQfs(ticker, "eps_basic", numYears),
                    eps_diluted = BuildQfs(ticker, "eps_diluted", numYears),
                    shares_basic = BuildQfs(ticker, "shares_basic", numYears),
                    shares_diluted = BuildQfs(ticker, "shares_diluted", numYears),
                    total_interest_income = BuildQfs(ticker, "total_interest_income", numYears),
                    total_interest_expense = BuildQfs(ticker, "total_interest_expense", numYears),
                    net_interest_income = BuildQfs(ticker, "net_interest_income", numYears),
                    total_noninterest_revenue = BuildQfs(ticker, "total_noninterest_revenue", numYears),
                    credit_losses_provision = BuildQfs(ticker, "credit_losses_provision", numYears),
                    net_interest_income_after_credit_losses_provision = BuildQfs(ticker, "net_interest_income_after_credit_losses_provision", numYears),
                    total_noninterest_expense = BuildQfs(ticker, "total_noninterest_expense", numYears),
                    premiums_earned = BuildQfs(ticker, "premiums_earned", numYears),
                    net_investment_income = BuildQfs(ticker, "net_investment_income", numYears),
                    fees_and_other_income = BuildQfs(ticker, "fees_and_other_income", numYears),
                    net_policyholder_claims_expense = BuildQfs(ticker, "net_policyholder_claims_expense", numYears),
                    policy_acquisition_expense = BuildQfs(ticker, "policy_acquisition_expense", numYears),
                    interest_expense_insurance = BuildQfs(ticker, "interest_expense_insurance", numYears),
                }
            });
        }
    }

    public class IncomeStatementJson
    {
        public List<decimal> revenue { get; set; }
        public List<decimal> cogs { get; set; }
        public List<decimal> gross_profit { get; set; }
        public List<decimal> sga { get; set; }
        public List<decimal> rnd { get; set; }
        public List<decimal> special_charges { get; set; }
        public List<decimal> other_opex { get; set; }
        public List<decimal> total_opex { get; set; }
        public List<decimal> operating_income { get; set; }
        public List<decimal> net_interest_income_normal { get; set; }
        public List<decimal> other_nonoperating_income { get; set; }
        public List<decimal> pretax_income { get; set; }
        public List<decimal> income_tax { get; set; }
        public List<decimal> net_income_continuing { get; set; }
        public List<decimal> interest_income { get; set; }
        public List<decimal> interest_expense { get; set; }
        public List<decimal> net_income_discontinued { get; set; }
        public List<decimal> income_allocated_to_minority_interest { get; set; }
        public List<decimal> other_income_statement_items { get; set; }
        public List<decimal> net_income { get; set; }
        public List<decimal> preferred_dividends { get; set; }
        public List<decimal> net_income_available_to_shareholders { get; set; }
        public List<decimal> eps_basic { get; set; }
        public List<decimal> eps_diluted { get; set; }
        public List<decimal> shares_basic { get; set; }
        public List<decimal> shares_diluted { get; set; }
        public List<decimal> total_interest_income { get; set; }
        public List<decimal> total_interest_expense { get; set; }
        public List<decimal> net_interest_income { get; set; }
        public List<decimal> total_noninterest_revenue { get; set; }
        public List<decimal> credit_losses_provision { get; set; }
        public List<decimal> net_interest_income_after_credit_losses_provision { get; set; }
        public List<decimal> total_noninterest_expense { get; set; }
        public List<decimal> premiums_earned { get; set; }
        public List<decimal> net_investment_income { get; set; }
        public List<decimal> fees_and_other_income { get; set; }
        public List<decimal> net_policyholder_claims_expense { get; set; }
        public List<decimal> policy_acquisition_expense { get; set; }
        public List<decimal> interest_expense_insurance { get; set; }
    }
}