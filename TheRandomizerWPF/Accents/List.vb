
Namespace Accents
    Friend NotInheritable Class List

        Private Sub New()
        End Sub

        Public Shared ReadOnly Property CustomAccents() As Dictionary(Of String, Uri)
            Get
                Dim value As New Dictionary(Of String, Uri)

                value.Add("Gray", New Uri("pack://application:,,,/Accents/Gray.xaml"))

                Return value
            End Get
        End Property


    End Class
End Namespace