using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace VSSolutionExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            HashSet<string> projectItemsTypes = new HashSet<string>();
            HashSet<string> binaryReferences = new HashSet<string>();
            HashSet<string> projectReferences = new HashSet<string>();
            List<string> msBuildProjects = new List<string>();
            List<ProjectInSolution> otherProjects = new List<ProjectInSolution>();
            var solutionFile = SolutionFile.Parse(@"C:\CasinoTools\BranchMergeTests\3.x\QA\Main\Build\GGPGameServer.sln");
            foreach(var projectInSolution in solutionFile.ProjectsInOrder)
            {
                if (projectInSolution.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
                {
                    
                    msBuildProjects.Add(projectInSolution.ProjectName);

                    Project project = new Project(projectInSolution.AbsolutePath);
                    
                    var assemblyName = project.Properties.FirstOrDefault(p => p.Name == "AssemblyName");
                    if (assemblyName != null)
                        Console.WriteLine(assemblyName.EvaluatedValue);

                    var outputType = project.Properties.FirstOrDefault(p => p.Name == "OutputType");
                    if (outputType != null)
                        Console.WriteLine(outputType.EvaluatedValue);
                    

                    foreach (var projectItem in project.AllEvaluatedItems)
                    {
                        projectItemsTypes.Add(projectItem.ItemType);

                        if (projectItem.ItemType == "Reference")
                        {
                            binaryReferences.Add(projectItem.EvaluatedInclude);
                        }
                        else if (projectItem.ItemType == "ProjectReference")
                        {
                            projectReferences.Add(projectItem.EvaluatedInclude);
                        }
                    }
                }
                else
                {
                    otherProjects.Add(projectInSolution);
                }

            }

            foreach(var proj in otherProjects.OrderBy(p => p.ProjectName))
            {
                Console.WriteLine(proj.ProjectType+  "\t" + proj.ProjectName);
            }

            Console.ReadLine();
        }

        
    }
}
