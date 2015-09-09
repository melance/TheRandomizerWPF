Imports System.Windows.Forms
Imports System.Drawing
Imports Grammars

Public Class Preferences

    Private ReadOnly DEFAULT_RESULT_FONT As New Font("Consolas", 12)

    Private Sub Help_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        Windows.Forms.Help.ShowHelp(Nothing,
                              IO.Path.Combine(My.Application.Info.DirectoryPath, My.Resources.HelpFile),
                              Forms.HelpNavigator.TopicId,
                              My.Resources.PreferencesHelpId)
    End Sub

    Private Function ConvertToFont(ByVal text As String) As Font
        Dim fc As New FontConverter
        If fc.IsValid(text) Then Return DirectCast(fc.ConvertFromString(text), Font)
        Return Nothing
    End Function

    Private Function ConvertFromFont(ByVal font As Font) As String
        Dim fc As New FontConverter
        Return fc.ConvertToString(font)
    End Function

    Private Sub btnOk_Click(sender As Object, e As RoutedEventArgs) Handles btnOk.Click
        Me.DialogResult = True
        My.Settings.CheckForUpdates = CBool(chkAutoUpdate.IsChecked)
        My.Settings.ShowLoadErrors = CBool(chkShowLoadErrors.IsChecked)
        My.Settings.DefaultResultFont = ConvertToFont(txtResultFont.Text)
        My.Settings.GrammarFilePath = txtCustomGrammarDirectory.Text
        My.Settings.TemporaryPath = txtTempDirectory.Text
        My.Settings.ThemeFilePath = txtThemeDirectory.Text
        My.Settings.Save()
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        Me.DialogResult = False
        Me.Close()
    End Sub

    Private Sub btnTempDirectory_Click(sender As Object, e As RoutedEventArgs) Handles btnTempDirectory.Click
        Dim dlg As New FolderBrowserDialog()
        dlg.SelectedPath = lblTempDirectory.Content.ToString
        If dlg.ShowDialog = Forms.DialogResult.OK Then
            txtTempDirectory.Text = dlg.SelectedPath
        End If
    End Sub

    Private Sub btnResultFont_Click(sender As Object, e As RoutedEventArgs) Handles btnResultFont.Click
        Dim dlg As New System.Windows.Forms.FontDialog
        Dim fc As New FontConverter
        Dim font As Font

        If fc.IsValid(txtResultFont.Text) Then
            font = DirectCast(fc.ConvertFromString(txtResultFont.Text), Font)
        Else
            font = DEFAULT_RESULT_FONT
        End If

        dlg.Font = font
        If dlg.ShowDialog = Forms.DialogResult.OK Then
            txtResultFont.Text = ConvertFromFont(dlg.Font)
        End If
    End Sub

    Private Sub btnCustomGrammarDirectory_Click(sender As Object, e As RoutedEventArgs) Handles btnCustomGrammarDirectory.Click
        Dim dlg As New FolderBrowserDialog()
        dlg.SelectedPath = lblTempDirectory.Content.ToString
        If dlg.ShowDialog = Forms.DialogResult.OK Then
            txtCustomGrammarDirectory.Text = dlg.SelectedPath
        End If
    End Sub

    Private Sub Preferences_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        chkAutoUpdate.IsChecked = My.Settings.CheckForUpdates
        chkShowLoadErrors.IsChecked = My.Settings.ShowLoadErrors
        txtCustomGrammarDirectory.Text = My.Settings.GrammarFilePath
        txtResultFont.Text = ConvertFromFont(My.Settings.DefaultResultFont)
        txtTempDirectory.Text = My.Settings.TemporaryPath
        txtThemeDirectory.Text = My.Settings.ThemeFilePath
    End Sub

    Private Sub btnThemeDirectory_Click(sender As Object, e As RoutedEventArgs) Handles btnThemeDirectory.Click
        Dim dlg As New FolderBrowserDialog()
        dlg.SelectedPath = lblTempDirectory.Content.ToString
        If dlg.ShowDialog = Forms.DialogResult.OK Then
            txtThemeDirectory.Text = dlg.SelectedPath
        End If
    End Sub
End Class
