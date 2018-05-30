using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;
using Spark.Infra.Exceptions;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.Dialogs
{
    public class LanguagesSelectionDialog : ViewModelBase, IOkCancelDialogBoxViewModel
    {
        
        public LanguagesSelectionDialog(Language[] languages)
        {
            
            this.AvailableLanguages = languages.OrderBy(lng => lng.Name)
                                               .Select(l => new ItemSelectorViewModel<Language>(l))
                                               .ToArray();

        }
        public void ExecuteCancel()
        {
            
        }


        public Action CustomOkAction { get; set; }

        public Language[] SelectedLanguages
        {
            get
            {
                return this.AvailableLanguages.Where(item => item.IsSelected)
                                              .Select(item => item.Item)
                                              .ToArray();
            }
        }

        public ItemSelectorViewModel<Language>[] AvailableLanguages { get; private set; }

        public void ExecuteOk()
        {
            if (SelectedLanguages.Length == 0)
                throw new ValidationException("You must select at least one language");

            CustomOkAction?.Invoke();

        }
    }
}
