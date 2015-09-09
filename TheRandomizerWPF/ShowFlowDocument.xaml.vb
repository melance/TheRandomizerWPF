Imports System.IO

Public Class ShowFlowDocument

    Public Property Document As String
        Get
            Return flwViewer.Document.ToString
        End Get
        Set(value As String)
            Dim reader As New MemoryStream(Text.Encoding.UTF8.GetBytes(value))
            Dim document As FlowDocument = DirectCast(System.Windows.Markup.XamlReader.Load(reader), FlowDocument)
            flwViewer.Document = document
        End Set
    End Property

End Class
