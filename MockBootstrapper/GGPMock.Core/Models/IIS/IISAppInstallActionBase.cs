using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;

namespace GGPMockBootstrapper.Models
{
    public abstract class IISAppInstallActionBase<TOwnerProduct> : ProductInstallAction<TOwnerProduct>    
        where TOwnerProduct : Product
    {
        public IISAppInstallActionBase(TOwnerProduct ownerProduct, string applicationName, ManagedPipelineMode appPoolManagedPipelineMode, string dotNetFrameworkVersion = "v4.0")
            : base(ownerProduct)
        {
            this.ApplicationName = applicationName;
            this.AppPoolManagedPipelineMode = appPoolManagedPipelineMode;
            this.DotNetFrameworkVersion = dotNetFrameworkVersion;
        }

        

        protected string ApplicationName { get; set; }

        private string ApplicationPath
        {
            get { return "/" + this.ApplicationName; }
        }

        protected ManagedPipelineMode AppPoolManagedPipelineMode { get; set; }
        string DotNetFrameworkVersion { get; set; }

        

     

        private Application CreateApplication(ServerManager iisManager)
        {

            var defaultWebSite = iisManager.Sites.First();
            var homeDirectory = defaultWebSite.GetWebSiteHomeDirectory();
            var vdirPath = Path.Combine(homeDirectory, this.ApplicationName);
            var app = defaultWebSite.Applications.Add(ApplicationPath, vdirPath);
            app.ApplicationPoolName = this.ApplicationName;

            return app;
        }

        private void CreateApplicationPool()
        {
            using (var iisManager = new ServerManager())
            {
                var appPool = FindApplicationPool(iisManager, this.ApplicationName);

                if (appPool != null)
                    return;

                appPool = iisManager.ApplicationPools.Add(this.ApplicationName);
                appPool.ManagedRuntimeVersion = this.DotNetFrameworkVersion;
                appPool.ManagedPipelineMode = this.AppPoolManagedPipelineMode;
                iisManager.CommitChanges();
            }
            

        }

        private ApplicationPool FindApplicationPool(ServerManager iisManager, string poolName)
        {
            return iisManager.ApplicationPools.FirstOrDefault(p => p.Name == poolName);
        }


        #region IInstallAction Members

        public override string Description
        {
            get 
            {
                return "Install " + this.ApplicationName;
            }
        }

        public string GetApplicationPhysicalPath(ServerManager iisManager)
        {
            var app = FindApplication(iisManager);
            if (app == null)
                return null;
            return app.VirtualDirectories["/"].PhysicalPath;
        }
        

        protected override void Install(IInstalationContext context, string zipFileName)
        {
            this.SubActionsCount = 4;
            IncrementSubactionIndex("Creating application pool");
            CreateApplicationPool();

            using (ServerManager iisManager = new ServerManager())
            {
                IncrementSubactionIndex("Creating application");
                var application = FindApplication(iisManager);

                if (application == null)
                {
                    application = CreateApplication(iisManager);
                }

                IncrementSubactionIndex("Extracting files");
                context.EnvironmentServices.UnzipFile(zipFileName, application.VirtualDirectories.First().PhysicalPath);
                

                iisManager.CommitChanges();

                IncrementSubactionIndex("Configure application");
                ConfigureApplication(context);

            }
        }

       


        private void ConfigureApplication(IInstalationContext context)
        {
            using (ServerManager iisManager = new ServerManager())
            {
                ConfigureApplicationCore(context, iisManager);

                iisManager.CommitChanges();
            }
        }


        protected virtual void ConfigureApplicationCore(IInstalationContext context, ServerManager iisManager)
        {
            SetLoadUserProfilePoolAttribute(iisManager, FindApplication(iisManager).ApplicationPoolName);
        }


        private void SetLoadUserProfilePoolAttribute(ServerManager iisManager, string poolName)
        {
            SetApplicationPoolAttribute(iisManager, poolName, "loadUserProfile", "False");
        }


        private void SetLoadUserProfilePoolAttribute(ServerManager iisManager, ApplicationPool pool)
        {
            SetApplicationPoolAttribute(iisManager, pool, "loadUserProfile", "False");
        }

        #endregion


        protected void SetApplicationPoolAttribute(ServerManager iisManager, string poolName, string attributeName, object attributeValue)
        {

            SetApplicationPoolAttribute(iisManager, iisManager.ApplicationPools[poolName], attributeName, attributeValue);

            
        }

        protected void SetApplicationPoolAttribute(ServerManager iisManager, ApplicationPool pool, string attributeName, object attributeValue)
        {
            
            var attribute = pool.Attributes.FirstOrDefault(a => string.Compare(a.Name,  attributeName, true) == 0);

            if (attribute != null)
            {
                attribute.Value = attributeValue;
            }
            else
            {
                var ch = pool.ChildElements.FirstOrDefault(c => c.Attributes.Any(a => string.Compare(a.Name, attributeName, true) == 0));

                if (ch != null)
                {
                    ch.SetAttributeValue(attributeName, attributeValue);
                }
            }
        }

        protected Application FindApplication(ServerManager iisManager)
        {
            return iisManager.FindApplication(this.ApplicationName);
        }

      


        protected void AddMimeTypes(Microsoft.Web.Administration.ServerManager iisManager, params MimeTypeDefinition[] mimeTypes)
        {

            var vDirConfig = FindApplication(iisManager).GetWebConfiguration();
            ConfigurationSection staticContentSection = vDirConfig.GetSection("system.webServer/staticContent");
            ConfigurationElementCollection staticContentCollection = staticContentSection.GetCollection();

            foreach (var mimeType in (mimeTypes ?? new MimeTypeDefinition[0]))
            {
                AddMimeType(staticContentCollection, mimeType);
            }

        }


        private void AddMimeType(ConfigurationElementCollection staticContentCollection, MimeTypeDefinition mimeType)
        {
            var configElement = GetExistingMimeType(staticContentCollection, mimeType);

            if (configElement == null)
            {
                configElement = staticContentCollection.CreateElement("mimeMap");
                configElement["fileExtension"] = mimeType.Extension;
                configElement["mimeType"] = mimeType.MimeType;
                staticContentCollection.Add(configElement);
            }
            else
            {
                configElement["mimeType"] = mimeType.MimeType;
            }

        }


        private ConfigurationElement GetExistingMimeType(ConfigurationElementCollection staticContentCollection, MimeTypeDefinition mimeType)
        {
            return staticContentCollection.FirstOrDefault(configElement => string.Compare(configElement.Attributes["fileExtension"].Value.ToString(), mimeType.Extension, true) == 0);
        }
       

    }
}
