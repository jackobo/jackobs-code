using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GGPMockBootstrapper.Views
{
    public static class ViewFactory
    {

        static Dictionary<Type, Type> _viewModel2ViewMapping = null;

        private static Dictionary<Type, Type> ViewModel2ViewMapping
        {
            get
            {
                if (_viewModel2ViewMapping == null)
                {
                    _viewModel2ViewMapping = new Dictionary<Type, Type>();

                    foreach (var viewType in typeof(ViewFactory).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IView).IsAssignableFrom(t)))
                    {
                        if (!typeof(UserControl).IsAssignableFrom(viewType) && !typeof(Window).IsAssignableFrom(viewType))
                            throw new ApplicationException(string.Format("{0} must be a {1}", viewType.FullName, typeof(UserControl).FullName));

                        var viewModelAttribute = viewType.GetCustomAttributes(typeof(ViewModelAttribute), false).FirstOrDefault() as ViewModelAttribute;
                        if (viewModelAttribute == null)
                            throw new ApplicationException(string.Format("Missing {0} on view type {1}", typeof(ViewModelAttribute).FullName, viewType.FullName));

                        _viewModel2ViewMapping.Add(viewModelAttribute.ViewModelType, viewType);
                    }
                }

                return _viewModel2ViewMapping;
            }

        }

        public static Control CreateView(object viewModel)
        {
            if (!ViewModel2ViewMapping.ContainsKey(viewModel.GetType()))
                throw new ApplicationException(string.Format("Can't find a view for {0}", viewModel.GetType().FullName));

            var control = Activator.CreateInstance(ViewModel2ViewMapping[viewModel.GetType()]) as Control;

            var view = control as IView;

            view.ViewModel = viewModel;

            return control;
        }



    }
}
