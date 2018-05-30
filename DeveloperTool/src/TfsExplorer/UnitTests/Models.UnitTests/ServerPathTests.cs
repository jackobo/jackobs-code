using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Spark.TfsExplorer.Models.TFS;

namespace Spark.TfsExplorer.Models
{
    [TestFixture]
    public class ServerPathTests
    {
        [TestCase("$/GamingX/Test")]
        [TestCase("$/GamingX/Test/")]
        public void GetName_ReturnsTheLastPartFromThePath(string path)
        {
            var serverPath = new ServerPath(path);
            Assert.AreEqual("Test", serverPath.GetName());
        }

        [TestCase("$/GamingX/GGP", "Test", "$/GamingX/GGP/Test")]
        [TestCase("$/GamingX/GGP", "/Test", "$/GamingX/GGP/Test")]
        [TestCase("$/GamingX/GGP", "/Test/", "$/GamingX/GGP/Test")]
        [TestCase("$/GamingX/GGP", "Test/X", "$/GamingX/GGP/Test/X")]
        [TestCase("$/GamingX/GGP", "Test/X/", "$/GamingX/GGP/Test/X")]
        [TestCase("$/GamingX/GGP", "/Test/X/", "$/GamingX/GGP/Test/X")]
        public void Subpath_CorectlyAppendsTheSubpath(string parentPath, string subpath, string expected)
        {
            var serverPath = new ServerPath(parentPath);

            Assert.AreEqual(expected, serverPath.Subpath(subpath).AsString());
        }
    }
}
