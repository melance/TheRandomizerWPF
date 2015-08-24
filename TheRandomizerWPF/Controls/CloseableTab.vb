Public Class CloseableTab
    Inherits TabItem

    Public Class TabAddedEventArgs
        Inherits EventArgs

        Public Sub New(ByVal tab As CloseableTab)
            _tab = tab
        End Sub

        Private _tab As CloseableTab

        Public ReadOnly Property Tab As CloseableTab
            Get
                Return _tab
            End Get
        End Property

    End Class

    Public Event TabAdded As EventHandler

    Private _closeableHeader As CloseableHeader

    Public Sub New()
        _closeableHeader = New CloseableHeader
        Me.Header = _closeableHeader
        AddHandler _closeableHeader.btnClose.Click, AddressOf btnClose_Click
        AddHandler _closeableHeader.btnAdd.Click, AddressOf btnAdd_Click
    End Sub

    Public Property CanCloseAll As Boolean = False

    Public Property Title As String
        Get
            Return CStr(_closeableHeader.lblTabTitle.Content)
        End Get
        Set(value As String)
            _closeableHeader.lblTabTitle.Content = value
        End Set
    End Property

    Protected Overrides Sub OnSelected(e As RoutedEventArgs)
        MyBase.OnSelected(e)
        ShowButtons(Me)
    End Sub

    Protected Overrides Sub OnUnselected(e As RoutedEventArgs)
        MyBase.OnUnselected(e)
        HideButtons(Me)
    End Sub

    Private Sub HideButtons(ByVal tab As CloseableTab)
        tab._closeableHeader.btnClose.Visibility = Windows.Visibility.Collapsed
        tab._closeableHeader.btnAdd.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private Sub ShowButtons(ByVal tab As CloseableTab)
        Dim tabControl As TabControl = DirectCast(tab.Parent, TabControl)
        tab._closeableHeader.btnAdd.Visibility = Windows.Visibility.Visible
        If tabControl.Items.Count = 1 AndAlso Not CanCloseAll Then
            tab._closeableHeader.btnClose.Visibility = Windows.Visibility.Collapsed
        Else
            tab._closeableHeader.btnClose.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Private Sub AddTab(ByVal tabControl As TabControl)
        Dim tab As New CloseableTab
        tabControl.Items.Add(tab)
        OnTabAdded(tab)
        tabControl.SelectedItem = tab
    End Sub

    Protected Overridable Sub OnTabAdded(ByVal tab As CloseableTab)
        RaiseEvent TabAdded(Me.Parent, New TabAddedEventArgs(tab))
    End Sub

    Private Sub btnClose_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim tabControl As TabControl = DirectCast(Me.Parent, TabControl)
        tabControl.Items.Remove(Me)
        If Not CanCloseAll AndAlso tabControl.Items.Count = 0 Then
            AddTab(tabControl)
        End If
    End Sub

    Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        AddTab(DirectCast(Me.parent, TabControl))
    End Sub

End Class
