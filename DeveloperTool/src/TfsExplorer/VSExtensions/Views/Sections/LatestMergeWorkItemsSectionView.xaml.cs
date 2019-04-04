using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.TeamFoundation.VersionControl.Controls.Extensibility;

namespace Spark.VisualStudio.Extensions.Views.Sections
{
    /// <summary>
    /// Interaction logic for LatestMergeWorkItemsSectionView.xaml
    /// </summary>
    public partial class LatestMergeWorkItemsSectionView : UserControl
    {
        public LatestMergeWorkItemsSectionView(ViewModels.Sections.LatestMergeWorkItemsSection section)
        {
            InitializeComponent();
            this.Section = section;
        }


        ViewModels.Sections.LatestMergeWorkItemsSection Section
        {
            get { return this.DataContext as ViewModels.Sections.LatestMergeWorkItemsSection; }
            set
            {
                this.DataContext = value;
            }
        }
    }
}
