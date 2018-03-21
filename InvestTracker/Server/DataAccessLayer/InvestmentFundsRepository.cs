using InvestTracker.DataAccessLayer.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvestTracker.DataAccessLayer
{
    public class InvestmentFundsRepository : IInvestmentFundsRepository
    {
        async public Task<IEnumerable<FundModel>> GetFunds()
        {
            return await Task.Run(() => new FundModel[]
            {
                new FundModel(Guid.NewGuid(), "Fund1", "EUR"),
                new FundModel(Guid.NewGuid(), "Fund2", "RON")
            });
        }
    }
}
