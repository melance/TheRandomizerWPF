Namespace ValueConverters
    <ValueConversion(GetType(Int32), GetType(Visibility))>
    Public Class IntegerToVisibilityConverter
        Inherits Markup.MarkupExtension
        Implements IValueConverter

        Public Sub New()
        End Sub

        Public Overrides Function ProvideValue(serviceProvider As IServiceProvider) As Object
            Return Me
        End Function

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            If CInt(value) = 0 Then
                Return Visibility.Collapsed
            Else
                Return Visibility.Visible
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace