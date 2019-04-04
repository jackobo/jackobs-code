using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Spark.Infra.Types;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using LayoutTool.ViewModels.DynamicLayout;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class MockupViewModel : ViewModelBase, IFiddlerOverrideProvider, INavigationAware
    {
        public MockupViewModel(IServiceLocator serviceLocator, SkinIndentity skinIdentity, TriggerViewModelCollection triggers)
        {
            _serviceLocator = serviceLocator;
            
            InitSkinSelector(skinIdentity);

            OpenClientCommand = new Command(OpenClient);
            
            UpdateUrl();

            PlayerAttributes = new PlayerAttributesSimulatorViewModel(serviceLocator, triggers);
        }

        

        protected override bool GlobalNotificationsEnabled
        {
            get
            {
                return false;
            }
        }

        IServiceLocator _serviceLocator;

        public PlayerAttributesSimulatorViewModel PlayerAttributes { get; private set; }

        private void InitSkinSelector(SkinIndentity skinIdentity)
        {
            SkinSelector = new SkinDefinitionBuilderSelectorViewModel(_serviceLocator.GetAllInstances<ISkinDefinitionBuilderViewModel>()
                                                                       .Where(b => b.CanProvideClientUrl)
                                                                       .ToArray());
            SkinSelector.StateRestored += SkinSelector_StateRestored;
            SkinSelector.LoadFrom(skinIdentity);
        }

        private void SkinSelector_StateRestored(object sender, EventArgs e)
        {
            SkinSelector.StateRestored -= SkinSelector_StateRestored;
            UpdateUrl();
        }

        

        private void SubscribeToSkinSelectorEvents()
        {
            SkinSelector.PropertyChanged += SkinSelector_PropertyChanged;
            SkinSelector.SelectedBuilderPropertyChanged += SkinSelector_SelectedReaderPropertyChanged;
        }

        private void UnsubscribeFromSkinSelectorEvents()
        {
            SkinSelector.PropertyChanged -= SkinSelector_PropertyChanged;
            SkinSelector.SelectedBuilderPropertyChanged -= SkinSelector_SelectedReaderPropertyChanged;
        }


        

        private SkinDefinition SkinDefinition
        {
            get
            {
                return _serviceLocator.GetInstance<ISkinDesigner>().BuildSkinDefinitionContext().SkinDefinition;
            }
            
        }

        
        IFiddlerServices FiddlerService
        {
            get { return _serviceLocator.GetInstance<IFiddlerServices>(); }
        }


        public SkinDefinitionBuilderSelectorViewModel SkinSelector { get; private set; }

        private void OpenClient()
        {
            Process.Start(this.ClientUrlBuilder.LaunchUrl.ToHttpUrlFormat());
        }

        public ICommand OpenClientCommand { get; private set; }

        SkinConversionResult _skinConversionResult;
        SkinConversionResult SkinConversionResult
        {
            get
            {
                if (this.SkinSelector.SelectedBuilder == null || this.SkinDefinition == null)
                    return null;

                if (_skinConversionResult == null)
                {
                    _skinConversionResult = SkinSelector.SelectedBuilder.Apply(this.SkinDefinition);
                }

                return _skinConversionResult;
            }
        }


        private void SkinSelector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
            if(e.PropertyName == nameof(SkinDefinitionBuilderSelectorViewModel.SelectedBuilder))
            {
                UpdateUrl();
            }

        }

        private void SkinSelector_SelectedReaderPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateUrl();
        }

        
        private void UpdateUrl()
        {
            if(HasSelectedSkin)
            {
                ClientUrlBuilder = this.SkinSelector.SelectedBuilder.GetClientUrlBuilder();
            }
            else
            {
                ClientUrlBuilder = null;
            }
        }


        private ClientUrlBuilderViewModel _clientUrlBuilder;

        public ClientUrlBuilderViewModel ClientUrlBuilder
        {
            get { return _clientUrlBuilder; }
            set
            {
                if(_clientUrlBuilder != null)
                {
                    FiddlerService.UnregisterFilesOverrideProvider(_clientUrlBuilder);
                }

                SetProperty(ref _clientUrlBuilder, value);

                if (_clientUrlBuilder != null)
                {
                    FiddlerService.RegisterFilesOverrideProvider(_clientUrlBuilder);
                }
            }
        }


        
       

        private bool HasSelectedSkin
        {
            get
            {
                return this.SkinSelector != null 
                    && this.SkinSelector.SelectedBuilder != null 
                    && this.SkinSelector.SelectedBuilder.IsValid
                    && this.SkinSelector.SelectedBuilder.CanProvideClientUrl;
            }
        }

        
        

        public FiddlerOverrideMode GetOverrideMode(string url)
        {

            if (this.ClientUrlBuilder?.LaunchUrl == null)
                return FiddlerOverrideMode.NoOverride;

            if (!url.StartsWith(this.ClientUrlBuilder.BaseUrl.ToHttpUrlFormat(), StringComparison.InvariantCultureIgnoreCase))
                return FiddlerOverrideMode.NoOverride;


            if (SkinConversionResult == null)
                return FiddlerOverrideMode.NoOverride;

              

            if (SkinConversionResult.Files.Any(f => url.StartsWith(f.OriginalFile.Location.ToHttpUrlFormat(), StringComparison.OrdinalIgnoreCase)))
            {
                return FiddlerOverrideMode.Normal;
            }

            if (SkinConversionResult.NewGames.Length == 0)
                return FiddlerOverrideMode.NoOverride;



            if (SkinConversionResult.NewGames.Any(game => game.IsInsideUrl(url)))
                return FiddlerOverrideMode.BypassServer;

            return FiddlerOverrideMode.NoOverride;

        }

        

        public FiddlerOverrideContent GetOverrideContent(string url, string currentBodyContent)
        {

            if (SkinConversionResult == null)
                return null;

         
            
            var fileOverride = GetConfigurationFileOverride(url);

            if (fileOverride != null)
            {
                return fileOverride;
            }
            
           
            return GetGameIconOverride(url);
        }


        private FiddlerOverrideContent GetConfigurationFileOverride(string url)
        {
            foreach (var file in SkinConversionResult.Files)
            {
                if (url.Contains(file.OriginalFile.FileName))
                {
                    return new FiddlerOverrideContent(file.NewContent);
                }
            }

            return null;
        }

        private FiddlerOverrideContent GetGameIconOverride(string url)
        {
            foreach (var newGame in SkinConversionResult.NewGames)
            {
                if (!newGame.IsInsideUrl(url))
                    continue;

                using (var image = new Bitmap(91, 97))
                {
                    using (var graphics = Graphics.FromImage(image))
                    using (var font = new Font("Tahoma", 10, FontStyle.Bold))
                    {
                        var gameName = newGame.Name + Environment.NewLine + "[" + newGame.Id.ToString() + "]";

                        StringFormat stringFormat = new StringFormat(StringFormatFlags.NoClip);
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        graphics.DrawString(gameName,
                                            font,
                                            Brushes.White,
                                            new RectangleF(0, 0, image.Width, image.Height),
                                            stringFormat);
                    }


                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return new FiddlerOverrideContent(ms.ToArray(), new KeyValuePair<string, string>("Content-Type", "image/png"));
                    }

                }
            }

            return null;
        }
        

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //this.SkinSelector.ShowOnlyReadersThatCanProvideClientUrl = true;
            SubscribeToSkinSelectorEvents();
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            UnsubscribeFromSkinSelectorEvents();
        }

        public void Reset()
        {
            _skinConversionResult = null;
        }
    }
}
