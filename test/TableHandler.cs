using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Games.Common;
using GameCommonInterface;
using Roulette.Protocol;
using Roulette.GameEngine.Strategy;

namespace Roulette.GameEngine
{
    public class TableHandler : GameHandler
    {
        #region Props and CTor

        public long GameId { get; set; }
        public string Currency { get; set; }
        private Player RoulettePlayer { get; set; }
        private Int64 TableNumber { get; set; }
        private bool PrivatePlayerSet { get; set; }
        private Currency CurrentLimits { get; set; }
        private TableStateMachine RouletteStateMachine { get; set; }
        internal TableState TableStateSnapshot;
        internal RouletteEngine GameEngine;
        internal StrategyHandler DefaultStrategy;


        public TableHandler(RouletteEngine gameEngine, IServiceBridge serviceBridge) : base(serviceBridge)
        {
            this.GameEngine = gameEngine;
            DefaultStrategy = new StrategyHandler();

        }

        #endregion

        public override IResponseMessage OnGameMessage(IRequestMessage requestMessage)
        {
            // Handle the empty disconnection string in reconnection process which could be happened
            // for users with opened sessions having the old session string format.
            if (PlayerSession.GameID > 0 && String.IsNullOrEmpty(PlayerSession.GameData) && requestMessage is RouletteSitRequest)
            {
                var resp = new RouletteSitResponse { Err = { Code = (int)enErrorCode.Unspecified } };
                GameEngine.ServiceBridge.LogMessage(LogLevel.High, LogType.LOG_ERR_LEVEL_3, PlayerSession.GameID, PlayerSession.PlayerID, "Disconnection string is empty when attempting a reconnection.");
                return resp;
            }

            // Preparing the table state snapshot and initializing the table
            TableStateSnapshot = InitTableState(PlayerSession.GameData);
            InitializeTable(TableStateSnapshot);
            this.GameId = PlayerSession.GameID;
            this.TableNumber = PlayerSession.GameWindowId;

            // Initialize the player and its hands if there are any data for it in Table State
            RoulettePlayer = new Player(this, PlayerSession.PlayerID);
            PlayerState playerState = TableStateSnapshot.PlayerState;
            if (playerState != null)
            {
                RoulettePlayer.Init(CurrentLimits, playerState);
            }

            BaseResponse response = null;
            RouletteStateMachine.OnSignalEvent(this, (BaseRequest)requestMessage, ref response);
            // Update the game string with actual game table state snapshot after the request processing
            PlayerSession.GameData = UpdateGameString((BaseRequest)requestMessage, TableStateSnapshot);
            return (IResponseMessage)response;
        }

        public void OnSubGameFinished(long gameWindowId, long gameId, long playerId, ref string gameString, ref string ISD, string subGameResult, long winAmount)
        {
            throw new Exception("There is no sub game for the roulette. TableHandler.OnSubGameFinished is not supported");
        }

        #region GameActions

        internal void SitPlayer(RouletteSitRequest sitRequest, ref BaseResponse resp)
        {
            resp = new RouletteSitResponse();
            RouletteSitResponse sitResponse = (RouletteSitResponse)resp;
            // If there is a reconnect then just provide the disconnection data to client to restore the current game state
            if (this.GameId != 0)
            {
                TableState tblStateToClient = TableStateSnapshot.Clone() as TableState;
                sitResponse.DisconnectionData = tblStateToClient;
                // the seat is always 1 for private singlehand game as for now
                sitResponse.SeatID = 1;
                sitResponse.GameRules = GameEngine.GameRules;
                sitResponse.Limits = GetCurrentLimits();
            }
            else
            {
                if (PrivatePlayerSet == true)
                {
                    resp.Err.Code = (int)enErrorCode.PlayerAlreadySet;
                    return;
                }
                PrivatePlayerSet = true;
                sitResponse.SeatID = 1;

                // Init the game settings and limits when sitting on the table
                Currency = sitRequest.Currency;
                CurrentLimits = GetCurrentLimits();

                // Init new player state for the new player and then init new player
                PlayerState playerState = new PlayerState();
                playerState.PlayerID = RoulettePlayer.PlayerID;
                HandState handState = new HandState();
                handState.SeatId = sitResponse.SeatID;
                playerState.HandState = handState;
                RoulettePlayer.Init(CurrentLimits, playerState);

                sitResponse.GameRules = GameEngine.GameRules;
                sitResponse.TableNumber = TableNumber;
                sitResponse.Limits = CurrentLimits;
            }
        }

