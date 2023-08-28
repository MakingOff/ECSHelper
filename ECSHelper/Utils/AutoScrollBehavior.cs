using System.Windows;
using System.Windows.Controls;

namespace ECSHelper.Utils; 

public static class AutoScrollBehavior {
    private static bool _autoScroll;
    public static readonly DependencyProperty AlwaysScrollToEndProperty =
        DependencyProperty.RegisterAttached("AlwaysScrollToEnd", typeof(bool), typeof(AutoScrollBehavior), new PropertyMetadata(false, AlwaysScrollToEndChanged));

    private static void AlwaysScrollToEndChanged(object sender, DependencyPropertyChangedEventArgs e) {
        if (sender is not ScrollViewer scroll) {
            return;
        }
        
        bool alwaysScrollToEnd = (e.NewValue != null) && (bool)e.NewValue;
            
        if (alwaysScrollToEnd) {
            scroll.ScrollToEnd();
            scroll.ScrollChanged += ScrollChanged;
        } else {
            scroll.ScrollChanged -= ScrollChanged;
        }
    }

    public static bool GetAlwaysScrollToEnd(ScrollViewer scroll) {
        return (bool)scroll.GetValue(AlwaysScrollToEndProperty);
    }

    public static void SetAlwaysScrollToEnd(ScrollViewer scroll, bool alwaysScrollToEnd) {
        scroll.SetValue(AlwaysScrollToEndProperty, alwaysScrollToEnd);
    }

    private static void ScrollChanged(object sender, ScrollChangedEventArgs e) {
        if (sender is not ScrollViewer scroll) {
            return;
        }

        if (e.ExtentHeightChange == 0) {
            _autoScroll = scroll.VerticalOffset == scroll.ScrollableHeight;
        }

        if (_autoScroll && e.ExtentHeightChange != 0) {
            scroll.ScrollToVerticalOffset(scroll.ExtentHeight);
        }
    }
}