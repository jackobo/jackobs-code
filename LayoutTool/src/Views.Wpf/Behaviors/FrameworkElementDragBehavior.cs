using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using LayoutTool.ViewModels;

//http://stackoverflow.com/questions/32024034/the-type-from-assembly-is-built-with-an-older-version-of-blend-sdk-and-is-not-su

namespace LayoutTool.Views.Wpf
{
    public class FrameworkElementDragBehavior : Behavior<FrameworkElement>
    {


        private bool isMouseClicked = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseLeftButtonDown += new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
            this.AssociatedObject.MouseLeftButtonUp += new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonUp);
            this.AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseLeave);
        }

        void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseClicked = true;
        }

        void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseClicked = false;
        }

        void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if (isMouseClicked)
                {
                    //set the item's DataContext as the data to be transferred
                    IDragableSource dragObject = this.AssociatedObject.DataContext as IDragableSource;
                    if (dragObject != null)
                    {
                        DataObject data = new DataObject();
                        data.SetData(typeof(IDragableSource), dragObject);
                        System.Windows.DragDrop.DoDragDrop(this.AssociatedObject, data, DragDropEffects.Move);
                    }
                }
                isMouseClicked = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }
}
