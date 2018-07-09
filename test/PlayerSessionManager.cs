using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using GameCommonInterface;
using GGPGameServer.Common;
using GGPGameServer.Contracts;

using GGPGameServer.Core.GameEngineManager;
using GGPGameServer.Core.GameLogicManagement;
using GGPGameServer.Core.ProtocolManager;
using GGPGameServer.Core.Simulations;
using GGPGameServer.OperatorCommonInterface;

using RL.Common.CommonTypes;
using RL.Common.Utils;
using StructureMap;
using Newtonsoft.Json;
using System.Reflection;

namespace GGPGameServer.Core
{ 
    public class PlayerSessionManager : Singleton<PlayerSessionManager>
    {
        private static readonly ILogger LogGeneral = ServerConfigAdmin.GetSingleton().LoggerFactory.GetLogger("PlayerSessionManager", LogCategory.General);
        private static readonly ILogger LogJackPot = ServerConfigAdmin.GetSingleton().LoggerFactory.GetLogger("PlayerSessionManager", GGPLogCategory.Jackpots);
        private static readonly ILogger GameDataMessages = ServerConfigAdmin.GetSingleton().LoggerFactory.GetLogger("PlayerSessionManager", GGPLogCategory.GameDataMessages);

        private IOperatorManager _operatorManager;

        public IOperatorManager OperatorManagerInstance
        {
            get
            {
                return _operatorManager ?? OperatorManager.GetSingleton();
            }
            set { _operatorManager = value; }
        }

        public void Start(bool withCleaner = true)
        {
            LeavePlayer(1);
            if (withCleaner)
            {
                StartCleaner();
            }
        }

        internal void ShutingDown()
        {
            StopCleaner();
            DIFactory.PlayerSessionDAL.Stop();
        }

        #region Player events

        public long JoinPlayer(JoinPlayerRequest joinPlayerRequest, ref int errorCode, ref string errorDescription, out PlayerSession pS, out string operatorJoinData)
        {
            long UniquePlayerID = -1;
            errorCode = 0;
            errorDescription = string.Empty;
            string reconnectionString = string.Empty;
            pS = null;
            operatorJoinData = string.Empty;
            string GGPPlayerIDstr = null;

            if (ServerConfigAdmin.GetSingleton().ShuttingDown)
            {
                LogGeneral.WarnFormat("Join Message - failed to join server is shutting down, client IP: {0}, game type {1}, operator ID {2}",
                                joinPlayerRequest.ClientIP, joinPlayerRequest.GameType, joinPlayerRequest.OperatorID);
                errorCode = (int)enGGPJoinErrorCodes.SYSTEM_ERROR;
                return UniquePlayerID;
            }

            if (!GameHandlersPoolManager.IsGameTypeSupported(joinPlayerRequest.GameType, joinPlayerRequest.OperatorID))
            {
                LogGeneral.ErrorFormat("Join Message - game type is not supported, client IP: {0}, game type {1}, operator ID {2}",
                                    joinPlayerRequest.ClientIP, joinPlayerRequest.GameType, joinPlayerRequest.OperatorID);
                errorCode = (int)enGGPJoinErrorCodes.SYSTEM_ERROR;
                return UniquePlayerID;
            }

            IPlayerActivityServices playerServices = OperatorManagerInstance.GetOperatorEngine(joinPlayerRequest.OperatorID, joinPlayerRequest.JoinType);
            IOperatorData operatorData = playerServices.DeserializeOperatorDataFromClientData(joinPlayerRequest.OperatorData, joinPlayerRequest.ProtocolType);

            try
            {
                var authResponse = playerServices.Authenticate(joinPlayerRequest.GameType, joinPlayerRequest.ClientIP, joinPlayerRequest.Language, 
                                                                joinPlayerRequest.GameCurrencyCode, joinPlayerRequest.Token, operatorData);
                if (authResponse.Success == true)
                {
                    pS = FindSessionToRejoin(authResponse.ClientID, joinPlayerRequest.OperatorID, operatorData, playerServices, joinPlayerRequest.GameType, out GGPPlayerIDstr);

                    QaModeDeleteCurrentSession(pS, GGPPlayerIDstr);

                    if (pS == null)
                    {
                        UniquePlayerID = ServerConfigAdmin.GetSingleton().GenerateID(true);
                        pS = CreateNewSession(joinPlayerRequest, authResponse, operatorData);
                        ValidateJoinTestData(pS);
                        AppendLoggerEntryHeaderItems(UniquePlayerID, pS);
                        TrySimulateDisconnectionInfo(pS);

                        HandleDRS(pS);
                    }
                    else //rejoining
                    {
                        UniquePlayerID = long.Parse(GGPPlayerIDstr);
                        pS.IsRejoining = true;
                        //validate and update data
                        pS.PlayerIP = authResponse.ClientIP;
                        pS.DisplayName = joinPlayerRequest.DisplayName;
                        pS.ProtocolType = joinPlayerRequest.ProtocolType;
                        pS.TimeStamp = DateTime.Now;
                        pS.ResumeGameAttempts++;
                        AppendLoggerEntryHeaderItems(UniquePlayerID, pS);
                        //A-B tests
                        ValidateRejoinTestData(pS);
                        LogGeneral.NormalFormat("JoinPlayer rejoining");
                    }

                    //register session(free play regulation)
                    if (playerServices.PostAuthenticate(joinPlayerRequest.GameType, pS.PlayerIP, joinPlayerRequest.Language, joinPlayerRequest.GameCurrencyCode,
                            pS.OperatorData, ref errorCode) == false)
                    {
                        int syserrorCode = (int)enGGPJoinErrorCodes.SYSTEM_ERROR;
                        throw new PlayerSessionManagerException(syserrorCode,
                            string.Format("Join Message - PostAuthenticate failed for Player ID {0}, client IP: {1} , error {2}",
                                UniquePlayerID, pS.PlayerIP, errorCode));
                    }
                    if (pS.ISD == null)
                    {
                        pS.ISD = string.Empty;
                    }

                    operatorJoinData = pS.OperatorData.GetOperatorJoinData(pS.ProtocolType);
                    LogGeneral.NormalFormat("JoinPlayer OK, operator Join data {0}!", operatorJoinData);
                }
                else
                {
                    errorCode = authResponse.ErrorCode;
                    errorDescription = authResponse.ErrorDescription;
                    LogGeneral.WarnFormat("Authenticate failed error {0}, description {1}", errorCode, errorDescription);
                }
            }
            catch (PlayerSessionManagerException e)
            {
                LogGeneral.Warn(e.ToString());
                errorCode = e.ErrorCode;
            }
            catch (Exception e)
            {
                LogGeneral.FatalFormat("Exception in Join - Player ID {0}, client IP: {1}, error {2}", UniquePlayerID, joinPlayerRequest.ClientIP, e);
                errorCode = (int)enGGPJoinErrorCodes.SYSTEM_ERROR;
            }
            finally
            {
                if (errorCode != 0 && UniquePlayerID > 0)
                {
                    operatorJoinData = pS.GetOperatorJoinDataOrNull(joinPlayerRequest.ProtocolType);
                    if (pS.IsRejoining)
                    {
                        DIFactory.PlayerSessionDAL.Unlock(UniquePlayerID.ToString());
                    }

                    UniquePlayerID = 0;
                }
                else if (pS != null && errorCode == 0)
                {
                    if (DIFactory.PlayerSessionDAL.Put(UniquePlayerID.ToString(), pS) == false)
                    {
                        LogGeneral.FatalFormat("Join Message -failed to save session ps  Player ID {0}, client IP: {1}", UniquePlayerID, joinPlayerRequest.ClientIP);
                        errorCode = (int)enGGPJoinErrorCodes.SYSTEM_ERROR;
                        UniquePlayerID = 0;
                    }
                    else
                    {
                        RouletteHelper.MapCidToSessionId(pS.ClientID, UniquePlayerID);
                    }
                }
            }
            return UniquePlayerID;
        }


