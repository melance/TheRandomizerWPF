Imports System.Windows

Public NotInheritable Class FocusAttacher

    Private Sub New()
    End Sub

    Public Shared ReadOnly FocusProperty As DependencyProperty = DependencyProperty.RegisterAttached("Focus", GetType(Boolean), GetType(FocusAttacher), New PropertyMetadata(False, New PropertyChangedCallback(AddressOf FocusChanged)))

    Public Shared Function GetFocus(ByVal d As DependencyObject) As Boolean
        Return CBool(d.GetValue(FocusProperty))
    End Function

    Public Shared Sub SetFocus(ByVal d As DependencyObject, ByVal value As Boolean)
        d.SetValue(FocusProperty, value)
    End Sub

    Private Shared Sub FocusChanged(ByVal sender As Object, ByVal e As DependencyPropertyChangedEventArgs)
        If CBool(e.NewValue) Then
            DirectCast(sender, UIElement).Focus()
        End If
    End Sub

End Class