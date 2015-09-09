Namespace Tools
    Public Class PreviewXML

        Public Property XML As String
            Get
                Return webBrowser.DocumentText
            End Get
            Set(value As String)
                webBrowser.DocumentText = value
            End Set
        End Property
    End Class
End Namespace