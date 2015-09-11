Public Class LoadErrors

    Public Property ErrorList As Dictionary(Of String, Exception)

    Private Sub LoadErrors_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        lstExceptions.DisplayMemberPath = "Key"
        lstExceptions.ItemsSource = ErrorList
    End Sub

    Private Sub lstExceptions_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If e.AddedItems.Count > 0 Then
            txtException.Text = DirectCast(e.AddedItems(0), KeyValuePair(Of String, Exception)).Value.ToString
        End If
    End Sub
End Class
