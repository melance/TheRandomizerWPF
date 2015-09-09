
Namespace ValueConverters
    <ValueConversion(GetType(String), GetType(String))>
    Public Class FileNameConverter
        Inherits Markup.MarkupExtension
        Implements IValueConverter

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Function ProvideValue(serviceProvider As IServiceProvider) As Object
            Return Me
        End Function

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            Return IO.Path.GetFileName(CStr(value))
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException
        End Function
    End Class
End Namespace