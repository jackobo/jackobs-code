using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.Interfaces
{
    public interface ISkinDefinitionConverter 
    {
        SkinConversionResult Convert(SkinDefinition skinDefinition);

    }


    public class SkinConversionResult
    {
        public SkinConversionResult(ConvertedClientConfigurationFile[] files, NewGameInformation[] newGames)
        {
            Files = files;
            NewGames = newGames;
        }
        public ConvertedClientConfigurationFile[] Files { get; private set; }
        public NewGameInformation[] NewGames { get; private set; }

        public class NewGameInformation
        {
            public NewGameInformation(int id, string name, string relativeImageUrl)
            {
                Id = id;
                Name = name;
                RelativeUrl = relativeImageUrl.ToLowerInvariant();
            }

            public int Id { get; private set; }
            public string Name { get; private set; }
            private string RelativeUrl { get; set; }

            public bool IsInsideUrl(string fullUrl)
            {
                return fullUrl.ToLowerInvariant().Contains(RelativeUrl);
            }
        }
    }

    public class ConvertedClientConfigurationFile
    {
        public ConvertedClientConfigurationFile(IClientConfigurationFile originalFile, string newContent)
        {
            this.OriginalFile = originalFile;
            this.NewContent = newContent;
        }
        public IClientConfigurationFile OriginalFile { get; private set; }
        public string NewContent { get; private set; }
    }
}
