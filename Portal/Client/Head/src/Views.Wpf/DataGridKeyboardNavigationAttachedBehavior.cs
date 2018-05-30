using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using GamesPortal.Client.ViewModels.Workspace;

namespace GamesPortal.Client.Views.Wpf
{
    public class DataGridKeyboardNavigationAttachedBehavior
    {
        public static readonly DependencyProperty
                KeyboardKey
                    = DependencyProperty.RegisterAttached(
                        "IsKeyboardNavigationEnabled",
                        typeof(bool),
                        typeof(DataGridKeyboardNavigationAttachedBehavior),
                        new PropertyMetadata(
                            false,
                            OnIsKeyboardNavigationEnabledChanged));

        public static bool GetIsKeyboardNavigationEnabled(DependencyObject depObj)
        {
            return (bool)depObj.GetValue(KeyboardKey);
        }

        public static void SetIsKeyboardNavigationEnabled(DependencyObject depObj, bool value)
        {
            depObj.SetValue(KeyboardKey, value);
        }

        private static void OnIsKeyboardNavigationEnabledChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = depObj as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.PreviewKeyDown += dataGrid_PreviewKeyDown;
                dataGrid.IsSynchronizedWithCurrentItem = true;
            }
        }

        static void dataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;

            if (dataGrid != null && dataGrid.CurrentCell != null)
            {
                if (e.Key == System.Windows.Input.Key.Down || e.Key == System.Windows.Input.Key.Up)
                {
                    ICollectionView view = CollectionViewSource.GetDefaultView(dataGrid.Items);

                    int oldPosition = view.CurrentPosition;

                    do
                    {
                        if (e.Key == System.Windows.Input.Key.Down)
                        {
                            view.MoveCurrentToNext();
                        }
                        if (e.Key == System.Windows.Input.Key.Up)
                        {
                            view.MoveCurrentToPrevious();
                        }

                        if (view.CurrentItem == null)
                        {
                            view.MoveCurrentToPosition(oldPosition);
                            break;
                        }

                    } while (view.CurrentItem != null && !((GameVersionsWorkspaceItem.GridItem)view.CurrentItem).IsVisible);

                    // We have to move the cell selection aswell.
                    dataGrid.CurrentCell = new DataGridCellInfo(view.CurrentItem, dataGrid.CurrentCell.Column);

                    e.Handled = true;
                    return;
                }
            }
        }

        
    }
}
