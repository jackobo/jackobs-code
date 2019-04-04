using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build.Actions
{
    [TestFixture]
    public class AssemblyInfoVersionUpdateActionTests : BuildActionTestBase
    {
        
        private static readonly ILocalPath AssemblyInfoFileName = new LocalPath(@"C:\temp\project\AssemblyInfo.cs");

        AssemblyInfoVersionUpdateAction CreateAction(string version)
        {
            return new AssemblyInfoVersionUpdateAction(AssemblyInfoFileName, new Infra.Types.VersionNumber(version));
        }

        [Test]
        public void Execute_CorrectlyReplaceTheVersionInsideTheFile()
        {
            var fileContentBefore = @"[assembly: AssemblyTitle(""Models.UnitTests"")]
                                [assembly: AssemblyDescription("""")]
                                // some comment 
                                [assembly: AssemblyConfiguration("""")] // some comment
                                [assembly: AssemblyVersion(""1.0.0.0"")]
                                [assembly: AssemblyFileVersion(""1.0.0.0"")]
                                // other comment
                                /* 
                                    some more comments
                                */
                               ";

            this.BuildContext.FileSystemAdapter.ReadAllText(AssemblyInfoFileName).Returns(fileContentBefore);
            string actualNewFileContent = null;
            this.BuildContext.FileSystemAdapter.WriteAllText(AssemblyInfoFileName, Arg.Do<string>(arg => actualNewFileContent = arg));

            var action = CreateAction("3.0.7.0");
            action.Execute(this.BuildContext);
            
            var expectedNewFileContent = @"[assembly: AssemblyTitle(""Models.UnitTests"")]
                                [assembly: AssemblyDescription("""")]
                                // some comment 
                                [assembly: AssemblyConfiguration("""")] // some comment
                                [assembly: AssemblyVersion(""3.0.7.0"")]
                                [assembly: AssemblyFileVersion(""3.0.7.0"")]
                                // other comment
                                /* 
                                    some more comments
                                */
                               ";


            Assert.AreEqual(expectedNewFileContent, actualNewFileContent);

        }
    }
}
