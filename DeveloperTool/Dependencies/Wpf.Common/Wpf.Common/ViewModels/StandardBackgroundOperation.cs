using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.ViewModels
{
    public class StandardBackgroundOperation : ViewModelBase, IBackgroundOperation
    {
        public void Update(decimal percentage, string description)
        {
            this.Percentage = percentage;
            this.OperationDescription = description;
            if (percentage < 100)
                this.Status = BackgroundOperationStatus.InProgress;
            else
                this.Status = BackgroundOperationStatus.Done;
        }


        public void Failed(string operationDescription, string errorDetails)
        {
            this.OperationDescription = operationDescription;
            this.ErrorDetailes = errorDetails;
            this.Percentage = 100;
            this.Status = BackgroundOperationStatus.Failed;
            
        }

        string _operationDescription;
        public string OperationDescription
        {
            get
            {
                return _operationDescription;
            }
            private set
            {
                SetProperty(ref _operationDescription, value);
            }
        }

      

        string _errorDetailes;
        public string ErrorDetailes
        {
            get
            {
                return _errorDetailes;
            }
            private set
            {
                SetProperty(ref _errorDetailes, value);
            }
        }

        decimal _percentage;
        public decimal Percentage
        {
            get
            {
                return _percentage;
            }
            private set
            {
                SetProperty(ref _percentage, value);

            }
        }

        BackgroundOperationStatus _status = BackgroundOperationStatus.NotStarted;
        public BackgroundOperationStatus Status
        {
            get
            {
                return _status;
            }
            private set
            {
                SetProperty(ref _status, value);
            }
        }

        
    }
}
