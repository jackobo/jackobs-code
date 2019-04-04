using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.TeamFoundation.VersionControl.Controls.Extensibility;
using Spark.Wpf.Common.ViewModels;

namespace Spark.VisualStudio.Extensions.ViewModels.Sections
{
    public class LatestMergeWorkItemsHandler : ExtensionViewModelBase
    {
        private static FileSystemWatcher _fileSystemWatcher;

        public LatestMergeWorkItemsHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            InitFileSystemWatcher();
            AddRelatedWorkItemsCommand = new Command(AddRelatedWorkItems, () =>  this.WorkItems.Length > 0 && this.GetService<IPendingChangesExt>() != null, this);
            Refresh();
        }


        private string StoragePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Spark Visual Studio Extensions");
            }
        }

        private string LatestMergeWorkItemsStorageFile
        {
            get { return Path.Combine(StoragePath, "LatestMergeWorkItems.xml"); }
        }
        private void InitFileSystemWatcher()
        {
            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);

            _fileSystemWatcher = new FileSystemWatcher(StoragePath);
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
            _fileSystemWatcher.InternalBufferSize = 50536;
            _fileSystemWatcher.IncludeSubdirectories = true;

            _fileSystemWatcher.Changed += FileSystemWatcherEventsHandler;
            _fileSystemWatcher.Created += FileSystemWatcherEventsHandler;
            _fileSystemWatcher.Deleted += FileSystemWatcherEventsHandler;    
            _fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        
        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (string.Compare(e.FullPath, LatestMergeWorkItemsStorageFile, true) == 0
                || string.Compare(e.OldFullPath, LatestMergeWorkItemsStorageFile, true) == 0)
            {
                Refresh();
            }
        }

        private void FileSystemWatcherEventsHandler(object sender, FileSystemEventArgs e)
        {
            if(string.Compare(e.FullPath, LatestMergeWorkItemsStorageFile, true) == 0)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            try
            {
                var newWorkItems = new WorkItemDescriptor[0];

                if (File.Exists(LatestMergeWorkItemsStorageFile))
                {
                    WaitForFileReady(LatestMergeWorkItemsStorageFile, TimeSpan.FromSeconds(3));

                    var fileContent = File.ReadAllText(LatestMergeWorkItemsStorageFile);

                    if (!string.IsNullOrEmpty(fileContent))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(LatestMergeWorkItemsSettings));

                        using (var reader = new StringReader(fileContent))
                        {
                            newWorkItems = ((LatestMergeWorkItemsSettings)serializer.Deserialize(reader)).WorkItems.ToArray();
                        }
                    }
                }

                ExecuteOnUIThread(() => this.WorkItems = newWorkItems);
            }
            catch(Exception ex)
            {
#warning Exception logging here
                Console.WriteLine(ex.ToString());
            }
        }

        private static void WaitForFileReady(string fileName, TimeSpan timeout)
        {
            DateTime startTime = DateTime.Now;
            while(!IsFileReady(fileName) && DateTime.Now.Subtract(startTime) < timeout)
            {
                Thread.Sleep(200);
            }

            if(DateTime.Now.Subtract(startTime) > timeout)
            {
                throw new TimeoutException($"Waiting for file '{fileName}' to be ready timed out");
            }

        }

        private static bool IsFileReady(string fileName)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    if (inputStream.Length > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch
            {
                return false;
            }
        }

        WorkItemDescriptor[] _workItems = new WorkItemDescriptor[0];
        public WorkItemDescriptor[] WorkItems
        {
            get
            {
                return _workItems ?? new WorkItemDescriptor[0];
            }
            set
            {
                SetProperty(ref _workItems, value);
            }

        }

        public ICommand AddRelatedWorkItemsCommand { get; private set; }

        private void AddRelatedWorkItems()
        {
            var service = this.GetService<IPendingChangesExt>();
          
            var fieldInfo = service.GetType().GetField("m_workItemsSection", BindingFlags.Instance | BindingFlags.NonPublic);
            Type fieldType = fieldInfo.FieldType;
            object value = fieldInfo.GetValue(service);
            MethodInfo method = fieldType.GetMethod("AddWorkItemById", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var workItem in this.WorkItems)
            {
                method.Invoke(value, new object[] { workItem.Id });
            }
            
        }

       
    }

    [XmlRoot("settings")]
    public class LatestMergeWorkItemsSettings
    {

        public LatestMergeWorkItemsSettings()
        {
            this.WorkItems = new List<WorkItemDescriptor>();
        }

        [XmlArray(ElementName = "workItems")]
        [XmlArrayItem(ElementName = "workItem")]
        public List<WorkItemDescriptor> WorkItems { get; set; }
    }

    public class WorkItemDescriptor
    {

        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("title")]
        public string Title { get; set; }
    }

}
