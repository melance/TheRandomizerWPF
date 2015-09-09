Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class TagEditor

    Public Class StringItem
        Implements INotifyPropertyChanged

        Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

        Private _value As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal value As String)
            Me.Value = value
        End Sub

        Public Property Value As String
            Get
                Return _value
            End Get
            Set(value As String)
                If _value <> value Then
                    _value = value

                End If
            End Set
        End Property

        Public Sub OnPropertyChanged(<CallerMemberName> Optional ByVal propertyName As String = "")
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class

    Private _tagList As New BindingList(Of StringItem)
    Private _allTags As New BindingList(Of String)

    Public Property TagListInternal As BindingList(Of StringItem)
        Get
            Return _tagList
        End Get
        Set(value As BindingList(Of StringItem))
            _tagList = value
        End Set
    End Property

    Public Property TagList As BindingList(Of String)
        Get
            Dim value As New BindingList(Of String)
            For Each item As StringItem In TagListInternal
                value.Add(item.Value)
            Next
            Return value
        End Get
        Set(value As BindingList(Of String))
            _tagList.Clear()
            For Each item As String In value
                _tagList.Add(New StringItem(item))
            Next
        End Set
    End Property

    Private Sub Save_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub Cancel_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Me.DialogResult = False
        Me.Close()
    End Sub

    Private Sub Add_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        TagListInternal.AddNew()
    End Sub

    Private Sub Delete_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Dim item As StringItem = CType(lstTags.SelectedItem, StringItem)
        If item IsNot Nothing Then TagListInternal.Remove(item)
    End Sub
End Class
