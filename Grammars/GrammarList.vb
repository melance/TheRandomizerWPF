Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.IO
Imports System.Text

Public Class GrammarList
    Inherits BindingList(Of GrammarListItem)
    Implements INotifyPropertyChanged

    Private Const GENERATOR_FILE_PATTERN As String = "*.rnd.xml"
    Private Const ERROR_HEADER As String = "{\rtf1\ansi\deff0 {\fonttbl {\f0 Courier;}}"
    Private Const LOADING_GRAMMAR_FILE_ERROR As String = "\b Error loading the grammar rules file {0} \b0"
    Private Const RTF_TAB As String = "\tab"

    Private _paths As List(Of String)
    Private _selectedTags As New List(Of String)
    Private _filteredList As GrammarList

    Public Event ReportProgress As EventHandler(Of ProgressChangedEventArgs)

    Public Property SelectedTags As List(Of String)
        Get
            Return _selectedTags
        End Get
        Set(value As List(Of String))
            _selectedTags = value
            FilterList()
            OnPropertyChanged("SelectedTags")
        End Set
    End Property

    Public Property FilteredList() As GrammarList
        Get
            Return _filteredList
        End Get
        Set(value As GrammarList)
            _filteredList = value
            OnPropertyChanged("FilteredList")
        End Set
    End Property

    Public Sub New()
    End Sub

    Public Sub New(ByVal paths As List(Of String))
        _paths = paths
    End Sub

    Public Function Load() As Dictionary(Of String, Exception)
        Dim errorList As New Dictionary(Of String, Exception)
        Dim count As Int32 = 0
        Dim done As Int32 = 0
        Dim fileList As New List(Of String)

        For Each path As String In _paths
            If Directory.Exists(path) Then fileList.AddRange(Directory.GetFiles(path, GENERATOR_FILE_PATTERN))
        Next

        count = fileList.Count

        For Each filename As String In fileList
            Try
                done += 1
                Dim grammar As BaseGrammar = BaseGrammar.Open(filename)
                RaiseEvent ReportProgress(Me, New ProgressChangedEventArgs(CInt((done / count) * 100), grammar.Name))
                If grammar.Visible Then
                    Dim info As New GrammarListItem(grammar.Name,
                                                    grammar.Description,
                                                    filename,
                                                    grammar.Tags)
                    Me.Add(info)
                End If
            Catch ex As Exception
                errorList.Add(Path.GetFileName(filename), ex)
            End Try
        Next

        Return errorList
    End Function

    Private Sub FilterList()
        Dim result As New GrammarList
        Dim sortedList As New GrammarList
        result.AddRange(Items.Where(Function(g As GrammarListItem) As Boolean
                                        For Each tag As String In g.Tags
                                            If SelectedTags.Contains(tag) Then Return True
                                        Next
                                        Return False
                                    End Function))
        sortedList.AddRange(result.OrderBy(Function(g As GrammarListItem) g.Name))
        FilteredList = sortedList
    End Sub

    Public Sub AddRange(ByVal items As IEnumerable(Of GrammarListItem))
        For Each item As GrammarListItem In items
            Add(item)
        Next
    End Sub

    Protected Sub OnPropertyChanged(ByVal propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
End Class
