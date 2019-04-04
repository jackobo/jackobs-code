using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace GamesPortal.Service
{
    [ServiceContract]
    public interface IGamesPortalToBuildMachineAdapter
    {
       
        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void StartGameSynchronization(string repositoryName, string gamesFolderName, int gameType, string versionFolder);

        [OperationContract]
        [WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void StartChillWrapperSynchronization(Entities.GamingComponentCategory componentType, string versionFolder);

    }

    [DataContract]
    public class ExplicitForceGameSynchronizationRequest
    {
        [DataMember]
        public string RepositoryName { get; set; }
        [DataMember]
        public string GamesFolderName { get; set; }
        [DataMember]
        public int GameType { get; set; }
        
        [DataMember]
        public string VersionFolder { get; set; }
    }
}
