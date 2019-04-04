using System;
using System.Collections.Generic;
using System.Linq;
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

namespace LayoutTool.Views.Wpf
{
    /// <summary>
    /// Interaction logic for ReportHost.xaml
    /// </summary>
    public partial class ReportHost : UserControl
    {
        public ReportHost()
        {
            InitializeComponent();
            reportViewer.Load += reportViewer_Load;
        }


        public static readonly DependencyProperty ReportFullNameProperty = DependencyProperty.Register("ReportFullName", typeof(string), typeof(ReportHost));
        public static readonly DependencyProperty DataSourceNameProperty = DependencyProperty.Register("DataSourceName", typeof(string), typeof(ReportHost), new PropertyMetadata("DataSource"));
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(ReportHost), new PropertyMetadata(null, ItemsSourcePropertyChangedCallback));

        public string ReportFullName
        {
            get { return (string)GetValue(ReportFullNameProperty); }
            set { SetValue(ReportFullNameProperty, value); }
        }

        public string DataSourceName
        {
            get { return (string)GetValue(DataSourceNameProperty); }
            set { SetValue(DataSourceNameProperty, value); }
        }

        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Console.WriteLine(e.NewValue.GetType().FullName);

            var reportHost = d as ReportHost;
            if (reportHost != null)
            {
                reportHost.RefreshReport();
            }
        }



        private bool _isReportViewerLoaded;
        void reportViewer_Load(object sender, EventArgs e)
        {
            if (!_isReportViewerLoaded)
            {

                RefreshReport();

                _isReportViewerLoaded = true;
            }
        }

        private void RefreshReport()
        {
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource(this.DataSourceName, this.ItemsSource);
            this.reportViewer.LocalReport.DataSources.Clear();
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer.LocalReport.ReportEmbeddedResource = this.ReportFullName;


            reportViewer.RefreshReport();
        }
    }
}
