using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using InvestTracker.DomainLogic.Contracts;

namespace InvestTracker.WebApi.Controllers
{
    
    public class HomeController : ApiController
    {
        public HomeController(IInvestementFundsServices investmentsFundsServices)
        {
            _investmentsFundsServices = investmentsFundsServices;
        }

        IInvestementFundsServices _investmentsFundsServices;

        [HttpGet]
        [ActionName("currentInvestments")]
        async public Task<IHttpActionResult> GetCurrentInvestments()
        {
            var funds = (await _investmentsFundsServices.GetSummary())
                        .Select(f => new GetCurrentInvestementsResponse.Fund(f.FundId, f.FundName, f.HoldingsAmount, f.Currency, f.ProfitPercentage))
                        .ToArray();

            return Ok(new GetCurrentInvestementsResponse(funds));
        }
    }

    public class GetCurrentInvestementsResponse
    {

        public GetCurrentInvestementsResponse()
        {

        }

        public GetCurrentInvestementsResponse(Fund[] funds)
        {
            this.Funds = funds;
        }

        public Fund[] Funds { get; set; }

        public class Fund
        {
            public Fund()
            {

            }

            public Fund(Guid fundId, string fundName, decimal holdingsAmount, string currency, decimal profitPercentage)
            {
                this.FundId = fundId;
                this.FundName = fundName;
                this.HoldingsAmount = holdingsAmount;
                this.Currency = currency;
                this.ProfitPercentage = profitPercentage;
            }
            public Guid FundId { get; set; }
            public string FundName { get; set; }
            public decimal HoldingsAmount { get; set; }
            public string Currency { get; set; }
            public decimal ProfitPercentage { get; set; }
        }


    }
}
