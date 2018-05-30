﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GamesPortal.Client.Interfaces.Services;
using GamesPortal.Client.ViewModels.Workspace;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Reporting.WinForms;

namespace GamesPortal.Client.Views.Wpf.Reports
{
    /// <summary>
    /// Interaction logic for LatestApprovedVersionForEachGameUserControl.xaml
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
        public static readonly DependencyProperty ReportParametersProperty = DependencyProperty.Register("ReportParameters", typeof(IList<ReportViewerParameter>), typeof(ReportHost), new PropertyMetadata(new ReportViewerParameter[0], ReportParametersPropertyChangedCallback));

        private static void ReportParametersPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var reportHost = d as ReportHost;
            if (reportHost != null)
            {
                reportHost.RefreshReport();
            }
        }

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

        public IList<ReportViewerParameter> ReportParameters
        {
            get { return (IList<ReportViewerParameter>)GetValue(ReportParametersProperty); }
            set { SetValue(ReportParametersProperty, value); }
        }

        private static void ItemsSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
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
            RefreshReportDataSource();

            RefreshReportParameters();

            reportViewer.RefreshReport();
        }

        private void RefreshReportParameters()
        {
            var parameters = new List<ReportParameter>();
            foreach (var p in this.ReportParameters ?? new ReportViewerParameter[0])
            {
                parameters.Add(new ReportParameter(p.Name, p.Value));
            }

            if (parameters.Any())
            {
                try
                {
                    this.reportViewer.LocalReport.SetParameters(parameters);
                }
                catch(Exception ex)
                {
#warning here is just a workaroud 
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void RefreshReportDataSource()
        {
            this.reportViewer.LocalReport.ReportEmbeddedResource = this.ReportFullName;

            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource(this.DataSourceName, this.ItemsSource);
            this.reportViewer.LocalReport.DataSources.Clear();
            this.reportViewer.LocalReport.DataSources.Add(reportDataSource1);
            
        }

    }
}
