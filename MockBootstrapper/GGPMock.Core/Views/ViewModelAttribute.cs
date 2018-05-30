using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Views
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewModelAttribute : Attribute
    {
        public ViewModelAttribute(Type viewModelType)
        {
            this.ViewModelType = viewModelType;
        }

        public Type ViewModelType { get; private set; }

        public bool CacheView { get; set; }
    }
}
