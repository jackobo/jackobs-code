using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class ABTestCaseSetViewModel : ViewModelBase
    {
        public ABTestCaseSetViewModel(ABTestCaseSet testCaseSet)
        {
            _testCaseSet = testCaseSet;
            this.TestCases = testCaseSet.Select(t => new ABTestCaseViewModel(t)).ToArray();
        }


        public override string ToString()
        {
            return $"Brand: {BrandId} | Skin: {SkinId} | Language: {Language}";
        }

        protected override bool GlobalNotificationsEnabled
        {
            get
            {
                return false;
            }
        }

        ABTestCaseSet _testCaseSet;

        public string Id
        {
            get { return _testCaseSet.Id; }
        }

        public string BrandId
        {
            get { return _testCaseSet.BrandId; }
        }


        public string SkinId
        {
            get { return _testCaseSet.SkinId; }
        }

        public string Language
        {
            get { return _testCaseSet.Language; }
        }
        

        

        ABTestCaseViewModel[] _testsCases;
        public ABTestCaseViewModel[] TestCases
        {
            get { return _testsCases; }
            set
            {
                SetProperty(ref _testsCases, value);
            }
        }

        
    }

    public class ABTestCaseViewModel : ViewModelBase
    {
        public ABTestCaseViewModel(ABTestCase abTestCase)
        {
            _abTestCase = abTestCase;
        }
        
        public override string ToString()
        {
            return $"Id: {Id} | Name: {Name} | Method: {Method} | Percentage: {Percentage} | IsDefault: {IsDefault}";
        }

        ABTestCase _abTestCase;
        
        public string Id
        {
            get { return _abTestCase.Id; }
        }

        public string Name
        {
            get { return _abTestCase.Name; }
        }

        public string Percentage
        {
            get { return _abTestCase.UsePercentage.ToString("N0") + "%"; }
        }

        public string Method
        {
            get
            {
                return _abTestCase.Method.ToString();
            }
        }

        public string IsDefault
        {
            get
            {
                if (_abTestCase.IsDefault)
                    return "Yes";

                return "No";
            }
        }

        public string Description
        {
            get { return _abTestCase.Description; }
        }

        internal ABTestCase GetABTest()
        {
            return _abTestCase;
        }

        protected override bool GlobalNotificationsEnabled
        {
            get
            {
                return false;
            }
        }
    }
}