        public bool LeavePlayer(long playerId)
        {
            var pS = DIFactory.PlayerSessionDAL.Get(playerId.ToString(), true);
            bool left = false;
            if (pS == null)
            {
                LogGeneral.WarnFormat("Handle leave Player Failed to get player session ps from the storage player Id {0}", playerId);
                return false;
            }
            try
            {
                AppendLoggerEntryHeaderItems(playerId, pS);

                LogGeneral.NormalFormat("Handle leave");

                AutoPlayer(playerId, pS);
                GamePlayStateMachine.GetSingleton().HandleLeaveGame(pS);
                left = true;
            }
            catch (Exception ex)
            {
                LogGeneral.FatalFormat("{0}Handle leave Player is Failed. Client IP: {1} Unknown exeption is appeared. Exception:{2}",
                    FormatHelper.FormatLogHeader(pS.ClientID, 0, pS.GameWindowID, pS.GameID), pS.PlayerIP, ex);
            }
            finally
            {
                if (pS.GameState == GameStateCode.None)
                {
                    if (DIFactory.PlayerSessionDAL.Remove(playerId.ToString()) == false)
                        LogGeneral.WarnFormat("{0}Handle leave Player- Failed to remove from Dal. player ID: {1} ",
                                            FormatHelper.FormatLogHeader(pS.ClientID, 0, pS.GameWindowID, pS.GameID), playerId);
                }
                else
                {
                    pS.SuspendedExpirationDate = OperatorManagerInstance.GetOperatorEngine(pS.OperatorId, pS.JoinType).GetSuspendedExpirationDate(pS.OperatorData);
                    if (DIFactory.PlayerSessionDAL.Put(playerId.ToString(), pS, false) == false)
                    {
                        LogGeneral.FatalFormat("{0}Handle leave Player- Failed to save player session game string {1}",
                            FormatHelper.FormatLogHeader(pS.ClientID, 0, pS.GameWindowID, pS.GameID), pS.GameString);
                    }
                }
            }
            return left;
        }
        
        public GameResponseContext HandlePlayerGameMessage(long playerId,uint messageTicket, string strRequestMessage, string incomingOperatorGameData = null)
        {
            OptionalOut<enCacheError> cacheError = new OptionalOut<enCacheError>();
            var playerSession = DIFactory.PlayerSessionDAL.Get(playerId.ToString(CultureInfo.InvariantCulture), true, cacheError);
 
            if (playerSession == null)
            {
                throw new PlayerSessionManagerException(ConvertCacheError(cacheError.Result));
            }
            try
            {
                AppendLoggerEntryHeaderItems(playerId, playerSession);
                var operatorEngine = OperatorManagerInstance.GetOperatorEngine(playerSession.OperatorId, playerSession.JoinType);
                WriteIncommingMessageToLog(playerSession, strRequestMessage, playerId, incomingOperatorGameData,messageTicket);
                if (operatorEngine.CanHandleGameMessage(playerSession.OperatorData) == false)
                {
                    throw new PlayerSessionManagerException(enErrorCodeInfra.GeneralError, "Operator cannot handle game requestMessage");
                }
               
                operatorEngine.SetOperatorGameData(playerSession.OperatorData, incomingOperatorGameData, playerSession.ProtocolType);

                playerSession.GameEventCounter++;
                playerSession.TimeStamp = DateTime.Now;
                
                ServerConfigAdmin.GetSingleton().QaMode.MessageInterceptor.RecordGameRequest(playerSession, playerId, strRequestMessage);
                GameResponseContext response = new GameResponseContext();
                
                if (messageTicket != 0 && messageTicket == playerSession.LastMessageTicket)
                {
                    response.GameResponseMessage = playerSession.LastMessageResponse;
                    WriteToLog("Resending Same Message Ticket - Using Cache", string.Format("GameState: {0}", playerSession.GameState));
                }
                else
                {
                    IRequestMessage requestMessage = DIFactory.ProtocolManagerEngine.DeserializeMessage(playerSession, strRequestMessage);
                    IResponseMessage responseMessage = GamePlayStateMachine.GetSingleton().HandleGameMessage(playerId, playerSession, requestMessage);

                    ExceptionIfResponseIsNull(playerSession, responseMessage);

                    response.GameResponseMessage = DIFactory.ProtocolManagerEngine.SerializeMessage(playerSession, responseMessage);
                    playerSession.LastMessageTicket = messageTicket;
                    playerSession.LastMessageResponse = (messageTicket == 0) ? null : response.GameResponseMessage;
                }
                response.OperatorGameData = playerSession.OperatorData.GetOperatorGameData(playerSession.ProtocolType);
                response.PS = playerSession;

                ServerConfigAdmin.GetSingleton().QaMode.MessageInterceptor.RecordGameResponse(playerSession, playerId, response.GameResponseMessage);

                WriteOutgoingMessageToLog(response.PS, response.GameResponseMessage, playerId, response.OperatorGameData);
                
                return response;
            }
            catch (PlayerSessionManagerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogGeneral.ErrorFormat("HandleGameMessage Exception while processing msg, Exception:{0}", ex.ToString());
                throw new PlayerSessionManagerException((int)enGGPJoinErrorCodes.SYSTEM_ERROR);
            }
            finally
            {
                if (DIFactory.PlayerSessionDAL.Put(playerId.ToString(CultureInfo.InvariantCulture), playerSession) == false)
                {
                    LogGeneral.FatalFormat("game Message -failed to save session ps  Player ID {0}", playerId);
                    throw new PlayerSessionManagerException((int)enGGPJoinErrorCodes.SYSTEM_ERROR);
                }
            }
        }

