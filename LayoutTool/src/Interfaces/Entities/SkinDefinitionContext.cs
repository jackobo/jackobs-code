using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LayoutTool.Interfaces.Entities
{

    public class ArenaType
    {

        public ArenaType()
        {

        }
        public ArenaType(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        //public static readonly int NewGamesArenaType = 1005;
    }

    public class ArenaTypeCollection : List<ArenaType>
    {
        public ArenaTypeCollection()
        {

        }

        public ArenaTypeCollection(IEnumerable<ArenaType> arenaTypes)
            : base(arenaTypes)
        {

        }
    }


    public class SkinIndentity
    {
        public SkinIndentity()
        {

        }

        public SkinIndentity(SkinSelectorIdentity selector, string clientVersion, int brandId, string brandName, int skinId, string skinName, ABTestIdentity abTestCase)
        {
            Selector = selector;
            ClientVersion = clientVersion;
            BrandId = brandId;
            BrandName = brandName;
            SkinId = skinId;
            SkinName = skinName;
            ABTestCase = abTestCase;
        }

        [XmlAttribute("clientVersion")]
        public string ClientVersion { get; set; }
        [XmlAttribute("brandId")]
        public int BrandId { get; set; }
        [XmlAttribute("brandName")]
        public string BrandName { get; set; }
        [XmlAttribute("skinId")]
        public int SkinId { get; set; }
        [XmlAttribute("skinName")]
        public string SkinName { get; set; }
        [XmlElement("abTestCaseIdentity")]
        public ABTestIdentity ABTestCase { get; set; }
        [XmlElement("selectorIdentity")]
        public SkinSelectorIdentity Selector { get; set; }

        public override string ToString()
        {
            if (ABTestCase != null)
            {
                return $"{Selector.Description} | Brand: {BrandId} - {BrandName} | Skin: {SkinId} - {SkinName} | Test case: {ABTestCase.TestCaseName}";
            }
            else
            {
                return $"{Selector.Description} | Brand: {BrandId} - {BrandName} | Skin: {SkinId} - {SkinName}";
            }
        }
    }

    public static class WellKnownSkinSourcesIds
    {
        public static readonly Guid LocalIIS = new Guid("022B858B-A49D-4C85-A575-DA6A8A8A6287");
        public static readonly Guid Production = new Guid("0A6B2DA3-AF86-485B-9BDA-7675ED1F8013");
        public static readonly Guid QA = new Guid("C2D2C587-88E0-44AA-8F85-289B7D8C3282");
    }

    public class SkinSelectorIdentity
    {
        public SkinSelectorIdentity()
        {

        }

        public SkinSelectorIdentity(Guid id, string description, IEnumerable<SkinSelectorProperty> properties)
        {
            Id = id;
            Description = description;
            Properties = properties?.ToList();
        }
        [XmlAttribute("id")]
        public Guid Id { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }


        [XmlArray(ElementName = "properties")]
        [XmlArrayItem(ElementName = "property")]
        public List<SkinSelectorProperty> Properties { get; set; }


        public override string ToString()
        {
            return Description;
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as SkinSelectorIdentity;

            if (theOther == null)
                return false;

            return this.Id == theOther.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public class SkinSelectorProperty
        {
            public SkinSelectorProperty()
            {

            }

            public SkinSelectorProperty(string name, object value)
            {
                this.Name = name;
                this.Value = value?.ToString();
            }
            [XmlAttribute("name")]
            public string Name { get; set; }
            [XmlAttribute("value")]
            public string Value { get; set; }
        }
    }

    public class ABTestIdentity
    {
        public ABTestIdentity()
        {

        }

        public ABTestIdentity(string testCaseSetId, string brand, string skin, string language, string testCaseId, string testCaseName)
        {
            this.TestCaseSetId = testCaseSetId;
            this.Brand = brand;
            this.Skin = skin;
            this.Language = language;
            this.TestCaseId = testCaseId;
            this.TestCaseName = testCaseName;
        }

        [XmlAttribute("testCaseSetId")]
        public string TestCaseSetId { get; set; }
        [XmlAttribute("brand")]
        public string Brand { get; set; }
        [XmlAttribute("skin")]
        public string Skin { get; set; }
        [XmlAttribute("language")]
        public string Language { get; set; }
        [XmlAttribute("testCaseId")]
        public string TestCaseId { get; set; }
        [XmlAttribute("testCaseName")]
        public string TestCaseName { get; set; }
    }


    [XmlRoot("skinDefinitionContext")]
    public class SkinDefinitionContext
    {
        public SkinDefinitionContext()
        {
            this.AvailableGames = new GameCollection();
            this.AvailableFilters = new FilterCollection();
            this.AvailableArenaTypes = new ArenaTypeCollection();
            this.Errors = new ErrorMessageCollection();
        }

        [XmlElement("publisher")]
        [DefaultValue("")]
        public string Publisher { get; set; }

        [XmlElement("sourceSkin")]
        public SkinIndentity SourceSkin { get; set; }

        [XmlElement("destinationSkin")]
        public SkinIndentity DestinationSkin { get; set; }

        [XmlElement("skinDefinition")]
        public SkinDefinition SkinDefinition { get; set; }

        [XmlArray(ElementName = "errors")]
        [XmlArrayItem(ElementName = "error")]
        public ErrorMessageCollection Errors { get; set; }

        [XmlArray(ElementName = "availableGames")]
        [XmlArrayItem(ElementName = "game")]
        public GameCollection AvailableGames { get; set; }

        [XmlArray(ElementName = "availableArenaTypes")]
        [XmlArrayItem(ElementName = "arenaType")]
        public ArenaTypeCollection AvailableArenaTypes { get; set; }

        [XmlArray(ElementName = "availableFilters")]
        [XmlArrayItem(ElementName = "filter")]
        public FilterCollection AvailableFilters { get; set; }

        

        public Game GetGame(int gameType)
        {
            return this.AvailableGames.FirstOrDefault(g => g.Id == gameType);
        }


        public ArenaType GetArenaType(int id)
        {
            var arenaType = this.AvailableArenaTypes.FirstOrDefault(a => a.Id == id);

            if (arenaType != null)
                return arenaType;

            var game = this.GetGame(id);

            if (game != null)
                return new ArenaType(game.Id, game.Name);

            return new ArenaType(id, id.ToString());

        }

        public Filter GetFilter(string label)
        {
            return this.AvailableFilters.FirstOrDefault(f => f.Id == label);
        }


        
    }

    public enum ErrorServerity
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    public class ErrorMessageCollection : List<ErrorMessage>
    {

    }

    public class ErrorMessage
    {
        public ErrorMessage()
        {

        }

        public ErrorMessage(string source, ErrorServerity severity, string message)
        {
            Source = source;
            Severity = severity;
            Message = message;
        }

        [XmlAttribute("severity")]
        public ErrorServerity Severity { get; set; }

        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlAttribute("message")]
        public string Message { get; set; }
    }
}
