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
#End Region

End Class
