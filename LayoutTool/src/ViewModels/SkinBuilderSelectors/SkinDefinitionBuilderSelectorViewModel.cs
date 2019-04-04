using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class SkinDefinitionBuilderSelectorViewModel : ViewModelBase
    {
        public SkinDefinitionBuilderSelectorViewModel(ISkinDefinitionBuilderViewModel[] skinBuilders)
        {
            LoadBuilders(skinBuilders);
        }

        protected override bool GlobalNotificationsEnabled
        {
            get
            {
                return false;
            }
        }

        public void LoadFrom(SkinIndentity skinIdentity)
        {
            var builder = this.Builders.FirstOrDefault(b => b.Id == skinIdentity.Selector.Id);
            if (builder == null)
                return;

            builder.RestoreStateFrom(skinIdentity);

            ActivateBuilder(builder);
        }
                

        private void ActivateBuilder(ISkinDefinitionBuilderViewModel builder)
        {
            foreach (var r in this.Builders)
            {
                r.IsActive = object.ReferenceEquals(r, builder);
            }
        }

        public override void Activate()
        {
            base.Activate();
            if (SelectedBuilder != null)
                SelectedBuilder.IsActive = true;
        }

        private void LoadBuilders(IEnumerable<ISkinDefinitionBuilderViewModel> builders)
        {
            this.Builders = builders.Where(r => r.IsVisible).OrderBy(r => r.Order).ToArray();

            foreach (var b in Builders)
            {
                b.StateRestored += Builder_StateRestored;
                b.PropertyChanged += Builder_PropertyChanged;
            }
        }

        private void Builder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ISkinDefinitionBuilderViewModel.IsActive))
            {
                OnPropertyChanged(nameof(SelectedBuilder));
                
            }

            if(object.ReferenceEquals(sender, SelectedBuilder))
            {
                SelectedBuilderPropertyChanged?.Invoke(sender, e);
            }
        }

        
        public event EventHandler StateRestored;
        private void Builder_StateRestored(object sender, EventArgs e)
        {
            StateRestored?.Invoke(this, EventArgs.Empty);
        }

        ISkinDefinitionBuilderViewModel[] _builders;


        public ISkinDefinitionBuilderViewModel[] Builders
        {
            get
            {
                return _builders;
            }
            private set
            {
                SetProperty(ref _builders, value);

            }
        }



        public event PropertyChangedEventHandler SelectedBuilderPropertyChanged;


        
        public ISkinDefinitionBuilderViewModel SelectedBuilder
        {
            get
            {
                return this.Builders.FirstOrDefault(b => b.IsActive);
            }
        }
        
    }
}
