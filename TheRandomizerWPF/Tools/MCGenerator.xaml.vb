Imports System.Windows.Forms

Namespace Tools
    Public Class MCGenerator

        Public Shared Generate As New RoutedUICommand("Generate",
                                                      "Generate",
                                                      GetType(MCGenerator),
                                                      New InputGestureCollection From {New KeyGesture(Key.G, ModifierKeys.Alt)})

        Private Sub GenerateCommand(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
            txtPreview.Text = Grammars.Assignment.AssignmentGrammar.MCGenerator(usrGrammarInfo.GrammarName,
                                                                                usrGrammarInfo.Description,
                                                                                usrGrammarInfo.Author,
                                                                                usrGrammarInfo.Tags,
                                                                                usrGrammarInfo.SupportsMaxLength,
                                                                                txtFile.Text,
                                                                                CInt(txtSyllableLength.Value),
                                                                                CInt(txtWeightLimit.Value),
                                                                                txtPrefix.Text,
                                                                                CBool(radCreateLibrary.IsChecked)).ToString
        End Sub

        Private Sub CanExecuteGenerateCommand(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            Dim valid As Boolean
            valid = Not String.IsNullOrEmpty(txtFile.Text) AndAlso
                    IO.File.Exists(txtFile.Text) 
            e.CanExecute = valid
        End Sub

        Private Sub SaveCommand(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
            Dim dlg As New SaveFileDialog
            If radCreateGrammar.IsChecked Then
                dlg.Title = "Save Grammar File"
                dlg.DefaultExt = ".rnd.xml"
                dlg.Filter = "Randomizer Grammar Files (*.rnd.xml)|*.rnd.xml"
            Else
                dlg.Title = "Save Library File"
                dlg.DefaultExt = ".lib.xml"
                dlg.Filter = "Randomizer Library Files (*.lib.xml)|*.lib.xml"
            End If
            If dlg.ShowDialog = Forms.DialogResult.OK Then
                IO.File.WriteAllText(dlg.FileName, txtPreview.Text)
            End If
        End Sub

        Private Sub CanExecuteSaveCommand(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            e.CanExecute = Not String.IsNullOrWhiteSpace(txtPreview.Text)
        End Sub

        Private Sub btnOpenFile_Click(sender As Object, e As RoutedEventArgs)
            Dim dlg As New OpenFileDialog
            dlg.Title = "Select Source File"
            dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            dlg.FileName = txtFile.Text
            If dlg.ShowDialog = Forms.DialogResult.OK Then
                txtFile.Text = dlg.FileName
                txtFile.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource()
            End If
        End Sub

        Private Sub txtFile_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
            txtFile.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource()
        End Sub
    End Class
End Namespace