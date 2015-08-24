Imports System.Xml.Serialization

Public Class Required
    <XmlAttribute("name")>
    Public Property Name As String
    <XmlText>
    Public Property Path As String
End Class
