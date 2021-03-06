﻿using System;
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
using Spark.Wpf.Common.ViewModels;

namespace Spark.Wpf.Common.Views
{
    /// <summary>
    /// Interaction logic for BackgroundOperationsRegionView.xaml
    /// </summary>
    public partial class BackgroundOperationsRegionView : UserControl
    {
        public BackgroundOperationsRegionView(BackgroundOperationsRegion viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
