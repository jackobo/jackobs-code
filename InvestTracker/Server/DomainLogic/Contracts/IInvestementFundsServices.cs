using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestTracker.DomainLogic.Contracts
{
    public interface IInvestementFundsServices
    {
        Task<IEnumerable<InvestmentSummary>> GetSummary();
    }

    public class InvestmentSummary
    {
        public InvestmentSummary(Guid fundId, string fundName, decimal holdingsAmount, string currency, decimal profitPercentage)
        {
            this.FundId = fundId;
            this.FundName = fundName;
            this.HoldingsAmount = holdingsAmount;
            this.Currency = currency;
            this.ProfitPercentage = profitPercentage;
        }

        public Guid FundId { get; private set; }
        public string FundName { get; private set; }
        public decimal HoldingsAmount { get; private set; }
        public string Currency { get; private set; }
        public decimal ProfitPercentage { get; private set; }
    }
}
