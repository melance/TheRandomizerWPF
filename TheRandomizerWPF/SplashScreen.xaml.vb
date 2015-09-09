Imports System.ComponentModel
Imports System.Windows.Media.Animation

Public Class SplashScreen

    Public Sub New()
        InitializeComponent()
        lblVersion.Content = My.Application.Info.Version.ToString(4)
        Application.SetTheme(Me, My.Settings.ThemeAccent, My.Settings.Theme)
    End Sub

    Private Sub Window_Closing(sender As Object, e As CancelEventArgs)
        RemoveHandler Closing, AddressOf Window_Closing
        e.Cancel = True
        Dim anim As New DoubleAnimation(0, New Duration(New TimeSpan(0, 0, 0, 0, 500)))
        AddHandler anim.Completed, Sub(s As Object, ce As EventArgs) Me.Close()
        Me.BeginAnimation(UIElement.OpacityProperty, anim)
    End Sub

    Public Sub SetProgress(ByVal value As Double)
        prgBar.Value = value
    End Sub

    Public Sub SetMessage(ByVal message As String)
        txtStatus.Text = message
    End Sub
End Class
