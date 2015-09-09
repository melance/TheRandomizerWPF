Imports MRU
Imports System.Collections.Specialized

Public Class SettingsStorage
    Implements IStorage

    Public Sub New(ByVal settingName As String)
        Me.SettingName = settingName
    End Sub

    Public Property SettingName As String

    Public Function ReadFileList() As IEnumerable(Of String) Implements IStorage.ReadFileList
        Dim fileList As StringCollection = CType(My.Settings.Item(SettingName), StringCollection)

        If fileList IsNot Nothing Then
            Dim value As New List(Of String)
            For Each fileName As String In fileList
                value.Add(fileName)
            Next
            Return value
        End If
        Return New List(Of String)
    End Function

    Public Sub WriteFileList(fileList As IEnumerable(Of String)) Implements IStorage.WriteFileList
        Dim value As New StringCollection
        For Each fileName As String In fileList
            value.Add(fileName)
        Next
        My.Settings.RecentGrammarFiles = value
        My.Settings.Save()
    End Sub
End Class
