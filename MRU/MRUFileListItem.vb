Imports System.Windows.Controls
Imports System.IO

Public Class MRUFileListItem
    Inherits MenuItem

#Region "Constructor"
    Public Sub New(ByVal fileName As String)
        Me.FileName = fileName
    End Sub
#End Region

#Region "Members"
    Private _fileName As String
#End Region

#Region "Properties"
    Public Property FileName As String
        Get
            Return _fileName
        End Get
        Set(value As String)
            If _fileName <> value Then
                _fileName = value
                Header = ShortenPath(_fileName)
            End If
        End Set
    End Property

    Public Property MaxLength As Int32 = 50
#End Region

#Region "Private Methods"
    Private Function ShortenPath(ByVal pathname As String) As String
        If pathname.Length <= maxLength Then
            Return pathname
        End If

        Dim root As String = Path.GetPathRoot(pathname)
        If root.Length > 3 Then
            root += Path.DirectorySeparatorChar
        End If

        Dim elements As String() = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)

        Dim filenameIndex As Integer = elements.GetLength(0) - 1

        If elements.GetLength(0) = 1 Then
            ' pathname is just a root and filename
            If elements(0).Length > 5 Then
                ' long enough to shorten
                ' if path is a UNC path, root may be rather long
                If root.Length + 6 >= maxLength Then
                    Return (root & elements(0).Substring(0, 3)) + "..."
                Else
                    Return pathname.Substring(0, maxLength - 3) + "..."
                End If
            End If
        ElseIf (root.Length + 4 + elements(filenameIndex).Length) > maxLength Then
            ' pathname is just a root and filename
            root += "...\"

            Dim len As Integer = elements(filenameIndex).Length
            If len < 6 Then
                Return root + elements(filenameIndex)
            End If

            If (root.Length + 6) >= maxLength Then
                len = 3
            Else
                len = maxLength - root.Length - 3
            End If
            Return (root & elements(filenameIndex).Substring(0, len)) + "..."
        ElseIf elements.GetLength(0) = 2 Then
            Return (root & Convert.ToString("...\")) + elements(1)
        Else
            Dim len As Integer = 0
            Dim begin As Integer = 0

            For i As Integer = 0 To filenameIndex - 1
                If elements(i).Length > len Then
                    begin = i
                    len = elements(i).Length
                End If
            Next

            Dim totalLength As Integer = pathname.Length - len + 3
            Dim [end] As Integer = begin + 1

            While totalLength > maxLength
                If begin > 0 Then
                    totalLength -= elements(System.Threading.Interlocked.Decrement(begin)).Length - 1
                End If

                If totalLength <= maxLength Then
                    Exit While
                End If

                If [end] < filenameIndex Then
                    totalLength -= elements(System.Threading.Interlocked.Increment([end])).Length - 1
                End If

                If begin = 0 AndAlso [end] = filenameIndex Then
                    Exit While
                End If
            End While

            ' assemble final string

            For i As Integer = 0 To begin - 1
                root += elements(i) + "\"c
            Next

            root += "...\"

            For i As Integer = [end] To filenameIndex - 1
                root += elements(i) + "\"c
            Next

            Return root + elements(filenameIndex)
        End If
        Return pathname
    End Function
#End Region
End Class
