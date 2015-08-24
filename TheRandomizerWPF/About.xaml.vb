Public Class About
    Private Sub About_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        lblTitle.Content = My.Application.Info.Title
        lblVersion.Content = My.Application.Info.Version.ToString(4)
        lblDescription.Content = My.Application.Info.Description
        lblCopyright.Content = My.Application.Info.Copyright
    End Sub

    Private Sub Hyperlink_RequestNavigate(sender As Object, e As RequestNavigateEventArgs)
        Process.Start(e.Uri.AbsoluteUri)
        e.Handled = True
    End Sub
End Class
