Imports System.Windows.Input

Public Class CustomCommands
    Public Shared About As New RoutedUICommand("About",
                                               "About",
                                               GetType(CustomCommands))

    Public Shared Close As New RoutedUICommand("Close",
                                               "Close",
                                               GetType(CustomCommands))

    Public Shared Generate As New RoutedUICommand("Generate",
                                                  "Generate",
                                                  GetType(CustomCommands))

    Public Shared SelectNone As New RoutedUICommand("Select None",
                                                    "SelectNone",
                                                    GetType(CustomCommands))

    Public Shared Cancel As New RoutedUICommand("Cancel",
                                                "Cancel",
                                                GetType(CustomCommands))
    Public Shared Add As New RoutedUICommand("Add",
                                             "Add",
                                             GetType(CustomCommands))
    Public Shared Delete As New RoutedUICommand("Delete",
                                                "Delete",
                                                GetType(CustomCommands))
End Class