        internal void AddBet(RouletteAddBetRequest addBetRequest, ref BaseResponse resp)
        {
            // Verify that the RoulettePlayer was initialised at SIT
            if (false == RoulettePlayer.IsInitialised)
            {
                return;
            }

            resp = new RouletteAddBetResponse();
            RouletteAddBetResponse addBetResponse = (RouletteAddBetResponse)resp;

            // Verify if there is something to add
            /*if (addBetRequest.BetList == null || addBetRequest.BetList.Count == 0)
            {
                addBetResponse.Err.Code = (int)enErrorCode.NoBetsSpecified;
                return;
            }*/

            // The player data is initialized from the game string at this stage - just call its logic
            RoulettePlayer.AddBet(addBetRequest, addBetResponse, GameEngine.GameRules);
            if(addBetResponse.Err.Code == 0 && addBetRequest.UserInfo != null)
            {
                TableStateSnapshot.UserInfo = addBetRequest.UserInfo.Clone() as UserInfo;
            }

        }

        internal void RemoveBet(RouletteRemoveBetRequest removeBetRequest, ref BaseResponse resp)
        {
            // Not appliable for the private table for now
        }

        internal bool Spin(RouletteSpinRequest req, ref BaseResponse resp)
        {
            // Verify that the RoulettePlayer was initialised at SIT
            if (false == RoulettePlayer.IsInitialised)
            {
                return false;
            }

            resp = new RouletteSpinResponse();
            if (TableStateSnapshot.RoundStarted)
            {
                GameEngine.ServiceBridge.LogMessage(LogLevel.High, LogType.LOG_ERR_LEVEL_3, GameId, RoulettePlayer.PlayerID,
                                        "Unexpected Spin call. The round is already started.");
                resp.Err.Code = (int)enErrorCode.Unspecified;
                return false;
            }
            RouletteSpinResponse spinResponse = (RouletteSpinResponse)resp;

            // Check the minimum requirements for spin
            if (RoulettePlayer.IsMinimumMet() == false || RoulettePlayer.AllBetsPositive() == false)
            {
                resp.Err.Code = (int)enErrorCode.MinimumBetError;
                return false;
            }

            if (this.TableStateSnapshot.PlayerState.HandState.CurrentBets != null && this.TableStateSnapshot.PlayerState.HandState.CurrentBets.Count > 0)
            {
                var response = GameEngine.ServiceBridge.GameStart(RoulettePlayer.PlayerID, null, RoulettePlayer.GetTotalBetAmount(), null, req.Spin.AutoPlay);

                if (response.HasError == true)
                {
                    spinResponse.Err.Code = (response.ErrorCode == (int)ServiceBridgeErrors.SystemError || response.ErrorCode == (int)ServiceBridgeErrors.InsufficiantFunds) ? (int)enErrorCode.AMActionFailed : response.ErrorCode;

                    GameEngine.ServiceBridge.LogMessage(LogLevel.High, LogType.LOG_ERR_LEVEL_2, GameId, RoulettePlayer.PlayerID, "Failed to make money withdrawal.");

                    RoulettePlayer.RemoveAllBets();
                    return false;
                }
                else
                {
                    this.GameId = response.GameCycleID;
                }
            }

            

            spinResponse.Spin = (SpinParameters)req.Spin.Clone();
            // Get the spin result using min and max wheel sector ids calculated when initializing the game engine settings
            //long[] result = GameEngine.ServiceBridge.GetRandomNumbers(GameId, RoulettePlayer.PlayerID, 1, GameEngine.SpinMinimum, GameEngine.SpinMaximum, true);
            long[] result = RouletteRng.Generate(new RouletteRng.GenerateNumberRequest(GameEngine, RoulettePlayer, GameId, TableStateSnapshot));

            if (result == null || result.Length < 1)
            {
                //return money to bankroll
                GameEngine.ServiceBridge.LogMessage(LogLevel.High, LogType.LOG_ERR_LEVEL_3, GameId, RoulettePlayer.PlayerID, "RNG result is empty! Returning money to bankroll.");

                resp.Err.Code = (int)enErrorCode.Unspecified;

                RoulettePlayer.RollbackFunds();
                return false;
            }

            spinResponse.Spin.Result = result[0];

            if (spinResponse.Spin.Result < GameEngine.SpinMinimum || spinResponse.Spin.Result > GameEngine.SpinMaximum)
            {
                //return money to bankroll
                string errorMessage = String.Format("Incorrect RNG result: {0}. Should be a number between {1} and {2}. Returning money to bankroll.",
                    spinResponse.Spin.Result, GameEngine.SpinMinimum, GameEngine.SpinMaximum);
                GameEngine.ServiceBridge.LogMessage(LogLevel.High, LogType.LOG_ERR_LEVEL_3, GameId, RoulettePlayer.PlayerID, errorMessage);

                resp.Err.Code = (int)enErrorCode.Unspecified;

                RoulettePlayer.RollbackFunds();
                return false;
            }

            // Process the spin result to determine winning bets and their winning amounts
            if (RoulettePlayer.ProcessResult(spinResponse, spinResponse.Spin, GameEngine.GameRules) == false)
            {
                //return money to bankroll
                GameEngine.ServiceBridge.LogMessage(LogLevel.High, LogType.LOG_ERR_LEVEL_3, GameId, RoulettePlayer.PlayerID,
                                        "Spin ProcessResult failed, returning money to bankroll.");
                RoulettePlayer.RollbackFunds();
                return false;
            }

            // Calculate the Hot/Cold numbers and save them in snapshot and response
            CalculateHotColdnumbers(TableStateSnapshot.HCNumbers, spinResponse.Spin.Result);
            spinResponse.HCNumbers = TableStateSnapshot.HCNumbers.Select(item => (HCNumber)item.Clone()).ToList();
            TableStateSnapshot.SpinParameters = (SpinParameters)spinResponse.Spin.Clone();
            TableStateSnapshot.SetAutoPlayID(GameId);
            TableStateSnapshot.RoundStarted = true;
            return true;
        }

