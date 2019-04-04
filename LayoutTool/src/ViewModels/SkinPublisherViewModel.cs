using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class SkinPublisherViewModel : ServicedViewModelBase
    {
        public SkinPublisherViewModel(SkinDefinitionBuilderSelectorViewModel skinSelector, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            SkinSelector = skinSelector;

            _publishCommand = new Command(Publish, () => SkinSelector.SelectedBuilder != null 
                                                        && SkinSelector.SelectedBuilder.IsValid);
            SkinSelector.SelectedBuilderPropertyChanged += SkinSelector_SelectedBuilderPropertyChanged;
        }

        private void SkinSelector_SelectedBuilderPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ServiceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(() => _publishCommand.RaiseCanExecuteChanged());
        }

        protected override bool GlobalNotificationsEnabled
        {
            get
            {
                return false;
            }
        }

        

        public SkinDefinitionBuilderSelectorViewModel SkinSelector
        {
            get;private set;
        }

        Command _publishCommand;

        public ICommand PublishCommand
        {
            get { return _publishCommand; }
        }

        private void Publish()
        {
            var skinDefinitionContext = ServiceLocator.GetInstance<ISkinDesigner>().BuildSkinDefinitionContext();

            if(skinDefinitionContext.Errors.Any(err => err.Severity == Interfaces.Entities.ErrorServerity.Error))
            {
                ServiceLocator.GetInstance<IMessageBox>().ShowMessage("This skin has Errors. You can't publish a skin containing errors!");
                return;
            }


            if (skinDefinitionContext.Errors.Any(err => err.Severity == Interfaces.Entities.ErrorServerity.Warning))
            {
                if(MessageBoxResponse.No == ServiceLocator.GetInstance<IMessageBox>().ShowYesNoMessage($"This skin has Warnings.{Environment.NewLine}{Environment.NewLine}Are you sure you want to publish it ?"))
                {
                    return;
                }
            }


            StartBusyAction(() =>
            {
                
                skinDefinitionContext.DestinationSkin = this.SkinSelector.SelectedBuilder.GetSkinIdentity();
                SkinSelector.SelectedBuilder.Publish(skinDefinitionContext);
                ServiceLocator.GetInstance<IMessageBox>().ShowMessage("Done!");
            });
        }
        
    }
}
