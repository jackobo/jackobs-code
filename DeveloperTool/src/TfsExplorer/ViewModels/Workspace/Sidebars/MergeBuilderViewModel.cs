using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    
    public interface IMergeBuilderViewModel : IViewModel
    {
        bool IsActive { get; }
    }



    public class MergeBuilderViewModel : SidebarItemBase, IMergeBuilderViewModel
    {
        public MergeBuilderViewModel(string title, Func<IEnumerable<IMergeSet>> getMergeSets, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.Title = title;
            this.RelatedWorkItems = new RelatedWorkItemCollection(
                () => this.MergeSets.SelectMany(g => g.ChangeSets.Where(cs => cs.IsSelected)
                                                                             .Select(cs => cs.ChangeSet))
                                                                             .ToList(),
                serviceLocator);

            this.OkCommand = new Command(StartMerge, () => this.HasSelectedChangeSets && !this.IsBusy, this);
            this.CancelCommand = new Command(Deactivate, () => !this.IsBusy, this);
            this.SelectAllCommand = new Command(SelectAll, () => !this.IsBusy, this);
            this.UnselectAllCommand = new Command(UnselectAll, () => !this.IsBusy, this);

            StartBusyAction(() => LoadMergeSets(getMergeSets), 
                            "Loading merge candidates...",
                            () => { ReloadGroups(); ReloadRelatedWorkItems(); });
        }


        private void LoadMergeSets(Func<IEnumerable<IMergeSet>> getMergeSets)
        {
            var mergeSets = new List<MergeSetViewModel>(getMergeSets().Select(s => new MergeSetViewModel(s)));

            ExecuteOnUIThread(() =>
            {
                this.MergeSets = mergeSets;
                if (mergeSets.Count == 0)
                {
                    this.ServiceLocator.GetInstance<IMessageBox>().ShowMessage("There is nothing to merge!");
                    this.CancelCommand.Execute(null);
                }
                else
                {
                    foreach (var ms in MergeSets)
                    {
                        if (ms.ChangeSets.Any())
                        {
                            foreach (var cs in ms.ChangeSets)
                                cs.PropertyChanged += ChangeSet_PropertyChanged;
                        }
                        else
                        {
                            ms.PropertyChanged += MergeSet_PropertyChanged;
                        }
                    }

                }
            });
            
           

        }

        private void MergeSet_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MergeSetViewModel.IsSelected))
            {
                OnPropertyChanged(nameof(HasSelectedChangeSets));
            }
        }

        private void ChangeSet_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ReloadRelatedWorkItems();
        }

        public string Title { get; private set; }

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public ICommand SelectAllCommand { get; private set; }
        public ICommand UnselectAllCommand { get; private set; }

        private void SelectAll()
        {
            ChangeSelectionStatusForAll(true);
        }

        private void UnselectAll()
        {
            ChangeSelectionStatusForAll(false);
        }
        
        private void ChangeSelectionStatusForAll(bool isSelected)
        {
            foreach (var g in this.Groups)
            {
                g.IsSelected = isSelected;
            }

            foreach (var newComp in this.NewComponentsMergeSets)
            {
                newComp.IsSelected = isSelected;
            }
        }

        private void StartMerge()
        {
            StartBusyAction(() =>
            {
                var mergeResult = MergeResult.Empty();
                foreach (var ms in this.MergeSets)
                {
                    mergeResult = mergeResult.Combine(ms.Merge());
                }

                WriteRelatedWorkItemsForVsExtension();

                ExecuteOnUIThread(Deactivate);

                if(mergeResult.NumberOfConflicts > 0)
                {
                    this.ServiceLocator.GetInstance<IMessageBox>().ShowMessage($"There are some merge conflicts!{Environment.NewLine}{Environment.NewLine}Open Visual Studio and go to{Environment.NewLine}'Team Explorer => Pending Changes => Resolve Conflicts'");
                }

                

            },
            "Merging...");
        }

        private void WriteRelatedWorkItemsForVsExtension()
        {
            var settings = new LatestMergeWorkItemsSettings();
            foreach(var workItem in this.RelatedWorkItems.WorkItems)
            {
                settings.WorkItems.Add(new WorkItemDescriptor(workItem.Id, workItem.Title));
            }

            var stringBuilder = new StringBuilder();
            var serializer = new XmlSerializer(typeof(LatestMergeWorkItemsSettings));
            using (var writer = new StringWriter(stringBuilder))
            {
                serializer.Serialize(writer, settings);
            }

            if (!Directory.Exists(SparkVsExtensionsAppData))
                Directory.CreateDirectory(SparkVsExtensionsAppData);

            File.WriteAllText(LatestMergeWorkItemsStorageFile, stringBuilder.ToString());
        }

        private string LatestMergeWorkItemsStorageFile
        {
            get { return Path.Combine(SparkVsExtensionsAppData, "LatestMergeWorkItems.xml"); }
        }

        private string SparkVsExtensionsAppData
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Spark Visual Studio Extensions");
            }
        }

        public RelatedWorkItemCollection RelatedWorkItems { get; private set; } 


        private void ReloadRelatedWorkItems()
        {
            RelatedWorkItems.Refresh();
        }
        
        public class RelatedWorkItemCollection : ServicedViewModelBase
        {
            public RelatedWorkItemCollection(Func<IEnumerable<IChangeSet>> getChangeSets, IServiceLocator serviceLocator) 
                : base(serviceLocator)
            {
                this.BusyActionDescription = "Loading related work items...";
                _getChangeSets = getChangeSets;
                _autoresetEvent = this.ServiceLocator.GetInstance<IThreadingServices>().CreateAutoResetEvent(false);
            }

            public override void Activate()
            {
                base.Activate();
                this.ServiceLocator.GetInstance<IThreadingServices>().StartNewTask(RefreshAsync);
            }

            public override void Deactivate()
            {
                base.Deactivate();
                StopRefresh();
            }

            Func<IEnumerable<IChangeSet>> _getChangeSets;
            
            public void Refresh()
            {
                _autoresetEvent.Set();               
            }

            bool _stopRefresh = false;
            IAutoresetEvent _autoresetEvent;
            private void RefreshAsync()
            {
                
                while (!_stopRefresh)
                {
                    _autoresetEvent.WaitOne();

                    if (_stopRefresh)
                        return;
                    
                    var appServices = ServiceLocator.GetInstance<IApplicationServices>();

                    //appServices.ExecuteOnUIThread(() => this.IsBusy = true);

                    var workItems = new RelatedWorkItemViewModel[0];
                    try
                    {
                        workItems = this.ServiceLocator.GetInstance<IComponentsRepository>().GetRelatedWorkItems(_getChangeSets())
                                       .Select(wi => new RelatedWorkItemViewModel(wi))
                                       .ToArray();
                    }
                    finally
                    {
                        appServices.ExecuteOnUIThread(() =>
                        {
                            this.WorkItems = workItems;
                        });
                    }
                }
            }

            RelatedWorkItemViewModel[] _workItems = new RelatedWorkItemViewModel[0];

            public RelatedWorkItemViewModel[] WorkItems
            {
                get
                {
                    return _workItems;
                }
                private set
                {
                    SetProperty(ref _workItems, value);
                }
            }

            protected override void Dispose(bool disposing)
            {
                if(disposing)
                {
                    StopRefresh();
                }
                base.Dispose(disposing);
            }

            private void StopRefresh()
            {
                _stopRefresh = true;
                _autoresetEvent.Set();
            }
        }


        public override void Activate()
        {
            base.Activate();

            this.RelatedWorkItems.Activate();
        }


        public override void Deactivate()
        {
            base.Deactivate();
            this.RelatedWorkItems.Deactivate();
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                this.RelatedWorkItems.Dispose();
            }

            base.Dispose(disposing);
        }

        private void ReloadGroups()
        {
            this.ServiceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(() =>
            {
                DisposeOldGroups();

                if (ViewByComponent)
                {
                    LoadViewByComponent();
                }
                else if (ViewByCommiter)
                {
                    LoadViewByCommiter();
                }
                else if (ViewByChangeset)
                {
                    LoadViewByChangeset();
                }

                SubscribeToGroupsPropertyChanged();
            });
        }

        private void LoadViewByChangeset()
        {
            if (this.ExistingComponentsMergeSets.Length == 0)
                this.Groups = new ObservableCollection<MergeableChangeSetsGroup>();
            else
                this.Groups = new ObservableCollection<MergeableChangeSetsGroup>()
                            {
                                 new MergeableChangeSetsGroup("All Changesets", 
                                                              this.ExistingComponentsMergeSets.SelectMany(ms => ms.ChangeSets)
                                                                                              .OrderByDescending(cs => cs.Id)
                                                                                              .ToArray())
                            };
        }

        private void LoadViewByCommiter()
        {
            this.Groups = new ObservableCollection<MergeableChangeSetsGroup>
                (
                    this.ExistingComponentsMergeSets.SelectMany(ms => ms.ChangeSets)
                                    .GroupBy(cs => cs.Commiter)
                                    .Select(g => new MergeableChangeSetsGroup(g.Key, g.ToArray()))


                );
        }

        private void LoadViewByComponent()
        {
            this.Groups = new ObservableCollection<MergeableChangeSetsGroup>
                (
                    this.ExistingComponentsMergeSets.OrderBy(ms => ms.ComponentName).Select(ms => new MergeableChangeSetsGroup(ms.ComponentName, ms.ChangeSets.ToArray()))
                );
        }

        private void SubscribeToGroupsPropertyChanged()
        {
            foreach(var g in Groups)
                g.PropertyChanged += Group_PropertyChanged;
        }

        private void UnsubscribeFromGroupsPropertyChanged()
        {
            foreach (var g in Groups)
                g.PropertyChanged -= Group_PropertyChanged;
        }

        private void Group_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MergeableChangeSetsGroup.IsSelected))
            {
                OnPropertyChanged(nameof(HasSelectedChangeSets));
            }
        }

        private void DisposeOldGroups()
        {
            UnsubscribeFromGroupsPropertyChanged();

            foreach (var group in this.Groups)
                group.Dispose();
        }

        private List<MergeSetViewModel> _mergeSets = new List<MergeSetViewModel>();

        public List<MergeSetViewModel> MergeSets
        {
            get { return _mergeSets; }
            set
            {
                if(SetProperty(ref _mergeSets, value))
                {
                    OnPropertyChanged(nameof(NewComponentsMergeSets));
                }
            }
        }

        

        private MergeSetViewModel[] ExistingComponentsMergeSets
        {
            get
            {
                return MergeSets.Where(ms => !ms.IsNewComponent).ToArray();
            }
        }

       
        public MergeSetViewModel[] NewComponentsMergeSets
        {
            get
            {
                return MergeSets.Where(ms => ms.IsNewComponent).ToArray();
            }
        }


        private bool _viewByComponent = true;
        public bool ViewByComponent
        {
            get
            {
                return _viewByComponent;
            }

            set
            {
                if (SetProperty(ref _viewByComponent, value))
                    ReloadGroups();
            }
        }

        private bool _viewByCommiter;
        public bool ViewByCommiter
        {
            get
            {
                return _viewByCommiter;
            }

            set
            {

                if (SetProperty(ref _viewByCommiter, value))
                    ReloadGroups();
            }
        }


        private bool _viewByChangeset;
        public bool ViewByChangeset
        {
            get
            {
                return _viewByChangeset;
            }
            set
            {
                if (SetProperty(ref _viewByChangeset, value))
                    ReloadGroups();
            }
        }

        private ObservableCollection<MergeableChangeSetsGroup> _groups = new ObservableCollection<MergeableChangeSetsGroup>();
        public ObservableCollection<MergeableChangeSetsGroup> Groups
        {
            get
            {
                return _groups;
            }

            private set
            {
                SetProperty(ref _groups, value);
            }
        }

        public bool HasSelectedChangeSets
        {
            get
            {
                return this.MergeSets.Any(g => g.IsSelected == true || g.ChangeSets.Any(cs => cs.IsSelected));
            }
        }

       
    }

    public class MergeableChangeSetsGroup : ViewModelBase
    {
        public MergeableChangeSetsGroup(string title, MergeableChangeSetViewModel[] changesets)
        {
            this.Title = title;
            this.ChangeSets = changesets;
            foreach(var cs in this.ChangeSets)
                cs.PropertyChanged += ChangeSet_PropertyChanged;

        }

        private void ChangeSet_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MergeableChangeSetViewModel.IsSelected))
                OnPropertyChanged(nameof(IsSelected));
        }

        public string Title { get; private set; }

        public MergeableChangeSetViewModel[] ChangeSets { get; private set; }

        
        public bool? IsSelected
        {
            get
            {

                if (this.ChangeSets.All(cs => cs.IsSelected))
                    return true;

                if (this.ChangeSets.All(cs => !cs.IsSelected))
                    return false;

                return null;
            }
            set
            {
                foreach(var cs in this.ChangeSets)
                {
                    cs.IsSelected = value ?? false;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                foreach (var cs in this.ChangeSets)
                    cs.PropertyChanged -= ChangeSet_PropertyChanged;
            }
            base.Dispose(disposing);
        }
    }

    public class MergeSetViewModel : ViewModelBase, ILogicalComponentVisitor
    {
        public MergeSetViewModel(IMergeSet mergeSet)
        {
            MergeSet = mergeSet;
            this.IsSelected = mergeSet.IsNew;
            this.MergeSet.SourceComponent.AcceptCommandVisitor(() => this);
            LoadChangesets();
        }

        private bool? _isSelected;

        public bool? IsSelected
        {
            get
            {
                if(this.MergeSet.IsNew)
                    return _isSelected;

                return this.ChangeSets.All(cs => cs.IsSelected);
            }
            set { SetProperty(ref _isSelected, value); }
        }


        private void LoadChangesets()
        {
            var changeSets = new List<MergeableChangeSetViewModel>();
            foreach(var cs in MergeSet.ChangeSets)
            {
                changeSets.Add(new MergeableChangeSetViewModel(cs, this));
            }

            this.ChangeSets = changeSets;
        }

        IMergeSet MergeSet { get; set; }

        public bool IsNewComponent
        {
            get { return this.MergeSet.IsNew; }
        }

        public MergeResult Merge()
        {
            var mergeResult = MergeResult.Empty();

            if (this.MergeSet.IsNew)
            {
                mergeResult = mergeResult.Combine(this.MergeSet.Merge());
            }
            else
            {
                var selectedChangeSets = this.ChangeSets.Where(cs => cs.IsSelected)
                                                        .OrderBy(cs => cs.Id)
                                                        .ToArray();
                if (this.ChangeSets.Count() == selectedChangeSets.Length)
                    mergeResult = mergeResult.Combine(this.MergeSet.Merge());
                else
                {
                    foreach(var changeSet in selectedChangeSets)
                    {
                        mergeResult = mergeResult.Combine(changeSet.Merge());
                        
                    }
                }
            }

            return mergeResult;
        }

        

        void ILogicalComponentVisitor.Visit(ICoreComponent coreComponent)
        {
            this.ComponentName = coreComponent.Name;
        }

        void ILogicalComponentVisitor.Visit(IGameEngineComponent gameEngine)
        {
            this.ComponentName = gameEngine.Name + " Engine";
        }

        void ILogicalComponentVisitor.Visit(IGameComponent game)
        {
            this.ComponentName = game.Name + " Game";
        }

        void ILogicalComponentVisitor.Visit(IServerPath location)
        {

        }

        public string ComponentName { get; private set; }


        public IEnumerable<MergeableChangeSetViewModel> ChangeSets { get; private set; }

        

    }

    public class MergeableChangeSetViewModel : ViewModelBase
    {
        public MergeableChangeSetViewModel(IMergeableChangeSet changeSet, MergeSetViewModel ownerMergeSet)
        {
            this.ChangeSet = changeSet;
            _ownerMergeSet = ownerMergeSet;
            this.ViewChangeSetCommand = new Command(ViewChangeSet);
        }
        
        MergeSetViewModel _ownerMergeSet;
        public IMergeableChangeSet ChangeSet { get; private set; }

        private bool _isSelected = true;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref _isSelected, value);
            }
        }

        public ICommand ViewChangeSetCommand { get; private set; }
        
        private void ViewChangeSet()
        {
            Process.Start($"http://tfs2012:8080/tfs/DefaultCollection_2010/GamingNDL/GGP%20Server/_versionControl/changeset/{this.ChangeSet.Id}");
        }

        public long Id
        {
            get { return this.ChangeSet.Id; }
        }

        public DateTime Date
        {
            get { return this.ChangeSet.Date; }
        }

        public string Commiter
        {
            get { return this.ChangeSet.CommiterDisplayName; }
        }


        public string Comments
        {
            get { return this.ChangeSet.Comments; }
        }


        public string ComponentName
        {
            get
            {
                return _ownerMergeSet.ComponentName;
            }
        }

        public MergeResult Merge()
        {
            return this.ChangeSet.Merge();
        }

        
    }

    public class RelatedWorkItemViewModel : ViewModelBase
    {
        public RelatedWorkItemViewModel(ISourceControlWorkItem workItem)
        {
            this.WorkItem = workItem;
        }

        ISourceControlWorkItem WorkItem { get; set; }

        public int Id
        {
            get { return WorkItem.Id; }
        }

        public string Title
        {
            get { return WorkItem.Title; }
        }

    }
}
