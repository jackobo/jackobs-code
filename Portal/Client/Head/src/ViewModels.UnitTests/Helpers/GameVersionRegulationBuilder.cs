using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Client.Interfaces.Entities;
using NSubstitute;


namespace GamesPortal.Client.ViewModels.Helpers
{
    public class GameVersionRegulationBuilder
    {
        RegulationType _regulationType = RegulationType.GetRegulation("Gibraltar");
        DownloadInfo _downloadInfo = new DownloadInfo("http://localhost/file.zip", "file.zip", 100, "123");
        List<GameVersionRegulationLanguage> _languages = new List<GameVersionRegulationLanguage>();

        private GameVersionRegulationBuilder()
        {

        }

        public static GameVersionRegulationBuilder GameVersionRegulation()
        {
            return new GameVersionRegulationBuilder();
        }

        IGameVersionApprovalInfo _approvalInfo = Substitute.For<IGameVersionApprovalInfo>();

        public GameVersionRegulationBuilder WithApprovalInfo(IGameVersionApprovalInfo approvalInfo)
        {
            _approvalInfo = approvalInfo;
            return this;
        }

        public GameVersionRegulationBuilder WithRegulation(string regulationname)
        {
            _regulationType = RegulationType.GetRegulation(regulationname);
            return this;
        }

        public GameVersionRegulationBuilder WithRegulation(RegulationType regulationType)
        {
            _regulationType = regulationType;
            return this;
        }
        
        
        public GameVersionRegulationBuilder WithLanguage(GameVersionRegulationLanguage language)
        {
            _languages.Add(language);
            return this;
        }

        public GameVersionRegulationBuilder WithLanguage(Language language)
        {
            _languages.Add(new GameVersionRegulationLanguage(language, Substitute.For<ILanguageApprovalInfo>()));
            return this;
        }

        public GameVersionRegulation Build()
        {
            return new GameVersionRegulation(_regulationType, _downloadInfo, _approvalInfo, _languages.ToArray());
        }
    }
}
