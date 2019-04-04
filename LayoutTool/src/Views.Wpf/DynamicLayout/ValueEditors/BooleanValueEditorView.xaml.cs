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

namespace LayoutTool.Views.Wpf.DynamicLayout.ValueEditors
{
    /// <summary>
    /// Interaction logic for BooleanValueEditor.xaml
    /// </summary>
    public partial class BooleanValueEditorView : UserControl
    {
        public BooleanValueEditorView()
        {
            InitializeComponent();
            cboValues.Items.Add(true);
            cboValues.Items.Add(false);
        }
    }
}
