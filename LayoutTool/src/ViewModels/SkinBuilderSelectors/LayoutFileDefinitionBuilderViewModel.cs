using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Spark.Infra.Types;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;
using Spark.Infra.Windows;

namespace LayoutTool.ViewModels
{
    public class LayoutFileDefinitionBuilderViewModel : ViewModelBase, ISkinDefinitionBuilderViewModel
    {
        public LayoutFileDefinitionBuilderViewModel(ISkinDefinitionSerializer serializer, 
                                                    IFileSystemManager fileSystemManager,
                                                    IDialogServices dialogServices)
        {
            this.Serializer = serializer;
            this.FileSystemManager = fileSystemManager;
            this.DialogServices = dialogServices;
            this.SelectFileCommand = new Command(SelectFile);
        }

        ISkinDefinitionSerializer Serializer { get; set; }
        IFileSystemManager FileSystemManager { get; set; }

        IDialogServices DialogServices { get; set; }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                SetProperty(ref _isActive, value);
                OnPropertyChanged("");
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                SetProperty(ref _isVisible, value);
            }
        }


        public bool CanPublish
        {
            get { return false; }
        }

        void ISkinDefinitionBuilderViewModel.Publish(SkinDefinitionContext skinDefinitionContext)
        {
            throw new NotSupportedException();
        }


        public bool CanProvideClientUrl
        {
            get
            {
                return false;
            }
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(this.FilePath)
                        && FileSystemManager.FileExists(this.FilePath);
            }
        }

        public int Order
        {
            get
            {
                return 250;
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                SetProperty(ref _filePath, value);
                OnPropertyChanged(nameof(SourceSkinDescription));
                OnPropertyChanged(nameof(TargetSkinDescription));
            }
        }



        public string SourceSkinDescription
        {
            get
            {
                return CurrentSkinDefinitionContext?.SourceSkin?.ToString();
            }
        }

        public string TargetSkinDescription
        {
            get
            {
                return CurrentSkinDefinitionContext?.DestinationSkin?.ToString();
            }
        }

        public ICommand SelectFileCommand { get; private set; }


        private void SelectFile()
        {
            var file = this.DialogServices.SelectFile("Layout file (*.lyt)|*.lyt"); 
            if(!string.IsNullOrEmpty(file))
            {
                this.FilePath = file;
            }

            
        }

        public string SourceName
        {
            get
            {
                return "Layout file";
            }
        }

        Guid ISkinDefinitionBuilderViewModel.Id
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public SkinConversionResult Apply(SkinDefinition skinDefinition)
        {
            throw new NotSupportedException();
        }
        
        SkinDefinitionContext CurrentSkinDefinitionContext
        {
            get
            {
                return ReadSkinDefinitionContext();
            }
        }

        SkinDefinitionContext ReadSkinDefinitionContext()
        {
            return this.Serializer.Deserialize(this.FileSystemManager.ReadAllText(this.FilePath));
        }

        public SkinDefinitionContext Build()
        {
            return CurrentSkinDefinitionContext;
        }

        public ClientUrlBuilderViewModel GetClientUrlBuilder()
        {
            return null;
        }

        void ISkinDefinitionBuilderViewModel.RestoreStateFrom(SkinIndentity skinIdentity)
        {
            throw new NotSupportedException();
        }

        SkinIndentity ISkinDefinitionBuilderViewModel.GetSkinIdentity()
        {
            throw new NotSupportedException();
        }

        public event EventHandler StateRestored;

        private void OnStateRestored()
        {
            StateRestored?.Invoke(this, EventArgs.Empty);
        }
    }
}
