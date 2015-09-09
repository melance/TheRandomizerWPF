Public Class LoadErrors

    Public Property ErrorText As String

    Private Sub LoadErrors_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Using stream As New IO.MemoryStream(Text.Encoding.ASCII.GetBytes(ErrorText))
            txtErrors.Selection.Load(stream, DataFormats.Rtf)
        End Using
    End Sub
End Class
