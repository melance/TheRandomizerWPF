Imports System.Windows.Controls
Imports System.Windows

Public Class MRUFileList
    Inherits MenuItem

#Region "Classes"
    Public Class FileSelectedEventArgs
        Inherits EventArgs

        Public Sub New(ByVal fileName As String)
            _fileName = fileName
        End Sub

        Private _fileName As String

        Public ReadOnly Property FileName As String
            Get
                Return _fileName
            End Get
        End Property
    End Class
#End Region

#Region "Constructor"
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal storage As IStorage)
        MyBase.New()
        Me.Storage = storage
    End Sub
#End Region

#Region "Events"
    Public Event FileSelected As EventHandler(Of FileSelectedEventArgs)
    Public Event MaxItemsChanged As EventHandler
#End Region

#Region "Members"
    Private _maximumItems As Int32 = 10
#End Region

    Public Shared StorageProperty As DependencyProperty = DependencyProperty.Register("Storage",
                                                                                      GetType(IStorage),
                                                                                      GetType(MRUFileList),
                                                                                      New PropertyMetadata(Nothing,
                                                                                                           AddressOf StoragePropertyChanged))

#Region "Public Properties"
    Public Property Storage As IStorage
        Get
            Return DirectCast(GetValue(StorageProperty), IStorage)
        End Get
        Set(value As IStorage)
            SetValue(StorageProperty, value)
        End Set
    End Property

    Public Property MaximumItems As Int32
        Get
            Return _maximumItems
        End Get
        Set(value As Int32)
            If _maximumItems <> value Then
                _maximumItems = value
            End If
        End Set
    End Property
#End Region

#Region "Public Methods"
    Public Sub AddItem(ByVal filename As String)
        Dim item As MRUFileListItem = GetItem(filename)
        If item IsNot Nothing Then Items.Remove(item)
        item = New MRUFileListItem(filename)
        Items.Insert(0, item)
        AddHandler item.Click, AddressOf mnuItem_Click
        UpdateStorage()
    End Sub

    Public Sub RemoveItem(ByVal filename As String)
        Dim item As MRUFileListItem = GetItem(filename)
        If item IsNot Nothing Then
            RemoveHandler item.Click, AddressOf mnuItem_Click
            Items.Remove(item)
        End If
        UpdateStorage()
    End Sub

    Public Sub RemoveItem(ByVal index As Int32)
        Dim item As MRUFileListItem = CType(Items(index), MRUFileListItem)
        RemoveHandler item.Click, AddressOf mnuItem_Click
        Items.Remove(item)
        UpdateStorage()
    End Sub

    Public Sub RemoveItem(ByVal item As MRUFileListItem)
        RemoveHandler item.Click, AddressOf mnuItem_Click
        Items.Remove(item)
        UpdateStorage()
    End Sub

    Public Function GetItem(ByVal filename As String) As MRUFileListItem
        Dim index As Int32 = -1
        For i As Int32 = 0 To Items.Count - 1
            If DirectCast(Me.Items(i), MRUFileListItem).FileName.Equals(filename, StringComparison.CurrentCultureIgnoreCase) Then
                index = i
            End If
        Next
        If index >= 0 AndAlso index < Items.Count Then Return DirectCast(Items(index), MRUFileListItem)
        Return Nothing
    End Function
#End Region

#Region "Protected Methods"
    Protected Overridable Sub UpdateStorage()
        Dim fileList As New List(Of String)
        For Each item As MRUFileListItem In Items
            fileList.Add(item.FileName)
        Next
        Storage.WriteFileList(fileList)
    End Sub

    Protected Overridable Sub OnFileSelected(ByVal fileName As String)
        RaiseEvent FileSelected(Me, New FileSelectedEventArgs(fileName))
    End Sub

    Protected Overridable Sub OnMaxItemsChanged()
        RaiseEvent MaxItemsChanged(Me, EventArgs.Empty)
        Do While Me.Items.Count > MaximumItems
            Me.Items.RemoveAt(Me.Items.Count - 1)
        Loop
    End Sub
#End Region

#Region "Event Handlers"
    Private Shared Sub StoragePropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim instance As MRUFileList = DirectCast(d, MRUFileList)
        Dim fileList As IEnumerable(Of String) = instance.Storage.ReadFileList
        instance.Items.Clear()
        For Each fileName As String In fileList
            instance.AddItem(fileName)
        Next
    End Sub

    Private Sub mnuItem_Click(sender As Object, e As RoutedEventArgs)
        RaiseEvent FileSelected(sender, New FileSelectedEventArgs(DirectCast(sender, MRUFileListItem).FileName))
    End Sub
#End Region

End Class
