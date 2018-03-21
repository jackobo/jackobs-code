using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestTracker.DataAccessLayer.Contracts
{
    public interface IInvestmentFundsRepository
    {
        Task<IEnumerable<FundModel>> GetFunds();
    }

    public class FundModel
    {
        public FundModel()
        {

        }

        public FundModel(Guid id, string name, string currency)
        {
            this.Id = id;
            this.Name = name;
            this.Currency = currency;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
    }

}