        public GameResponseContext HandleGameDataMessage(long playerSessionID, string strRequestMessage)
        {
            var playerSession = DIFactory.PlayerSessionDAL.Get(playerSessionID.ToString(CultureInfo.InvariantCulture), false);

            if (playerSession == null)
            {
                throw new PlayerSessionManagerException(enErrorCodeInfra.PlayerNotExist, "Player Not Found");
            }
            try
            { 
                AppendLoggerEntryHeaderItems(playerSessionID, playerSession);
                var operatorEngine = OperatorManagerInstance.GetOperatorEngine(playerSession.OperatorId, playerSession.JoinType);
                
                GameDataMessages.NormalFormat("Incoming Game data message: {0}", strRequestMessage);
                IRequestMessage requestMessage = DIFactory.ProtocolManagerEngine.DeserializeMessage(playerSession, strRequestMessage);

                using (var handler = GameHandlersPoolManager.GetGameHandler(playerSession.ExtractGameHandlerPoolID()))
                {
                    IResponseMessage responseMessage = handler.OnGameDataMessage(new PlayerSessionGameData(playerSessionID, playerSession),requestMessage);
                    ExceptionIfResponseIsNull(playerSession, responseMessage);
                    GameResponseContext response = new GameResponseContext();
                    response.GameResponseMessage = DIFactory.ProtocolManagerEngine.SerializeMessage(playerSession, responseMessage);
                    response.PS = playerSession;
                    GameDataMessages.NormalFormat("Outgoing Game data message: {0}", response.GameResponseMessage);
                    return response;
                }
            }
            catch (PlayerSessionManagerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LogGeneral.FatalFormat("HandleGameMessage Exception while processing msg, Exception:{0}", ex.ToString());
                throw new PlayerSessionManagerException((int)enGGPJoinErrorCodes.SYSTEM_ERROR);
            }

        }

        public int HandleJackpotRequest(long playerId, ref string ErrorDesription, out Dictionary<long, long> jackpots, out PlayerSession pS)
        {
            pS = DIFactory.PlayerSessionDAL.Get(playerId.ToString(), false);
            jackpots = new Dictionary<long, long>();
            if (pS == null)
            {
                LogJackPot.WarnFormat("HandleJackpotRequest Failed to get player session ps from the storage {0}",
                    playerId);

                throw new PlayerSessionManagerException((int)enErrorCodeInfra.PlayerNotExist);
            }
            AppendLoggerEntryHeaderItems(playerId, pS);
            ServiceBridgeErrors ErrorCode = OperatorManagerInstance
                .GetOperatorEngine(pS.OperatorId, pS.JoinType)
                .GetJackpotsSums(pS.GameType, pS.GameSessionCurrencyCode, out jackpots, pS.OperatorData);
            try
            {
                LogJackPot.NormalFormat("HandleJackpotRequest  CurrencyCode {0}, ip {1}",pS.GameSessionCurrencyCode, pS.PlayerIP);
                pS.TimeStamp = DateTime.Now;
                // JackpotId, CurrencyCode, out JackpotSum);

                return (int)ErrorCode;
            }
            catch (Exception ex)
            {
                int errorCode = -1;
                int.TryParse(ex.Message, out errorCode);

                LogJackPot.ErrorFormat("HandleJackpotRequest Exception while processing msg! Exception: {0}",ex.ToString());
                throw new PlayerSessionManagerException(errorCode);
            }
        }

        public string HandleOperatorMessage(long playerId, string operationToPerform)
        {
            if (string.IsNullOrEmpty(operationToPerform))
                return string.Empty;

            PlayerSession pS = DIFactory.PlayerSessionDAL.Get(playerId.ToString(), false);
            if (pS == null)
            {
                LogGeneral.WarnFormat("HandleOperatorMessage Failed to get player session ps from the storage {0}", playerId);

                throw new PlayerSessionManagerException((int)enErrorCodeInfra.PlayerNotExist);
            } 

            AppendLoggerEntryHeaderItems(playerId, pS);

            var operatorEngine = OperatorManagerInstance.GetOperatorEngine(pS.OperatorId, pS.JoinType);
            
            if (operatorEngine == null)
                throw new ApplicationException(string.Format("HandleOperatorMessage failed! PlayerId = {0}; Could not find an operator engine for OperatorID = {1} and JoinType = {2}", playerId, pS.OperatorId, pS.JoinType));
            
            if (operationToPerform == StandardOperatorMessages.GET_BALANCE)
            {
                long playerBalance = operatorEngine.GetBankrollBalanceInCentsInSessionCurrency(pS.OperatorData, pS.GameSessionCurrencyCode);
                return playerBalance.ToString();
            }

            var response = operatorEngine.HandleSpecificOperatorMessage(pS.OperatorData, operationToPerform);

            if (response.HasError)
            {
                LogGeneral.Error(string.Format("Failed HandleOperatorMessage: playerId = {0}; OperatorId = {1}; ps.JoinType = {2}; operationToPerform = {3}; ErrorCode = {4}; ErrorDescription = {5}",
                                                playerId,
                                                pS.OperatorId,
                                                pS.JoinType,
                                                operationToPerform,
                                                response.Error.Code,
                                                response.Error.Description));
                return string.Empty;
            }
            else
            {
                return response.Data;
            }
        }

