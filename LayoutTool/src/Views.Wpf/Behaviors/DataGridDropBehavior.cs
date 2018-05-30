using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using LayoutTool.ViewModels;

namespace LayoutTool.Views.Wpf
{
    public class DataGridDropBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.DragEnter += AssociatedObject_DragEnter;
            AssociatedObject.DragLeave += AssociatedObject_DragLeave;
            AssociatedObject.DragOver += AssociatedObject_DragOver;
            AssociatedObject.Drop += AssociatedObject_Drop;

        }

        private void AssociatedObject_DragLeave(object sender, DragEventArgs e)
        {
            DistroyAdorner();
        }

        private void AssociatedObject_DragEnter(object sender, DragEventArgs e)
        {
            DistroyAdorner();
        }


        private void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            DistroyAdorner();

            ScrollIfNecessary(e);

            var dropOverFrameworkElement = GetDropOverFrameworkElement(e);

            if (CanDrop(dropOverFrameworkElement, e))
            {
                e.Effects = DragDropEffects.Move | DragDropEffects.Copy;
                CreateAdorner(dropOverFrameworkElement, GetAdornerOrientation());
                UpdateAdorner(e.GetPosition(dropOverFrameworkElement));
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;

        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            var element = GetDropOverFrameworkElement(e);

            if (CanDrop(element, e))
            {
                var dragable = ExtractDragable(e);

                AssociatedObject.SelectedItems.Clear();

                foreach (var droppedItem in ExtractDropable(element).Drop(dragable, CreateDropContext(element, e)).DroppedItems)
                {
                    AssociatedObject.SelectedItems.Add(droppedItem);
                }
                
            }

            DistroyAdorner();
        }

     

        private IDropable ExtractDropable(FrameworkElement frameworkElement)
        {
            if (object.ReferenceEquals(frameworkElement, AssociatedObject))
                return AssociatedObject.ItemsSource as IDropable;
            else 
            {
                return frameworkElement.DataContext as IDropable;
            }
        }

        FrameworkElementDragAndDropAdorner Adorner { get; set; }

        

        private void UpdateAdorner(Point location)
        {
            if (this.Adorner != null)
            {
                this.Adorner.Update(location);
            }
        }


        private AdornerOrientation GetAdornerOrientation()
        {
            if (AssociatedObject.Items.Count == 0)
                return AdornerOrientation.All;
            else
                return AdornerOrientation.Horizontal;
        }

        private FrameworkElement GetDropOverFrameworkElement(DragEventArgs e)
        {
            if (AssociatedObject.Items.Count == 0)
            {
                return AssociatedObject;
            }
            else
            {
                return GetGridRowFromDragEventArgs(e);
            }

        }

        private bool CanDrop(FrameworkElement element, DragEventArgs e)
        {
            if (element == null)
                return false;

            var dropable = ExtractDropable(element);

            if (dropable == null)
                return false;

            var source = ExtractDragable(e);

            if (source == null || source.Items.Length == 0)
                return false;

            var canDrop = dropable.CanDrop(source, CreateDropContext(element, e));

            return canDrop;
        }


        private DropContext CreateDropContext(FrameworkElement element, DragEventArgs e)
        {
            return new DropContext(AssociatedObject.ItemsSource)
            {
                DropLocation = GetDropLocation(element,  e)
            };
        }

        private DropLocation GetDropLocation(FrameworkElement element, DragEventArgs e)
        {
            if (Adorner == null)
                return DropLocation.Before;

            return Adorner.ComputeDropLocation(e.GetPosition(element));
        }

        private IDragableSource ExtractDragable(DragEventArgs e)
        {
            return e.Data.GetData(typeof(IDragableSource)) as IDragableSource;
        }

        private DataGridRow GetGridRowFromDragEventArgs(DragEventArgs e)
        {
            return AssociatedObject.HitTest<DataGridRow>(e.GetPosition(AssociatedObject));
        }


        private void CreateAdorner(FrameworkElement element, AdornerOrientation orientation)
        {
            Adorner = new FrameworkElementDragAndDropAdorner(element);
            Adorner.AllowDrop = true;
            Adorner.Orientation = orientation;
        }

        private void DistroyAdorner()
        {
            if (this.Adorner != null)
            {
                this.Adorner.Remove();
                this.Adorner = null;
            }
        }
        

        private void ScrollIfNecessary(DragEventArgs e)
        {
         

            ScrollViewer scrollViewer = AssociatedObject.GetFirstVisualChild<ScrollViewer>();

            if (scrollViewer == null)
            {
                return;
            }
            
            double tolerance = 10;
            double verticalPos = e.GetPosition(AssociatedObject).Y;
            double offset = 1;

            if (verticalPos < tolerance) // Top of visible list? 
            {
                //Scroll up
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset);
            }
            else if (verticalPos > AssociatedObject.ActualHeight - tolerance) //Bottom of visible list? 
            {
                //Scroll down
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
            }
        }
    }
}
