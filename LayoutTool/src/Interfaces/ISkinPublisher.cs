using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public interface ISkinPublisher
    {
        void PublishForQA(Entities.SkinDefinitionContext skinDefinitionContext, string environment);
        void PublishForProduction(Entities.SkinDefinitionContext skinDefinitionContext);
    }
}
