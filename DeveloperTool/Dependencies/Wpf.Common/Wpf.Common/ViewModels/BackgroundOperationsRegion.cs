using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.ViewModels
{
    public class BackgroundOperationsRegion : ServicedViewModelBase, IBackgroundOperationsRegion
    {
        public BackgroundOperationsRegion(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.Operations = new ObservableCollection<BackgroundOperationViewModel>();
        }
        public void RegisterOperation(IBackgroundOperation operation)
        {
            ApplicationServices.ExecuteOnUIThread(() => Operations.Add(new BackgroundOperationViewModel(operation)));
        }
        

        IApplicationServices ApplicationServices
        {
            get { return ServiceLocator.GetInstance<IApplicationServices>(); }
        }

        public void UnregisterOperation(IBackgroundOperation operation)
        {

            var operationViewModel = this.Operations.FirstOrDefault(op => op.Contains(operation));
            if (operationViewModel != null)
                ApplicationServices.ExecuteOnUIThread(() => this.Operations.Remove(operationViewModel));
        }

        public ObservableCollection<BackgroundOperationViewModel> Operations { get; private set; }
    }


    public sealed class BackgroundOperationViewModel : ViewModelBase
    {

        public BackgroundOperationViewModel(IBackgroundOperation operation)
        {
            Operation = operation;
            Operation.PropertyChanged += Operation_PropertyChanged;
            
        }


        public bool Contains(IBackgroundOperation operation)
        {
            return this.Operation.Equals(operation);
        }

        private void Operation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("");
        }

        IBackgroundOperation Operation { get; set; }

        
        public bool InProgress
        {
            get
            {
                return this.Percentage < 100;
            }    
        }
        
        public string Description
        {
            get
            {
                return Operation.OperationDescription;
            }
        }

        
        public decimal Percentage
        {
            get { return Operation.Percentage; }
        
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as BackgroundOperationViewModel;
            if (theOther == null)
                return false;

            return this.Operation.Equals(theOther.Operation);
        }

        public override int GetHashCode()
        {
            return Operation.GetHashCode();
        }

        public override string ToString()
        {
            return Operation.ToString();
        }
    }
}
