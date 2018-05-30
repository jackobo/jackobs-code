using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public class QAEnvironmentEntity
    {
        public QAEnvironmentEntity(string id, string name, string path)
        {
            this.Id = id;
            this.Name = name;
            this.Path = path;
            this.ClientVersions = new List<ClientVersionEntity>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public List<ClientVersionEntity> ClientVersions { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as QAEnvironmentEntity;

            if (theOther == null)
                return false;

            return this.Id == theOther.Id && this.Name == theOther.Name;

        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }


    public class ClientVersionEntity
    {
        public ClientVersionEntity(string id)
        {
            this.Id = id;
            BuildVersionName(id);

            this.Path = this.Name.Replace(" ", "_");

            this.Brands = new List<BrandEntity>();
        }

        private void BuildVersionName(string versionID)
        {
            var name = versionID.Substring(0, 7) + " " + versionID.Substring(7);
            name = name[0].ToString().ToUpper() + name.Substring(1);
            name = name.Replace("_", ".");

            this.Name = name;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public string Path { get; private set; }

        public List<BrandEntity> Brands { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as ClientVersionEntity;

            if (theOther == null)
                return false;

            return this.Id == theOther.Id && this.Name == theOther.Name;

        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

    }


}
