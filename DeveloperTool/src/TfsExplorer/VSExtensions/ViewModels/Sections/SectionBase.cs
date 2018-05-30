using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Controls;
using Spark.Wpf.Common.ViewModels;

namespace Spark.VisualStudio.Extensions.ViewModels.Sections
{
    public abstract class SectionBase : ViewModelBase, ITeamExplorerSection
    {

        protected IServiceProvider ServiceProvider { get; private set; }

        public abstract object SectionContent { get; }

        public abstract string Title { get; }

        public virtual void Initialize(object sender, SectionInitializeEventArgs e)
        {
            this.ServiceProvider = e.ServiceProvider;
        }

       


        public virtual void Cancel()
        {
        }



        public object GetExtensibilityService(Type serviceType)
        {
            return null;
        }
        

        private bool _isBusy = false;
        public bool IsBusy
        {
            get
            {
                return this._isBusy;
            }
            private set
            {
                SetProperty(ref _isBusy,value);
            }
        }

        private bool _isExpanded = true;
        public bool IsExpanded
        {
            get
            {
                return this._isExpanded;
            }
            set
            {
                SetProperty(ref _isExpanded, value);
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }
            set
            {
                SetProperty(ref _isVisible, value);
            }
        }


        public virtual void Loaded(object sender, SectionLoadedEventArgs e)
        {
        }

        public virtual void Refresh()
        {
        }

        public virtual void SaveContext(object sender, SectionSaveContextEventArgs e)
        {
        }


        protected T GetService<T>()
        {
            if (this.ServiceProvider != null)
            {
                return (T)this.ServiceProvider.GetService(typeof(T));
            }
            return default(T);
        }
    }
}
