using System.Windows.Input;

namespace Utility
{
    public class CustomCommands
    {
        public static RoutedUICommand About = new RoutedUICommand("About", "About", typeof (CustomCommands));
        public static RoutedUICommand Close = new RoutedUICommand("Close", "Close", typeof (CustomCommands));
        public static RoutedUICommand Generate = new RoutedUICommand("Generate", "Generate", typeof (CustomCommands));
        public static RoutedUICommand SelectNone = new RoutedUICommand("Select None", "SelectNone", typeof (CustomCommands));
        public static RoutedUICommand Cancel = new RoutedUICommand("Cancel", "Cancel", typeof (CustomCommands));
        public static RoutedUICommand Add = new RoutedUICommand("Add", "Add", typeof (CustomCommands));
        public static RoutedUICommand Delete = new RoutedUICommand("Delete", "Delete", typeof (CustomCommands));
    }
}