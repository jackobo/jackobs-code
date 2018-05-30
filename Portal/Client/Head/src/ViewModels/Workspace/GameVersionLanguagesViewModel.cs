using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;
using Spark.Wpf.Common.ViewModels.Matrix;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class GameVersionLanguagesViewModel : ServicedViewModelBase
    {
        public GameVersionLanguagesViewModel(Game game, GameVersion gameVersion, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.Game = game;
            this.GameVersion = gameVersion;
            this.Items = MatrixBuilder<RegulationLanguageViewModel>.Matrix()
                                     .AddRowHeader("Language", item => item.LanguageName)
                                     .AddRowHeader("Previously approved in", item => item.PreviouslyApprovedInVersion)
                                     .NoMoreRowHeaders()
                                     .AddColumnHeader(item => item.RegulationName)
                                     .AggregateBy(items => items.Select(item => item.GetStatus()).FirstOrDefault())
                                     .Build();


            this.Items.Load(gameVersion.Regulations.SelectMany(regulation => regulation.Languages.Select(lang => new RegulationLanguageViewModel(game, gameVersion, regulation, lang))));

            this.QAApproveCommand = new Command(QAApprove, () => this.GameVersion.GetUnApprovedLanguages().Any());
        }

        Game Game { get; set; }

        private GameVersion GameVersion { get; set; }
        private void QAApprove()
        {
            var dlg = new Dialogs.LanguagesSelectionDialog(this.GameVersion.GetUnApprovedLanguages());
            dlg.CustomOkAction = () =>
            {
                ServiceLocator.GetInstance<IGamesRepository>().LanguageApprove(this.GameVersion.Id, dlg.SelectedLanguages.Select(lng => lng.Name).ToArray());
                Game.ResetVersions();
            };

            ServiceLocator.GetInstance<IDialogServices>().ShowOkCancelDialogBox(dlg);

        }

        public ICommand QAApproveCommand { get; protected set; }

        public MatrixViewModel<RegulationLanguageViewModel> Items { get; private set; }


        public class RegulationLanguageViewModel : ViewModelBase
        {
            public RegulationLanguageViewModel(Game game, GameVersion gameVersion, GameVersionRegulation regulation, GameVersionRegulationLanguage language)
            {
                _game = game;
                _gameVersion = gameVersion;
                _regulation = regulation;
                _language = language;
            }

            Game _game;
            GameVersion _gameVersion;
            GameVersionRegulation _regulation;
            GameVersionRegulationLanguage _language;

            public string LanguageName
            {
                get { return _language.Language.Name; }
            }

            public string PreviouslyApprovedInVersion
            {
                get
                {
                    var result = _game.GetPreviouslyApprovedVersionForLanguage(_language.Language, _gameVersion);
                    if (result.Any())
                        return result.First().ToString();
                    else
                        return string.Empty;
                }
            }

            public string RegulationName { get { return _regulation.RegulationType.Name; } }

            public string GetStatus()
            {
                return _language.ApprovalInfo.Status;
              
            }


        }
    }
}
