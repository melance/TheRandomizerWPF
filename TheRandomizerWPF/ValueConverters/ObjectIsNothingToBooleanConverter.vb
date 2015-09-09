Namespace ValueConverters
    <ValueConversion(GetType(Object), GetType(Boolean))>
    Public Class ObjectIsNothingToBooleanConverter
        Inherits Markup.MarkupExtension
        Implements IValueConverter

        Public Sub New()
        End Sub

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            If value Is Nothing Then Return False
            Return True
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException
        End Function

        Public Overrides Function ProvideValue(serviceProvider As IServiceProvider) As Object
            Return Me
        End Function
    End Class
End Namespace