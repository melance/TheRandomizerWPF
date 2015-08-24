Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class Category
    Inherits Collection(Of CategoryItem)

    Public Class CategoryItem
        Implements INotifyPropertyChanged

        Public Sub New(ByVal name As String,
                       ByVal path As String,
                       ByVal description As String)
            Me.Name = name
            Me.Path = path
            Me.Description = description
        End Sub

        Private _name As String
        Private _path As String
        Private _description As String

        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                If _name <> value Then
                    _name = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Public Property Path As String
            Get
                Return _path
            End Get
            Set(value As String)
                If _path <> value Then
                    _path = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Public Property Description As String
            Get
                Return _description
            End Get
            Set(value As String)
                If _description <> value Then
                    _description = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(<CallerMemberNameAttribute> Optional ByVal propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class

    'Public Sub New(ByVal type As String,
    '               ByVal genre As String,
    '               ByVal system As String)
    '    Me.Category = type
    '    Me.Genre = genre
    '    Me.System = system
    'End Sub

    Public Sub New(ByVal tags As List(Of String))
        Me.Tags = tags
    End Sub

    Public Property Tags As List(Of String)

    Public Property Category As String
    Public Property Genre As String
    Public Property System As String

End Class