using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media.Imaging;

namespace LayoutTool.Views.Wpf
{
    public class RemoveCollectionItemBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
            this.AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
        }

        public static DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(RemoveCollectionItemBehavior), new PropertyMetadata(true));
        public static DependencyProperty ConfirmationMessageProperty = DependencyProperty.Register("ConfirmationMessage", typeof(string), typeof(RemoveCollectionItemBehavior));
        public static DependencyProperty CollectionProperty = DependencyProperty.Register("Collection", typeof(object), typeof(RemoveCollectionItemBehavior));
        public static DependencyProperty HorizontalAlignmentProperty = DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(RemoveCollectionItemBehavior), new PropertyMetadata(HorizontalAlignment.Right));
        public static DependencyProperty VerticalAlignmentProperty = DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(RemoveCollectionItemBehavior), new PropertyMetadata(VerticalAlignment.Top));


        public bool Enabled
        {
            get { return (bool)GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }


        public string ConfirmationMessage
        {
            get { return (string)GetValue(ConfirmationMessageProperty); }
            set { SetValue(ConfirmationMessageProperty, value); }
        }

        public object Collection
        {
            get { return GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        RemoveCollectionItemAdorner Adorner { get; set; }

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(!this.Enabled)
            {
                return;
            }

            Rect adornedElementRect = new Rect(this.AssociatedObject.RenderSize);
            if (Adorner != null &&  !adornedElementRect.Contains(e.GetPosition(this.AssociatedObject)))
            {
                DistroyAdorner();
            }
        }

        private void DistroyAdorner()
        {
            if (Adorner != null)
            {
                Adorner.Remove();
                Adorner = null;
            }
        }

        private void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!this.Enabled)
                return;

            if (Adorner != null)
            {
                DistroyAdorner();
            }
            CreateAdorner();
            UpdateAdorner();
        }

        private void UpdateAdorner()
        {
            if(Adorner != null)
                Adorner.Update();
        }

        private void CreateAdorner()
        {
            var supportsRemoveControl = this.AssociatedObject.DataContext as ViewModels.ISupportRemoveControl;

            if (supportsRemoveControl != null && !supportsRemoveControl.CanRemove)
                return;

            Adorner = new RemoveCollectionItemAdorner(this.AssociatedObject);
            Adorner.HorizontalAlignment = this.HorizontalAlignment;
            Adorner.VerticalAlignment = this.VerticalAlignment;
            Adorner.Content = CreateRemoveButton();
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DistroyAdorner();
        }

        private object CreateRemoveButton()
        {
            Button button = new Button();
            button.Cursor = System.Windows.Input.Cursors.Hand;
            button.Width = 16;
            button.Height = 16;
            
            button.PreviewMouseLeftButtonDown += Button_PreviewMouseLeftButtonDown;
            button.MouseLeave += Button_MouseLeave;

            button.ToolTip = "Remove";
            
            var imageSource = new BitmapImage(new Uri(string.Format("pack://application:,,,/{0};component/Resources/{1}", this.GetType().Assembly.GetName().Name, "Remove.png")));
            button.Content = new Image() { Source = imageSource };
            return button;
        }

        private void Button_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RemoveItem();
        }
        

        private void RemoveItem()
        {
            if (!string.IsNullOrEmpty(this.ConfirmationMessage))
            {
                if (MessageBox.Show(this.ConfirmationMessage, "Confirmation", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    return;
            }

            var collection = this.Collection as System.Collections.IList;
            if (collection != null)
            {
                collection.Remove(this.AssociatedObject.DataContext);
            }

            if (this.Adorner != null)
            {
                this.Adorner.Remove();
            }
        }
    }
}
