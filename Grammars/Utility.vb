Imports Grammars.Assignment
Imports System.ComponentModel

Public NotInheritable Class Utility

#Region "Constructor"
    Private Sub New()
    End Sub
#End Region

#Region "Constants"
    Private Const GENERATOR_FILE_PATTERN As String = "*.rnd.xml"
    Private Const LIBRARY_FILE_PATTERN As String = "*.lib.xml"
    Private Const LOADING_GRAMMAR_FILE_ERROR As String = "Error loading the grammar rules file {0}"
#End Region

#Region "Members"
    Private Shared _grammarFilePaths As List(Of String)
    Private Shared _tempPath As String
#End Region

#Region "Properties"
    Friend Shared Property TempPath As String
        Get
            If String.IsNullOrWhiteSpace(_tempPath) Then _tempPath = IO.Path.Combine(IO.Path.GetTempPath, My.Application.Info.AssemblyName)
            Return Environment.ExpandEnvironmentVariables(_tempPath)
        End Get
        Set(value As String)
            _tempPath = value
        End Set
    End Property

    Public Shared ReadOnly Property GrammarFilePaths As List(Of String)
        Get
            If _grammarFilePaths Is Nothing Then _grammarFilePaths = New List(Of String)
            Return _grammarFilePaths
        End Get
    End Property

#End Region

#Region "Methods"
    Public Shared Function GetPropertyDescriptor(ByVal type As Type,
                                                 ByVal name As String) As PropertyDescriptor
        Return TypeDescriptor.GetProperties(type).Item(name)
    End Function

    'Public Shared Function GetCategories() As List(Of Category)
    '    Dim categories As New List(Of Category)
    '    Try
    '        For Each grammarFilePath As String In GrammarFilePaths
    '            For Each fileName As String In IO.Directory.GetFiles(grammarFilePath, GENERATOR_FILE_PATTERN)
    '                Try
    '                    Dim grammar As BaseGrammar = BaseGrammar.Open(fileName)
    '                    Dim category As Category = categories.FirstOrDefault(Function(c As Category) (c.Category = grammar.Category AndAlso c.Genre = grammar.Genre AndAlso c.System = grammar.System))

    '                    If category Is Nothing Then
    '                        category = New Category(grammar.Tags.ToList)
    '                        categories.Add(category)
    '                    End If

    '                    category.Add(New Category.CategoryItem(grammar.Name, fileName, grammar.Description & Environment.NewLine & "by " & grammar.Author))
    '                Catch ex As Exception
    '                    Trace.WriteLine(String.Format(LOADING_GRAMMAR_FILE_ERROR, fileName))
    '                    Trace.Indent()
    '                    Trace.WriteLine(ex.ToString)
    '                    Trace.Unindent()
    '                End Try
    '            Next
    '        Next
    '        Try
    '            categories.Sort(Function(a As Category, b As Category) a.Category.CompareTo(b.Category))
    '        Catch ex As Exception
    '        End Try
    '    Catch ex As Exception
    '        Trace.WriteLine(ex.ToString)
    '    End Try
    '    Return categories
    'End Function
#End Region

#Region "Public Methods"
    'Public Shared Function FilterGrammarFiles(ByVal categories As List(Of Category),
    '                                          ByVal tags As List(Of String)) As List(Of Category.CategoryItem)
    '    Dim list As New List(Of Category)
    '    Dim generators As New List(Of Category.CategoryItem)
    '    list.AddRange(categories.Where(Function(c As Category) As Boolean
    '                                       If tags Is Nothing OrElse tags.Count = 0 Then Return True
    '                                       For Each tag As String In c.Tags
    '                                           If tags.Contains(tag) Then Return True
    '                                       Next
    '                                       Return False
    '                                   End Function))
    '    For Each Item As Category In list
    '        generators.AddRange(Item)
    '    Next

    '    Return generators
    'End Function
#End Region

End Class
