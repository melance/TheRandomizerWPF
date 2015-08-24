Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class GrammarListItem
    Public Sub New(ByVal name As String,
                   ByVal description As String,
                   ByVal filepath As String,
                   ByVal tags As BindingList(Of String))
        _name = name
        _description = description
        _filepath = filepath
        _tags = tags
    End Sub

    Private _name As String
    Private _description As String
    Private _filepath As String
    Private _tags As BindingList(Of String)

    Public ReadOnly Property Name As String
        Get
            Return _name
        End Get
    End Property

    Public ReadOnly Property Description As String
        Get
            Return _description
        End Get
    End Property

    Public ReadOnly Property FilePath As String
        Get
            Return _filepath
        End Get
    End Property

    Public ReadOnly Property Tags As BindingList(Of String)
        Get
            Return _tags
        End Get
    End Property

    Public Function Grammar() As BaseGrammar
        Return BaseGrammar.Open(FilePath)
    End Function

End Class
