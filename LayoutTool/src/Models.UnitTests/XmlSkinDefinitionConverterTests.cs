using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;

namespace LayoutTool.Models
{
    [TestFixture]
    public class XmlSkinDefinitionConverterTests
    {
        [Test]
        public void TestConvert()
        {
            var parser = new XmlSkinDefinitionParser();
            parser.GamesPropertiesXml = File.ReadAllText(@"C:\GamingNDL\Develop\CasinoFlashClient\Version_6.6_Head\build\versionX\navigation\plan\games_properties.xmm");
            parser.NavigationPlanXml = File.ReadAllText(@"C:\GamingNDL\Develop\CasinoFlashClient\Version_6.6_Head\build\versionX\navigation\plan\4\navigation_plan_ndl.xmm");
            parser.GamesTextsXml = File.ReadAllText(@"C:\GamingNDL\Develop\CasinoFlashClient\Version_6.6_Head\build\versionX\navigation\language\en\games_texts.xmm");
            parser.LobbyTextsXml = File.ReadAllText(@"C:\GamingNDL\Develop\CasinoFlashClient\Version_6.6_Head\build\versionX\navigation\language\en\lobby_text.xmm");

            parser.BrandXml = File.ReadAllText(@"C:\GamingNDL\Develop\CasinoFlashClient\Version_6.6_Head\build\versionX\brand\brand_0\brand.xml");
            parser.SkinXml = File.ReadAllText(@"C:\GamingNDL\Develop\CasinoFlashClient\Version_6.6_Head\build\versionX\brand\brand_0\skin_4\skin.xml");


            var skinDefiniton = parser.Parse();

            var converter = new XmlSkinDefinitionConverter();
            converter.GamesPropertiesXml = File.ReadAllText(@"C:\GamingNDL\Develop\CasinoFlashClient\Version_6.6_Head\build\versionX\navigation\plan\games_properties.xmm");
            converter.NavigationPlanXml = File.ReadAllText(@"C:\GamingNDL\Develop\CasinoFlashClient\Version_6.6_Head\build\versionX\navigation\plan\4\navigation_plan_ndl.xmm");
            converter.SkinXml = File.ReadAllText(@"C:\GamingNDL\Develop\CasinoFlashClient\Version_6.6_Head\build\versionX\brand\brand_0\skin_4\skin.xml");

            var result = converter.Convert(skinDefiniton.SkinDefinition);
            
            File.WriteAllText(@"C:\Temp\navigation_plan_result.xmm", result[0].NewContent);

        }
    }
}