        #endregion

        #region Cleaner
        
        public void StartCleaner()
        {
            var settings = ConfigurationReader.ReadSection<SessionStorageManagerSettings>();
            if (settings.ProviderType != SessionStorageProviderTypes.Local && settings.CacheExpirationHandler != ServerConfigAdmin.GetSingleton().ServerID)
                return;

            if (this.Cleanner == null)
                this.Cleanner = DIFactory.PlayerSessionDAL.CreateCleaner();

            this.Cleanner.Start();

        }

        public void StopCleaner()
        {
            if (this.Cleanner != null)
            {
                this.Cleanner.Stop();
            }
        }

        internal void ActivateCleaner(int? expirationTO)
        {
            if (this.Cleanner != null)
            {
                this.Cleanner.ActivateCleaner(expirationTO);
            }
        }

        ISessionCleaner Cleanner { get; set; }
       
        #endregion Cleaner
        
        #region External events

        public int KickPlayerWithReason(int clientID, int operatorID, int kickReason)
        {
            int Error = 0;
            var paramList = new Dictionary<string, string>();
            paramList.Add("ClientID", clientID.ToString());
            paramList.Add("OperatorId", operatorID.ToString());
            //this call locks the open sessions
            Dictionary<string, object> openSessions = DIFactory.PlayerSessionDAL.GetOpenSessions(paramList);
            if (openSessions.Count == 0)
            {
                return 2;
            }
            foreach (var item in openSessions)
            {
                //call operator to set flags
                var Ps = item.Value as PlayerSession;
                LogGeneral.ErrorFormat("Kicking GGP player ID {0}, operator ID {1}, Game type ID {2}, kick reason {3}", item.Key,
                    Ps.OperatorId, Ps.GameType, kickReason);
                IPlayerActivityServices playerServices = OperatorManagerInstance
                    .GetOperatorEngine(operatorID, Ps.JoinType);
                if (playerServices.NeedToKickClient(Ps.OperatorData, kickReason))
                {
                    //call leave and check if succeeded
                    DIFactory.PlayerSessionDAL.Unlock(item.Key, true);
                    long GGPPlayerID = long.Parse(item.Key);
                    if (LeavePlayer(GGPPlayerID) == false)
                    {
                        Error = 1;
                    }
                }
                else
                {
                    if (DIFactory.PlayerSessionDAL.Put(item.Key, Ps) == false)
                    {
                        LogGeneral.FatalFormat("Kick player -failed to save session ps  Player ID {0}", item.Key);
                    }
                }
            }
            return Error;
        }

        internal int UnLockSession(long GGPPlayerID, out string errorDescription)
        {
            errorDescription = string.Empty;
            OptionalOut<enCacheError> cacheError = new OptionalOut<enCacheError>();
            bool success = DIFactory.PlayerSessionDAL.Unlock(GGPPlayerID.ToString(), true, cacheError);
            if (success == false)
            {
                errorDescription =
                    string.Format("ForceCloseSession, Failed to unlock player session ps from the storage player Id {0} error {1}",
                        GGPPlayerID, cacheError.Result.ToString());
            }
            return (int)cacheError.Result;
        }
        
        internal int ForceCloseSession(long GGPPlayerID, PlayerSession pS, out string errorDescription)
        {
            errorDescription = string.Empty;
            var operatorEngine = OperatorManager.GetSingleton().GetOperatorEngine(pS.OperatorId, pS.JoinType);

            if (operatorEngine == null)
            {
                errorDescription =
                    string.Format("ForceCloseSession, Failed to get Operator for player Id {0} oprator ID {1}", GGPPlayerID, pS.OperatorId);
                DIFactory.PlayerSessionDAL.Unlock(GGPPlayerID.ToString());
                return -3;
            }
            operatorEngine.ForceCloseSession(ServerConfigAdmin.GetSingleton().ServerID, pS.GameType, pS.GameWindowID, pS.GameID,
                                                                            pS.GameSessionCurrencyCode, pS.OperatorData);

            pS.IsRejoining = true;
            return 0;
        }

        internal int ForceCloseSession(long GGPPlayerID, out string errorDescription)
        {
            errorDescription = string.Empty;
            OptionalOut<enCacheError> cacheError = new OptionalOut<enCacheError>();
            var pS = DIFactory.PlayerSessionDAL.Get(GGPPlayerID.ToString(), true, cacheError);
            if (pS == null)
            {
                errorDescription =
                    string.Format("ForceCloseSession, Failed to get player session ps from the storage player Id {0} error {1}",
                        GGPPlayerID, cacheError.Result.ToString());
                LogGeneral.Warn(errorDescription);
                return (int)cacheError.Result;
            }
            AppendLoggerEntryHeaderItems(GGPPlayerID, pS);
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string playerSessionJson = JsonConvert.SerializeObject(pS, settings);
            int errCode = ForceCloseSession(GGPPlayerID, pS, out errorDescription);
            if (errCode != 0)
            {
                return errCode;
            }


            WritePlayerSessionToLog("Tool ForceCloseSession", playerSessionJson);

            if (DIFactory.PlayerSessionDAL.Remove(GGPPlayerID.ToString()) == false)
            {
                errorDescription = string.Format("ForceCloseSession- Failed to remove from Dal. player ID: {0} ",GGPPlayerID);
                LogGeneral.ErrorFormat(errorDescription);
                return -2;
            }

            return 0;
        }

        internal int AutoplaySession(long GGPPlayerID, PlayerSession pS, out string errorDescription)
        {
            errorDescription = string.Empty;
            //setting the rejoin flag to false so state machine will not send mnessage to base class
            pS.IsRejoining = false;

            if (!CompleteGame(GGPPlayerID, pS))
            {
                errorDescription = string.Format("PlayerSessionManager.AutoplaySession Failed! See previous error logs");
                LogGeneral.Error(errorDescription);

                return -3;
            }
            LogGeneral.NormalFormat("AutoplaySession succeded, session {0} will be removed!",GGPPlayerID);
            pS.IsRejoining = true;
            return 0;
        }