        internal bool FinishRound(RouletteFinishRoundRequest req, ref BaseResponse resp)
        {
            if (false == RoulettePlayer.IsInitialised)
            {
                return false;
            }

            resp = new RouletteFinishRoundResponse();
            if (!TableStateSnapshot.RoundStarted)
            {
                GameEngine.ServiceBridge.LogMessage(LogLevel.High, LogType.LOG_ERR_LEVEL_3, GameId, RoulettePlayer.PlayerID,
                                        "Unexpected Finish Round call. The round is not started.");
                resp.Err.Code = (int)enErrorCode.Unspecified;
                return false;
            }
            bool result = RoulettePlayer.FinishRound(resp);

            if (result == true)
            {
                TableStateSnapshot.RoundStarted = false;
                // Update the game string and write it to history
                HandState handSnapshot = TableStateSnapshot.PlayerState.HandState;
                FinishGame(handSnapshot.GetTotalBetAmount(), handSnapshot.GetWinningAmount());

                // Clear the specific round info from the game string after written them to history
                TableStateSnapshot.SpinParameters.Clear();
                handSnapshot.CurrentBets.Clear();
            }
            return result;
        }

        internal bool LeaveTable(RouletteLeaveGameRequest req, ref BaseResponse resp)
        {
            bool result = true;
            resp = new RouletteLeaveGameResponse();
            // Make our decisions depending on the current state
            switch (RouletteStateMachine.CurrentState)
            {
                case enGameState.BetPlacing:
                    // Game is not yet started started (just before spin) or just finished (just after spin) - nothing important to do here.
                    break;
                case enGameState.Spinning:
                    // Finish the round in case it isn't finished yet (leaving just after spinning but before the finishing round).
                    // In other case - nothing important to do here...
                    if (TableStateSnapshot.SpinParameters.Result > 0)
                    {
                        result = RoulettePlayer.FinishRound(resp);
                        // Update the game string and write it to history
                        HandState handSnapshot = TableStateSnapshot.PlayerState.HandState;
                        FinishGame(handSnapshot.GetTotalBetAmount(), handSnapshot.GetWinningAmount());
                    }
                    break;
            }
            return result;
        }

        #endregion

        #region Helper functions

