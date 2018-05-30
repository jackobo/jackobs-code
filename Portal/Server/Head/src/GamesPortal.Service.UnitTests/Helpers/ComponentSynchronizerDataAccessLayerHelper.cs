using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.DataAccessLayer;
using NSubstitute;

namespace GamesPortal.Service.Helpers
{
    internal static class ComponentSynchronizerDataAccessLayerHelper
    {
        public static void AddMockData(this IArtifactorySynchronizerDataAccessLayer synchronizerDal, params GameTypeDescriptor[] gameTypesInTheDictionary)
        {
            synchronizerDal.AddMockData(null, gameTypesInTheDictionary);
        }

        public static void AddMockData(this IArtifactorySynchronizerDataAccessLayer synchronizerDal, Game gameRowToReturn, params GameTypeDescriptor[] gameTypesInTheDictionary)
        {

            synchronizerDal.GetGame(Arg.Any<int>(), Arg.Any<bool>()).Returns(gameRowToReturn);

            if ((gameTypesInTheDictionary ?? new GameTypeDescriptor[0]).Length > 0)
            {
                synchronizerDal.GetGameName(Arg.Any<int>()).Returns(gameTypesInTheDictionary.First().GameName,
                                                                    gameTypesInTheDictionary.Skip(1).Select(item => item.GameName).ToArray());


                synchronizerDal.GetGameTypes(Arg.Any<int>()).Returns(gameTypesInTheDictionary);
            }

        }
    }
}
