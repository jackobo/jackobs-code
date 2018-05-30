using System;
using System.Collections.Generic;
using System.IO;
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
using Tools.ViewModels;

namespace Tools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(App.ConfigurationReader);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                var comparer = new ViewModels.LanguagesComparer(App.ConfigurationReader);

                StringBuilder sb = new StringBuilder();
                foreach (var compare in comparer.Compare())
                    sb.AppendLine(compare.ToString());

                File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result.txt"), sb.ToString());

                MessageBox.Show("Done");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
