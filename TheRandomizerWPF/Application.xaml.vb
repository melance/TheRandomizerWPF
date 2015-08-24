Imports System.IO
Imports System.Windows.Markup
Imports MahApps.Metro
Imports System.ComponentModel

Class Application

    Friend Enum RenderEngineType
        WebBrowser
        HtmlRenderer
    End Enum


    Public Sub New()
        AddHandler My.Application.DispatcherUnhandledException, AddressOf UnhandledExceptionHandler
    End Sub

    Friend Shared ReadOnly Property RenderEngine As RenderEngineType
        Get
            Return RenderEngineType.HtmlRenderer
        End Get
    End Property

    Private Sub UnhandledExceptionHandler(ByVal sender As Object, ByVal e As System.Windows.Threading.DispatcherUnhandledExceptionEventArgs)
        Dim dialog As New ExceptionDialog("Unhandled Exception", e.Exception)
        dialog.ShowDialog()
    End Sub

    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        MyBase.OnStartup(e)
        LoadCustomAccents()
    End Sub

    Public Shared Sub LoadCustomAccents()
        For Each item As KeyValuePair(Of String, Uri) In Accents.List.CustomAccents
            ThemeManager.AddAccent(item.Key, item.Value)
        Next
    End Sub

    Public Shared Sub SetTheme(ByVal window As Window,
                               ByVal accent As String,
                               ByVal theme As String)
        If Not String.IsNullOrWhiteSpace(accent) AndAlso
           Not String.IsNullOrWhiteSpace(theme) AndAlso
           ThemeManager.AppThemes.Any(Function(t As AppTheme) (t.Name = theme)) AndAlso
           ThemeManager.Accents.Any(Function(a As Accent) (a.Name = accent)) Then
            Dim currentTheme As Tuple(Of AppTheme, Accent) = ThemeManager.DetectAppStyle()
            ThemeManager.ChangeAppStyle(window,
                                        ThemeManager.GetAccent(accent),
                                        ThemeManager.GetAppTheme(theme))
        End If
    End Sub

    Public Shared Sub SetTheme(ByVal accent As String,
                               ByVal theme As String)
        If Not String.IsNullOrWhiteSpace(accent) AndAlso
           Not String.IsNullOrWhiteSpace(theme) AndAlso
           ThemeManager.AppThemes.Any(Function(t As AppTheme) (t.Name = theme)) AndAlso
           ThemeManager.Accents.Any(Function(a As Accent) (a.Name = accent)) Then
            Dim currentTheme As Tuple(Of AppTheme, Accent) = ThemeManager.DetectAppStyle()
            ThemeManager.ChangeAppStyle(Application.Current,
                                        ThemeManager.GetAccent(accent),
                                        ThemeManager.GetAppTheme(theme))
        End If
    End Sub
End Class
