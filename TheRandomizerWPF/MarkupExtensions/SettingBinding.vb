
Namespace MarkupExtensions
    Public Class SettingBinding
        Inherits Binding

        Public Sub New()
            Initialize()
        End Sub

        Public Sub New(path As String)
            MyBase.New(path)
            Initialize()
        End Sub

        Private Sub Initialize()
            Me.Source = TheRandomizerWPF.My.Settings
            Me.Mode = BindingMode.TwoWay
        End Sub
    End Class
End Namespace