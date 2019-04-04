using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Prism.Logging;
using Spark.Infra.Logging;

namespace LayoutTool.ViewModels
{
    public class LocalIISSkinDefinitionBuilderViewModel : OnSiteSkinDefinitionBuilderViewModel, ISkinDefinitionBuilderViewModel
    {
        public LocalIISSkinDefinitionBuilderViewModel(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
           
            _webServerManager = serviceLocator.TryResolve<IWebServerManager>();

            if (_webServerManager != null)
            {
                try
                {
                    this.WebSites = _webServerManager.GetWebSites();
                }
                catch(Exception ex)
                {
                    //If fails I'm ignoring this builder. Most of the users can live without it. 
                    //But still report the exception.
                    var logger = serviceLocator.GetInstance<ILoggerFactory>().CreateLogger(this.GetType());
                    logger.Exception($"{nameof(LocalIISSkinDefinitionBuilderViewModel)} failed!", ex);
                    this.IsVisible = false;
                }
            }
            else
            {
                this.IsVisible = false;
            }
        }

        

        public override bool CanPublish
        {
            get
            {
                return false;
            }
        }

        public override void Publish(SkinDefinitionContext skinDefinitionContext)
        {
            throw new NotSupportedException();
        }
               

        IWebServerManager _webServerManager;
        

        private IWebSite[] _webSites = null;

        public IWebSite[] WebSites
        {
            get
            {
                return _webSites;
            }
            set
            {
                if (SetProperty(ref _webSites, value))
                {
                    if (_webSites != null && _webSites.Length == 1)
                    {
                        this.SelectedWebSite = _webSites[0];
                    }
                }
            }
        }


        private IWebSite _selectedWebSite;

        public IWebSite SelectedWebSite
        {
            get { return _selectedWebSite; }
            set
            {
                if (SetProperty(ref _selectedWebSite, value))
                {
                    RefreshVirtualDirectories();
                }


            }
        }

        private void RefreshVirtualDirectories()
        {
            this.SelectedVirtualDirectory = null;

            if (SelectedWebSite == null)
            {
                this.VirtualDirectories = new IVirtualDirectory[0];
            }
            else
            {
                this.VirtualDirectories = SelectedWebSite.GetApps()
                                                         .SelectMany(webApp => webApp.GetVirtualDirectories())
                                                         .Where(virtualDir => IsNdlVirtualDir(virtualDir))
                                                         .OrderBy(vd => vd.Name).ToArray();
                if (this.VirtualDirectories.Length == 1)
                    this.SelectedVirtualDirectory = this.VirtualDirectories[0];
                else
                    this.SelectedVirtualDirectory = null;
            }
        }

        private bool IsNdlVirtualDir(IVirtualDirectory virtualDir)
        {
            return null != TryDetectNavigationPlanRelativePath(virtualDir.PhysicalPath);
        }

        private IVirtualDirectory[] _virtualDirectories;
        public IVirtualDirectory[] VirtualDirectories
        {
            get { return _virtualDirectories; }
            set
            {
                SetProperty(ref _virtualDirectories, value);
            }
        }

        IVirtualDirectory _selectedVirtualDirectory;

        public IVirtualDirectory SelectedVirtualDirectory
        {
            get { return _selectedVirtualDirectory; }
            set
            {
                if (SetProperty(ref _selectedVirtualDirectory, value))
                {
                    ReloadSkinCodes();
                }
            }
        }

        private void ReloadSkinCodes()
        {
            this.SelectedSkinCode = null;

            var rootPath = this.SelectedVirtualDirectory.PhysicalPath;

            var navigationPlanFolder = TryDetectNavigationPlanRelativePath(rootPath);
            var skinCodes = new List<SkinCode>();

            if(navigationPlanFolder == null)
            {
                this.SkinCodes = skinCodes.ToArray();
                return;
            }

            navigationPlanFolder = new PathDescriptor(rootPath) + navigationPlanFolder;

            foreach (var skinCodeFolder in Directory.EnumerateDirectories(navigationPlanFolder.ToFileSystemFormat()))
            {
                int skinCode;
                if (!int.TryParse(new DirectoryInfo(skinCodeFolder).Name, out skinCode))
                    continue;

                skinCodes.Add(new SkinCode(skinCode));

            }

            this.SkinCodes = skinCodes.OrderBy(sc => sc.BrandId).ThenBy(sc => sc.SkinId).ToArray();


        }

