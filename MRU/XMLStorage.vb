Imports System.Reflection
Imports System.Configuration

Imports System.Xml.Serialization
Imports System.IO

Public Class XMLStorage
    Implements IStorage

    Private _fileName As String

    Public ReadOnly Property FileName As String
        Get
            Return _fileName
        End Get
    End Property

    Public Sub New(ByVal fileName As String)
        _fileName = fileName
    End Sub

    Public Function ReadFileList() As IEnumerable(Of String) Implements IStorage.ReadFileList
        Dim serializer As New XmlSerializer(GetType(List(Of String)))
        Using listFile As FileStream = File.OpenRead(FileName)
            Return DirectCast(serializer.Deserialize(listFile), IEnumerable(Of String))
        End Using
    End Function

    Public Sub WriteFileList(fileList As IEnumerable(Of String)) Implements IStorage.WriteFileList
        Dim serializer As New XmlSerializer(GetType(List(Of String)))
        Dim list As New List(Of String)(fileList)
        Using listFile As FileStream = File.OpenWrite(FileName)
            serializer.Serialize(listFile, list)
        End Using
    End Sub
End Class
