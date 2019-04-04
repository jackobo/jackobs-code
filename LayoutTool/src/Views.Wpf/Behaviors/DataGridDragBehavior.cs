using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using LayoutTool.ViewModels;

namespace LayoutTool.Views.Wpf
{

    public class DataGridDragBehavior : Behavior<DataGrid>
    {
        
        protected override void OnAttached()
        {
            base.OnAttached();

            SetRowStyle();

            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += AssociatedObject_PreviewMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObject_PreviewMouseLeftButtonUp;
            
        }

        public static DependencyProperty ExcludedColumnsProperty = DependencyProperty.Register("ExcludedColumns", typeof(string), typeof(DataGridDragBehavior), new PropertyMetadata(""));

        public string ExcludedColumns
        {
            get { return (string)GetValue(ExcludedColumnsProperty); }
            set { SetValue(ExcludedColumnsProperty, value); }
        }

        private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _shouldInitializeDragDrop = false;
            
            if (AssociatedObject.SelectedItems.Count == AssociatedObject.Items.Count)
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    var dataGridRow = AssociatedObject.HitTest<DataGridRow>(e.GetPosition(AssociatedObject));

                    AssociatedObject.SelectedItems.Clear();
                    if (dataGridRow != null)
                    {
                        dataGridRow.IsSelected = true;
                        AssociatedObject.CurrentItem = dataGridRow.Item;
                    }
                }
                
            }
        }

        private bool _shouldInitializeDragDrop = false;

        private void AssociatedObject_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(_shouldInitializeDragDrop)
            {
                _shouldInitializeDragDrop = false;
                SetDragData();
            }
        }

        private void SetRowStyle()
        {
            if (AssociatedObject.RowStyle == null)
            {
                AssociatedObject.RowStyle = new Style(typeof(DataGridRow));
            }

            var cursorSetter = new Setter(DataGridRow.CursorProperty, Cursors.SizeAll);

            var dataTrigger = new Trigger();
            dataTrigger.Property = DataGridRow.IsSelectedProperty;
            dataTrigger.Value = true;

            dataTrigger.Setters.Add(cursorSetter);

            AssociatedObject.RowStyle.Triggers.Add(dataTrigger);
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

           
            
            var dataGridRow = AssociatedObject.HitTest<DataGridRow>(e.GetPosition(AssociatedObject));

            if (dataGridRow != null)
            {

                if (!string.IsNullOrEmpty(ExcludedColumns))
                {
                    var cell = AssociatedObject.HitTest<DataGridCell>(e.GetPosition(AssociatedObject));
                    if (cell != null)
                    {
                        var excludedColumns = ExcludedColumns.Split(',').Select(c => int.Parse(c));

                        if(excludedColumns.Contains(AssociatedObject.Columns.IndexOf(cell.Column)))
                        {
                            return;
                        }
                    }

                    

                }

                //e.Handled = AssociatedObject.SelectedItems.Contains(dataGridRow.Item);

                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    dataGridRow.IsSelected = !dataGridRow.IsSelected;
                    e.Handled = true;
                }
                else
                {
                    e.Handled = dataGridRow.IsSelected;
                }

                

                _shouldInitializeDragDrop = true;   
            }


            
        }
        

        private void SetDragData()
        {
            if (AssociatedObject.SelectedItems.Count > 0)
            {
                DataObject data = new DataObject();
                data.SetData(typeof(IDragableSource), new DraggableSet(AssociatedObject.SelectedItems.Cast<object>().OrderBy(item => AssociatedObject.Items.IndexOf(item))));
                DragDrop.DoDragDrop(this.AssociatedObject, data, DragDropEffects.Move);
            }
        }
    }
}
