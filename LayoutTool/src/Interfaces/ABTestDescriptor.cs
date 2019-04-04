using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public class ABTestCaseDescriptor
    {
        public ABTestCaseDescriptor(string testCaseSetId, string brand, string skin, string language, ABTestCase test)
        {
            TestCaseSetId = testCaseSetId;
            Brand = brand;
            Skin = skin;
            Language = language;
            Test = test;
        }
        public string TestCaseSetId { get; private set; }
        public string Brand { get; private set; }
        public string Skin { get; private set; }
        public string Language { get; private set; }
        public ABTestCase Test { get; private set; }


    }
}