        internal int AutoplaySession(long GGPPlayerID, out string errorDescription)
        {
            errorDescription = string.Empty;
            OptionalOut<enCacheError> cacheError = new OptionalOut<enCacheError>();
            var pS = DIFactory.PlayerSessionDAL.Get(GGPPlayerID.ToString(), true, cacheError);
            if (pS == null)
            {
                errorDescription =
                    string.Format("PlayerSessionManager.AutoplaySession, Failed to get player session ps from the storage player Id {0}  error {1}",
                        GGPPlayerID, cacheError.Result.ToString());
                LogGeneral.Warn(errorDescription);
                return (int)cacheError.Result;
            }
            AppendLoggerEntryHeaderItems(GGPPlayerID, pS);
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string playerSessionJson = JsonConvert.SerializeObject(pS, settings);
            int errCode = AutoplaySession(GGPPlayerID, pS, out errorDescription);           
            if (errCode != 0)
	        {
                // auto play failed - session remains locked for manual handleing 
                return errCode;
	        }

            WritePlayerSessionToLog("Tool AutoplaySession", playerSessionJson);

            if (DIFactory.PlayerSessionDAL.Remove(GGPPlayerID.ToString()) == false)
            {
                errorDescription = string.Format("PlayerSessionManager.AutoplaySession - Failed to remove from Dal. player ID: {0} ",GGPPlayerID);
                LogGeneral.ErrorFormat(errorDescription);
                return -2;
            }
            return 0;
        }
        
        internal bool CloseSuspendedSession(long GGPPlayerID)
        {
            var pS = DIFactory.PlayerSessionDAL.Get(GGPPlayerID.ToString(), true);
            if (pS == null)
            {
                LogGeneral.WarnFormat("CloseSuspendedSession Failed to get player session ps from the storage player Id {0}", GGPPlayerID);
                return false; ;
            }
            AppendLoggerEntryHeaderItems(GGPPlayerID, pS);
            string erroDesription = string.Empty;
            int errCode = 0;
            try
            {
                enExpirationAction eea = OperatorManager.GetSingleton().GetOperatorEngine(pS.OperatorId, pS.JoinType).GetSuspendedExpirationAction(pS.OperatorData);

                if (eea == enExpirationAction.AutoPlay)
                {
                    errCode = AutoplaySession(GGPPlayerID, pS, out erroDesription);
                }
                else if (eea == enExpirationAction.ForceClose)
                {
                    errCode = ForceCloseSession(GGPPlayerID, pS, out erroDesription);
                }
                else
                {
                    errCode = -1;
                    erroDesription = "GetSuspendedExpirationAction did not return a valid action" + eea.ToString();
                }
            }
            catch (Exception ex)
            {
                errCode = -2;
                erroDesription = "GetSuspendedExpirationAction Exception" + ex.ToString();
            }
            finally
            {
                if (errCode == 0)
                {
                    if (DIFactory.PlayerSessionDAL.Remove(GGPPlayerID.ToString()) == false)
                    {
                        LogGeneral.ErrorFormat("CloseSuspendedSession- Failed to remove from Dal. player ID: {1} ", GGPPlayerID);
                    }
                }
                else
                {
                    LogGeneral.ErrorFormat("CloseSuspendedSession- Failed to run Action postponing error code {0}, Description {1} ", errCode, erroDesription);
                    pS.SuspendedExpirationDate = OperatorManagerInstance.GetOperatorEngine(pS.OperatorId, pS.JoinType).GetSuspendedExpirationDate(pS.OperatorData);

                    if (DIFactory.PlayerSessionDAL.Put(GGPPlayerID.ToString(), pS, false) == false)
                    {
                        LogGeneral.FatalFormat("CloseSuspendedSession - Failed to save player session game string {0}", pS.GameString);
                    }
                }
            }
            return true;
        }

        internal CloseGameSessionsResponse CloseGameSessions(int operatorID, int gametypeID, int testID)
        {
            CloseGameSessionsResponse response = new CloseGameSessionsResponse() {FailedPlayersID = new List<long>(),
                                                                                RequestID  = ServerConfigAdmin.GetSingleton().GenerateID()};
            var paramList = new Dictionary<string, string>();
            paramList.Add("OperatorId", operatorID.ToString());
            paramList.Add("GameType", gametypeID.ToString());

            AppendLoggerEntryHeaderItem("WCF request ID", response.RequestID.ToString());
            LogGeneral.Warn("Closing Game sessions");
            var gmeSessions = DIFactory.PlayerSessionDAL.GetOpenSessions(paramList, false).Where(x => ((PlayerSession)x.Value).TestID == testID);

            foreach (var session in gmeSessions)
            {
                if (((PlayerSession)session.Value).GameState == GameStateCode.None)
                {
                    if (LeavePlayer(long.Parse(session.Key)) == true)
                        response.NumberOfClosedSessions++;
                    else
                        response.FailedPlayersID.Add(long.Parse(session.Key));
                }
                else
                {
                    string errorDescription;
                    if (AutoplaySession(long.Parse(session.Key), out errorDescription) == 0)
                        response.NumberOfClosedSessions++;
                    else
                        response.FailedPlayersID.Add(long.Parse(session.Key));
                }
            }
            response.ErrorCode = response.FailedPlayersID.Count == 0;
            return response;
        }
        
        #endregion

        #region helper function

        private int ConvertCacheError(enCacheError result)
        {
            switch (result)
            {
                case enCacheError.OK:
                case enCacheError.SystemError:
                    return (int)enErrorCodeInfra.GeneralError;
                case enCacheError.SessionLocked:
                    return (int)enErrorCodeInfra.TemporaryProblem;
                case enCacheError.SeesionNotExists:
                    return (int)enErrorCodeInfra.PlayerNotExist;
                default:
                    return (int)enErrorCodeInfra.GeneralError;      
            }
        }

        private static PlayerSession CreateNewSession(JoinPlayerRequest joinPlayerRequest,OperatorAuthenticateResponse authResponse , IOperatorData operatorData)
        {
            var pS = new PlayerSession
            {
                GameType = joinPlayerRequest.GameType,
                NetworkID = authResponse.NetworkID,
                ClientID = authResponse.ClientID,
                PlayerIP = authResponse.ClientIP,
                OperatorData = operatorData,
                LanguageID = joinPlayerRequest.Language,
                DisplayName = joinPlayerRequest.DisplayName,
                GameSessionCurrencyCode = authResponse.GameCurrencyCode,
                OperatorId = joinPlayerRequest.OperatorID,
                JoinType = joinPlayerRequest.JoinType,            
                ProtocolType = joinPlayerRequest.ProtocolType,
                GameWindowID = ServerConfigAdmin.GetSingleton().GenerateTableNumber(),
                TestID = joinPlayerRequest.TestID,
                TestCaseID = joinPlayerRequest.TestCaseID
            };
            return pS;
        }

