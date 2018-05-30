using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Exceptions;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public interface IFeatureBranchBuilderViewModel : IViewModel
    {
        bool IsActive { get; }
    }

    public class FeatureBranchBuilderViewModel : SidebarItemBase, IFeatureBranchBuilderViewModel
    {
        public FeatureBranchBuilderViewModel(IExplorerBar sourceComponents, IMainBranch mainBranch, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            _mainBranch = mainBranch;
            SourceComponents = sourceComponents;
            SubscribeToRootItemsPropertyChanged();
            UpdateSelectedComponents();

            CreateCommand = new Command(CreateFeatureBranch, () => CanCreate(), this);
            CancelCommand = new Command(Cancel);
        }

        private bool CanCreate()
        {
            return 
                !string.IsNullOrEmpty(this.FeatureName)
                && SourceComponents.Items.Any(item => item.IsChecked != false)
                && !IsBusy;
        }

        IMainBranch _mainBranch;

        public override void Activate()
        {
            SourceComponents.AllowItemsCheck = true;
            SourceComponents.CheckAll();
            base.Activate();

        }

        public override void Deactivate()
        {
            ApplicationServices.ExecuteOnUIThread(() => SourceComponents.AllowItemsCheck = false);
            base.Deactivate();
        }

        public ICommand CreateCommand { get; private set; }
        private void CreateFeatureBranch()
        {
            Validate();

            StartBusyAction(() =>
            {

                var selectedComponents = ExtractSelectedComponents(this.SelectedComponents.Items);

                var backgroundOperation = new StandardBackgroundOperation();
                ServiceLocator.GetInstance<IBackgroundOperationsRegion>().RegisterOperation(backgroundOperation);

                try
                {
                    _mainBranch.CreateFeatureBranch(this.FeatureName, selectedComponents,
                        (args) =>
                        {
                            backgroundOperation.Update(args.Percentage, args.ActionDescription);
                        });
                }
                catch (Exception ex)
                {
                    backgroundOperation.Failed($"Failed to create feature '{this.FeatureName}'", ex.ToString());
                    throw;
                }

                ServiceLocator.GetInstance<IBackgroundOperationsRegion>().UnregisterOperation(backgroundOperation);
                Deactivate();
            });
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(this.FeatureName))
                throw new ValidationException("Please fill in the feature name!");

            if(this.FeatureName.Any(Char.IsWhiteSpace))
                throw new ValidationException("Feature name should not contain white spaces!");
        }

        IApplicationServices ApplicationServices
        {
            get { return this.ServiceLocator.GetInstance<IApplicationServices>(); }
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
        

        public ICommand CancelCommand { get; private set; }

        private void Cancel()
        {
            Deactivate();
        }
        
        private void SubscribeToRootItemsPropertyChanged()
        {
            foreach (var item in SourceComponents.Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void UnsubscribeFromRootItemsPropertyChanged()
        {
            foreach (var item in SourceComponents.Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        private string _featureName;
        public string FeatureName
        {
            get
            {
                return _featureName;
            }

            set
            {
                SetProperty(ref _featureName, value?.Replace(" ", ""));
            }
        }

        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {
                UnsubscribeFromRootItemsPropertyChanged();
            }

            base.Dispose(disposing);
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IExplorerBarItem.IsChecked))
            {
                UpdateSelectedComponents();
                OnPropertyChanged(nameof(AllComponentsAreSelected));
            }
        }


        private void UpdateSelectedComponents()
        {
            var selectedComponents = SourceComponents.GetCheckedItemsAsExplorerBar();
            selectedComponents.ExpandAll();
            selectedComponents.AllowSearching = false;
            this.SelectedComponents = selectedComponents;
        }

      
        IExplorerBar SourceComponents { get; set; }

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

        public bool AllComponentsAreSelected
        {
            get
            {
                return SourceComponents.Items.All(item => item.IsChecked == true);
            }
        }
        
    }
}
