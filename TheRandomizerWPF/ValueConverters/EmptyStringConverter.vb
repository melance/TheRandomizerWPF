
Namespace ValueConverters
    <ValueConversion(GetType(String), GetType(Object))>
    Public Class EmptyStringConverter
        Inherits Markup.MarkupExtension
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            If String.IsNullOrEmpty(DirectCast(value, String)) Then Return Nothing
            Return value
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException
        End Function

        Public Overrides Function ProvideValue(serviceProvider As IServiceProvider) As Object
            Return Me
        End Function
    End Class
End Namespace