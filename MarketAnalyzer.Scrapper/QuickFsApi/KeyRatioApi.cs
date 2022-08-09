using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketAnalyzer.Scrapper.QuickFsApi
{
    public class KeyRatioApi : BaseApi
    {
        public async Task<string> GetDividends(string ticker, int numYears)
        {
            var request = await ComposeWebRequest();

            return await CallWebRequest(request, new
            {
                data =
                new
                {
                    dividends = BuildQfs(ticker, "dividends", numYears),
                }
            });
        }

        public async Task<string> GetKeyRatios(string ticker, int numYears)
        {
            var request = await ComposeWebRequest();

            return await CallWebRequest(request, new
            {
                data =
                new
                {
                    ebitda = BuildQfs(ticker, "ebitda", numYears),
                    fcf = BuildQfs(ticker, "fcf", numYears),
                    book_value = BuildQfs(ticker, "book_value", numYears),
                    tangible_book_value = BuildQfs(ticker, "tangible_book_value", numYears),
                    roa = BuildQfs(ticker, "roa", numYears),
                    roe = BuildQfs(ticker, "roe", numYears),
                    roic = BuildQfs(ticker, "roic", numYears),
                    roce = BuildQfs(ticker, "roce", numYears),
                    rotce = BuildQfs(ticker, "rotce", numYears),
                    gross_margin = BuildQfs(ticker, "gross_margin", numYears),
                    ebitda_margin = BuildQfs(ticker, "ebitda_margin", numYears),
                    operating_margin = BuildQfs(ticker, "operating_margin", numYears),
                    pretax_margin = BuildQfs(ticker, "pretax_margin", numYears),
                    net_income_margin = BuildQfs(ticker, "net_income_margin", numYears),
                    fcf_margin = BuildQfs(ticker, "fcf_margin", numYears),
                    assets_to_equity = BuildQfs(ticker, "assets_to_equity", numYears),
                    equity_to_assets = BuildQfs(ticker, "equity_to_assets", numYears),
                    debt_to_equity = BuildQfs(ticker, "debt_to_equity", numYears),
                    debt_to_assets = BuildQfs(ticker, "debt_to_assets", numYears),
                    revenue_per_share = BuildQfs(ticker, "revenue_per_share", numYears),
                    ebitda_per_share = BuildQfs(ticker, "ebitda_per_share", numYears),
                    operating_income_per_share = BuildQfs(ticker, "operating_income_per_share", numYears),
                    pretax_income_per_share = BuildQfs(ticker, "pretax_income_per_share", numYears),
                    fcf_per_share = BuildQfs(ticker, "fcf_per_share", numYears),
                    book_value_per_share = BuildQfs(ticker, "book_value_per_share", numYears),
                    tangible_book_per_share = BuildQfs(ticker, "tangible_book_per_share", numYears),
                    market_cap = BuildQfs(ticker, "market_cap", numYears),
                    enterprise_value = BuildQfs(ticker, "enterprise_value", numYears),
                    price_to_earnings = BuildQfs(ticker, "price_to_earnings", numYears),
                    price_to_book = BuildQfs(ticker, "price_to_book", numYears),
                    price_to_tangible_book = BuildQfs(ticker, "price_to_tangible_book", numYears),
                    price_to_sales = BuildQfs(ticker, "price_to_sales", numYears),
                    price_to_fcf = BuildQfs(ticker, "price_to_fcf", numYears),
                    price_to_pretax_income = BuildQfs(ticker, "price_to_pretax_income", numYears),
                    enterprise_value_to_earnings = BuildQfs(ticker, "enterprise_value_to_earnings", numYears),
                    enterprise_value_to_book = BuildQfs(ticker, "enterprise_value_to_book", numYears),
                    enterprise_value_to_tangible_book = BuildQfs(ticker, "enterprise_value_to_tangible_book", numYears),
                    enterprise_value_to_sales = BuildQfs(ticker, "enterprise_value_to_sales", numYears),
                    enterprise_value_to_fcf = BuildQfs(ticker, "enterprise_value_to_fcf", numYears),
                    enterprise_value_to_pretax_income = BuildQfs(ticker, "enterprise_value_to_pretax_income", numYears),
                    revenue_growth = BuildQfs(ticker, "revenue_growth", numYears),
                    gross_profit_growth = BuildQfs(ticker, "gross_profit_growth", numYears),
                    ebitda_growth = BuildQfs(ticker, "ebitda_growth", numYears),
                    operating_income_growth = BuildQfs(ticker, "operating_income_growth", numYears),
                    pretax_income_growth = BuildQfs(ticker, "pretax_income_growth", numYears),
                    net_income_growth = BuildQfs(ticker, "net_income_growth", numYears),
                    eps_diluted_growth = BuildQfs(ticker, "eps_diluted_growth", numYears),
                    shares_diluted_growth = BuildQfs(ticker, "shares_diluted_growth", numYears),
                    cash_and_equiv_growth = BuildQfs(ticker, "cash_and_equiv_growth", numYears),
                    ppe_growth = BuildQfs(ticker, "ppe_growth", numYears),
                    total_assets_growth = BuildQfs(ticker, "total_assets_growth", numYears),
                    total_equity_growth = BuildQfs(ticker, "total_equity_growth", numYears),
                    cfo_growth = BuildQfs(ticker, "cfo_growth", numYears),
                    capex_growth = BuildQfs(ticker, "capex_growth", numYears),
                    fcf_growth = BuildQfs(ticker, "fcf_growth", numYears),
                    revenue_cagr_10 = BuildQfs(ticker, "revenue_cagr_10", numYears),
                    eps_diluted_cagr_10 = BuildQfs(ticker, "eps_diluted_cagr_10", numYears),
                    total_assets_cagr_10 = BuildQfs(ticker, "total_assets_cagr_10", numYears),
                    total_equity_cagr_10 = BuildQfs(ticker, "total_equity_cagr_10", numYears),
                    fcf_cagr_10 = BuildQfs(ticker, "fcf_cagr_10", numYears),
                    payout_ratio = BuildQfs(ticker, "payout_ratio", numYears),
                    //gross_margin_median = BuildQfs(ticker, "gross_margin_median", numYears),
                    //pretax_margin_median = BuildQfs(ticker, "pretax_margin_median", numYears),
                    //operating_income_margin_median = BuildQfs(ticker, "operating_income_margin_median", numYears),
                    //fcf_margin_median = BuildQfs(ticker, "fcf_margin_median", numYears),
                    //roa_median = BuildQfs(ticker, "roa_median", numYears),
                    //roe_median = BuildQfs(ticker, "roe_median", numYears),
                    //roic_median = BuildQfs(ticker, "roic_median", numYears),
                    //assets_to_equity_median = BuildQfs(ticker, "assets_to_equity_median", numYears),
                    //debt_to_assets_median = BuildQfs(ticker, "debt_to_assets_median", numYears),
                    //debt_to_equity_median = BuildQfs(ticker, "debt_to_equity_median", numYears),
                    //earning_assets = BuildQfs(ticker, "earning_assets", numYears),
                    //net_interest_margin = BuildQfs(ticker, "net_interest_margin", numYears),
                    //earning_assets_to_equity = BuildQfs(ticker, "earning_assets_to_equity", numYears),
                    //loans_to_deposits = BuildQfs(ticker, "loans_to_deposits", numYears),
                    //loan_loss_reserve_to_loans = BuildQfs(ticker, "loan_loss_reserve_to_loans", numYears),
                    //net_interest_income_growth = BuildQfs(ticker, "net_interest_income_growth", numYears),
                    //loans_gross_growth = BuildQfs(ticker, "loans_gross_growth", numYears),
                    //loans_net_growth = BuildQfs(ticker, "loans_net_growth", numYears),
                    //deposits_growth = BuildQfs(ticker, "deposits_growth", numYears),
                    //earning_assets_growth = BuildQfs(ticker, "earning_assets_growth", numYears),
                    //net_interest_income_cagr_10 = BuildQfs(ticker, "net_interest_income_cagr_10", numYears),
                    //loans_gross_cagr_10 = BuildQfs(ticker, "loans_gross_cagr_10", numYears),
                    //earning_assets_cagr_10 = BuildQfs(ticker, "earning_assets_cagr_10", numYears),
                    //deposits_cagr_10 = BuildQfs(ticker, "deposits_cagr_10", numYears),
                    //nim_median = BuildQfs(ticker, "nim_median", numYears),
                    //earning_assets_to_equity_median = BuildQfs(ticker, "earning_assets_to_equity_median", numYears),
                    //equity_to_assets_median = BuildQfs(ticker, "equity_to_assets_median", numYears),
                    //loans_to_deposits_median = BuildQfs(ticker, "loans_to_deposits_median", numYears),
                    //loan_loss_reserve_to_loans_median = BuildQfs(ticker, "loan_loss_reserve_to_loans_median", numYears),
                    //policy_revenue = BuildQfs(ticker, "policy_revenue", numYears),
                    //underwriting_profit = BuildQfs(ticker, "underwriting_profit", numYears),
                    //roi = BuildQfs(ticker, "roi", numYears),
                    //underwriting_margin = BuildQfs(ticker, "underwriting_margin", numYears),
                    //premiums_per_share = BuildQfs(ticker, "premiums_per_share", numYears),
                    //premiums_growth = BuildQfs(ticker, "premiums_growth", numYears),
                    //policy_revenue_growth = BuildQfs(ticker, "policy_revenue_growth", numYears),
                    //total_investments_growth = BuildQfs(ticker, "total_investments_growth", numYears),
                    //premiums_cagr_10 = BuildQfs(ticker, "premiums_cagr_10", numYears),
                    //total_investments_cagr_10 = BuildQfs(ticker, "total_investments_cagr_10", numYears),
                    //roi_median = BuildQfs(ticker, "roi_median", numYears),
                    //underwriting_margin_median = BuildQfs(ticker, "underwriting_margin_median", numYears),
                    dividends = BuildQfs(ticker, "dividends", numYears)
                }
            });
        }

        public class KeyRatioJson
        {
            public List<double> ebitda { get; set; }
            public List<decimal> fcf { get; set; }
            public List<decimal> book_value { get; set; }
            public List<decimal> tangible_book_value { get; set; }
            public List<double> roa { get; set; }
            public List<double> roe { get; set; }
            public List<double> roic { get; set; }
            public List<double> roce { get; set; }
            public List<double> rotce { get; set; }
            public List<double> gross_margin { get; set; }
            public List<double> ebitda_margin { get; set; }
            public List<double> operating_margin { get; set; }
            public List<double> pretax_margin { get; set; }
            public List<double> net_income_margin { get; set; }
            public List<double> fcf_margin { get; set; }
            public List<double> assets_to_equity { get; set; }
            public List<double> equity_to_assets { get; set; }
            public List<double> debt_to_equity { get; set; }
            public List<double> debt_to_assets { get; set; }
            public List<double> revenue_per_share { get; set; }
            public List<double> ebitda_per_share { get; set; }
            public List<double> operating_income_per_share { get; set; }
            public List<double> pretax_income_per_share { get; set; }
            public List<double> fcf_per_share { get; set; }
            public List<double> book_value_per_share { get; set; }
            public List<double> tangible_book_per_share { get; set; }
            public List<decimal> market_cap { get; set; }
            public List<double> enterprise_value { get; set; }
            public List<double> price_to_earnings { get; set; }
            public List<double> price_to_book { get; set; }
            public List<double> price_to_tangible_book { get; set; }
            public List<double> price_to_sales { get; set; }
            public List<double> price_to_fcf { get; set; }
            public List<double> price_to_pretax_income { get; set; }
            public List<double> enterprise_value_to_earnings { get; set; }
            public List<double> enterprise_value_to_book { get; set; }
            public List<double> enterprise_value_to_tangible_book { get; set; }
            public List<double> enterprise_value_to_sales { get; set; }
            public List<double> enterprise_value_to_fcf { get; set; }
            public List<double> enterprise_value_to_pretax_income { get; set; }
            public List<double> revenue_growth { get; set; }
            public List<double> gross_profit_growth { get; set; }
            public List<double> ebitda_growth { get; set; }
            public List<double> operating_income_growth { get; set; }
            public List<double> pretax_income_growth { get; set; }
            public List<double> net_income_growth { get; set; }
            public List<double> eps_diluted_growth { get; set; }
            public List<double> shares_diluted_growth { get; set; }
            public List<double> cash_and_equiv_growth { get; set; }
            public List<double> ppe_growth { get; set; }
            public List<double> total_assets_growth { get; set; }
            public List<double> total_equity_growth { get; set; }
            public List<double> cfo_growth { get; set; }
            public List<double> capex_growth { get; set; }
            public List<double> fcf_growth { get; set; }
            public List<double> revenue_cagr_10 { get; set; }
            public List<double> eps_diluted_cagr_10 { get; set; }
            public List<double> total_assets_cagr_10 { get; set; }
            public List<double> total_equity_cagr_10 { get; set; }
            public List<double> fcf_cagr_10 { get; set; }
            public List<double> payout_ratio { get; set; }
            //public double gross_margin_median { get; set; }
            //public double pretax_margin_median { get; set; }
            //public double operating_income_margin_median { get; set; }
            //public double fcf_margin_median { get; set; }
            //public double roa_median { get; set; }
            //public double roe_median { get; set; }
            //public double roic_median { get; set; }
            //public double assets_to_equity_median { get; set; }
            //public double debt_to_assets_median { get; set; }
            //public double debt_to_equity_median { get; set; }

            //public List<double> earning_assets { get; set; }
            //public List<double> net_interest_margin { get; set; }
            //public List<double> earning_assets_to_equity { get; set; }
            //public List<double> loans_to_deposits { get; set; }

            //public List<double> loan_loss_reserve_to_loans { get; set; }
            //public List<double> net_interest_income_growth { get; set; }

            //public List<double> loans_gross_growth { get; set; }
            //public List<double> loans_net_growth { get; set; }

            //public List<double> deposits_growth { get; set; }
            //public List<double> earning_assets_growth { get; set; }

            //public List<double> net_interest_income_cagr_10 { get; set; }
            //public List<double> loans_gross_cagr_10 { get; set; }

            //public List<double> earning_assets_cagr_10 { get; set; }
            //public List<double> deposits_cagr_10 { get; set; }

            //public List<double> nim_median { get; set; }
            //public List<double> earning_assets_to_equity_median { get; set; }

            //public List<double> equity_to_assets_median { get; set; }
            //public List<double> loans_to_deposits_median { get; set; }

            //public List<double> loan_loss_reserve_to_loans_median { get; set; }
            //public List<double> policy_revenue { get; set; }

            //public List<double> underwriting_profit { get; set; }
            //public List<double> roi { get; set; }

            //public List<double> underwriting_margin { get; set; }
            //public List<double> premiums_per_share { get; set; }

            //public List<double> premiums_growth { get; set; }
            //public List<double> policy_revenue_growth { get; set; }

            //public List<double> total_investments_growth { get; set; }
            //public List<double> premiums_cagr_10 { get; set; }

            //public List<double> total_investments_cagr_10 { get; set; }
            //public List<double> roi_median { get; set; }

            //public List<double> underwriting_margin_median { get; set; }

            public List<double> dividends { get; set; }
        }
    }
}