using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.NotificationArea
{
    public class NotificationAreaViewModel : ViewModelBase, INotificationArea
    {
        public NotificationAreaViewModel(IUnityContainer container)
        {
            this.Container = container;
            this.Categories = new ObservableCollection<INotificationCategory>();
            var backgroundActionsArea = container.Resolve<BackgroundActionsArea>();
            backgroundActionsArea.IsSelected = true;
            RegisterCategory<IBackgroundActionsArea>(backgroundActionsArea);
            RegisterCategory<IInformationArea>(container.Resolve<InformationArea>());
        }
        
        IUnityContainer Container { get; set; }

        public ObservableCollection<INotificationCategory> Categories { get; private set; }
        

        #region INotificationArea Members

        public void RegisterCategory<TInterface>(TInterface implementation) where TInterface : INotificationCategory
        {
            Container.RegisterInstance<TInterface>(implementation);
            this.Categories.Add(implementation);
        }

        public TInterface GetCategory<TInterface>() where TInterface : INotificationCategory
        {
            return Container.Resolve<TInterface>();
        }

        #endregion
    }


    public interface INotificationArea
    {
        ObservableCollection<INotificationCategory> Categories { get; }
        void RegisterCategory<TInterface>(TInterface implementation) where TInterface : INotificationCategory;
        TInterface GetCategory<TInterface>() where TInterface : INotificationCategory;
    }


    public interface INotificationCategory
    {  
        bool IsSelected { get; }
        string Caption { get; }
    }
}
