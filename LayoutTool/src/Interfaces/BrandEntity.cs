using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public class BrandEntity
    {
        public BrandEntity(int id, string name, string cdnUrl, params SkinEntity[] skins)
            : this(id, name, new PathDescriptor(cdnUrl), skins)
        {

        }

        public BrandEntity(int id, string name, PathDescriptor cdnUrl, params SkinEntity[] skins)
        {
            this.Id = id;
            this.Name = name;
            this.CDNUrl = cdnUrl;
            this.Skins = skins;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public PathDescriptor CDNUrl { get; set; }

        public override string ToString()
        {
            return $"{Name} [{Id}]";
        }
        
        public SkinEntity[] Skins { get; set; }
        

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as BrandEntity;

            if (theOther == null)
                return false;

            return this.Id == theOther.Id && this.Name == theOther.Name && this.CDNUrl == theOther.CDNUrl;
        }
    }
}