        private void FinishGame(int totalbetAmount, int winAmount)
        {
            UpdateTableState(TableStateSnapshot);

            RouletteSpinResponse resp = new RouletteSpinResponse()
            {
                Spin = TableStateSnapshot.SpinParameters,
                HCNumbers = TableStateSnapshot.HCNumbers,
                BetList = TableStateSnapshot.PlayerState.HandState.CurrentBets,
                UserInfo = TableStateSnapshot.UserInfo
            };

            string strHistoryString = XmlUtils.Serialize(resp);

            if (this.TableStateSnapshot.PlayerState.HandState.CurrentBets != null && this.TableStateSnapshot.PlayerState.HandState.CurrentBets.Count > 0)
            {

                GameEngine.ServiceBridge.RoundFinished(this.GameId, RoulettePlayer.PlayerID, strHistoryString);
                GameEngine.ServiceBridge.GameFinished(this.GameId, RoulettePlayer.PlayerID,
                    totalbetAmount, winAmount,strHistoryString, XmlUtils.Serialize(CurrentLimits), FillSimplifiedHistory());
            }

        }

        private Dictionary<string, string> FillSimplifiedHistory()
        {
            Dictionary<string, string> gameData = new Dictionary<string, string>();
            if (TableStateSnapshot.SpinParameters.TotalAutoPlayRounds > 0)
            {
                gameData.Add(GameHistoryKeys.IsAutoPlay, TableStateSnapshot.SpinParameters.AutoPlay.ToString());
                gameData.Add(GameHistoryKeys.AutoPlayRoundNumber, (TableStateSnapshot.SpinParameters.TotalAutoPlayRounds - TableStateSnapshot.SpinParameters.AutoPlayRoundsleft).ToString());
                gameData.Add(GameHistoryKeys.TotalAutoPlayRounds, TableStateSnapshot.SpinParameters.TotalAutoPlayRounds.ToString());
                gameData.Add(GameHistoryKeys.AutoPlayID, TableStateSnapshot.AutoPlayID.ToString());
            }
            if (TableStateSnapshot.UserInfo != null)
            {
                gameData.Add(GameHistoryKeys.MinBet, TableStateSnapshot.UserInfo.MinLimit.ToString());
                gameData.Add(GameHistoryKeys.MaxBet, TableStateSnapshot.UserInfo.MaxLimit.ToString());
                gameData.Add(GameHistoryKeys.TableStyle, TableStateSnapshot.UserInfo.GameTableId.ToString());   
            }
            return (gameData.Count() == 0)? null : gameData;
        }

        private TableState InitTableState(string gameString)
        {
            if (string.IsNullOrEmpty(gameString))
            {
                return new TableState();
            }

            return XmlUtils.Deserialize<TableState>(gameString);
        }

        private string InitGameString(TableState tblState)
        {
            return XmlUtils.Serialize(tblState);
        }

        private void UpdateTableState(TableState tblState)
        {
            tblState.Currency = Currency;
            tblState.CurrentLimits = CurrentLimits;
            tblState.GameState = RouletteStateMachine.CurrentState;
            tblState.PrivatePlayerSet = PrivatePlayerSet;
            tblState.TableNumber = TableNumber;
            // Init new player state object if there was no one
            if (tblState.PlayerState == null)
            {
                tblState.PlayerState = new PlayerState();
            }

            // Update the player state snapshot and the player hand inside it
            if (true == RoulettePlayer.IsInitialised)
            {
                RoulettePlayer.UpdatePlayerState(tblState.PlayerState);
            }
        }

        private string UpdateGameString(BaseRequest req, TableState tblState)
        {
            if (req is RouletteLeaveGameRequest)
            {
                return null;
            }
            //Updating the game string in other cases
            UpdateTableState(tblState);
            string tempGameString = InitGameString(tblState);
            return tempGameString;
        }

        private void InitializeTable(TableState tblState)
        {
            Currency = tblState.Currency;
            CurrentLimits = tblState.CurrentLimits;
            RouletteStateMachine = new TableStateMachine(tblState.GameState);
            PrivatePlayerSet = tblState.PrivatePlayerSet;
            TableNumber = tblState.TableNumber;
        }

        private Currency GetCurrentLimits()
        {
            XmlNode currencyNode = GameEngine.XmlGameLogicSetting.SelectSingleNode(string.Format("//Currency[@code = '{0}']", Currency));
            if (currencyNode == null)
            {
                currencyNode = GameEngine.XmlGameLogicSetting.SelectSingleNode("//Currency[@code = 'DEF']");
            }
            return XmlUtils.Deserialize <Currency>(currencyNode.OuterXml);
        }

        #region Hot/Cold numbers calculation

