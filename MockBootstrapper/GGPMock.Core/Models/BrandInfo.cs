using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models
{
    public partial class BrandInfo
    {
        public BrandInfo(int brandId, string fullName, string shortName, int regulationId)
        {
            this.BrandId = brandId;
            this.FullName = fullName;
            this.ShortName = shortName;
            this.RegulationId = regulationId;
        }

        public int BrandId { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public int RegulationId { get; set; }


        public override string ToString()
        {
            return this.FullName + " (" + this.BrandId + ")";
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as BrandInfo;
            if(theOther == null)
            {
                return false;
            }

            return this.BrandId == theOther.BrandId;

        }
        
        public override int GetHashCode()
        {
            return this.BrandId.GetHashCode();
        }

        public static BrandInfo[] GetBrandsForRegulation(int regulationId)
        {
            if(regulationId == 0)
            {
                regulationId = 4;
            }
            return AllBrands.Where(b => b.RegulationId == regulationId).ToArray();
        }
    }


}
