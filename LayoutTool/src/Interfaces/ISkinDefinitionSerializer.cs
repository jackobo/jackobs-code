using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.Interfaces
{
    public interface ISkinDefinitionSerializer
    {
        string Serialize(Entities.SkinDefinitionContext skinDefintionContext);
        SkinDefinitionContext Deserialize(string content);
    }
}
