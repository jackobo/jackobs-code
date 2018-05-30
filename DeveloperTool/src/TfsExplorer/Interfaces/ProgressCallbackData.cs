using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces
{
    public class ProgressCallbackData
    {
        public ProgressCallbackData(decimal percentage, string actionDescription)
        {
            this.Percentage = percentage;
            this.ActionDescription = actionDescription;
        }

        public decimal Percentage { get; private set; }

        public string ActionDescription { get; private set; }

        public static ProgressCallbackData Create(decimal current, decimal total, string description)
        {
            if (total <= 0)
                throw new ArgumentException($"{nameof(total)} must be greather than zero!");

            return new ProgressCallbackData((current / total) * 100, description);
        }
    }
}
