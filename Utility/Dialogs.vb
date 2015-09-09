Imports Microsoft.WindowsAPICodePack.Dialogs
Imports Microsoft.WindowsAPICodePack.Shell

Public NotInheritable Class Dialogs

    Private Sub New()
    End Sub

    Public Class FileDialogResult

        Public Sub New()
        End Sub

        Public Sub New(ByVal result As CommonFileDialogResult)
            Me.DialogResult = result
        End Sub

        Public Sub New(ByVal result As CommonFileDialogResult,
                       ByVal fileName As String)
            Me.DialogResult = result
            Me.Filename = fileName
        End Sub

        Public Sub New(ByVal result As CommonFileDialogResult,
                       ByVal fileNames As List(Of String))
            Me.DialogResult = result
            Me.Filenames = fileNames
        End Sub

        Public Property DialogResult As CommonFileDialogResult
        Public Property Filename As String
        Public Property Filenames As List(Of String)
    End Class

    Public Shared Function SaveFileDialog(ByVal owner As Windows.Window,
                                          ByVal title As String,
                                          ByVal filters As CommonFileDialogFilterCollection,
                                          ByVal defaultExtension As String,
                                          ByVal defaultFileName As String,
                                          ByVal places As Dictionary(Of String, FileDialogAddPlaceLocation)) As FileDialogResult
        Dim dlg As New CommonSaveFileDialog
        dlg.Title = title
        For Each filter As CommonFileDialogFilter In filters
            dlg.Filters.Add(filter)
        Next
        dlg.AddToMostRecentlyUsedList = True
        dlg.DefaultExtension = defaultExtension
        dlg.DefaultFileName = defaultFileName
        dlg.ShowPlacesList = True

        If places IsNot Nothing Then
            For Each place As KeyValuePair(Of String, FileDialogAddPlaceLocation) In places
                dlg.AddPlace(place.Key, place.Value)
            Next
        End If

        If dlg.ShowDialog(owner) = CommonFileDialogResult.Ok Then
            Return New FileDialogResult(CommonFileDialogResult.Ok,
                                        dlg.FileName)
        End If

        Return New FileDialogResult(CommonFileDialogResult.Cancel)
    End Function

    Public Shared Function OpenFileDialog(ByVal owner As Windows.Window,
                                          ByVal title As String,
                                          ByVal filters As CommonFileDialogFilterCollection,
                                          ByVal defaultExtension As String,
                                          ByVal defaultFilename As String,
                                          ByVal defaultDirectory As String,
                                          ByVal allowMultiples As Boolean,
                                          ByVal places As Dictionary(Of String, FileDialogAddPlaceLocation)) As FileDialogResult
        Dim dlg As New CommonOpenFileDialog
        dlg.Title = title
        For Each filter As CommonFileDialogFilter In filters
            dlg.Filters.Add(filter)
        Next

        dlg.AddToMostRecentlyUsedList = True
        dlg.DefaultExtension = defaultExtension
        dlg.DefaultFileName = defaultFilename
        dlg.Multiselect = allowMultiples
        dlg.ShowPlacesList = True

        If places IsNot Nothing Then
            For Each place As KeyValuePair(Of String, FileDialogAddPlaceLocation) In places
                dlg.AddPlace(place.Key, place.Value)
            Next
        End If
        If dlg.ShowDialog(owner) = CommonFileDialogResult.Ok Then
            If allowMultiples Then
                Return New FileDialogResult(CommonFileDialogResult.Ok,
                                            New List(Of String)(dlg.FileNames))
            Else
                Return New FileDialogResult(CommonFileDialogResult.Ok,
                                            dlg.FileName)
            End If
        End If

        Return New FileDialogResult(CommonFileDialogResult.Cancel)
    End Function

End Class
