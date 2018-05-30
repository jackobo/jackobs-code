using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class CreateComponentDescriptionTxtAction : IBuildAction
    {
        public CreateComponentDescriptionTxtAction(IComponentBuilder componentBuilder)
        {
            _componentBuilder = componentBuilder;
        }

        IComponentBuilder _componentBuilder;

        public void Execute(IBuildContext buildContext)
        {
            var componentDescriptionTxtFile = _componentBuilder
                                              .ResolveDistributionLocalPath(buildContext.BuildConfiguration.DistributionLocalPath)
                                              .Subpath("component_description.txt");
            buildContext.Logger.Info($"Create file {componentDescriptionTxtFile.AsString()}");
            if (buildContext.FileSystemAdapter.FileExists(componentDescriptionTxtFile))
            {
                buildContext.SourceControlAdapter.CheckoutForEdit(componentDescriptionTxtFile);

                buildContext.FileSystemAdapter.WriteAllText(componentDescriptionTxtFile,
                                            _componentBuilder.GetComponentDescriptionTxtContent());
                
            }
            else
            {
                buildContext.FileSystemAdapter.WriteAllText(componentDescriptionTxtFile,
                                                            _componentBuilder.GetComponentDescriptionTxtContent());
                buildContext.SourceControlAdapter.PendAdd(componentDescriptionTxtFile);
            }
            
        }
    }
}