        private PathDescriptor TryDetectNavigationPlanRelativePath(string rootPath)
        {

            var applicationParentFolder = TryGetApplicationParentFolder(rootPath);

            if (applicationParentFolder == null)
                return null;

            return applicationParentFolder + new PathDescriptor("versionX/navigation/plan");
        }
                

        private PathDescriptor TryGetApplicationParentFolder(string rootPath)
        {
         
            if (Directory.Exists(Path.Combine(rootPath, "application")))
                return new PathDescriptor("");

            if (Directory.Exists(Path.Combine(rootPath, "build", "application")))
                return new PathDescriptor("build");

            return null;

        }



        private SkinCode[] _skinCodes;
        public SkinCode[] SkinCodes
        {
            get { return _skinCodes; }
            set
            {
                SetProperty(ref _skinCodes, value);
            }
        }


        private SkinCode _selectedSkinCode;
        public SkinCode SelectedSkinCode
        {
            get { return _selectedSkinCode; }
            set
            {
                if(SetProperty(ref _selectedSkinCode, value))
                {
                    this.SelectedBrand = GetSelectedBrand();
                    this.SelectedSkin = GetSelectedSkin();
                }
            }
        }


        public override Guid Id
        {
            get
            {
                return WellKnownSkinSourcesIds.LocalIIS;
            }
        }

        public override int Order
        {
            get
            {
                return 225;
            }
        }

        public override string SourceName
        {
            get
            {
                return "Local IIS";
            }
        }


        protected override IEnumerable<SkinSelectorIdentity.SkinSelectorProperty> SaveState()
        {
            var properties = new List<SkinSelectorIdentity.SkinSelectorProperty>();
            
            properties.Add(new SkinSelectorIdentity.SkinSelectorProperty("SelectedWebSite", SelectedWebSite.Name));
            properties.Add(new SkinSelectorIdentity.SkinSelectorProperty("SelectedVirtualDirectory", SelectedVirtualDirectory.Name));
            properties.Add(new SkinSelectorIdentity.SkinSelectorProperty("MachineName", Environment.MachineName));
            properties.Add(new SkinSelectorIdentity.SkinSelectorProperty("UserName", Environment.UserName));

            return properties;
        }


        protected override void RestoreStateCore(SkinIndentity skinIdentity)
        {
            var property = skinIdentity.Selector.Properties.FirstOrDefault(p => p.Name == "SelectedWebSite");
            if (property != null)
            {
                this.SelectedWebSite = this.WebSites.FirstOrDefault(w => w.Name == property.Value);
            }

            property = skinIdentity.Selector.Properties.FirstOrDefault(p => p.Name == "SelectedVirtualDirectory");
            if (property != null)
            {
                this.SelectedVirtualDirectory = this.VirtualDirectories.FirstOrDefault(d => d.Name == property.Value);
            }

            var skinCode = new SkinCode(skinIdentity.BrandId, skinIdentity.SkinId);

            this.SelectedSkinCode = this.SkinCodes.FirstOrDefault(sc => sc.Code == skinCode.Code);


        }

        private BrandEntity GetSelectedBrand()
        {
            if (SelectedSkinCode == null)
                return null;

            var cdnUrl = SelectedVirtualDirectory.HttpAddress + TryGetApplicationParentFolder(SelectedVirtualDirectory.PhysicalPath);
            
            return new BrandEntity(SelectedSkinCode.BrandId, SelectedSkinCode.BrandId.ToString(), cdnUrl);   
        }

        private SkinEntity GetSelectedSkin()
        {
            if (SelectedSkinCode == null)
                return null;

            return new SkinEntity(SelectedSkinCode.SkinId, SelectedSkinCode.SkinId.ToString());
        } 
    }
}
