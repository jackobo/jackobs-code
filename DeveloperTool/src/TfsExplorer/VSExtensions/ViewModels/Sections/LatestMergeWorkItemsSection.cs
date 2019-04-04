using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.VersionControl.Controls.Extensibility;
using Spark.Wpf.Common.ViewModels;

namespace Spark.VisualStudio.Extensions.ViewModels.Sections
{
    [TeamExplorerSection(GuidsList.LatestMergeWorkItemsSection, TeamExplorerPageIds.PendingChanges, 24)]
    public class LatestMergeWorkItemsSection : SectionBase
    {
        public LatestMergeWorkItemsSection()
        {
            
        }
        

        public override string Title
        {
            get
            {
                return "Latest Merge Work Items";
            }
        }

        public override object SectionContent
        {
            get
            {
                return new Views.Sections.LatestMergeWorkItemsSectionView(this);
            }
        }

        public LatestMergeWorkItemsHandler LatestMergeWorkItems { get; private set; }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            LatestMergeWorkItems = new LatestMergeWorkItemsHandler(this.ServiceProvider);
        }

        public override void Refresh()
        {
            base.Refresh();
            this.LatestMergeWorkItems.Refresh();
        }
    }
}
