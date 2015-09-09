Imports System.Windows.Forms
Imports Grammars.Phonotactics

Namespace Tools
    Public Class PhonotacticsGenerator

        Public Shared Generate As New RoutedUICommand("Generate",
                                                      "Generate",
                                                      GetType(PhonotacticsGenerator),
                                                      New InputGestureCollection From {New KeyGesture(Key.G, ModifierKeys.Alt)})

        Private Sub GenerateCommand(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
            txtPreview.Text = PhonotacticsGrammar.GenerateGrammar(usrGrammarInfo.GrammarName,
                                                                  usrGrammarInfo.Description,
                                                                  usrGrammarInfo.Author,
                                                                  usrGrammarInfo.Tags,
                                                                  usrGrammarInfo.SupportsMaxLength,
                                                                  txtSourceFile.Text,
                                                                  txtDefinitionFile.Text,
                                                                  CInt(txtWeightLimit.Value)).ToString
        End Sub

        Private Sub CanExecuteGenerateCommand(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            Dim valid As Boolean
            valid = valid OrElse Not String.IsNullOrEmpty(txtDefinitionFile.Text) AndAlso IO.File.Exists(txtDefinitionFile.Text)
            valid = valid OrElse Not String.IsNullOrEmpty(txtSourceFile.Text) AndAlso IO.File.Exists(txtSourceFile.Text)
            e.CanExecute = valid
        End Sub

        Private Sub SaveCommand(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
            Dim dlg As New SaveFileDialog
            dlg.Title = "Save Grammar File"
            dlg.DefaultExt = ".rnd.xml"
            dlg.Filter = "Randomizer Grammar Files (*.rnd.xml)|*.rnd.xml"
            If dlg.ShowDialog = Forms.DialogResult.OK Then
                IO.File.WriteAllText(dlg.FileName, txtPreview.Text)
            End If
        End Sub

        Private Sub CanExecuteSaveCommand(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            e.CanExecute = Not String.IsNullOrWhiteSpace(txtPreview.Text)
        End Sub

        Private Sub btnOpenSourceFile_Click(sender As Object, e As RoutedEventArgs)
            Dim dlg As New OpenFileDialog
            dlg.Title = "Select Source File"
            dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            dlg.FileName = txtSourceFile.Text
            If dlg.ShowDialog = Forms.DialogResult.OK Then
                txtSourceFile.Text = dlg.FileName
                txtSourceFile.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource()
            End If
        End Sub

        Private Sub txtSourceFile_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
            txtSourceFile.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource()
        End Sub

        Private Sub btnOpenDefinitionFile_Click(sender As Object, e As RoutedEventArgs)
            Dim dlg As New OpenFileDialog
            dlg.Title = "Select Source File"
            dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            dlg.FileName = txtDefinitionFile.Text
            If dlg.ShowDialog = Forms.DialogResult.OK Then
                txtDefinitionFile.Text = dlg.FileName
                txtDefinitionFile.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource()
            End If
        End Sub

        Private Sub txtDefinitionFile_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
            txtDefinitionFile.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty).UpdateSource()
        End Sub
    End Class
End Namespace