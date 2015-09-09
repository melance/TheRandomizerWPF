Imports Grammars.Table
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Collections.ObjectModel
Imports Grammars.Assignment.AssignmentGrammar
Imports Grammars.Assignment
Imports Utility

Namespace Tools
    Public Class ItemListConverter

#Region "Classes"
        Public Class FileList
            Inherits ObservableCollection(Of ItemFile)
            Implements IDataErrorInfo

            Public ReadOnly Property [Error] As String Implements IDataErrorInfo.Error
                Get
                    If Me.Count = 0 Then Return "At least one file is required."
                    For Each file As ItemFile In Me
                        If Not String.IsNullOrEmpty(file.Item(HelperMethods.GetPropertyName(Function(i As ItemFile) i.FileName))) OrElse
                           Not String.IsNullOrEmpty(file.Item(HelperMethods.GetPropertyName(Function(i As ItemFile) i.Label))) Then
                            Return "There are errors in the file records."
                        End If
                    Next
                    Return String.Empty
                End Get
            End Property

            Default Public ReadOnly Property ValidationItem(columnName As String) As String Implements IDataErrorInfo.Item
                Get
                    Return String.Empty
                End Get
            End Property
        End Class
#End Region

#Region "Constructor"
        Public Sub New()
            InitializeComponent()
            _grammar = New TableGrammar
            Me.DataContext = _grammar
            grdFileNames.DataContext = Me
        End Sub
#End Region

#Region "Members"
        Private _grammar As TableGrammar
#End Region

#Region "Properties"
        Public Property FileNames As New FileList
#End Region

#Region "Private Methods"
        Private Sub Generate()
            Dim weightAdjustment As Int32 = 0

            If radWeightAscending.IsChecked Then
                weightAdjustment = 1
            ElseIf radWeightDescending.IsChecked Then
                weightAdjustment = -1
            End If

            If radAssignmentGrammar.IsChecked Then
                txtResults.Text = AssignmentGrammar.ItemListConverter(usrGrammarInfo.GrammarName,
                                                                      usrGrammarInfo.Description,
                                                                      usrGrammarInfo.Author,
                                                                      usrGrammarInfo.Tags,
                                                                      usrGrammarInfo.SupportsMaxLength,
                                                                      FileNames,
                                                                      weightAdjustment,
                                                                      CBool(chkRemoveDuplicates.IsChecked),
                                                                      CBool(chkCaseSensitive.IsChecked)).ToString

            ElseIf radAssignmentLibrary.IsChecked Then
                txtResults.Text = Library.ItemListConverter(FileNames,
                                                            weightAdjustment,
                                                            CBool(chkRemoveDuplicates.IsChecked),
                                                            CBool(chkCaseSensitive.IsChecked)).ToString
            End If
        End Sub
#End Region

#Region "Event Handlers"
        Private Sub btnFileName_Click(sender As Object, e As RoutedEventArgs)
            Dim dlg As New System.Windows.Forms.OpenFileDialog
            dlg.Title = "Select Input File"
            dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            dlg.Multiselect = False
            If dlg.ShowDialog = Forms.DialogResult.OK Then
                DirectCast(sender, Button).Tag = dlg.FileName
            End If
        End Sub
#End Region

#Region "Commands"
        Private Sub Save_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            Dim dlg As New System.Windows.Forms.SaveFileDialog
            dlg.Title = "Save Grammar"
            dlg.Filter = "Randomizer Grammar File (*.rnd.xml)|*.rnd.xml|Randomizer Library File (*.lib.xml)|*.lib.xml"
            If radAssignmentGrammar.IsChecked Then
                dlg.FilterIndex = 0
            ElseIf radAssignmentLibrary.IsChecked Then
                dlg.FilterIndex = 1
            End If
            If dlg.ShowDialog = Forms.DialogResult.OK Then
                IO.File.WriteAllText(dlg.FileName, txtResults.Text)
            End If
        End Sub

        Private Sub Save_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = txtResults IsNot Nothing AndAlso Not String.IsNullOrEmpty(txtResults.Text)
        End Sub

        Private Sub SelectAll_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            txtResults.SelectAll()
        End Sub

        Private Sub SelectAll_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = txtResults IsNot Nothing AndAlso Not String.IsNullOrEmpty(txtResults.Text)
        End Sub
        Private Sub Copy_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            txtResults.Copy()
        End Sub

        Private Sub Copy_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = txtResults IsNot Nothing AndAlso Not String.IsNullOrEmpty(txtResults.Text)
        End Sub

        Private Sub Delete_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            txtResults.Clear()
        End Sub

        Private Sub Delete_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = txtResults IsNot Nothing AndAlso Not String.IsNullOrEmpty(txtResults.Text)
        End Sub

        Private Sub Generate_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            Generate()
        End Sub

        Private Sub Generate_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = True
            If usrGrammarInfo.IsEnabled AndAlso Not usrGrammarInfo.IsValid() Then e.CanExecute = False
            If Not String.IsNullOrEmpty(FileNames.Error) Then e.CanExecute = False
        End Sub

        Private Sub SelectNone_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            txtResults.SelectionLength = 0
        End Sub

        Private Sub SelectNone_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = txtResults IsNot Nothing AndAlso Not String.IsNullOrEmpty(txtResults.Text) AndAlso txtResults.SelectionLength > 0
        End Sub
#End Region
    End Class
End Namespace