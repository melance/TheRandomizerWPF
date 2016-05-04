using System;
using System.Windows;

namespace Utility
{
    public static class FocusAttacher
    {
        public static readonly DependencyProperty FocusProperty = DependencyProperty.RegisterAttached(
            "Focus",
            typeof (bool),
            typeof (FocusAttacher),
            new PropertyMetadata(false, FocusChanged));

        public static bool GetFocus(DependencyObject d)
        {
            return Convert.ToBoolean(d.GetValue(FocusProperty));
        }

        public static void SetFocus(DependencyObject d, bool value)
        {
            d.SetValue(FocusProperty, value);
        }

        private static void FocusChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Convert.ToBoolean(e.NewValue))
            {
                ((UIElement) sender).Focus();
            }
        }
    }
}