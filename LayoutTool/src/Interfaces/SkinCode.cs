using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public sealed class SkinCode
    {
        public SkinCode(int skinCode)
        {
            this.Code = skinCode;
            var skinCodeAsString = skinCode.ToString();
            int firstZeroIndex = FindSeparatorIndex(skinCodeAsString);

            if (firstZeroIndex < 0 || firstZeroIndex == skinCodeAsString.Length - 1)
            {
                this.BrandId = 0;
                this.SkinId = skinCode;
            }
            else
            {
                this.BrandId = int.Parse(skinCodeAsString.Substring(0, firstZeroIndex));
                this.SkinId = int.Parse(skinCodeAsString.Substring(firstZeroIndex + 1));
            }
        }

        private int FindSeparatorIndex(string skinCodeAsString)
        {

            for(int i = 0; i < skinCodeAsString.Length - 1; i++)
            {
                if (skinCodeAsString[i] == '0')
                {
                    if(skinCodeAsString[i + 1] != '0' || (i + 1 == skinCodeAsString.Length - 1))
                        return i;
                }
            }

            return -1;
        }

        public SkinCode(int brandId, int skinId)
        {
            this.BrandId = brandId;
            this.SkinId = skinId;

            this.Code = int.Parse(brandId.ToString() + "0" + skinId.ToString());
        }

        public int Code { get; private set; }


        public int BrandId { get; private set; }

        public int SkinId { get; private set; }

        public override string ToString()
        {
            return $"Brand {BrandId}; Skin {SkinId}";
        }


        public override bool Equals(object obj)
        {
            var theOther = obj as SkinCode;
            if (theOther == null)
                return false;

            return this.Code == theOther.Code;
        }

        public override int GetHashCode()
        {
            return this.Code.GetHashCode();
        }
    }
}