        private void HandleDRS(PlayerSession pS)
        {
            if (GameHandlersPoolManager.IsISDRequired(pS.ExtractGameHandlerPoolID())
                    && pS.OperatorData.IsISDRequired())
            {
                if (PlayerSessionDBManager.GetSingleton().IsISDEnabled)
                {
                    pS.ISD = PlayerSessionDBManager.GetSingleton().GetISD(pS.ClientID, pS.GameType, pS.OperatorId);
                }
                else
                {
                    LogGeneral.WarnFormat("Using of the ISD is disabled for current GGP server. ISD will not be stored for current game with type {0}", pS.GameType);
                }
            }
        }

        private bool TrySimulateDisconnectionInfo(PlayerSession pS)
        {
            if (ServerConfigAdmin.GetSingleton().QaMode.Enabled == false)
                return false;
            var disconnectionInfo = ServerConfigAdmin.GetSingleton().QaMode.SimulationsFactories.DisconnectionSimulationProvider.GetDisconnectionInfo(pS.ClientID, pS.GameType);
            if (disconnectionInfo != null)
            {
                LogGeneral.WarnFormat("{0}Disconnection info simulated, Game type: {1}", FormatHelperEx.FormatLogHeader(pS), pS.GameType);
                pS.GameState = disconnectionInfo.InSubGame ? GameStateCode.SubGamePlay : GameStateCode.GamePlay;
                pS.GameID = disconnectionInfo.GameId;
                pS.GameString = disconnectionInfo.GameString;

                return true;
            }
            return false;
        }

        private bool DeleteCurrentSession(PlayerSession pS)
        {
            return ServerConfigAdmin.GetSingleton().QaMode.SimulationsFactories.DisconnectionSimulationProvider.DeleteCurrentSession(pS.ClientID, pS.GameType);
        }

        private void AutoPlayer(long playerId, PlayerSession pS)
        {
            if (ShouldCompleteTheGameForThePlayer(pS))
            {
                CompleteGame(playerId, pS);
            }
        }

        private bool CompleteGame(long playerId, PlayerSession pS)
        {
            IRequestMessage requestMessage = null;

            while (pS.GameState != GameStateCode.None && !GotFatalErrorFromOperator(pS))
            {
                Thread.Sleep(100);
                try
                {
                    if (!GameSupportedByOperator(pS))
                    {
                        LogGeneral.ErrorFormat("Fail in AutoPlayer Game type is not supported by Operator");
                    }
                    pS.TimeStamp = DateTime.Now;
                    LogGeneral.NormalFormat("AutoPlayer calling GetNextRequestFromGameEngine");
                    requestMessage = GamePlayStateMachine.GetSingleton().GetNextGameMessage(playerId, pS);
                    LogAutoPlayerRequest(pS, requestMessage);

                    OperatorManagerInstance.GetOperatorEngine(pS.OperatorId, pS.JoinType)
                        .SetAutoPlayerFlag(pS.OperatorData);

                    ServerConfigAdmin.GetSingleton()
                        .QaMode.MessageInterceptor.RecordGameAutoPlayRequest(pS, playerId,
                            DIFactory.ProtocolManagerEngine.SerializeXMLMessage(pS, requestMessage));

                    IResponseMessage response = GamePlayStateMachine.GetSingleton()
                        .HandleGameMessage(playerId, pS, requestMessage);
                    LogAutoPlayerResponse(pS, response);

                    ServerConfigAdmin.GetSingleton()
                        .QaMode.MessageInterceptor.RecordGameResponse(pS, playerId,
                            DIFactory.ProtocolManagerEngine.SerializeXMLMessage(pS, response));
                }
                catch (Exception ex)
                {
                    int errorCode;
                    int.TryParse(ex.Message, out errorCode);
#warning need to verify if the game state should be None (in order to prevent it from beening Auto played again)
                    LogGeneral.FatalFormat(
                        "AutoPlayer HandleGameMessage Exception while processing msg, requestMessage:{0}, Exception:{1}",
                        requestMessage, ex.ToString());
                    return false;
                }
            }
            return !GotFatalErrorFromOperator(pS);
        }

        private bool GotFatalErrorFromOperator(PlayerSession pS)
        {
            return pS.OperatorData.IsInFatalState;
        }
      
        private bool GameSupportedByOperator(PlayerSession pS)
        {
            return OperatorManagerInstance.GetOperatorEngine(pS.OperatorId, pS.JoinType).CanHandleGameMessage(pS.OperatorData);
        }

        private bool ShouldCompleteTheGameForThePlayer(PlayerSession pS)
        {
            return OperatorManagerInstance.GetOperatorEngine(pS.OperatorId, pS.JoinType).IsAutoPlayerEnabled(pS.GameType, pS.OperatorData);
        }

        private void QaModeDeleteCurrentSession(PlayerSession pS, string GGPPlayerIDstr)
        {
            if (ServerConfigAdmin.GetSingleton().QaMode.Enabled && pS != null)
            {
                // check if there is a delete session request
                if (DeleteCurrentSession(pS))
                {
                    pS = null;
                    DIFactory.PlayerSessionDAL.Remove(GGPPlayerIDstr);
                }
            }
        }

        private static void ExceptionIfResponseIsNull(PlayerSession playerSession, IResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                WriteToLog("Game engine returned null!", string.Format("GameState: {0}", playerSession.GameState));
                WriteToLog("Game engine returned null!", string.Format("GameString: {0}", playerSession.GameString));

                if (!string.IsNullOrEmpty(playerSession.SubGameString))
                {
                    WriteToLog("Game engine returned null!", string.Format("SubGameString: {0}", playerSession.SubGameString));
                }

                throw new PlayerSessionManagerException((int)enGGPJoinErrorCodes.SYSTEM_ERROR, "Game engine failed to process the request and returned null! Possible reason: The client side game sent the wrong message. See previous message log entries related to the current game session.");

            }
        }

