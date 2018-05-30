using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.TFS
{
    internal class TfsWorkItem : ISourceControlWorkItem
    {
        public TfsWorkItem(WorkItem workItem)
        {
            _workItem = workItem;
        }

        WorkItem _workItem;
        public string AssignedTo
        {
            get
            {
                
                return _workItem.CreatedBy;
            }
        }

        public int Id
        {
            get
            {
                return _workItem.Id;
            }
        }

        public string Title
        {
            get
            {
                return _workItem.Title;
            }
        }


    }
}
