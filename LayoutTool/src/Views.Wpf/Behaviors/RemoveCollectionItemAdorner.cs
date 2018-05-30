using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LayoutTool.Views.Wpf
{
    public class RemoveCollectionItemAdorner : Adorner
    {
        private AdornerLayer adornerLayer;
        private VisualCollection _visuals;
        private ContentPresenter _contentPresenter;

        public RemoveCollectionItemAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _visuals = new VisualCollection(this);
            _contentPresenter = new ContentPresenter();
            _visuals.Add(_contentPresenter);
            
            
            this.adornerLayer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
            this.adornerLayer.Add(this);

            this.HorizontalAlignment = HorizontalAlignment.Right;
            this.VerticalAlignment = VerticalAlignment.Top;

        }


        
        protected override Size MeasureOverride(Size constraint)
        {
            _contentPresenter.Measure(constraint);
            return _contentPresenter.DesiredSize;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);
            _contentPresenter.Arrange(new Rect(GetX(adornedElementRect, finalSize.Width), GetY(adornedElementRect, finalSize.Height), finalSize.Width, finalSize.Height));
            return _contentPresenter.RenderSize;
        }
        
        private double GetX(Rect adornedElementRect, double finalWidth)
        {
            switch(this.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    return 0;
                case HorizontalAlignment.Center:
                    return (adornedElementRect.Width - finalWidth) / 2d;
                case HorizontalAlignment.Right:
                    return adornedElementRect.Width - finalWidth - 2;
                default:
                    return 0;
            }
        }

        private double GetY(Rect adornedElementRect, double finalHeight)
        {
            switch (this.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    return 2;
                case VerticalAlignment.Center:
                    return (adornedElementRect.Height - finalHeight) / 2d;
                case VerticalAlignment.Bottom:
                    return adornedElementRect.Height - finalHeight;
                default:
                    return 0;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }

        public void Remove()
        {
            this.adornerLayer.Remove(this);
        }

        internal void Update()
        {
            
            this.Visibility = System.Windows.Visibility.Visible;

            if (this.adornerLayer != null)
            {
                this.adornerLayer.Update(this.AdornedElement);
            }
        }
        

        public object Content
        {
            get { return _contentPresenter.Content; }
            set { _contentPresenter.Content = value; }
        }

    }
}