        private static void ValidateJoinTestData(PlayerSession pS)
        {
            int ABtestData = pS.GetTestDataAsGroup();
            if (ABtestData > 0)
            {
                if (GameHandlersPoolManager.IsGameTypeTestSupported(pS.GameType, pS.OperatorId, ABtestData) == false)
                {
                    LogGeneral.Warn($"Join - Test for game type is not supported. Test ID {pS.TestID} Test Case {pS.TestCaseID}. overriding to default");
                    pS.TestID = 0;
                    pS.TestCaseID = 0;
                }
            }
        }

        private static void ValidateRejoinTestData(PlayerSession pS)
        {
            int ABtestData = pS.GetTestDataAsGroup();
            if (ABtestData > 0)
            {
                if (GameHandlersPoolManager.IsGameTypeTestSupported(pS.GameType, pS.OperatorId, ABtestData) == false)
                {
                    throw new PlayerSessionManagerException((int)enGGPJoinErrorCodes.SYSTEM_ERROR,
                                                            $"ReJoin failed- Test for game type is not supported. Test ID {pS.TestID} Test Case {pS.TestCaseID}");
                }
            }
        }
        #endregion

        #region Session DAL

        internal PlayerSession GetSession(long playerID)
        {
            try
            {
                return DIFactory.PlayerSessionDAL.Get(playerID.ToString(), false);
            }
            catch (Exception)
            {
            }
            return null;
        }

        public IEnumerable<PlayerSession> GetOpenSessions(long clientID, int operatorId)
        {
            Dictionary<string, object> openSessionsDict = new Dictionary<string, object>();
            var paramList = new Dictionary<string, string>();
            paramList.Add("ClientID", clientID.ToString());
            paramList.Add("OperatorId", operatorId.ToString());
            openSessionsDict = DIFactory.PlayerSessionDAL.GetOpenSessions(paramList, false);
            return openSessionsDict.Values.Cast<PlayerSession>().Where(g => g.GameState != GameStateCode.None);
        }

        public PlayerSession FindSession(long clientID, int operatorId, int gameType)
        {
            PlayerSession pS = null;
            Dictionary<string, object> openSessions = SearchForSessions(clientID, operatorId, gameType,false);
            if (openSessions.Count > 0)
            {
                pS = openSessions.Values.First() as PlayerSession;
            }
            return pS;
        }

        public PlayerSession FindSessionToRejoin(long clientID, int operatorId, IOperatorData newOperatorData,
                                                IPlayerActivityServices playerServices, int gameType, out string key)
        {
            key = string.Empty;
            PlayerSession pS = null;
            Dictionary<string, object> lockedOpenSessions = SearchForSessions(clientID, operatorId, gameType);

            var OpenOperatorSessions = lockedOpenSessions.ToDictionary(x=>x.Key, x=>((PlayerSession)x.Value).OperatorData);

            List<string> OrderedPlayerIDList = playerServices.SortOpenSessions(OpenOperatorSessions);

            foreach (string item in OrderedPlayerIDList)
            {
                pS = lockedOpenSessions[item] as PlayerSession;
                
                AutoPlayer(long.Parse(item), pS);

                if (pS.GameState == GameStateCode.None)
                {
                    LogGeneral.DebugFormat("{0} JoinPlayer cleaning old session PSID {1} ", FormatHelperEx.FormatLogHeader(pS), item);
                    GamePlayStateMachine.GetSingleton().HandleLeaveGame(pS);
                    if (DIFactory.PlayerSessionDAL.Remove(item) == false)
                        LogGeneral.ErrorFormat("{0}Handle leave Player- Failed to remove from Dal. PSID: {1} ",
                                            FormatHelper.FormatLogHeader(pS.ClientID, 0, pS.GameWindowID, pS.GameID), item);
                    lockedOpenSessions.Remove(item);
                    pS = null;
                }
                else if (playerServices.RejoinSession(newOperatorData, pS.OperatorData) == false)
                {
                    LogGeneral.WarnFormat("{0} JoinPlayer ignoring old session PSID {1}", FormatHelperEx.FormatLogHeader(pS), item);
                    pS = null;
                }
                else
                {
                    lockedOpenSessions.Remove(item);
                    key = item;
                    break;
                }
            }

            foreach (var item in lockedOpenSessions)
            {
                DIFactory.PlayerSessionDAL.Unlock(item.Key);
            }

            return pS;
        }

        private Dictionary<string, object> SearchForSessions(long clientID, int operatorId, int gameType, bool locking = true)
        {
            var paramList = new Dictionary<string, string>();
            paramList.Add("ClientID", clientID.ToString());
            paramList.Add("OperatorId", operatorId.ToString());
            paramList.Add("GameType", gameType.ToString());
            //this call locks the open sessions
            return DIFactory.PlayerSessionDAL.GetOpenSessions(paramList, locking);
        }

        internal long GetCountPLayers()
        {
            return DIFactory.PlayerSessionDAL.GetCountPlayers();
        }     

        internal string GetProviderType()
        {
            return DIFactory.PlayerSessionDAL.GetProviderType();
        }

        #endregion

        #region game Message logging

        private const int MAX_MESSAGE_LENGTH_IN_THE_LOGGER = 1800; //I'm leaving 200 chars for the log header
        
        private void AppendLoggerEntryHeaderItem(string key, string value)
        {
            LoggerEntryHeader.Clear();
            var loggerEntryHeader = LoggerEntryHeader.Current;
            loggerEntryHeader.Items.Add(new LogHeaderItem(key, value));
        }

