Imports System.Text
Imports System.Windows

''' <summary>      
''' Helper to  encode and set HTML fragment to clipboard.<br/>      
''' See <br/>      
''' <seealso  cref="CreateDataObject"/>.      
'''  </summary>      
''' <remarks>      
''' The MIT License  (MIT) Copyright (c) 2014 Arthur Teplitzki.      
'''  </remarks>      
Public NotInheritable Class ClipboardHelper

    Private Sub New()
    End Sub

#Region "Fields and Consts"

    ''' <summary>      
    ''' The string contains index references to  other spots in the string, so we need placeholders so we can compute the  offsets. <br/>      
    ''' The  <![CDATA[<<<<<<<]]>_ strings are just placeholders.  We'll back-patch them actual values afterwards. <br/>      
    ''' The string layout  (<![CDATA[<<<]]>) also ensures that it can't appear in the body  of the html because the <![CDATA[<]]> <br/>      
    ''' character must be escaped. <br/>      
    ''' </summary>      
    Private Const Header As String = "Version:0.9      " & vbCr & vbLf & "StartHTML:<<<<<<<<1      " & vbCr & vbLf & "EndHTML:<<<<<<<<2      " & vbCr & vbLf & "StartFragment:<<<<<<<<3      " & vbCr & vbLf & "EndFragment:<<<<<<<<4      " & vbCr & vbLf & "StartSelection:<<<<<<<<3      " & vbCr & vbLf & "EndSelection:<<<<<<<<4"

    ''' <summary>      
    ''' html comment to point the beginning of  html fragment      
    ''' </summary>      
    Public Const StartFragment As String = "<!--StartFragment-->"

    ''' <summary>      
    ''' html comment to point the end of html  fragment      
    ''' </summary>      
    Public Const EndFragment As String = "<!--EndFragment-->"

    ''' <summary>      
    ''' Used to calculate characters byte count  in UTF-8      
    ''' </summary>      
    Private Shared ReadOnly _byteCount As Char() = New Char(0) {}

#End Region


    ''' <summary>      
    ''' Create <see  cref="DataObject"/> with given html and plain-text ready to be  used for clipboard or drag and drop.<br/>      
    ''' Handle missing  <![CDATA[<html>]]> tags, specified startend segments and Unicode  characters.      
    ''' </summary>      
    ''' <remarks>      
    ''' <para>      
    ''' Windows Clipboard works with UTF-8  Unicode encoding while .NET strings use with UTF-16 so for clipboard to  correctly      
    ''' decode Unicode string added to it from  .NET we needs to be re-encoded it using UTF-8 encoding.      
    ''' </para>      
    ''' <para>      
    ''' Builds the CF_HTML header correctly for  all possible HTMLs<br/>      
    ''' If given html contains start/end  fragments then it will use them in the header:      
    '''  <code><![CDATA[<html><body><!--StartFragment-->hello  <b>world</b><!--EndFragment--></body></html>]]></code>      
    ''' If given html contains html/body tags  then it will inject start/end fragments to exclude html/body tags:      
    '''  <code><![CDATA[<html><body>hello  <b>world</b></body></html>]]></code>      
    ''' If given html doesn't contain html/body  tags then it will inject the tags and start/end fragments properly:      
    ''' <code><![CDATA[hello  <b>world</b>]]></code>      
    ''' In all cases creating a proper CF_HTML  header:<br/>      
    ''' <code>      
    ''' <![CDATA[      
    ''' Version:1.0      
    ''' StartHTML:000000177      
    ''' EndHTML:000000329      
    ''' StartFragment:000000277      
    ''' EndFragment:000000295      
    ''' StartSelection:000000277      
    ''' EndSelection:000000277      
    ''' <!DOCTYPE HTML PUBLIC  "-//W3C//DTD HTML 4.0 Transitional//EN">      
    '''  <html><body><!--StartFragment-->hello  <b>world</b><!--EndFragment--></body></html>      
    ''' ]]>      
    ''' </code>      
    ''' See format specification here: [http://msdn.microsoft.com/library/default.asp?url=/workshop/networking/clipboard/htmlclipboard.asp][9]      
    ''' </para>      
    ''' </remarks>      
    ''' <param name="html">a  html fragment</param>      
    ''' <param  name="plainText">the plain text</param>      
    Private Shared Function CreateDataObject(html As String, plainText As String) As DataObject
        html = If(html, [String].Empty)
        Dim htmlFragment As String = GetHtmlDataString(html)

        ' re-encode the string so it will work  correctly (fixed in CLR 4.0)      
        If Environment.Version.Major < 4 AndAlso html.Length <> Encoding.UTF8.GetByteCount(html) Then
            htmlFragment = Encoding.[Default].GetString(Encoding.UTF8.GetBytes(htmlFragment))
        End If

        Dim dataObject As New DataObject()
        dataObject.SetData(DataFormats.Html, htmlFragment)
        dataObject.SetData(DataFormats.Text, plainText)
        dataObject.SetData(DataFormats.UnicodeText, plainText)
        Return dataObject
    End Function

    ''' <summary>      
    ''' Clears clipboard and sets the given  HTML and plain text fragment to the clipboard, providing additional  meta-information for HTML.<br/>      
    ''' See <see  cref="CreateDataObject"/> for HTML fragment details.<br/>      
    ''' </summary>      
    ''' <example>      
    '''  ClipboardHelper.CopyToClipboard("Hello <b>World</b>",  "Hello World");      
    ''' </example>      
    ''' <param name="html">a  html fragment</param>      
    ''' <param  name="plainText">the plain text</param>      
    Public Shared Sub CopyToClipboard(html As String, plainText As String)
        Dim dataObject As DataObject = CreateDataObject(html, plainText)
        Clipboard.SetDataObject(dataObject, True)
    End Sub

    ''' <summary>      
    ''' Generate HTML fragment data string with  header that is required for the clipboard.      
    ''' </summary>      
    ''' <param name="html">the  html to generate for</param>      
    ''' <returns>the resulted  string</returns>      
    Private Shared Function GetHtmlDataString(html As String) As String
        Dim sb As New StringBuilder()
        sb.AppendLine(Header)
        sb.AppendLine("<!DOCTYPE HTML  PUBLIC ""-//W3C//DTD HTML 4.0  Transitional//EN"">")

        ' if given html already provided the  fragments we won't add them      
        Dim fragmentStart As Integer, fragmentEnd As Integer
        Dim fragmentStartIdx As Integer = html.IndexOf(StartFragment, StringComparison.OrdinalIgnoreCase)
        Dim fragmentEndIdx As Integer = html.LastIndexOf(EndFragment, StringComparison.OrdinalIgnoreCase)

        ' if html tag is missing add it  surrounding the given html (critical)      
        Dim htmlOpenIdx As Integer = html.IndexOf("<html", StringComparison.OrdinalIgnoreCase)
        Dim htmlOpenEndIdx As Integer = If(htmlOpenIdx > -1, html.IndexOf(">"c, htmlOpenIdx) + 1, -1)
        Dim htmlCloseIdx As Integer = html.LastIndexOf("</html", StringComparison.OrdinalIgnoreCase)

        If fragmentStartIdx < 0 AndAlso fragmentEndIdx < 0 Then
            Dim bodyOpenIdx As Integer = html.IndexOf("<body", StringComparison.OrdinalIgnoreCase)
            Dim bodyOpenEndIdx As Integer = If(bodyOpenIdx > -1, html.IndexOf(">"c, bodyOpenIdx) + 1, -1)

            If htmlOpenEndIdx < 0 AndAlso bodyOpenEndIdx < 0 Then
                ' the given html doesn't  contain html or body tags so we need to add them and place start/end fragments  around the given html only      
                sb.Append("<html><body>")
                sb.Append(StartFragment)
                fragmentStart = GetByteCount(sb)
                sb.Append(html)
                fragmentEnd = GetByteCount(sb)
                sb.Append(EndFragment)
                sb.Append("</body></html>")
            Else
                ' insert start/end fragments  in the proper place (related to html/body tags if exists) so the paste will  work correctly      
                Dim bodyCloseIdx As Integer = html.LastIndexOf("</body", StringComparison.OrdinalIgnoreCase)

                If htmlOpenEndIdx < 0 Then
                    sb.Append("<html>")
                Else
                    sb.Append(html, 0, htmlOpenEndIdx)
                End If

                If bodyOpenEndIdx > -1 Then
                    sb.Append(html, If(htmlOpenEndIdx > -1, htmlOpenEndIdx, 0), bodyOpenEndIdx - (If(htmlOpenEndIdx > -1, htmlOpenEndIdx, 0)))
                End If

                sb.Append(StartFragment)
                fragmentStart = GetByteCount(sb)

                Dim innerHtmlStart As Int32 = If(bodyOpenEndIdx > -1, bodyOpenEndIdx, (If(htmlOpenEndIdx > -1, htmlOpenEndIdx, 0)))
                Dim innerHtmlEnd As Int32 = If(bodyCloseIdx > -1, bodyCloseIdx, (If(htmlCloseIdx > -1, htmlCloseIdx, html.Length)))
                sb.Append(html, innerHtmlStart, innerHtmlEnd - innerHtmlStart)

                fragmentEnd = GetByteCount(sb)
                sb.Append(EndFragment)

                If innerHtmlEnd < html.Length Then
                    sb.Append(html, innerHtmlEnd, html.Length - innerHtmlEnd)
                End If

                If htmlCloseIdx < 0 Then
                    sb.Append("</html>")
                End If
            End If
        Else
            ' handle html with existing  startend fragments just need to calculate the correct bytes offset (surround  with html tag if missing)      
            If htmlOpenEndIdx < 0 Then
                sb.Append("<html>")
            End If
            Dim start As Integer = GetByteCount(sb)
            sb.Append(html)
            fragmentStart = start + GetByteCount(sb, start, start + fragmentStartIdx) + StartFragment.Length
            fragmentEnd = start + GetByteCount(sb, start, start + fragmentEndIdx)
            If htmlCloseIdx < 0 Then
                sb.Append("</html>")
            End If
        End If

        ' Back-patch offsets (scan only the  header part for performance)      
        sb.Replace("<<<<<<<<1", Header.Length.ToString("D9"), 0, Header.Length)
        sb.Replace("<<<<<<<<2", GetByteCount(sb).ToString("D9"), 0, Header.Length)
        sb.Replace("<<<<<<<<3", fragmentStart.ToString("D9"), 0, Header.Length)
        sb.Replace("<<<<<<<<4", fragmentEnd.ToString("D9"), 0, Header.Length)

        Return sb.ToString()
    End Function

    ''' <summary>      
    ''' Calculates the number of bytes produced  by encoding the string in the string builder in UTF-8 and not .NET default  string encoding.      
    ''' </summary>      
    ''' <param name="sb">the  string builder to count its string</param>      
    ''' <param  name="start">optional: the start index to calculate from (default  - start of string)</param>      
    ''' <param  name="end">optional: the end index to calculate to (default - end  of string)</param>      
    ''' <returns>the number of bytes  required to encode the string in UTF-8</returns>      
    Private Shared Function GetByteCount(sb As StringBuilder, Optional start As Integer = 0, Optional [end] As Integer = -1) As Integer
        Dim count As Integer = 0
        [end] = If([end] > -1, [end], sb.Length)
        For i As Integer = start To [end] - 1
            _byteCount(0) = sb(i)
            count += Encoding.UTF8.GetByteCount(_byteCount)
        Next
        Return count
    End Function
End Class

