using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.Models
{
    public abstract class Product : INotifyPropertyChanged
    {
        public Product(string name, IInstalationContext installationContext)
        {
            this.Name = name;
            this.InstallationContext = installationContext;
        }
        
        public string Name { get; private set; }

        protected IInstalationContext InstallationContext { get; private set; }

        protected abstract string GetContentSectionName();
        public abstract IInstallAction[] GetInstallActions();

        public virtual string GetEmbededResourceFullPath(string resourceName)
        {
            //return string.Format("GGPMockBootstrapper.Content.{0}.{1}", GetContentSectionName(), resourceName);

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", GetContentSectionName(), resourceName);
        }

        public string GetInstalledPackagePath(string packageName)
        {
            
            return Path.Combine(this.InstallationContext.Parameters.AppDataFolder, "InstalledPackages", this.GetContentSectionName(), packageName);
        }

        public GGPMockDataProvider.PlayerData GetMockData()
        {
            return GGPMockDataManager.Singleton.MockData;
        }

        public string GetMachineHostName()
        {
            var ipV4Addresses = GGPGameServer.Installer.Models.Utils.GetAllIPv4Addresses();

            if (ipV4Addresses.IsNullOrEmpty() || ipV4Addresses.Length > 1)
                return GGPGameServer.Installer.Models.Utils.GetLocalhostFqdn();
            else
                return ipV4Addresses[0];
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;

            if (ev != null)
                ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }   


    
}


