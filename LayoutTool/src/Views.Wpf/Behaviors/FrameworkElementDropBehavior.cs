using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using LayoutTool.ViewModels;

namespace LayoutTool.Views.Wpf
{
    public class FrameworkElementDropBehavior : Behavior<FrameworkElement>
    {
        
        

        public static DependencyProperty DropContextDataProperty = DependencyProperty.Register("DropContextData", typeof(object), typeof(FrameworkElementDropBehavior));

        public static DependencyProperty AdornerOrientationProperty = DependencyProperty.Register("AdornerOrientation", typeof(AdornerOrientation), typeof(FrameworkElementDropBehavior));



        public object DropContextData
        {
            get { return GetValue(DropContextDataProperty); }
            set { SetValue(DropContextDataProperty, value); }

        }

        public AdornerOrientation AdornerOrientation
        {
            get { return (AdornerOrientation)GetValue(AdornerOrientationProperty); }
            set { SetValue(AdornerOrientationProperty, value); }

        }

        private FrameworkElementDragAndDropAdorner Adorner { get; set; }
        private IDropable Dropable { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.AllowDrop = true;
            this.AssociatedObject.DragEnter += new DragEventHandler(AssociatedObject_DragEnter);
            this.AssociatedObject.DragOver += new DragEventHandler(AssociatedObject_DragOver);
            this.AssociatedObject.DragLeave += new DragEventHandler(AssociatedObject_DragLeave);
            this.AssociatedObject.Drop += new DragEventHandler(AssociatedObject_Drop);
        }

        void AssociatedObject_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                DistroyAdorner();

                if (Dropable == null)
                {
                    Dropable = this.AssociatedObject.DataContext as IDropable;
                }

                if (Dropable != null && Adorner == null)
                {
                    CreateAdorner();
                }

                e.Handled = true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void CreateAdorner()
        {
            this.Adorner = new FrameworkElementDragAndDropAdorner(this.AssociatedObject);
            this.Adorner.AllowDrop = true;
            this.Adorner.Orientation = this.AdornerOrientation;
        }

        void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (Dropable != null)
                {
                    if (CanDrop(e))
                    {
                        e.Effects = DragDropEffects.Move;
                        UpdateAdorner(e.GetPosition(this.AssociatedObject));
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                        HideAdorner();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void UpdateAdorner(Point location)
        {
            if (this.Adorner != null)
            {
                this.Adorner.Update(location);
            }
        }

        private void HideAdorner()
        {
            if(this.Adorner != null)
            {
                this.Adorner.Visibility = Visibility.Hidden;
            }
        }

        void AssociatedObject_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                DistroyAdorner();

                Dropable = null;

                e.Handled = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void DistroyAdorner()
        {
            if (this.Adorner != null)
            {
                this.Adorner.Remove();
                this.Adorner = null;
            }
        }



        void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (Dropable != null && CanDrop(e))
                {
                    Dropable.Drop(ExtractDragable(e), CreateDropContext(e));
                }

                if (this.Adorner != null)
                {
                    this.Adorner.Remove();
                }

                Dropable = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private DropContext CreateDropContext(DragEventArgs e)
        {
            return new DropContext(this.DropContextData)
            {
                DropLocation = GetDropLocation(e)
            };
        }

        private bool CanDrop(DragEventArgs e)
        {
            var source = ExtractDragable(e);

            if (source == null)
                return false;

            return this.Dropable.CanDrop(source, CreateDropContext(e));
            
        }
        
        private IDragableSource ExtractDragable(DragEventArgs e)
        {
            return e.Data.GetData(typeof(IDragableSource)) as IDragableSource;
        }

        private DropLocation GetDropLocation(DragEventArgs e)
        {
            return this.Adorner.ComputeDropLocation(e.GetPosition(this.AssociatedObject));
        }       
    }
}
