
Public Class WindowSettings

    Public Sub New()
        Load()
        SizeToFit()
        MoveIntoView()
    End Sub

    Public Property WindowTop As Double
    Public Property WindowLeft As Double
    Public Property WindowHeight As Double
    Public Property WindowWidth As Double
    Public Property WindowState As WindowState
    Public Property GeneratorsWidth As Double
    Public Property DetailsWidth As Double

    Private Sub Load()
        WindowTop = My.Settings.WindowTop
        WindowLeft = My.Settings.WindowLeft
        WindowHeight = My.Settings.WindowHeight
        WindowWidth = My.Settings.WindowWidth
        WindowState = My.Settings.WindowState
        GeneratorsWidth = My.Settings.GeneratorsWidth
        DetailsWidth = My.Settings.DetailsWidth
    End Sub

    Private Sub MoveIntoView()
        If WindowTop + WindowHeight / 2 > SystemParameters.VirtualScreenHeight + SystemParameters.VirtualScreenTop Then
            WindowTop = SystemParameters.VirtualScreenHeight + SystemParameters.VirtualScreenTop - WindowHeight
        End If
        If WindowLeft + WindowWidth / 2 > SystemParameters.VirtualScreenWidth + SystemParameters.VirtualScreenLeft Then
            WindowLeft = SystemParameters.VirtualScreenWidth + SystemParameters.VirtualScreenLeft - WindowWidth
        End If
        If WindowTop < SystemParameters.VirtualScreenTop Then WindowTop = SystemParameters.VirtualScreenTop
        If WindowLeft < SystemParameters.VirtualScreenLeft Then WindowLeft = SystemParameters.VirtualScreenLeft
    End Sub

    Public Sub Save()
        If WindowState <> Windows.WindowState.Minimized Then
            My.Settings.WindowTop = WindowTop
            My.Settings.WindowLeft = WindowLeft
            My.Settings.WindowHeight = WindowHeight
            My.Settings.WindowWidth = WindowWidth
            My.Settings.WindowState = WindowState
            My.Settings.GeneratorsWidth = GeneratorsWidth
            My.Settings.DetailsWidth = DetailsWidth
            My.Settings.Save()
        End If
    End Sub

    Private Sub SizeToFit()
        WindowHeight = Math.Min(WindowHeight, SystemParameters.VirtualScreenHeight)
        WindowWidth = Math.Min(WindowWidth, SystemParameters.VirtualScreenWidth)
    End Sub

End Class
