using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;
using GGPGameServer.ApprovalSystem.Common;
using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class GameTechnologyWorkspaceItem : GameRelatedWorkspaceItem
    {
        public GameTechnologyWorkspaceItem(Interfaces.Entities.Game game, GameInfrastructure infrastructure, Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator)
            : base(game, serviceLocator)
        {
            
            this.Infrastructure = infrastructure;
            
            LoadVersions();
            this.DownloadVersionCommand = new Command<GridItem>(DownloadVersion);
            this.QAApproveCommand = new Command<GridItem>(QAApprove);
            this.PMApproveCommand = new Command<GridItem>(PMApprove);
        }


        protected override void OnGamePropertyChanged(string propertyName)
        {
            base.OnGamePropertyChanged(propertyName);

            if (propertyName == this.Game.GetPropertyName(t => t.Versions))
            {
                Refresh();
            }

        }

        public ICommand QAApproveCommand { get; protected set; }
        public ICommand PMApproveCommand { get; protected set; }

        public ICommand DownloadVersionCommand { get; private set; }

        private void DownloadVersion(GridItem item)
        {
            this.ServiceLocator.GetInstance<IDialogServices>().ShowOkCancelDialogBox(new Dialogs.DownloadGameVersionDialog(this.Game, item.GameVersion, this.ServiceLocator));
        }

        
        private void QAApprove(GridItem gridItem)
        {

            var gamesRepository = ServiceLocator.GetInstance<IGamesRepository>();

            var dlg = CreateApprovalDialog(gridItem.GameVersion, gamesRepository.GetAvailableQAApprovalStates());
            dlg.AvailableStatesLabel = "QA State";

            dlg.CustomOkAction = () =>
            {
                ServiceLocator.GetInstance<IGamesRepository>().QAApprove(gridItem.GameVersion.Id, dlg.SelectedState, dlg.GetSelectedClientTypes());
            };

            if (ServiceLocator.GetInstance<IDialogServices>().ShowOkCancelDialogBox(dlg) == OkCancelDialogBoxResult.Ok)
            {
                this.Refresh();
            }

        }



        private void PMApprove(GridItem gridItem)
        {
            var gamesRepository = ServiceLocator.GetInstance<IGamesRepository>();

            var dlg = CreateApprovalDialog(gridItem.GameVersion, gamesRepository.GetAvailablePMApprovalStates());

            dlg.AvailableStatesLabel = "PM Approved";

            dlg.CustomOkAction = () =>
            {
                gamesRepository.PMApprove(gridItem.GameVersion.Id, dlg.SelectedState, dlg.GetSelectedClientTypes());
            };

            if (ServiceLocator.GetInstance<IDialogServices>().ShowOkCancelDialogBox(dlg) == OkCancelDialogBoxResult.Ok)
            {
                this.Refresh();
            }

        }


        private Dialogs.GameVersionApprovalDialog CreateApprovalDialog(GameVersion gameVersion, string[] availableStates)
        {
            var dlg = new Dialogs.GameVersionApprovalDialog(availableStates, GetClientTypesForApproval(gameVersion));
            dlg.Title = string.Format("{0} [{1}] - {2} version {3}", this.Game.Name, this.Game.MainGameType, gameVersion.Infrastructure, gameVersion.Version.ToString());
            return dlg;
        }


        private ClientType[] GetClientTypesForApproval(GameVersion gameVersion)
        {
            var clientTypes = gameVersion.PropertySets.Where(ps => ps.HasStateProperty)
                                                            .GroupBy(ps => ps.PropertySetName)
                                                            .Select(g => new ClientType(g.Key, g.Select(item => item.Regulation).ToArray()))
                                                            .OrderByDescending(ct => ct.Regulations.Length)
                                                            .ToArray();
            return clientTypes;
        }



        public SelectedVersionViewModel SelectedVersion
        {
            get
            {
                return new SelectedVersionViewModel((this.Versions.CurrentItem as GridItem).GameVersion);
            }
          
        }

        private void LoadVersions()
        {

            this.Versions = CreateVersionsCollectionView();
            this.Versions.CurrentChanged += Versions_CurrentChanged;
        }

        private void Refresh()
        {
            var currentGridItem = this.Versions.CurrentItem as GridItem;
            Guid? currentGameVersionID = null;
            if (currentGridItem != null)
            {
                currentGameVersionID = currentGridItem.GameVersion.Id;
            }

            
            this.LoadVersions();


            if (currentGameVersionID.HasValue)
            {
                foreach (GridItem gridItem in this.Versions)
                {
                    if (gridItem.GameVersion.Id == currentGameVersionID.Value)
                    {
                        this.Versions.MoveCurrentTo(gridItem);
                        break;
                    }
                }
            }
        }

        private System.Windows.Data.ListCollectionView CreateVersionsCollectionView()
        {
            var versions = this.Game.Versions.Where(v => v.Infrastructure == this.Infrastructure).OrderByDescending(v => v.Version).ToArray();

            var supportedRegulations = versions.SelectMany(v => v.PropertySets.Where(ps => ps.HasStateProperty)
                                                                              .Select(ps => ps.Regulation))
                                               .Distinct()
                                               .ToArray();


            var results = new GridItemCollection(supportedRegulations);

            foreach (var v in versions)
            {
                var versionGridItem = new VersionGridItem(v, this);
                results.Add(versionGridItem);
                foreach (var propertySetName in v.GetClientSpecificPropertySets().GroupBy(ps => ps.PropertySetName).OrderBy(item => item.Key))
                {

                    var clientTypeGridItem = new ClientTypeGridItem(versionGridItem, propertySetName.ToArray());
                    results.Add(clientTypeGridItem);

                    foreach (var propertyKey in propertySetName.SelectMany(ps => ps.Properties)
                                   .GroupBy(prop => prop.Key)
                                   .OrderBy(item => item.Key))
                    {
                        results.Add(new PropertyGridItem(clientTypeGridItem, propertyKey.ToArray()));
                    }


                }
            }

            var collectionView = new System.Windows.Data.ListCollectionView(results);
            return collectionView;
        }

        void Versions_CurrentChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(() => SelectedVersion);
        }

        public override string Title
        {
            get
            {
                return string.Format("{0} [{1}] - {2}", this.Game.Name, this.Game.MainGameType, this.Infrastructure);
            }
        }

        

        public GameInfrastructure Infrastructure
        {
            get;
            private set;
        }


        ICollectionView _versions;
        public ICollectionView Versions
        {
            get { return _versions; }
            set
            {   
                _versions = value;
                OnPropertyChanged(() => Versions);
            }
        }

        public class GridItemCollection : ObservableCollection<GridItem> , ITypedList
        {
            public GridItemCollection(RegulationType[] supportedRegulations)
            {
                var props = new List<PropertyDescriptor>();

                //props.Add(SparkReflector.GetPropertyDescriptor<GameVersionWorkspaceItem>(x => x.Version));
                //props.Add(SparkReflector.GetPropertyDescriptor<GridItem>(x => x.Name));

                foreach (var r in supportedRegulations.OrderBy(r => r))
                    props.Add(new RegulationPropertyDescriptor(r));

                props.Add(SparkReflector.GetPropertyDescriptor<GridItem>(x => x.CreatedDate));
                props.Add(SparkReflector.GetPropertyDescriptor<GridItem>(x => x.CreatedBy));
                props.Add(SparkReflector.GetPropertyDescriptor<GridItem>(x => x.TriggeredBy));

                this.CustomProperties = new PropertyDescriptorCollection(props.ToArray());

            }

            PropertyDescriptorCollection CustomProperties { get; set; }


            #region ITypedList Members

            public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
            {
                return CustomProperties;

             
            }

            public string GetListName(PropertyDescriptor[] listAccessors)
            {
                return string.Empty;
            }

            #endregion


            protected override void InsertItem(int index, GridItem item)
            {
                item.CustomProperties = this.CustomProperties;
                base.InsertItem(index, item);
            }
         
        }


        public class SelectedVersionViewModel : ViewModelBase
        {
            public SelectedVersionViewModel(GameVersion gameVersion)
            {
                this.GameVersion = gameVersion;
            }

            GameVersion GameVersion { get; set; }

            public VersionNumber Version
            {
                get { return GameVersion.Version; }
            }

            public GameVersionPropertyChangedHistory[] History
            {
                get
                {
                    return this.GameVersion.PropertiesChangeHistory.OrderByDescending(h => h.ChangeDate).ToArray();
                }
            }

        }
        

        public abstract class GridItem : ViewModelBase, ICustomTypeDescriptor
        {
            public GridItem(GameTechnologyWorkspaceItem workspaceItem, GameVersion gameVersion)
                : this((GridItem)null)
            {
                this.WorkspaceItem = workspaceItem;
                this.GameVersion = gameVersion;
            }


            public GridItem(GridItem parent)
            {
                this.Parent = parent;
                this.ExpandCommand = new Command(() => this.IsExpanded = !this.IsExpanded, () => this.CanExpand);
                
                if(this.Parent != null)
                    this.Parent.PropertyChanged += Parent_PropertyChanged;
            }


            GameTechnologyWorkspaceItem _workspaceItem;

            public GameTechnologyWorkspaceItem WorkspaceItem
            {
                get
                {
                    if (this.Parent != null)
                        return this.Parent.WorkspaceItem;

                    return _workspaceItem;
                }
                private set { _workspaceItem = value; }
            }

        

            

            GameVersion _gameVersion;
            public GameVersion GameVersion
            {
                get
                {
                    if (_gameVersion == null && this.Parent != null)
                        return this.Parent.GameVersion;

                    return _gameVersion;
                }

                private set
                {
                    _gameVersion = value;
                    OnPropertyChanged(() => GameVersion);
                }
            }
            public abstract string GetValueByRegulation(RegulationType regulation);
            protected abstract string GetDisplayName();
            public abstract bool CanExpand { get; }
            public abstract int Level { get; }


            void Parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                OnPropertyChanged(() => IsVisible);
            }

            private bool _isExpanded;

            public bool IsExpanded
            {
                get { return _isExpanded; }
                set
                {
                    _isExpanded = value;
                    OnPropertyChanged(() => IsExpanded);
                }
            }

            public Command ExpandCommand { get; private set; }

            public bool IsVisible
            {
                get
                {
                    if (this.Parent == null)
                        return true;

                    return this.Parent.IsVisible && this.Parent.IsExpanded;
                }
            }

            

            public string Name
            {
                get { return GetDisplayName(); }
            }


            public string CreatedDate
            {
                get
                {
                    return this.GameVersion.CreatedDate.ToString();
                }
            }

            public string CreatedBy
            {
                get
                {
                    return this.GameVersion.CreatedBy;
                }
            }

            public string TriggeredBy
            {
                get
                {
                    return this.GameVersion.TriggeredBy;
                }
            }
            
            
            GridItem Parent { get; set; }


            #region ICustomTypeDescriptor Members

            AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
                return new AttributeCollection();
            }

            string ICustomTypeDescriptor.GetClassName()
            {
                return TypeDescriptor.GetClassName(this, true);
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return TypeDescriptor.GetComponentName(this, true);
            }

            TypeConverter ICustomTypeDescriptor.GetConverter()
            {
                throw new NotImplementedException();
            }

            EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
                return null;
            }

            PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
                return null;
            }

            object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
                return null;
            }

            EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
                return new EventDescriptorCollection(new EventDescriptor[0]);
            }

            EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
                return new EventDescriptorCollection(new EventDescriptor[0]);
            }

            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
                return CustomProperties;
            }

            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
                return CustomProperties;
            }

            object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
            {
                return this;
            }

            #endregion


            public PropertyDescriptorCollection CustomProperties { get; set; }
        }


        public class VersionGridItem : GridItem
        {
            public VersionGridItem(GameVersion gameVersion, GameTechnologyWorkspaceItem workspaceItem)
                : base(workspaceItem, gameVersion)
            {
                
            }


            public override string GetValueByRegulation(RegulationType regulation)
            {
                var versionApprovalStates = GameVersion.PropertySets.Where(ps => ps.Regulation == regulation)
                    .Select(ps => ps.CurrentApprovalState == "NotTested" ? "InProgress" : ps.CurrentApprovalState)
                    .Where(state => !string.IsNullOrEmpty(state))
                    .Distinct()
                    .ToArray();


                if (versionApprovalStates.Length == 0)
                    return null;

                if (versionApprovalStates.Length == 1)
                    return versionApprovalStates[0];

                return "Partial"; 
                
            }


            public override int Level
            {
                get { return 0; }
            }

            protected override string GetDisplayName()
            {
                return this.GameVersion.Version.ToString();
            }

            public override bool CanExpand
            {
                get { return true; }
            }
        }

        public class ClientTypeGridItem : GridItem
        {

            public ClientTypeGridItem(VersionGridItem parent, GameVersionPropertySet[] propertySets)
                : base(parent)
            {
                this.PropertySets = propertySets;
            }

            GameVersionPropertySet[] PropertySets { get; set; }

            public override string GetValueByRegulation(RegulationType regulation)
            {
                if (this.PropertySets.IsNullOrEmpty())
                    return null;

                return this.PropertySets.Where(p => p.Regulation.Name == regulation.Name)
                                        .Select(p => p.CurrentApprovalState)
                                        .FirstOrDefault();

            }

            protected override string GetDisplayName()
            {
                return this.PropertySets.First().PropertySetName;
            }

            public override bool CanExpand
            {
                get { return true; }
            }

            public override int Level
            {
                get { return 1; }
            }
        }

        public class PropertyGridItem : GridItem
        {
            public PropertyGridItem(ClientTypeGridItem parent, GameVersionProperty[] properties)
                : base(parent)
            {
                this.Properties = properties;
            }

            GameVersionProperty[] Properties { get; set; }


            public override string GetValueByRegulation(RegulationType regulation)
            {
                return this.Properties.Where(p => p.Regulation == regulation).Select(p => p.Value).FirstOrDefault();
            }

            protected override string GetDisplayName()
            {
                return this.Properties.First().Name;
            }

            public override bool CanExpand
            {
                get { return false; }
            }

            public override int Level
            {
                get { return 2; }
            }
        }

        public class RegulationPropertyDescriptor : PropertyDescriptor
        {
            public RegulationPropertyDescriptor(RegulationType regulation)
                : base(regulation.Name, new Attribute[0])
            {
                this.Regulation = regulation;
            }

            public override bool IsBrowsable
            {
                get
                {
                    return true;
                }
            }

            public override string DisplayName
            {
                get
                {
                    return this.Regulation.Name;
                }
            }

            public override string Name
            {
                get
                {
                    return this.Regulation.Name;
                }
            }

            
            
            RegulationType Regulation { get; set; }
            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get { return typeof(GridItem); }
            }

            public override object GetValue(object component)
            {
                var game = component as GridItem;

                return game.GetValueByRegulation(this.Regulation);
                
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override Type PropertyType
            {
                get
                {
                    return typeof(string);
                }
            }

            public override void ResetValue(object component)
            {
                
            }

            public override void SetValue(object component, object value)
            {
                
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            
        }


       
    }
}
