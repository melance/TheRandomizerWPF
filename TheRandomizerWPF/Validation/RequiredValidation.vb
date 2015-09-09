Namespace Validation
    Public Class RequiredValidation
        Inherits ValidationRule

        Public Overloads Overrides Function Validate(value As Object, cultureInfo As Globalization.CultureInfo) As ValidationResult
            If value Is Nothing Then Return New ValidationResult(False, "This field is required.")
            Select Case value.GetType
                Case GetType(String)
                    If String.IsNullOrWhiteSpace(value.ToString) Then
                        Return New ValidationResult(False, "This field is required.")
                    Else
                        Return ValidationResult.ValidResult
                    End If
                Case Else
                    If value Is Nothing Then
                        Return New ValidationResult(False, "This field is required.")
                    Else
                        Return ValidationResult.ValidResult
                    End If
            End Select
        End Function
    End Class
End Namespace