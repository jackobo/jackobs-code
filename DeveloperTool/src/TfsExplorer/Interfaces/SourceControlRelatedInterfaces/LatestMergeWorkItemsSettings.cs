using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Spark.TfsExplorer.Interfaces
{
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
        public WorkItemDescriptor()
        {

        }

        public WorkItemDescriptor(int id, string title)
        {
            this.Id = id;
            this.Title = title;
        }

        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("title")]
        public string Title { get; set; }
    }
}
