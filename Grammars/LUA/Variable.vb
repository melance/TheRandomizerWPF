Imports System.Xml.Serialization

Namespace LUA
    Public Class Variable
        <XmlAttribute("name")>
        Public Property Name As String
        <XmlText()>
        Public Property Value As String
    End Class
End Namespace