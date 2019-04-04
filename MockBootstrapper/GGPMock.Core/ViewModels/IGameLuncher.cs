using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public interface IGameLuncher
    {
        void OpenGame(OpenGameParametersViewModel game);
        GGPMockDataProvider.LanguageMock[] GetLanguages();
        void OpenSimulator(int gameType);
    }
}
