Imports Grammars.Assignment
Imports System.Windows.Forms
Imports Utility

Public Class AssignmentGrammarEditor

#Region "Properties"
    Public ReadOnly Property Grammar As AssignmentGrammar
        Get
            Return DirectCast(DataContext, AssignmentGrammar)
        End Get
    End Property
#End Region

#Region "Event Handlers"
    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs)
        Dim dlg As New OpenFileDialog
        dlg.Filter = "Randomizer Library Files (*.lib.xml)|*.lib.xml"
        dlg.Multiselect = True
        dlg.SupportMultiDottedExtensions = True
        If dlg.ShowDialog = DialogResult.OK Then
            For Each fileName As String In dlg.FileNames
                Grammar.Import.Add(fileName)
            Next
        End If
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As RoutedEventArgs)
        If lstImports.SelectedItem IsNot Nothing Then
            lstImports.Items.Remove(lstImports.SelectedItem)
        End If
    End Sub
#End Region

End Class