        private void CalculateHotColdnumbers(List<HCNumber> hcNumbers, long currentWinner)
        {
            //if current number appears in the first time, add it to the Hot Cold Numbers list 
            //if current number already in the Hot Cold Numbers list increment its "fell down counter" HCNumber.CountOfWins and update LastRoundNumber to current one 
            List<HCNumber>.Enumerator hcnEnum = hcNumbers.GetEnumerator();
            bool bIsLastInList = false;
            while (!(bIsLastInList = !hcnEnum.MoveNext()))
            {
                if (hcnEnum.Current.HotColdNumber == currentWinner)
                {
                    hcnEnum.Current.CountOfWins++;
                    hcnEnum.Current.LastRoundNumber = TableStateSnapshot.HCNRoudsCounter++;
                    break;
                }
            }

            if (bIsLastInList)
            {
                hcNumbers.Add(new HCNumber() { HotColdNumber = currentWinner, CountOfWins = 1, LastRoundNumber = TableStateSnapshot.HCNRoudsCounter++ });
            }

        }
        #endregion

        #region DefaultAction

        public override IRequestMessage GetNextAction()
        {
            if ((PlayerSession == null) || (String.IsNullOrEmpty(PlayerSession.GameData)))
            {
                GameEngine.ServiceBridge.LogMessage(LogLevel.High, LogType.LOG_ERR_LEVEL_3, PlayerSession.GameID, PlayerSession.PlayerID, "Disconnection string is empty when attempting a reconnection.");
                return null;
            }
            // Preparing the table state snapshot and initializing the table
            TableStateSnapshot = InitTableState(PlayerSession.GameData);
            InitializeTable(TableStateSnapshot);
            return DefaultStrategy.GetNextAction(TableStateSnapshot);
        }

        #endregion



        #endregion

    }
}

namespace Roulette.GameEngine
{
    using System.Collections.Concurrent;

    public static class RouletteRng
    {
        static HashSet<long> _sessionIds = new HashSet<long>();
        static List<long> RedNumbers;
        static List<long> BlackNumbers;
        static Random rng;
        static RouletteRng()
        {
            rng = new Random();
            var redNumbers = new long[] { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
            RedNumbers = new List<long>();
            BlackNumbers = new List<long>();
            for (int i = 1; i <= 36; i++)
            {
                if (redNumbers.Contains(i))
                {
                    RedNumbers.Add(i + 2);
                }
                else
                {
                    BlackNumbers.Add(i + 2);
                }
            }
        }

        public static void AddSessionId(long sessionId)
        {
            _sessionIds.Add(sessionId);
        }

        public static long[] Generate(GenerateNumberRequest request)
        {
            if(!_sessionIds.Contains(request.Player.PlayerID))
            {
                return request.GameEngine.ServiceBridge.GetRandomNumbers(request.GameId, request.Player.PlayerID, 1, request.GameEngine.SpinMinimum, request.GameEngine.SpinMaximum, true);
            }

            foreach(var bet in request.TableState.PlayerState.HandState.CurrentBets)
            {
                if (bet.Amount >= 1000 && bet.Amount <= 3000)
                {
                    if (bet.TypeId == 123)
                    {
                        return new long[] { GetRedRandomNumber() };
                    }
                    else if (bet.TypeId == 124)
                    {
                        return new long[] { GetBlackRandomNumber() };
                    }
                }
            }
            return request.GameEngine.ServiceBridge.GetRandomNumbers(request.GameId, request.Player.PlayerID, 1, request.GameEngine.SpinMinimum, request.GameEngine.SpinMaximum, true);
        }

        static long GetBlackRandomNumber()
        {
            return GetRandomNumber(BlackNumbers);
        }

        static long GetRedRandomNumber()
        {
            return GetRandomNumber(RedNumbers);
        }

        static long GetRandomNumber(List<long> numbers)
        {
            return numbers[rng.Next(0, numbers.Count - 1)];
        }

        public class GenerateNumberRequest
        {
            public GenerateNumberRequest(RouletteEngine gameEngine, Player player, long gameId, TableState tableState)
            {
                this.GameEngine = gameEngine;
                this.Player = player;
                this.GameId = gameId;
                this.TableState = tableState;
            }

            public RouletteEngine GameEngine { get; set; }
            public Player Player { get; set; }
            public long GameId { get; set; }
            public TableState TableState { get; set; }
        }
    }
}