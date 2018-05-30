using Microsoft.Build.Evaluation;
using NUnit.Framework;
using System.Linq;
using Spark.TfsExplorer.Models.Build;
using Spark.TfsExplorer.Models.TFS;

namespace Spark.TfsExplorer.Models
{
    [TestFixture]
    public class GGPSolutionExplorerTests
    {
        [Test]
        public void Test()
        {
            var serverPath = new ServerPath("$/GamingX/GGP/");

            //var slnExplorer = new GGPSolutionStructureBuilder();
            //slnExplorer.Build();
        }

        [Test]
        public void ProjectWrapperTest()
        {
            
            var project = new VisualStudioProject(@"C:\CasinoTools\BranchMergeTests\3.x\QA\Main\Components\Core\GGPCore\GGPGameServer\GGPGameServer.Core\GGPGameServer.Core.csproj");
            Assert.AreEqual(2, project.OutputFilesNames.Count());
        }

        [Test]
        public void CopyToOutputDirFilesTest()
        {
            var project = new VisualStudioProject(@"C:\CasinoTools\BranchMergeTests\3.x\QA\Main\Components\Core\GGPCore\GGPGameServer\GGPGameServer.Core\GGPGameServer.Core.csproj");
            var result = project.CopyToOutputDirFiles.ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("RandomGenerator\\CryptoWrapper.dll", result[0]);
        }
    }
}
