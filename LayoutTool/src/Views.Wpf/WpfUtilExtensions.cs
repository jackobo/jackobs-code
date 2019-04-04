using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LayoutTool.Views.Wpf
{
    public static class WpfUtilExtensions
    {

        public static T HitTest<T>(this Visual element, Point point)
            where T : Visual
        {
            var hitTestResult = VisualTreeHelper.HitTest(element, point);

            if (hitTestResult == null || hitTestResult.VisualHit == null)
                return null;
            
            return hitTestResult.VisualHit.GetParentOfType<T>();

        }
        public static T GetParentOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            Type type = typeof(T);
            if (element == null) return null;
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent == null && ((FrameworkElement)element).Parent is DependencyObject) parent = ((FrameworkElement)element).Parent;
            if (parent == null) return null;
            else if (parent.GetType() == type || parent.GetType().IsSubclassOf(type)) return parent as T;
            return GetParentOfType<T>(parent);
        }

        public static T GetFirstVisualChild<T>(this DependencyObject element) where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(element);

            if (childrenCount == 0)
                return null;

            for (int i = 0; i < childrenCount; i++)
            {
                if (VisualTreeHelper.GetChild(element, i).GetType().Equals(typeof(T)))
                {
                    return (T)VisualTreeHelper.GetChild(element, i);
                }
            }


            for (int i = 0; i < childrenCount; i++)
            {
                var child = GetFirstVisualChild<T>(VisualTreeHelper.GetChild(element, i));
                if (child != null)
                    return child;

            }

            return null;
        }
    }
}
