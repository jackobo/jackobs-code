using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.Models
{
    public class SkinPublisher : Interfaces.ISkinPublisher
    {
        public SkinPublisher(Interfaces.ISkinDefinitionSerializer serializer, IWcfServiceFactory wcfServiceFactory)
        {
            _serializer = serializer;
            _wcfServiceFactory = wcfServiceFactory;
        }

        Interfaces.ISkinDefinitionSerializer _serializer;
        IWcfServiceFactory _wcfServiceFactory;

        public void PublishForProduction(SkinDefinitionContext skinDefinitionContext)
        {
            using (var service = _wcfServiceFactory.CreateLayoutPublisherService())
            {
                
                service.PublishSkinForProduction(new LayoutToolPublisherService.PublishSkinToProductionRequest()
                {
                    BrandId = skinDefinitionContext.DestinationSkin.BrandId,
                    SkinId = skinDefinitionContext.DestinationSkin.SkinId,
                    ClientVersion = skinDefinitionContext.DestinationSkin.ClientVersion,
                    HasWarnings = skinDefinitionContext.Errors.Any(err => err.Severity == ErrorServerity.Warning),
                    SkinContent = _serializer.Serialize(skinDefinitionContext),                    
                    NavigationPlanContent = ConvertNavigationPlanForProduction(skinDefinitionContext, service)
                });
            }
        }

        private string ConvertNavigationPlanForProduction(SkinDefinitionContext skinDefinitionContext, 
                                                          IDisposableLayoutToolPublisherService service)
        {
            var navigationPlanContentResponse = service.GetCurrentProductionNavigationPlan(new LayoutToolPublisherService.GetCurrentProductionNavigationPlanRequest()
            {
                BrandId = skinDefinitionContext.DestinationSkin.BrandId,
                SkinId = skinDefinitionContext.DestinationSkin.SkinId,
                ClientVersion = Builders.NdlVersionParser.ExtractMajorVersion(skinDefinitionContext.DestinationSkin.ClientVersion),
                ClientVersionJobNumber = Builders.NdlVersionParser.ExtractJobNumber(skinDefinitionContext.DestinationSkin.ClientVersion)
            });
            
            if (string.IsNullOrEmpty(navigationPlanContentResponse.NavigationPlanContent))
                return string.Empty;

            var converter = new Builders.Xml.XmlSkinDefinitionConverter();
            converter.NavigationPlan = new InputNavigationPlan(navigationPlanContentResponse.NavigationPlanContent);
            var converstionResult = converter.Convert(skinDefinitionContext.SkinDefinition);

            return converstionResult.Files.First().NewContent;
        }

        private class InputNavigationPlan : IClientConfigurationFile
        {
            public InputNavigationPlan(string content)
            {
                this.FileName = "navigation_plan_ndl.xmm";
                this.Content = content;
                this.Location = null;
            }

            public string FileName { get; private set; }
            public PathDescriptor Location { get; private set; }
            public string Content { get; private set; }


        }

        public void PublishForQA(SkinDefinitionContext skinDefinitionContext, string environment)
        {
            using (var service = _wcfServiceFactory.CreateLayoutPublisherService())
            {
                service.PublishSkinForQA(new LayoutToolPublisherService.PublishSkinToQARequest()
                {
                    Environment = environment,
                    BrandId = skinDefinitionContext.DestinationSkin.BrandId,
                    SkinId = skinDefinitionContext.DestinationSkin.SkinId,
                    ClientVersion = skinDefinitionContext.DestinationSkin.ClientVersion,
                    HasWarnings = skinDefinitionContext.Errors.Any(err => err.Severity == ErrorServerity.Warning),
                    SkinContent = _serializer.Serialize(skinDefinitionContext)

                });
            }
        }

    }
}
