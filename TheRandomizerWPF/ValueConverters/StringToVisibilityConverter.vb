Imports System.Globalization

Namespace ValueConverters
    <ValueConversion(GetType(String), GetType(Visibility))>
    Public Class StringToVisibilityConverter
        Inherits Markup.MarkupExtension
        Implements IValueConverter

        Public Sub New()
        End Sub

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            If [String].IsNullOrEmpty(DirectCast(value, String)) Then
                Return Visibility.Collapsed
            Else
                Return Visibility.Visible
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ProvideValue(serviceProvider As IServiceProvider) As Object
            Return Me
        End Function
    End Class
End Namespace