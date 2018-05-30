using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class MyAccountItemViewModel : DragableDropableViewModel, ISupportRemoveControl
    {
        
        public MyAccountItemViewModel(string id, string name, AttributeValueCollection attributes)
        {
            this.Id = id;
            this.Name = name;
            this.Attributes = attributes;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public AttributeValueCollection Attributes { get; private set; }

        public bool CanRemove
        {
            get
            {
                if (Constants.MyAccountItemsThatCannotBeRemoved.Contains(this.Id))
                    return false;

                return true;
            }
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as MyAccountItemViewModel;
            if (theOther == null)
                return false;

            return this.Id == theOther.Id;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        
    }
}
