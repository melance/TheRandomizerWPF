Option Strict Off

Namespace Tools
    Public Class Analyze
        Public Property HTML As String
            Get
                Return webBrowser.DocumentText
            End Get
            Set(value As String)
                webBrowser.DocumentText = value
            End Set
        End Property
    End Class
End Namespace