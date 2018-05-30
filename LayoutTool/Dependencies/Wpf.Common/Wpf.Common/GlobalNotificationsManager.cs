using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Wpf.Common
{
    public static class GlobalNotificationsManager
    {
        public static event PropertyChangedEventHandler PropertyChanged;
        public static event NotifyCollectionChangedEventHandler CollectionChanged;

        public static void RaisePropertyChanged(object sender, string propertyName)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }

        public static void RaiseCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(sender, args);
        }
    }
}
