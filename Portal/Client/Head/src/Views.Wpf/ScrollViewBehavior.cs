using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GamesPortal.Client.Views.Wpf
{
    public static class ScrollViewBehavior
    {

        public static bool GetEnableMouseWheelScroll(ScrollViewer treeViewItem)
        {
            return (bool)treeViewItem.GetValue(EnableMouseWheelScrollProperty);
        }

        public static void SetEnableMouseWheelScroll(ScrollViewer treeViewItem, bool value)
        {
            treeViewItem.SetValue(EnableMouseWheelScrollProperty, value);
        }


        public static readonly DependencyProperty EnableMouseWheelScrollProperty = DependencyProperty.RegisterAttached( "EnableMouseWheelScroll", typeof(bool), typeof(ScrollViewer), new UIPropertyMetadata(false, OnEnableMouseWheelScrollChanged));


        static void OnEnableMouseWheelScrollChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer item = depObj as ScrollViewer;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                item.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
            else
                item.PreviewMouseWheel -= ScrollViewer_PreviewMouseWheel;
        }

        private static void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
                scrollviewer.LineUp();
            else
                scrollviewer.LineDown();
            e.Handled = true;
        }
    }
}
