using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common.ViewModels
{
    public class ObservableCollectionExtended<TItem> : ObservableCollection<TItem>, IViewModel, IActivationAware
    {
        public ObservableCollectionExtended()
        {

        }

        public ObservableCollectionExtended(IEnumerable<TItem> collection)
            : base(collection)
        {

        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            
            base.OnCollectionChanged(e);

            if (GlobalNotificationsEnabled)
            {
                GlobalNotificationsManager.RaiseCollectionChanged(this, e);
            }
        }

        protected virtual bool GlobalNotificationsEnabled
        {
            get { return true; }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            GlobalNotificationsManager.RaisePropertyChanged(this, e.PropertyName);
            base.OnPropertyChanged(e);
        }


        public event EventHandler Activated;

        public void Activate()
        {
            Activated?.Invoke(this, EventArgs.Empty);
        }


    }
}
