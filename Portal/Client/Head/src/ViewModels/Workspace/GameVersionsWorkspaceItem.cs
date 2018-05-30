using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;

using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Spark.Infra.Types;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class GameVersionsWorkspaceItem : GameRelatedWorkspaceItem
    {
        public GameVersionsWorkspaceItem(Interfaces.Entities.Game game, GameInfrastructure infrastructure, Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator)
            : base(game, serviceLocator)
        {
            
            this.Infrastructure = infrastructure;
            
            LoadVersions();
            this.DownloadVersionCommand = new Command<GridItem>(DownloadVersion);
            
        }


        protected override void OnGamePropertyChanged(string propertyName)
        {
            base.OnGamePropertyChanged(propertyName);

            if (propertyName == this.Game.GetPropertyName(t => t.Versions))
            {
                Refresh();
            }

        }
        
        public ICommand DownloadVersionCommand { get; private set; }

        private void DownloadVersion(GridItem item)
        {
            this.ServiceLocator.GetInstance<IDialogServices>().ShowOkCancelDialogBox(new Dialogs.DownloadGameVersionDialog(this.Game, item.GameVersion, this.ServiceLocator));
        }
        
        public SelectedVersionViewModel SelectedVersion
        {
            get
            {
                return new SelectedVersionViewModel(this.Game, 
                                                   (this.Versions.CurrentItem as GridItem).GameVersion, 
                                                   this.ServiceLocator);
            }
          
        }

        

        private void LoadVersions()
        {
            this.Versions = CreateVersionsCollectionView();
            LoadHistory();
            this.Versions.CurrentChanged += Versions_CurrentChanged;
        }

        private void LoadHistory()
        {
            this.HistoryItems = GetGameVersions().SelectMany(version => version.PropertiesChangeHistory.Select(histItem => new { version.Version, histItem }))
                                        .OrderByDescending(item => item.histItem.ChangeDate)
                                        .Select(item => new PropertyChangeHistoryItem(item.Version, item.histItem))
                                        .ToArray();
        }

        PropertyChangeHistoryItem[] _historyItems = new PropertyChangeHistoryItem[0];

        public PropertyChangeHistoryItem[] HistoryItems
        {
            get { return _historyItems; }
            set
            {
                SetProperty(ref _historyItems, value);
            }
        }

        private void Refresh()
        {

            Guid? currentGameVersionID = (this.Versions.CurrentItem as GridItem)?.GameVersion.Id;
            
            this.LoadVersions();

            MoveToVersion(currentGameVersionID);
        }

        private void MoveToVersion(Guid? gameVersionID)
        {
            if (gameVersionID.HasValue)
            {
                var gridItem = FindGridItemById(gameVersionID.Value);
                gridItem.Do(item => this.Versions.MoveCurrentTo(item));
                OnPropertyChanged(nameof(SelectedVersion));
            }
            
        }

        private Optional<GridItem> FindGridItemById(Guid id)
        {
            foreach (GridItem gridItem in this.Versions)
            {
                if (gridItem.GameVersion.Id == id)
                {
                    return Optional<GridItem>.Some(gridItem);
                }
            }
            return Optional<GridItem>.None();
        }

        private IEnumerable<GameVersion> GetGameVersions()
        {
            return this.Game.Versions.Where(v => v.Infrastructure == this.Infrastructure);
        }

        private System.Windows.Data.ListCollectionView CreateVersionsCollectionView()
        {
            var versions = GetGameVersions().OrderByDescending(v => v.Version).ToArray();

            var supportedRegulations = versions.SelectMany(v => v.GetSupportedRegulations())
                                               .Distinct()
                                               .ToArray();

            var results = new GridItemCollection(supportedRegulations);

            foreach (var version in versions)
            {               
                results.Add(new VersionGridItem(version, this));
            }

            return new System.Windows.Data.ListCollectionView(results);
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


        public class SelectedVersionViewModel : ServicedViewModelBase
        {
            public SelectedVersionViewModel(Game game, GameVersion gameVersion, IServiceLocator serviceLocator)
                : base(serviceLocator)
            {
                this.GameVersion = gameVersion;
                this.Languages = new GameVersionLanguagesViewModel(game, gameVersion, serviceLocator);
                this.Regulations = new GameVersionRegulationsViewModel(game, gameVersion, serviceLocator);
            }

            GameVersion GameVersion { get; set; }

            public VersionNumber Version
            {
                get { return GameVersion.Version; }
            }

          
            public GameVersionLanguagesViewModel Languages { get; }
            public GameVersionRegulationsViewModel Regulations { get; }

        }
        

        public class PropertyChangeHistoryItem
        {
            public PropertyChangeHistoryItem(VersionNumber gameVersion, GameVersionPropertyChangedHistory propertyHistory)
            {
                this.GameVersion = gameVersion;
                _propertyHistory = propertyHistory;
            }

            GameVersionPropertyChangedHistory _propertyHistory;

            public VersionNumber GameVersion { get; }

            public string PropertyKey { get { return _propertyHistory.PropertyKey; } }

            public string OldValue { get { return _propertyHistory.OldValue; } }

            public string NewValue { get { return _propertyHistory.NewValue; } }

            public string Regulation { get { return _propertyHistory.Regulation; } }

            public DateTime ChangeDate { get { return _propertyHistory.ChangeDate; } }

            public string ChangedBy { get { return _propertyHistory.ChangedBy; } }

            public Interfaces.ChangeType ChangeType { get { return _propertyHistory.ChangeType; } }
        }


        public abstract class GridItem : ViewModelBase, ICustomTypeDescriptor
        {
            protected const string NA = ""; // N/A

            public GridItem(GameVersionsWorkspaceItem workspaceItem, GameVersion gameVersion)
                : this((GridItem)null)
            {
                this.WorkspaceItem = workspaceItem;
                this.GameVersion = gameVersion;
            }


            public GridItem(GridItem parent)
            {
                this.Parent = parent;
            }


            GameVersionsWorkspaceItem _workspaceItem;

            public GameVersionsWorkspaceItem WorkspaceItem
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
            public VersionGridItem(GameVersion gameVersion, GameVersionsWorkspaceItem workspaceItem)
                : base(workspaceItem, gameVersion)
            {
                
            }


            public override string GetValueByRegulation(RegulationType regulation)
            {
                if (this.GameVersion.Regulations.IsNullOrEmpty())
                    return NA;

                
                var gameVersionRegulation = this.GameVersion.Regulations.FirstOrDefault(x => x.RegulationType == regulation);

                if (gameVersionRegulation == null)
                    return NA;


                return gameVersionRegulation.ApprovalStatusDescription;
            }
            
            protected override string GetDisplayName()
            {
                return this.GameVersion.Version.ToString();
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
