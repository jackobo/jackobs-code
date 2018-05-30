using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public interface ISkinDefinitionBuilderViewModel : IViewModel
    {
        Guid Id { get; }
        int Order { get; }
        string SourceName { get; }
        bool IsValid { get; }
        bool IsActive { get; set; }

        bool IsVisible { get; }

        SkinDefinitionContext Build();

        SkinConversionResult Apply(SkinDefinition skinDefinition);

        bool CanProvideClientUrl { get; }
        ClientUrlBuilderViewModel GetClientUrlBuilder();
        void RestoreStateFrom(SkinIndentity skinIdentity);
        void Publish(SkinDefinitionContext skinDefinitionContext);

        event EventHandler StateRestored;
        SkinIndentity GetSkinIdentity();

        bool CanPublish { get; }
    }

    
}
