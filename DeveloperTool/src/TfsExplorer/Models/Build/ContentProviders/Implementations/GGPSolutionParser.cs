using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Construction;
using Spark.Infra.Exceptions;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public class GGPSolutionParser : IGGPSolutionParser
    {
        ComponentsFolder _componentsFolder;
        SolutionFile _solutionFile;
        IBuildCustomizationProvider _customizationProvider;
        public GGPSolutionParser(ComponentsFolder componentsFolder, IBuildCustomizationProvider customizationProvider)
        {
            _componentsFolder = componentsFolder;
            _solutionFile = SolutionFile.Parse(GetSolutionFile(componentsFolder));
            _customizationProvider = customizationProvider;
            LoadComponents();
        }

        private static string GetSolutionFile(ComponentsFolder componentsFolder)
        {
            return componentsFolder.GGPGameServerSln.ToSourceControlFile().GetLocalPath().AsString();
        }

        string[] _solutionFoldersToIgnore = null;
        private string[] SolutionFoldersToIgnore
        {
            get
            {
                if (_solutionFoldersToIgnore == null)
                {
                    _solutionFoldersToIgnore = _solutionFile.ProjectsInOrder
                                                               .Where(p => p.ProjectType == SolutionProjectType.SolutionFolder
                                                                      && (0 == string.Compare(p.ProjectName, "UnitTests", false)
                                                                            || 0 == string.Compare(p.ProjectName, "Tools", false)))
                                                               .Select(prj => prj.ProjectGuid)
                                                               .ToArray();
                }

                return _solutionFoldersToIgnore;
            }
        }

        private void LoadComponents()
        {
            var compilableCoreComponents = ReadCompilableCoreComponents();
            var gameEngines = ReadGameEngines();
            var allCompilableComponents = compilableCoreComponents.Union(gameEngines);

            
            var packagesCoreComponent = ReadPackagesComponent(allCompilableComponents);

            var coreComponents = new List<ICoreComponentContentProvider>();
            coreComponents.AddRange(compilableCoreComponents.Cast<ICoreComponentContentProvider>());
            coreComponents.Add(packagesCoreComponent);


            ValidateUniqueIds(coreComponents.Select(c => c.ComponentUniqueIdBuilder)
                              .Concat(gameEngines.Cast<IGameEngineContentProvider>()
                                                 .SelectMany(ge => ge.GetUniqueIdBuilders())));

            this.CoreComponents = coreComponents;
            this.GameEngines = gameEngines.Cast<IGameEngineContentProvider>().ToList();
            
            

        }

        private void ValidateUniqueIds(IEnumerable<IComponentUniqueIdBuilder> uniqueIdBuilders)
        {
            var duplicates = uniqueIdBuilders.GroupBy(builder => builder.Id.Value)
                            .Where(g => g.Count() > 1)
                            .SelectMany(g => g.Select(item => item.Id.ToString()))
                            .ToList();

            if (duplicates.Count > 0)
            {
                var message = string.Join(Environment.NewLine, duplicates);
                throw new ValidationException($"The following components IDs are duplicated:{Environment.NewLine}{message}");
            }
        }

        private IEnumerable<ICompilableComponentContentProvider> ReadCompilableCoreComponents()
        {
            var compilableCoreComponents = new List<ICompilableComponentContentProvider>();
            foreach (var coreComponentFolder in _componentsFolder.Core.AllCoreComponents)
            {
                var vsProjects = ReadVisualStudioProjects(coreComponentFolder.ToSourceControlFolder().GetLocalPath());
                if (!vsProjects.Any())
                {
                    throw new ApplicationException($"Failed to detect any Visual Studio project inside the GGP solution file {GetSolutionFile(_componentsFolder)} for {coreComponentFolder.Name} core component");
                }

                compilableCoreComponents.Add(new CompilableCoreComponentContentProvider(coreComponentFolder,
                                                                vsProjects,
                                                                _customizationProvider));

            }
            return compilableCoreComponents;
        }

        private IEnumerable<ICompilableComponentContentProvider> ReadGameEngines()
        {
            var gameEngines = new List<ICompilableComponentContentProvider>();


            foreach (var gameEngineFolder in _componentsFolder.EnginesAndGames.AllGameEngines)
            {
                var vsProjects = ReadVisualStudioProjects(gameEngineFolder.Engine.ToSourceControlFolder().GetLocalPath());
                if(!vsProjects.Any())
                {
                    throw new ApplicationException($"Failed to detect any Visual Studio project inside the GGP solution file {GetSolutionFile(_componentsFolder)} for {gameEngineFolder.Name} game engine ");
                }
                gameEngines.Add(new GameEngineContentProvider(gameEngineFolder,
                                                             vsProjects,
                                                             _customizationProvider));

            }

            return gameEngines;
        }
        
        private PackagesContentProvider ReadPackagesComponent(IEnumerable<ICompilableComponentContentProvider> compilableComponents)
        {
          
            return new PackagesContentProvider(
                _componentsFolder.Packages,
                FindReferencedAssemblies(_componentsFolder.Packages.ToSourceControlFolder().GetLocalPath(),
                                        GetUsedReferencesByCompilableComponents(compilableComponents)),
                _customizationProvider);
        }

        private IEnumerable<string> GetUsedReferencesByCompilableComponents(IEnumerable<ICompilableComponentContentProvider> compilableComponents)
        {
            return compilableComponents.SelectMany(cc => cc.ReferencedAssemblies).Distinct();
        }

        private IEnumerable<string> FindReferencedAssemblies(ILocalPath folderContainingAssemblies, IEnumerable<string> allUsedReferences)
        {
            var allUsedReferencesDic = allUsedReferences.Distinct().ToDictionary(r => r, StringComparer.OrdinalIgnoreCase);
            var assemblies = new List<string>();
            foreach (var file in Directory.EnumerateFiles(folderContainingAssemblies.AsString(),
                                                         "*.dll",
                                                         SearchOption.AllDirectories))
            {
                if (allUsedReferencesDic.ContainsKey(file))
                {
                    assemblies.Add(file);
                }
            }

            return assemblies;
        }

        private IEnumerable<IVisualStudioProject> ReadVisualStudioProjects(ILocalPath inFolder)
        {

            var projects = new List<IVisualStudioProject>();
            foreach (var projectInSolution in _solutionFile.ProjectsInOrder.Where(p => p.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat))
            {
                if (!this.SolutionFoldersToIgnore.Contains(projectInSolution.ParentProjectGuid) 
                    && projectInSolution.AbsolutePath.StartsWith(inFolder.AsString(), StringComparison.OrdinalIgnoreCase))
                {
                    projects.Add(new VisualStudioProject(projectInSolution.AbsolutePath));
                }
            }

            return projects;
        }
    
        public ICoreComponentContentProvider GetCoreComponentContentProvider(string name)
        {
            var component = this.CoreComponents.FirstOrDefault(c => c.Name == name);
            if (component == null)
                throw new ArgumentException($"Can't find a core component with name {name}");

            return component;
        }

        public IGameEngineContentProvider GetGameEngineContentProvider(GameEngineName name)
        {
            var gameEngine = this.GameEngines.FirstOrDefault(c => name == new GameEngineName(c.Name));
            if (gameEngine == null)
                throw new ArgumentException($"Can't find a game engine with name {name}");

            return gameEngine;
        }

        public IEnumerable<ICoreComponentContentProvider> CoreComponents { get; private set; } = new ICoreComponentContentProvider[0];

        public IEnumerable<IGameEngineContentProvider> GameEngines { get; private set; } = new IGameEngineContentProvider[0];

    }
}
