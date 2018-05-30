using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{

    public interface IToolBox
    {
        void AddCommand(ICommand command, string Text);
        void RemoveCommand(ICommand command);
    }
    public class ToolBoxViewModel : ViewModelBase, IToolBox
    {
        public ToolBoxViewModel()
        {
            this.Items = new ObservableCollection<ToolBoxItem>();

            this.Items.Add(new ToolBoxItem(new Command(Test), "Test"));
        }
        
        private void Test()
        {

        }

        public void AddCommand(ICommand command, string Text)
        {
            if(!this.Items.Any(item => object.ReferenceEquals(command, item.Command)))
            {
                this.Items.Add(new ToolBoxItem(command, Text));
            }
        }

        public ObservableCollection<ToolBoxItem> Items { get; private set; }
         

        public void RemoveCommand(ICommand command)
        {
            var toolBoxItem = this.Items.FirstOrDefault(item => object.ReferenceEquals(command, item.Command));
            if (toolBoxItem != null)
                this.Items.Remove(toolBoxItem);
        }

    }

    public class ToolBoxItem : ViewModelBase
    {
        public ToolBoxItem(ICommand command, string caption)
        {
            this.Command = command;
            this.Caption = caption;

        }

        public ICommand Command { get; private set; }
        public string Caption { get; private set; }
    }
}
