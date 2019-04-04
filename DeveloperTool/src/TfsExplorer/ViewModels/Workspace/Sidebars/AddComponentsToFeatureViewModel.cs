using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class AddComponentsToFeatureViewModel : SidebarItemBase
    {
        public AddComponentsToFeatureViewModel(IFeatureBranch featureBranch, IServiceLocator serviceLocator) 
            : base(serviceLocator)
        {
            this.FeatureBranch = featureBranch;
            LoadAvailableComponents(featureBranch);

            this.OkCommand = new Command(AddMissingComponents, () => !this.IsBusy && (SelectedComponents?.Items.Count ?? 0) > 0, this);
            this.CancelCommand = new Command(() => this.Deactivate(), () => !this.IsBusy, this);

        }

        IFeatureBranch FeatureBranch { get; set; }

        private void LoadAvailableComponents(IFeatureBranch featureBranch)
        {
            AvailableComponents = new ComponentsExplorerBar(featureBranch.GetMissingComponentsFromMain(),
                                                            this.ServiceLocator)
            {
                AllowItemsCheck = true
            };
            SubscribeToRootItemsPropertyChanged();
        }

        public override void Deactivate()
        {
            UnsubscribeFromRootItemsPropertyChanged();
            base.Deactivate();
        }

        ComponentsExplorerBar _availableComponents;

        public ComponentsExplorerBar AvailableComponents
        {
            get { return _availableComponents; }
            private set
            {
                SetProperty(ref _availableComponents, value);
            }
        }

        private void SubscribeToRootItemsPropertyChanged()
        {
            foreach (var item in AvailableComponents.Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void UnsubscribeFromRootItemsPropertyChanged()
        {
            foreach (var item in AvailableComponents.Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IExplorerBarItem.IsChecked))
            {
                UpdateSelectedComponents();
            }
        }

        private void UpdateSelectedComponents()
        {
            var selectedComponents = AvailableComponents.GetCheckedItemsAsExplorerBar();
            selectedComponents.ExpandAll();
            this.SelectedComponents = selectedComponents;
        }


        IExplorerBar _selectedComponents;
        public IExplorerBar SelectedComponents
        {
            get
            {
                return _selectedComponents;
            }
            private set
            {
                _selectedComponents = value;
                OnPropertyChanged(nameof(SelectedComponents));
            }

        }
        
        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private void AddMissingComponents()
        {
            var selectedComponents = ExtractSelectedComponents(this.SelectedComponents.Items);

            StartBusyAction(() =>
            {
                var backgroundOperation = new StandardBackgroundOperation();
                ServiceLocator.GetInstance<IBackgroundOperationsRegion>().RegisterOperation(backgroundOperation);

                try
                {
                    FeatureBranch.AddMissingComponents(selectedComponents.ToArray(), (eventData) =>
                    {
                        backgroundOperation.Update(eventData.Percentage, eventData.ActionDescription);
                    });
                }
                catch(Exception ex)
                {
                    backgroundOperation.Failed($"Failed to add missing components to '{this.FeatureBranch.Name}' feature", ex.ToString());
                    throw;
                }

                ServiceLocator.GetInstance<IBackgroundOperationsRegion>().UnregisterOperation(backgroundOperation);
                this.Deactivate();
            });


        }



        private List<ILogicalComponent> ExtractSelectedComponents(ObservableCollection<IExplorerBarItem> items)
        {
            var components = new List<ILogicalComponent>();

            foreach (var item in items)
            {
                var componentHolder = item.As<ILogicalComponentHolder>();
                componentHolder.Do(ch => ch.GetComponent().Do(c => components.Add(c)));

                components.AddRange(ExtractSelectedComponents(item.Items));
            }

            return components;
        }
    }
}
