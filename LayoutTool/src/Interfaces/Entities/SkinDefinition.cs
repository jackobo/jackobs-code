using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LayoutTool.Interfaces.Entities
{
    
    [XmlRoot("skinDefinition")]
    public class SkinDefinition
    {
        public SkinDefinition()
        {
            
        }
              
        

        [XmlElement("skinContent")]
        public SkinContent SkinContent { get; set; }
        
    }

    

    public class Trigger
    {
        public Trigger()
        {
            InitializeCollections();
        }

      

        public Trigger(string name, int priority)
        {
            Name = name;
            Priority = priority;
            InitializeCollections();
        }

        private void InitializeCollections()
        {
            Actions = new TriggerActionCollection();
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("priority")]
        public int Priority { get; set; }

        [XmlArray(ElementName = "actions")]
        [XmlArrayItem(ElementName = "action")]
        public TriggerActionCollection Actions { get; set; }
    }


    public class TriggerActionCollection : List<TriggerAction>
    {
        
    }

    public class TriggerAction
    {

        public static string[] POSSIBLE_ACTION_NAMES
        {
            get
            {
                return PlayerStatusType.All.Where(ps => ps.IsDynamicLayout).Select(ps => ps.ActionName).ToArray();
            }
        }

        public TriggerAction()
        {
            InitializeCollections();
        }

     

        public TriggerAction(string name)
        {
            Conditions = new ConditionCollection();
            this.Name = name;
            InitializeCollections();
        }

        private void InitializeCollections()
        {
            Conditions = new ConditionCollection();
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlArray(ElementName = "conditions")]
        [XmlArrayItem(ElementName = "condition")]
        public ConditionCollection Conditions { get; set; }


    }


    public class Condition
    {
        

        public Condition()
        {
            InitializeCollections();
        }

      

        public Condition(string updateType, string type, string equationType)
        {
            this.UpdateType = updateType;
            this.Type = type;
            this.EquationType = equationType;
            InitializeCollections();
        }

        private void InitializeCollections()
        {
            Values = new ConditionValueCollection();
        }

        [XmlAttribute("updateType")]
        public string UpdateType { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("equationType")]
        public string EquationType { get; set; }

        [XmlArray(ElementName = "values")]
        [XmlArrayItem(ElementName = "value")]
        public ConditionValueCollection Values { get; set; }
    }

    public class ConditionCollection : List<Condition>
    {

    }

    public class ConditionValue 
    {
        public ConditionValue()
        {

        }

        public ConditionValue(string value)
        {
            Value = value;
        }

        [XmlAttribute("value")]
        public string Value { get; set; }
        
    }

    public class ConditionValueCollection : List<ConditionValue>
    {

    }

    public class TriggerCollection : List<Trigger>
    {

    }
    
    
    public class SkinContent
    {
        public SkinContent()
        {
            InitCollections();
        }

        private void InitCollections()
        {
            this.TopGames = new GameGroupLayoutCollection();
            this.VipTopGames = new GameGroupLayoutCollection();
            this.Arenas = new ArenaCollection();
            this.Lobbies = new LobbyCollection();
            
            this.MyAccountLobby = new MyAccountItemCollection();
            this.MyAccountHistory = new MyAccountItemCollection();

            this.Triggers = new TriggerCollection();
        }

        [XmlArray(ElementName = "topGames")]
        [XmlArrayItem(ElementName = "layout")]
        public GameGroupLayoutCollection TopGames { get; set; }

        [XmlArray(ElementName = "vipTopGames")]
        [XmlArrayItem(ElementName = "layout")]
        public GameGroupLayoutCollection VipTopGames { get; set; }

        [XmlArray(ElementName = "arenas")]
        [XmlArrayItem(ElementName = "arena")]
        public ArenaCollection Arenas { get; set; }

        [XmlArray(ElementName = "lobbies")]
        [XmlArrayItem(ElementName = "lobby")]
        public LobbyCollection Lobbies { get; set; }

       
        [XmlArray("myAccountLobby")]
        [XmlArrayItem("item")]
        public MyAccountItemCollection MyAccountLobby { get; set; }

        [XmlArray("myAccountHistory")]
        [XmlArrayItem("item")]
        public MyAccountItemCollection MyAccountHistory { get; set; }

        [XmlArray("triggers")]
        [XmlArrayItem("trigger")]
        public TriggerCollection Triggers { get; set; }
    }


    public class GameGroupLayoutCollection : List<GameGroupLayout>
    {

    }

    public class GameGroupLayout
    {
        public GameGroupLayout()
        {
            InitCollections();
        }
        public GameGroupLayout(string playerStatus)
        {
            InitCollections();

            this.PlayerStatus = playerStatus;
        }

        private void InitCollections()
        {
            this.Games = new GameCollection();
        }

        private string _playerStatus;
        [XmlAttribute("playerStatus")]
        [DefaultValue("")]
        public string PlayerStatus
        {
            get { return (_playerStatus ?? string.Empty).Trim(); }
            set
            {
                _playerStatus = value;
            }
        }

        [XmlArray(ElementName = "games")]
        [XmlArrayItem(ElementName = "game")]
        public GameCollection Games { get; set; }

        

    }
    
    public class MyAccountItem
    {
        public MyAccountItem()
        {
                
        }

        public MyAccountItem(string id, string name, IEnumerable<AttributeValue> attributes)
        {
            Id = id;
            this.Name = name;
            this.Attributes = new AttributeValueCollection(attributes);
        }

        [XmlAttribute ("id")]
        public string Id { get; set; }


        [XmlAttribute("name")]
        
        public string Name { get; set; }

        [XmlArray(ElementName = "attributes")]
        [XmlArrayItem(ElementName = "attribute")]
        public AttributeValueCollection Attributes { get; set; }



    }

    public class MyAccountItemCollection : List<MyAccountItem>
    {
        
    }

    public class GameCollection : List<Game>
    {
        public GameCollection()
        {

        }

        public GameCollection(IEnumerable<Game> games)
            : base(games)
        {

        }
    }

    public class ArenaGameCollection : List<ArenaGame>
    {
        
    }

    public class Lobby
    {
        public Lobby()
        {
            InitCollections();
        }

        public Lobby(int favoriteSize, string playerStatus)
        {
            this.PlayerStatus = playerStatus;
            this.FavoriteSize = favoriteSize;
            InitCollections();
            
        }


        
        private void InitCollections()
        {
            this.Items = new LobbyItemCollection();
        }

        [XmlAttribute("favoriteSize")]
        public int FavoriteSize { get; set; }

        private string _playerStatus;
        [XmlAttribute("playerStatus")]
        [DefaultValue("")]
        public string PlayerStatus
        {
            get { return (_playerStatus ?? string.Empty).Trim(); }
            set
            {
                _playerStatus = value;
            }
        }

        [XmlArray(ElementName = "items")]
        [XmlArrayItem(ElementName = "item")]
        public LobbyItemCollection Items { get; set; }


    }


    public class LobbyCollection : List<Lobby>
    {

    }

    public class LobbyItem
    {
        public LobbyItem()
        {

        }

        public LobbyItem(int id, bool jackpotVisible)
        {
            this.Id = id;
            this.JackpotVisible = jackpotVisible;
        }


        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("jackpotVisible")]
        [DefaultValue(false)]
        public bool JackpotVisible { get; set; }
    }

    public class LobbyItemCollection : List<LobbyItem>
    {

    }

    public class ArenaGame
    {
        public ArenaGame()
        {
            UserMode = UserModes.Both;
        }

        public ArenaGame(int gameType, string name, bool newGame, int userMode)
        {
            GameType = gameType;
            Name = name;
            NewGame = newGame;
            UserMode = userMode;
        }

        [XmlAttribute("gameType")]
        public int GameType { get; set; }

        [XmlAttribute("gameName")]
        public string Name { get; set; }

        [XmlAttribute("newGame")]
        [DefaultValue(false)]
        public bool NewGame { get; set; }

        [XmlAttribute("userMode")]
        [DefaultValue(3)]
        public int UserMode { get; set; }

        public override bool Equals(object obj)
        {
            var theOther = obj as ArenaGame;

            if (theOther == null)
                return false;

            return this.GameType == theOther.GameType
                    && this.NewGame == theOther.NewGame
                    && this.UserMode == theOther.UserMode;

        }

        public override string ToString()
        {
            return this.Name + " [" + this.GameType + "]";
        }

        public override int GetHashCode()
        {
            return this.GameType.GetHashCode();
        }

        public static string GameTypeToRect(int gameType)
        {
            if (gameType < 5000)
                return "GAME_RECT";

            switch (gameType.ToString().Substring(0, 3))
            {
                case "130":
                    return "MODERN_GAME_RECT";
                case "201":
                    return "LIVEDEALER_RECT";
                case "231":
                    return "NETENT_GAME_RECT";
                case "232":
                    return "WI_GAME_RECT";
                case "233":
                    return "PLAYTECH_GAME_RECT";
                case "300":
                    return "GAME_RECT_WIDE";
                default:
                    return "";
            }
        }

       
    }

    public class Arena
    {
        public Arena()
        {
            InitCollections();
        }
        public Arena(int type, string name, bool isNewGameArena)
        {
            this.Type = type;
            this.Name = name;
            this.IsNewGameArena = isNewGameArena;
            InitCollections();
        }

        private void InitCollections()
        {
            this.Layouts = new ArenaLayoutCollection();
        }

        [XmlAttribute("type")]
        public int Type { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("newGame")]
        public bool IsNewGameArena { get; set; }

        [XmlArray(ElementName = "layouts")]
        [XmlArrayItem(ElementName = "layout")]
        public ArenaLayoutCollection Layouts { get; set; }

  
        public override bool Equals(object obj)
        {
            var theOther = obj as Arena;

            if (theOther == null)
                return false;

            return this.Type == theOther.Type;
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

    }

  
    public class AttributeValue
    {
        public AttributeValue()
        {

        }
        public AttributeValue(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }


        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    public class AttributeValueCollection : List<AttributeValue>
    {
        public AttributeValueCollection()
        {

        }

        public AttributeValueCollection(IEnumerable<AttributeValue> attributes)
            : base(attributes)
        {

        }

        public AttributeValueCollection Clone()
        {
            return new AttributeValueCollection(this.Select(a => new AttributeValue(a.Name, a.Value)));
        }

        public AttributeValue FindAttribute(string name)
        {
            return this.FirstOrDefault(a => string.Compare(a.Name, name, true) == 0);
        }

        public T GetAttributeValue<T>(string name)
        {
            return GetAttributeValue<T>(name, default(T));
        }

        public T GetAttributeValue<T>(string name, T defaultIfMissing)
        {
            var attribute = FindAttribute(name);
            if (attribute == null)
                return defaultIfMissing;
                        
            var converter = TypeDescriptor.GetConverter(typeof(T));

            return (T)converter.ConvertFromInvariantString(attribute.Value);
        }

        public void SetAttributeValue(string name, string value)
        {
            var attribute = FindAttribute(name);
            if(attribute == null)
            {
                attribute = new AttributeValue(name, value?.ToString());
                this.Add(attribute);
            }
            else
            {
                attribute.Value = value?.ToString();
            }
        }

    }

    public class ArenaLayout
    {
        public static readonly string JPVisibleAttributeName = "JPVisible";

        public ArenaLayout()
        {
            InitCollections();
        }

        public ArenaLayout(string playerStatus, IEnumerable<AttributeValue> attributes)
        {

            this.PlayerStatus = playerStatus;

            InitCollections();

            Attributes = new AttributeValueCollection(attributes ?? new AttributeValue[0]);
        }

        

        private void InitCollections()
        {
            this.DataGridInfo = new ArenaGridColumnCollection();
            this.FilteringInfo = new FilterCollection();
            this.AlsoPlayingGames = new GameCollection();
            this.Games = new ArenaGameCollection();
        }


        [XmlArray("attributes")]
        [XmlArrayItem("attribute")]
        public AttributeValueCollection Attributes { get; set; }
        
        
        private string _playerStatus;
        [XmlAttribute("playerStatus")]
        [DefaultValue("")]
        public string PlayerStatus
        {
            get { return (_playerStatus ?? string.Empty).Trim(); }
            set
            {
                _playerStatus = value;
            }
        }

        
        public bool JackpotVisible
        {
            get
            {
                var jpVisibleAttribute = Attributes.FindAttribute(JPVisibleAttributeName);
                if (jpVisibleAttribute == null)
                    return false;

                return bool.Parse(jpVisibleAttribute.Value);
            }
        }


        [XmlArray("dataGridInfo")]
        [XmlArrayItem("column")]
        public ArenaGridColumnCollection DataGridInfo { get; set; }

        [XmlArray("filteringInfo")]
        [XmlArrayItem("filter")]
        public FilterCollection FilteringInfo { get; set; }

        [XmlArray("alsoPlayingGames")]
        [XmlArrayItem("game")]
        public GameCollection AlsoPlayingGames { get; set; }

        [XmlArray("games")]
        [XmlArrayItem("game")]
        public ArenaGameCollection Games { get; set; }
        
    }


    public class ArenaLayoutCollection : List<ArenaLayout>
    {

    }

    public class ArenaCollection : List<Arena>
    {
        public bool IsJackpotVisible(int arenaTypeId, string playerStatus)
        {
            var arena = this.FirstOrDefault(a => a.Type == arenaTypeId);
            if (arena == null)
                return false;


            ArenaLayout arenaLayout = null;

            if(string.IsNullOrEmpty(playerStatus))
            {
                arenaLayout = arena.Layouts.FirstOrDefault(l => string.IsNullOrEmpty(l.PlayerStatus));
            }
            else
            {
                arenaLayout = arena.Layouts.FirstOrDefault(l => l.PlayerStatus == playerStatus);
            }

            if (arenaLayout == null)
                return false;

            return arenaLayout.JackpotVisible;
        }
    }


    public class Filter
    {
        public Filter()
        {
            InititalizeCollections();
        }

        public Filter(string id, string name, IEnumerable<AttributeValue> attributes)
        {
            this.Id = id;
            this.Name = name;

            InititalizeCollections();

            foreach (var a in (attributes ?? new AttributeValue[0]))
                Attributes.Add(a);
        }


        public Filter(string id, string name, params AttributeValue[] attributes)
            : this(id, name, (IEnumerable<AttributeValue>)attributes)
        {
            
        }



        private void InititalizeCollections()
        {
            Attributes = new AttributeValueCollection();
        }

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlArray(ElementName = "attributes")]
        [XmlArrayItem(ElementName = "attribute")]
        public AttributeValueCollection Attributes { get; set; }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as Filter;

            if (theOther == null)
                return false;

            return this.Id == theOther.Id;
        }

        public override string ToString()
        {
            return this.Id;
        }
    }


    public class FilterCollection : List<Filter>
    {
        public FilterCollection()
        {

        }

        public FilterCollection(IEnumerable<Filter> filters)
            : base(filters)
        {

        }
        
    }

    public class ArenaGridColumn
    {
        public ArenaGridColumn()
        {
            this.Attributes = new AttributeValueCollection();
        }

        public ArenaGridColumn(IEnumerable<AttributeValue> attributes)
        {
            this.Attributes = new AttributeValueCollection(attributes ?? new AttributeValue[0]);
        }

        [XmlArray(ElementName = "attributes")]
        [XmlArrayItem(ElementName = "attribute")]
        public AttributeValueCollection Attributes { get; set; }

        public ArenaGridColumn Clone()
        {
            return new ArenaGridColumn(Attributes.Clone());
        }
    }

    public class ArenaGridColumnCollection : List<ArenaGridColumn>
    {
        public ArenaGridColumnCollection()
        {

        }

        public ArenaGridColumnCollection(IEnumerable<ArenaGridColumn> columns)
            : base(columns)
        {

        }
        public ArenaGridColumnCollection Clone()
        {
            return new ArenaGridColumnCollection(this.Select(c => c.Clone()));
        }
    }
}
