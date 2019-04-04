using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using GamesPortal.Client.Interfaces.Services;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;
using Spark.Wpf.Common.ViewModels.Matrix;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class GameVersionRegulationsViewModel : ServicedViewModelBase
    {
        GameVersion _gameVersion;
        Game _game;

        public GameVersionRegulationsViewModel(Game game, GameVersion gameVersion, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            _game = game;

            _gameVersion = gameVersion;
            this.QAApproveCommand = new Command(QAApprove, () => _gameVersion.CanQAApprove(GetMandatoryLanguagesProvider()));
            this.PMApproveCommand = new Command(PMApprove, () => _gameVersion.CanPMApprove(GetMandatoryLanguagesProvider()));

          

            this.Items = gameVersion.Regulations.Select(r => new RegulationViewModel(gameVersion, r, serviceLocator)).ToList();
        }

        private IMandatoryLanguagesProvider GetMandatoryLanguagesProvider()
        {
            return ServiceLocator.GetInstance<IGamesRepository>().GetMandatoryLanguagesPerRegulationProvider();
        }

        public string RegulationsThatCantBeApprovedMessage
        {
            get
            {
                var messages = new List<string>();
                messages.Add(GetUnapprovedMandatoryLanguagesMessage());
                messages.Add(GetMissingMandatoryLanguagesMessage());

                return string.Join(Environment.NewLine, messages.Where(msg => !string.IsNullOrEmpty(msg)).Select(msg => "# " + msg));
            }
        }

        private string GetMissingMandatoryLanguagesMessage()
        {

            var regulationNames = _gameVersion.GetMissingMandatoryLanguagesPerRegulations(GetMandatoryLanguagesProvider())
                                    .Select(item => item.RegulationType.Name)
                                    .ToArray();


            if (regulationNames.Length == 0)
                return null;

            if (regulationNames.Length == 1)
                return $"Regulation {regulationNames[0]} cannot be approved because mandatory languages are missing!";

            return $"{string.Join(", ", regulationNames)} regulations cannot be approved because mandatory languages are missing!";
        }

        private string GetUnapprovedMandatoryLanguagesMessage()
        {
            var regulationNames = _gameVersion.GetRegulationsWithMandatoryUnApprovedLanguages()
                                              .Select(regulation => regulation.RegulationType.Name)
                                              .ToArray();


            if (regulationNames.Length == 0)
                return null;

            if (regulationNames.Length == 1)
                return $"Regulation {regulationNames[0]} cannot be approved because there are mandatory languages that must be approved first!";

            return $"{string.Join(", ", regulationNames)} regulations cannot be approved because there are mandatory languages that must be approved first!";
        }

        public ICommand QAApproveCommand { get; protected set; }
        public ICommand PMApproveCommand { get; protected set; }

        public List<RegulationViewModel> Items { get; }

        public class RegulationViewModel : ServicedViewModelBase
        {
            public RegulationViewModel(GameVersion gameVersion, GameVersionRegulation regulation, IServiceLocator serviceLocator)
                : base(serviceLocator)
            {
                _gameVersion = gameVersion;
                _regulation = regulation;

            }

            GameVersion _gameVersion;
            GameVersionRegulation _regulation;

            public string RegulationName
            {
                get { return _regulation.RegulationType.Name; }
            }


            public string UnapprovedMandatoryLanguages
            {
                get
                {
                    
                    return string.Join(", ", _gameVersion.GetUnApprovedMandatoryLanguages(_regulation.RegulationType).Select(lng => lng.Name));
                }
            }

            private IMandatoryLanguagesProvider GetMandatoryLanguagesProvider()
            {
                return ServiceLocator.GetInstance<IGamesRepository>().GetMandatoryLanguagesPerRegulationProvider();
            }

            public string MissingMandatoryLanguages
            {
                get
                {                   
                    return string.Join(", ", _gameVersion.GetMissingMandatoryLanguages(_regulation.RegulationType, GetMandatoryLanguagesProvider())
                                                        .Select(lang => lang.Name));
                }
            }

           
            public string Status
            {
                get { return _regulation.ApprovalStatusDescription; }
            }
        }



        private void QAApprove()
        {

            var gamesRepository = ServiceLocator.GetInstance<IGamesRepository>();

            
            var dlg = CreateApprovalDialog(_gameVersion,
                                           _gameVersion.Regulations.Where(r => r.CanQAApprove(_gameVersion.Category, GetMandatoryLanguagesProvider()))
                                           .Select(r => r.RegulationType).ToArray());


            dlg.IsReadOnly = _game.Category == GamingComponentCategory.Game;

            dlg.CustomOkAction = () =>
            {
                ServiceLocator.GetInstance<IGamesRepository>().QAApprove(_gameVersion.Id, dlg.RegulationsSelector.SelectedRegulations);
                _game.ResetVersions();
            };

            ServiceLocator.GetInstance<IDialogServices>().ShowOkCancelDialogBox(dlg);
        }
        

        private void PMApprove()
        {
            var gamesRepository = ServiceLocator.GetInstance<IGamesRepository>();

            var dlg = CreateApprovalDialog(_gameVersion, 
                                          _gameVersion.Regulations.Where(r => r.CanPMApprove(_gameVersion.Category, GetMandatoryLanguagesProvider())).Select(r => r.RegulationType).ToArray());
            
            dlg.CustomOkAction = () =>
            {
                gamesRepository.PMApprove(_gameVersion.Id, dlg.RegulationsSelector.SelectedRegulations);
                _game.ResetVersions();
            };

            ServiceLocator.GetInstance<IDialogServices>().ShowOkCancelDialogBox(dlg);
            

        }


        private Dialogs.GameVersionApprovalDialog CreateApprovalDialog(GameVersion gameVersion, 
                                                                       RegulationType[] regulations)
        {
            var dlg = new Dialogs.GameVersionApprovalDialog(regulations);
            dlg.Title = $"{_game.Name} [{_game.MainGameType}] - {gameVersion.Infrastructure} version {gameVersion.Version}";
            return dlg;
        }


       

    }
}
