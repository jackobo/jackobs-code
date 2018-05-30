using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using LayoutTool.ViewModels;

namespace LayoutTool.Views.Wpf
{
    public enum AdornerOrientation
    {
        Horizontal,
        Vertical,
        All
    }

    class FrameworkElementDragAndDropAdorner : Adorner
    {
        private AdornerLayer adornerLayer;

        public FrameworkElementDragAndDropAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this.IsHitTestVisible = false;
            this.Orientation = AdornerOrientation.Vertical;
            this.adornerLayer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
            

            this.adornerLayer.Add(this);
        }


        public AdornerOrientation Orientation { get; set; }

        internal void Update(Point currentMousePosition)
        {
            this.CurrentMousePosition = currentMousePosition;
            this.Visibility = System.Windows.Visibility.Visible;
            this.adornerLayer.Update(this.AdornedElement);
            
        }

        Point CurrentMousePosition { get; set; }

        public void Remove()
        {
            this.adornerLayer.Remove(this);
        }

        

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);
            
            
            if (!adornedElementRect.Contains(CurrentMousePosition))
                return;

                        
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Red), 2.5);

            if (this.Orientation == AdornerOrientation.All)
            {
                drawingContext.DrawRectangle(null, renderPen, adornedElementRect);
                
            }
            else
            {
                if (this.Orientation == AdornerOrientation.Vertical)
                {
                 
                    if (ComputeDropLocation(CurrentMousePosition) == DropLocation.Before)
                    {
                        drawingContext.DrawLine(renderPen, adornedElementRect.TopLeft, adornedElementRect.BottomLeft);
                    }
                    else
                    {
                        drawingContext.DrawLine(renderPen, adornedElementRect.TopRight, adornedElementRect.BottomRight);
                    }
                }
                else
                {
                    if (ComputeDropLocation(CurrentMousePosition) == DropLocation.Before)
                    {
                        drawingContext.DrawLine(renderPen, adornedElementRect.TopLeft, adornedElementRect.TopRight);
                    }
                    else
                    {
                        drawingContext.DrawLine(renderPen, adornedElementRect.BottomLeft, adornedElementRect.BottomRight);
                    }
                }
            }
        }

        public DropLocation ComputeDropLocation(Point position)
        {

            if (this.Orientation == AdornerOrientation.All)
                return DropLocation.After;

            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);

            if (this.Orientation == AdornerOrientation.Vertical)
            {
                return (position.X < (adornedElementRect.X + adornedElementRect.Width / 2)) ? DropLocation.Before : DropLocation.After;
            }
            else
            {
                return (position.Y < (adornedElementRect.Y + adornedElementRect.Height / 2)) ? DropLocation.Before : DropLocation.After;
            }
        }
    }
}