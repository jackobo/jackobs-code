using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Types;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Design
{
    internal class MergeSetsReader : IMergeSetsReader
    {
        public MergeSetsReader(IThreadingServices threadingServices, ILoggerFactory loggerFactory)
        {
            _threadingServices = threadingServices;
            this.Logger = loggerFactory.CreateLogger(this.GetType());
        }

        IThreadingServices _threadingServices;
        ILogger Logger { get; set; }

        public IEnumerable<IMergeSet> ReadMergeSets(IEnumerable<ILogicalComponent> components, 
                                                    Folders.ComponentsFolder targetComponentsFolder)
        {
            var workers = new List<Worker>();
            foreach (var component in components)
            {
                component.As<ISupportBranching>()
                    .Do(sb => workers.Add(new Worker(sb, targetComponentsFolder, _threadingServices, this.Logger)));
            }

            var tasks = new List<Task>();

            foreach(var worker in workers)
            {
                tasks.Add(worker.Start());
            }

            Task.WaitAll(tasks.ToArray());

            if(workers.Any(w => w.Exception != null))
            {
                throw new ApplicationException("Reading MergeSets failed! See the logs for more details!");
            }

            return workers.SelectMany(w => w.MergeSet).ToList();
        }

        private class Worker
        {
            public Worker(ISupportBranching branchableComponent,
                          Folders.ComponentsFolder targetComponentsFolder,
                          IThreadingServices threadingServices,
                          ILogger logger)
            {
                _branchableComponent = branchableComponent;
                _targetComponentsFolder = targetComponentsFolder;
                _threadingServices = threadingServices;
                _logger = logger;
            }

            ILogger _logger;
            ISupportBranching _branchableComponent;
            Folders.ComponentsFolder _targetComponentsFolder;
            IThreadingServices _threadingServices;

            public Task Start()
            {
                return _threadingServices.StartNewTask(ReadMergeSets);
            }

            private void ReadMergeSets()
            {
                try
                {
                    this.MergeSet = _branchableComponent.GetMergeSet(_targetComponentsFolder);
                }
                catch(Exception ex)
                {
                    this.Exception = ex;
                    _logger.Exception(ex);
                }
            }

            public Exception Exception { get; private set; } = null;
            public Optional<IMergeSet> MergeSet { get; private set; } = Optional<IMergeSet>.None();
        }
    }
}