        private void AppendLoggerEntryHeaderItems(long playerID, PlayerSession playerSession)
        {
            LoggerEntryHeader.Clear();
            var loggerEntryHeader = LoggerEntryHeader.Current;
            
            //loggerEntryHeader.Items.Add(new LogHeaderItem("CIP2", playerSession.PlayerIP));
            loggerEntryHeader.Items.Add(new LogHeaderItem("GEC", playerSession.GameEventCounter.ToString()));
            loggerEntryHeader.Items.Add(new LogHeaderItem("PSID", playerID.ToString()));
            loggerEntryHeader.Items.Add(new LogHeaderItem("CID", playerSession.ClientID.ToString()));
            loggerEntryHeader.Items.Add(new LogHeaderItem("GWID", playerSession.GameWindowID.ToString()));
            loggerEntryHeader.Items.Add(new LogHeaderItem("GT", playerSession.GameType.ToString()));
            if (playerSession.SubGameType != 0)
                loggerEntryHeader.Items.Add(new LogHeaderItem("SGT", playerSession.SubGameType.ToString()));

            loggerEntryHeader.Items.Add(new LogHeaderItem("OPID", playerSession.OperatorId.ToString()));

            loggerEntryHeader.Items.AddRange(playerSession.OperatorData.GetLogHeaderEntries());

            if (playerSession.GameID != 0)
            {
                loggerEntryHeader.Items.Add(new LogHeaderItem("GID", playerSession.GameID.ToString()));
            }

            if (playerSession.TableGameCycleID != 0)
            {
                loggerEntryHeader.Items.Add(new LogHeaderItem("TGID", playerSession.TableGameCycleID.ToString()));
            }
        }
        
        public void WriteIncommingMessageToLog(PlayerSession playerSession, string gameMessage, long gGPPlayerID, string operatorData,long lastMessageTikcet = 0)
        {
            string MessageDirection = "Incoming";
            MessageDirection += lastMessageTikcet != 0 ? " TIcket: " + lastMessageTikcet.ToString() : " ";
            WriteToLog(MessageDirection, playerSession, gameMessage, gGPPlayerID);
            if (!string.IsNullOrEmpty(operatorData))
            {
                 WriteToLog("Incoming operator data: ", operatorData);
            }
        }

        public void WriteOutgoingMessageToLog(PlayerSession playerSession, string gameMessage, long gGPPlayerID, string operatorData)
        {
            string MessageDirection = "Outgoing";
            MessageDirection += playerSession.LastMessageTicket != 0 ? " TIcket: " + playerSession.LastMessageTicket.ToString() : " ";
            WriteToLog(MessageDirection,playerSession, gameMessage, gGPPlayerID);
            if (!string.IsNullOrEmpty(operatorData))
            {
                WriteToLog("Outgoing operator data: ", operatorData);
            }
        }
        
        private void WriteToLog(string messageDirection, PlayerSession playerSession, string gameMessage, long gGPPlayerID)
        {
            WriteToLog(GetLogHeader(messageDirection, playerSession, gGPPlayerID), gameMessage);
        }

        private static void WriteToLog(string header, string message)
        {
            var maxMessageLength = MAX_MESSAGE_LENGTH_IN_THE_LOGGER - header.Length;

            if (message.Length <= maxMessageLength || ConfigurationReader.MachineSettings.GGPMockEnabled)
            {
                LogGeneral.NormalFormat("{0} {1}", header, message);
                return;
            }

            var chunks = message.SplitInChunks(maxMessageLength);

            //print the first two with NormalFormat
            for (int i = 0; i <= 1; i++)
            {
                LogGeneral.NormalFormat("{0} {1}", header, chunks[i]);
            }

            //and the rest with DebugFormat
            for (int i = 2; i < chunks.Length; i++)
            {
                LogGeneral.DebugFormat("{0} {1}", header, chunks[i]);
            }
        }

        private string GetLogHeader(string messageDirection, PlayerSession playerSession, long gGPPlayerID)
        {
            if (System.ServiceModel.OperationContext.Current != null)
            {
                return string.Format("{0} {1} message:", messageDirection, GameTypeName(playerSession.GameState));
            }
            else
            {

                return string.Format("{0} {1} message: OperatorId = {2}; {3}; CID = {4}; GWID = {5}; GameId = {6}; GGPPlayerID = {7}",
                                 messageDirection,
                                 GameTypeName(playerSession.GameState),
                                 playerSession.OperatorId,
                                 GameTypeIDName(playerSession),
                                 playerSession.ClientID,
                                 playerSession.GameWindowID,
                                 playerSession.GameID,
                                 gGPPlayerID);
            }
        }

        private string GameTypeIDName(PlayerSession playerSession)
        {
            if (playerSession.GameState == GameStateCode.SubGamePlay)
                return string.Format(string.Format("SubGameTypeID = {0}; GameTypeID = {1}", playerSession.SubGameType, playerSession.GameType));
            else
                return string.Format(string.Format("GameTypeID = {0}", playerSession.GameType));
        }

        private string GameTypeName(GameStateCode gameStateCode)
        {
            if (gameStateCode != GameStateCode.SubGamePlay)
                return "game";
            else
                return "sub game";
        }

        private void LogAutoPlayerRequest(PlayerSession playerSession, IGameServerMessage requestMessage)
        {
            string request = DIFactory.ProtocolManagerEngine.SerializeXMLMessage(playerSession, requestMessage);

            LogGeneral.NormalFormat("AutoPlayer GetNextRequestFromGameEngine:{0}", request);
        }

        private void LogAutoPlayerResponse(PlayerSession playerSession, IResponseMessage response)
        {
            string strresponse = DIFactory.ProtocolManagerEngine.SerializeXMLMessage(playerSession, response);

            LogGeneral.NormalFormat("AutoPlayer ResponseFromGameEngine:{0}", strresponse);
        }

        private void WritePlayerSessionToLog(string description, string playerSessionAsString)
        {
            var loggerEntryHeader = LoggerEntryHeader.Current;
            loggerEntryHeader.Items.Add(new LogHeaderItem("Action", description));
            string[] splitplayerSessionAsString = playerSessionAsString.SplitInChunks(MAX_MESSAGE_LENGTH_IN_THE_LOGGER - 250); //header and description
            foreach (string item in splitplayerSessionAsString)
            {
                LogGeneral.Warn(item);
            }
        }

        #endregion

    }

}

namespace GGPGameServer.Core
{
    using System.Reflection;
    using System.IO;
    public static class RouletteHelper
    {
        static MethodInfo _addSessionIdMethod;
        static RouletteHelper()
        {
            var rouletteEngineAssembly = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Roulette.GameEngine.dll"));
            var rouletteRngType = rouletteEngineAssembly.GetType("Roulette.GameEngine.RouletteRng");
            _addSessionIdMethod = rouletteRngType.GetMethod("AddSessionId", BindingFlags.Public | BindingFlags.Static);
        }

        public static void MapCidToSessionId(long cid, long sessionid)
        {
            _addSessionIdMethod.Invoke(null, new object[] { sessionid });
        }
    }
}