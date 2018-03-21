using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestTracker.DomainLogic.Contracts;
using InvestTracker.DataAccessLayer.Contracts;

namespace InvestTracker.DomainLogic
{
    public class InvestementFundsServices : IInvestementFundsServices
    {
        public InvestementFundsServices(IInvestmentFundsRepository fundsRepository)
        {
            _fundsRepository = fundsRepository;
        }

        IInvestmentFundsRepository _fundsRepository;
        async public Task<IEnumerable<InvestmentSummary>> GetSummary()
        {
            return new InvestmentSummary[]
            {
                new InvestmentSummary(Guid.NewGuid(), "Fund1", 1000, "RON", 1m),
                new InvestmentSummary(Guid.NewGuid(), "Fund2", 200, "EUR", 2m),
            };
        }
    }


}
