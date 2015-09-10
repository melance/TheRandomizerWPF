Imports System.Runtime.CompilerServices
Imports System.Reflection
Imports System.Collections.Specialized
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media

Public Module Extensions
    <Extension>
    Public Function GetParentWindow(ByVal extended As DependencyObject) As Window
        Dim parent As DependencyObject = VisualTreeHelper.GetParent(extended)

        If parent Is Nothing Then Return Nothing

        If TypeOf parent Is Window Then Return CType(parent, Window)

        Return parent.GetParentWindow
    End Function

    <Extension>
    Public Function RemoveIlegalCharacters(ByVal extended As String) As String
        Dim invalidChars As Char() = IO.Path.GetInvalidFileNameChars()
        Return extended.Where(Function(x) Not invalidChars.Contains(x)).ToArray()
    End Function

    <Extension>
    Public Function IsValid(obj As DependencyObject) As Boolean
        ' The dependency object is valid if it has no errors and all
        ' of its children (that are dependency objects) are error-free.
        Return Not Validation.GetHasError(obj) AndAlso LogicalTreeHelper.GetChildren(obj).OfType(Of DependencyObject)().All(AddressOf IsValid)
    End Function

    <Extension>
    Public Function Stuff(ByVal extended As String,
                          ByVal index As Int32,
                          ByVal length As Int32,
                          ByVal insert As String) As String
        Dim value As String = extended
        value = value.Remove(index, length)
        Return value.Insert(index, insert)
    End Function

    ''' <summary>
    ''' Adds the ordinal suffix to an integer
    ''' </summary>
    ''' <param name="extended">The integer to add the suffix to</param>
    ''' <returns>The orginal with it's appropriate suffix</returns>
    ''' <remarks>Currently only supports English</remarks>
    <Extension>
    Public Function ToOrdinal(ByVal extended As Int32) As String
        Dim lastDigit As Int32 = CInt(Right(extended.ToString, 1))

        If extended <= 0 Then Return extended.ToString

        Select Case extended Mod 100
            Case 11, 12, 13 : Return extended & "th"
        End Select

        Select Case lastDigit
            Case 1 : Return extended & "st"
            Case 2 : Return extended & "nd"
            Case 3 : Return extended & "rd"
            Case Else : Return extended & "th"
        End Select
    End Function

    ''' <summary>
    ''' Converts an integer to its numeric word
    ''' </summary>
    ''' <param name="extended">The integer to convert</param>
    ''' <returns>The numeric word for the integer</returns>
    ''' <remarks>Currently only supports English</remarks>
    <Extension>
    Public Function ToText(ByVal extended As Integer) As String
        If extended < 0 Then
            Return Convert.ToString("negative ") & ToText(-extended)
        ElseIf extended = 0 Then
            Return ""
        ElseIf extended <= 19 Then
            Return New String() {"one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"}(extended - 1) + " "
        ElseIf extended <= 99 Then
            Return Convert.ToString(New String() {"twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"}(extended \ 10 - 2) + " ") & ToText(extended Mod 10)
        ElseIf extended <= 999 Then
            Return Convert.ToString(ToText(extended \ 100) & Convert.ToString("hundred ")) & ToText(extended Mod 100)
        ElseIf extended <= 999999 Then
            Return Convert.ToString(ToText(extended \ 1000) & Convert.ToString("thousand ")) & ToText(extended Mod 1000)
        ElseIf extended <= 999999999 Then
            Return Convert.ToString(ToText(extended \ 1000000) & Convert.ToString("million ")) & ToText(extended Mod 1000000)
        Else
            Return Convert.ToString(ToText(extended \ 1000000000) & Convert.ToString("billion ")) & ToText(extended Mod 1000000000)
        End If
    End Function

    <Extension>
    Public Sub AppendFormatLine(ByVal extended As StringBuilder,
                                ByVal format As String,
                                ByVal ParamArray args() As Object)
        extended.AppendFormat(format, args)
        extended.AppendLine()
    End Sub

    <Extension>
    Public Sub AddRange(ByVal extended As List(Of String), ByVal source As StringCollection)
        For Each item As String In source
            extended.Add(item)
        Next
    End Sub

    <Extension>
    Public Function ToStringCollection(ByVal extended As List(Of String)) As StringCollection
        Dim value As New StringCollection
        For Each item As String In extended
            value.Add(item)
        Next
        Return value
    End Function

    <Extension>
    Public Function ToList(ByVal extended As StringCollection) As List(Of String)
        Dim value As New List(Of String)
        For Each item As String In extended
            value.Add(item)
        Next
        Return value
    End Function

    ''' <summary>
    ''' Does basic validation of a filename or path
    ''' </summary>
    ''' <param name="extended">The string to validate</param>
    ''' <returns>True if basic validation is passed; False otherwise</returns>
    <Extension()>
    Public Function CheckPathForInvalidCharacters(ByVal extended As String) As Boolean
        If String.IsNullOrWhiteSpace(extended) Then Return False

        For Each badChar As Char In System.IO.Path.GetInvalidPathChars
            If extended.Contains(badChar) Then Return False
        Next

        Return True
    End Function

    ''' <summary>
    ''' Returns the <paramref>extended</paramref> string repeated as many times as indictated 
    ''' by the <paramref>count</paramref> parameter
    ''' </summary>
    ''' <param name="extended">The string to repeat</param>
    ''' <param name="count">The number of times to repeat <paramref>extended</paramref></param>
    ''' <returns>The extended string repeated <paramref>count</paramref> times</returns>
    <Extension()>
    Public Function Repeat(ByVal extended As String, ByVal count As Int32) As String
        Dim value As String = String.Empty
        If count > 0 Then
            For i As Int32 = 1 To count
                value &= extended
            Next
        Else
            Throw New ArgumentException("Value must be greater than 0", "count")
        End If
        Return value
    End Function

    <Extension()>
    Public Function [In](Of T)(ByVal extended As T, ByVal ParamArray values() As T) As Boolean
        If GetType(T) = GetType(String) Then
            Dim stringValues() As String = Array.ConvertAll(values, AddressOf Convert.ToString)
            Return extended.ToString.In(StringComparison.InvariantCultureIgnoreCase, stringValues)
        Else
            For Each item As T In values
                If extended.Equals(item) Then Return True
            Next
        End If
        Return False
    End Function

    <Extension()>
    Public Function [In](ByVal extended As String,
                     ByVal comparison As StringComparison,
                     ByVal ParamArray values() As String) As Boolean
        For Each value As String In values
            If extended.Equals(value, comparison) Then Return True
        Next
        Return False
    End Function

    <Extension>
    Public Function IsAlphaNumeric(ByVal extended As String) As Boolean
        Return extended.IsAlphaNumeric(False)
    End Function

    <Extension>
    Public Function IsAlphaNumeric(ByVal extended As String,
                                   ByVal allowWhitespace As Boolean) As Boolean
        Return extended.ToArray.Where(Function(c As Char) Not Char.IsLetterOrDigit(c) AndAlso Not (allowWhitespace AndAlso Char.IsWhiteSpace(c))).Count = 0
    End Function

    <Extension>
    Public Function Remove(ByVal extended As String,
                               ByVal ParamArray items() As String) As String
        Dim value As String = extended
        For Each item As String In items
            value = value.Replace(item, String.Empty)
        Next
        Return value
    End Function

    <Extension>
    Public Function Val(ByVal extended As String) As Int32
        Return CInt(extended.Val(False))
    End Function

    <Extension>
    Public Function Val(ByVal extended As String,
                            ByVal allowDecimal As Boolean) As Double
        Dim decimalFound As Boolean
        Dim signFound As Boolean
        Dim value As String = String.Empty

        For Each c As Char In extended
            If Char.IsNumber(c) Then
                value &= c
            ElseIf allowDecimal AndAlso Not decimalFound AndAlso c = "."c Then
                value &= c
                decimalFound = True
            ElseIf Not signFound AndAlso "+-".Contains(c) Then
                value &= c
                signFound = True
            End If
        Next
        Return CDbl(value)
    End Function

    <Extension>
    Public Function GetProperty(ByVal extended() As PropertyInfo, ByVal name As String, ByVal ignoreCase As Boolean) As PropertyInfo
        If ignoreCase Then
            Return extended.FirstOrDefault(Function(p As PropertyInfo) p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
        Else
            Return extended.FirstOrDefault(Function(p As PropertyInfo) p.Name.Equals(name))
        End If
    End Function

End Module