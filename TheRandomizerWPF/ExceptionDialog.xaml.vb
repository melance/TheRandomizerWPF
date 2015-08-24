Public Class ExceptionDialog

    Public Sub New(ByVal title As String,
                   ByVal ex As Exception)
        InitializeComponent()
        lblMessage.Text = ex.Message
        txtDetails.Text = ex.ToString
        Me.Title = title
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Me.Close()
    End Sub
End Class
