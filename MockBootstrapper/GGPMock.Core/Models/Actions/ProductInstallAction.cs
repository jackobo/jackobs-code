using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;

namespace GGPMockBootstrapper.Models
{
    public abstract class ProductInstallAction<TOwnerProduct> : IInstallAction
        where TOwnerProduct : Product
    {

        public ProductInstallAction(TOwnerProduct ownerProduct)
        {
            Product = ownerProduct;
        }


        protected TOwnerProduct Product { get; private set; }


        public abstract string Description { get; }
        protected abstract string GetPackageName();
        protected abstract void Install(IInstalationContext context, string zipFileName);

        private int _subActionsCount = 0;
        public int SubActionsCount 
        {
            get{return _subActionsCount;}
            set
            {
                _subActionsCount = value;
                OnPropertyChanged(this.GetPropertyName(t => t.SubActionsCount));
            }
        }

        private int _currentActionIndex = 0;
        public int CurrentSubActionIndex
        {
            get { return _currentActionIndex; }
            set
            {
                _currentActionIndex = value;
                OnPropertyChanged(this.GetPropertyName(t => t.CurrentSubActionIndex));
            }
        }

        private string _currentSubActionDescription;
        public string CurrentSubActionDescription
        {
            get { return _currentSubActionDescription; }
            set
            {
                _currentSubActionDescription = value;
                OnPropertyChanged(this.GetPropertyName(t => t.CurrentSubActionDescription));
            }
        }


        protected virtual void IncrementSubactionIndex(string description)
        {
            this.CurrentSubActionIndex = Math.Min(this.CurrentSubActionIndex + 1, this.SubActionsCount);
            this.CurrentSubActionDescription = description;
        }

        protected virtual bool ShouldForceInstall(IInstalationContext context)
        {
            return false;
        }

        public virtual void Execute(IInstalationContext context)
        {
            var tempZipFileName = ExtractResourceToTempFile(context);

            if (!ShouldForceInstall(context) && context.EnvironmentServices.FileSystem.FilesAreEquals(tempZipFileName, this.Product.GetInstalledPackagePath(GetPackageName())))
            {
                return;
            }

            Install(context, tempZipFileName);

            context.EnvironmentServices.FileSystem.CopyFile(tempZipFileName, this.Product.GetInstalledPackagePath(GetPackageName()));
            context.EnvironmentServices.FileSystem.DeleteFile(tempZipFileName);


        }

        private string ExtractResourceToTempFile(IInstalationContext context)
        {
            var tempZipFileName = context.EnvironmentServices.FileSystem.GetTempFileName();
            context.EnvironmentServices.FileSystem.CopyFile(Product.GetEmbededResourceFullPath(GetPackageName()), tempZipFileName);
            //var tempZipFileName = context.EnvironmentServices.ExtractEmbededResourceToTempFile(Product.GetEmbededResourceFullPath(GetPackageName()), this.GetType().Assembly);
            return tempZipFileName;
        }





        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null)
                ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
