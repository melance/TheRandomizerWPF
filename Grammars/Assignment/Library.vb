Imports Utility
Imports System.Xml.Serialization
Imports System.Collections.ObjectModel

Namespace Assignment
    <XmlType(AnonymousType:=True)>
    <XmlRoot("library", Namespace:=Nothing)>
    Public Class Library

#Region "Shared Methods"
        Public Shared Function FromFile(ByVal fileName As String) As Library
            Dim serializer As New XmlSerializer(GetType(Library))
            Using stream As New IO.FileStream(fileName, IO.FileMode.Open)
                Return DirectCast(serializer.Deserialize(stream), Library)
            End Using
        End Function

        Public Shared Function ItemListConverter(ByVal listFiles As IEnumerable(Of AssignmentGrammar.ItemFile),
                                                 ByVal weightAdjustment As Int32,
                                                 ByVal removeDuplicates As Boolean,
                                                 ByVal caseSensitive As Boolean) As Library
            Dim value As New Library
            Dim rules As List(Of LineItem) = AssignmentGrammar.CreateRules(listFiles,
                                                                           weightAdjustment,
                                                                           removeDuplicates,
                                                                           caseSensitive)
            For Each rule As LineItem In rules
                value.Rules.Add(rule)
            Next

            Return value
        End Function
#End Region

#Region "Members"
        Private _rules As New ObservableCollection(Of LineItem)
#End Region

#Region "Propeties"
        <XmlElement("item")>
        Public ReadOnly Property Rules As ObservableCollection(Of LineItem)
            Get
                Return _rules
            End Get
        End Property
#End Region

#Region "Public Methods"
        Public Overrides Function ToString() As String
            Dim deserializer As New XmlSerializer(GetType(Library))
            Using stream As New IO.MemoryStream
                deserializer.Serialize(stream, Me)
                Return System.Text.Encoding.UTF8.GetString(stream.ToArray)
            End Using
        End Function
#End Region

    End Class
End Namespace