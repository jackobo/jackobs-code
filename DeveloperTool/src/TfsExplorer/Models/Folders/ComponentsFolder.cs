using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class ComponentsFolder : ChildFolderHolder<ComponentsFolder, IBranchFolder>
    {
        public ComponentsFolder(IBranchFolder branchFolder)
            : base("Components", branchFolder)
        {

        }

        
        public CoreFolder Core
        {
            get { return new CoreFolder(this); }
        }
        
        public EnginesAndGamesFolder EnginesAndGames
        {
            get { return new EnginesAndGamesFolder(this); }
        }
        
        public PackagesFolder Packages
        {
            get { return new PackagesFolder(this); }
        }

        public NonDeployableFolder NonDeployable
        {
            get { return new NonDeployableFolder(this); }
        }

        public GGPGameServerSln<ComponentsFolder> GGPGameServerSln
        {
            get { return new GGPGameServerSln<ComponentsFolder>(this); }
        }


        public BuildCustomizationXml<ComponentsFolder> BuildCustomizationXml
        {
            get { return new BuildCustomizationXml<ComponentsFolder>(this); }
        }

    }
}
