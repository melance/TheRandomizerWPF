Imports Grammars
Imports TheRandomizerWPF.MainWindow
Imports System.Windows.Forms
Imports Grammars.Assignment
Imports Grammars.Dice
Imports Grammars.LUA
Imports Grammars.Phonotactics
Imports Grammars.Table
Imports Utility
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports TheRandomizerWPF.Settings

Namespace Tools
    Public Class GrammarEditor
        Implements INotifyPropertyChanged

#Region "Events"
        Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
#End Region

#Region "Constants"
        Private Const GRAMMAR_FILE_FILTER As String = "Randomizer Grammar Files (*.rnd.xml)|*.rnd.xml"
        Private Const OPEN_RANDOMIZER_TITLE As String = "Open Randomizer Grammar File"
        Private Const SAVE_RANDOMIZER_FILE As String = "Save Randomizer Grammar File"
#End Region

#Region "Members"
        Private _fileName As String
#End Region

#Region "Dependency Properties"
        Public Shared ReadOnly BaseGrammarProperty As DependencyProperty =
                           DependencyProperty.Register("Grammar",
                                                       GetType(BaseGrammar),
                                                       GetType(GrammarEditor),
                                                       New PropertyMetadata(Nothing))
        Public Shared ReadOnly GrammarAvailableProperty As DependencyProperty =
                            DependencyProperty.Register("GrammarAvailable",
                                                        GetType(Boolean),
                                                        GetType(GrammarEditor),
                                                        New PropertyMetadata(Nothing))
        Public Shared ReadOnly MRUStorageProperty As DependencyProperty =
                            DependencyProperty.Register("MRUStorage",
                                                        GetType(MRU.IStorage),
                                                        GetType(GrammarEditor),
                                                        New PropertyMetadata(Nothing))
#End Region

#Region "Properties"
        Public Property MRUStorage As MRU.IStorage
            Get
                Return DirectCast(GetValue(MRUStorageProperty), MRU.IStorage)
            End Get
            Set(value As MRU.IStorage)
                SetValue(MRUStorageProperty, value)
            End Set
        End Property

        Public Property Grammar As BaseGrammar
            Get
                Return DirectCast(GetValue(BaseGrammarProperty), BaseGrammar)
            End Get
            Set(value As BaseGrammar)
                SetValue(BaseGrammarProperty, value)
                GrammarAvailable = True
                If Me.DataContext Is Nothing Then Me.DataContext = Grammar
            End Set
        End Property

        Public Property GrammarAvailable As Boolean?
            Get
                Return DirectCast(GetValue(GrammarAvailableProperty), Boolean)
            End Get
            Set(value As Boolean?)
                SetValue(GrammarAvailableProperty, value)
            End Set
        End Property
#End Region

#Region "Protected Methods"
        Protected Sub OnNotifyPropertyChanged(<CallerMemberNameAttribute> Optional ByVal propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
#End Region

#Region "Private Methods"
        Private Sub OpenFile()
            Dim dlg As New OpenFileDialog
            dlg.Filter = GRAMMAR_FILE_FILTER
            dlg.Title = OPEN_RANDOMIZER_TITLE
            If dlg.ShowDialog = Forms.DialogResult.OK Then
                OpenFile(dlg.FileName)
            End If
        End Sub

        Private Sub OpenFile(ByVal fileName As String)
            Grammar = BaseGrammar.Open(fileName)
            _fileName = fileName
            pnlDetails.Children.Clear()
            LoadDetailControl()
            mnuRecent.AddItem(fileName)
        End Sub

        Private Sub LoadDetailControl()
            Select Case Grammar.GetType
                Case GetType(AssignmentGrammar)
                    Dim age As New AssignmentGrammarEditor
                    age.DataContext = Grammar
                    pnlDetails.Children.Clear()
                    pnlDetails.Children.Add(age)
                Case GetType(PhonotacticsGrammar)
                    Dim pge As New PhonotacticsGrammarEditor
                    pge.DataContext = Grammar
                    pnlDetails.Children.Clear()
                    pnlDetails.Children.Add(pge)
                Case GetType(DiceRoll)
                    Dim drge As New DiceRollGrammarEditor
                    drge.DataContext = Grammar
                    pnlDetails.Children.Clear()
                    pnlDetails.Children.Add(drge)
                Case GetType(LUAGrammar)
                    Dim lge As New LUAGrammarEditor
                    lge.DataContext = Grammar
                    pnlDetails.Children.Clear()
                    pnlDetails.Children.Add(lge)
                Case GetType(TableGrammar)
                    Dim tge As New TableGrammarEditor
                    tge.DataContext = Grammar
                    pnlDetails.Children.Clear()
                    pnlDetails.Children.Add(tge)
            End Select
        End Sub

        Private Function SaveFile() As Boolean
            If String.IsNullOrEmpty(_fileName) Then
                Return SaveFileAs()
            Else
                IO.File.WriteAllText(_fileName, Grammar.ToString)
                mnuRecent.AddItem(_fileName)
                Return True
            End If
        End Function

        Private Function SaveFileAs() As Boolean
            Dim dlg As New SaveFileDialog
            dlg.Filter = GRAMMAR_FILE_FILTER
            dlg.Title = SAVE_RANDOMIZER_FILE
            If dlg.ShowDialog = Forms.DialogResult.OK Then
                _fileName = dlg.FileName
                Return SaveFile()
            End If
            Return False
        End Function

        Private Sub NewFile(ByVal grammarType As Type)
            Try
                Grammar = CType(System.Activator.CreateInstance(grammarType), BaseGrammar)
                LoadDetailControl()
            Catch ex As Exception

            End Try
        End Sub

        Private Sub PreviewXML()
            If Grammar IsNot Nothing Then
                Dim pxml As New PreviewXML
                pxml.XML = Grammar.ToString
                pxml.ShowDialog()
            End If
        End Sub

        Private Sub Analyze()
            If Grammar IsNot Nothing Then
                Dim a As New Analyze
                a.HTML = Grammar.Analyze
                a.ShowDialog()
            End If
        End Sub
#End Region

#Region "Event Handlers"
        Private Sub mnuExit_Click(sender As Object, e As RoutedEventArgs)
            Me.Close()
        End Sub

        Private Sub mnuAssignmentGrammar_Click(sender As Object, e As RoutedEventArgs)
            NewFile(GetType(AssignmentGrammar))
        End Sub

        Private Sub mnuDiceGrammar_Click(sender As Object, e As RoutedEventArgs)
            NewFile(GetType(DiceRoll))
        End Sub

        Private Sub mnuLuaGrammar_Click(sender As Object, e As RoutedEventArgs)
            NewFile(GetType(LUAGrammar))
        End Sub

        Private Sub mnuPhotacticsGrammar_Click(sender As Object, e As RoutedEventArgs)
            NewFile(GetType(PhonotacticsGrammar))
        End Sub

        Private Sub mnuTableGrammar_Click(sender As Object, e As RoutedEventArgs)
            NewFile(GetType(TableGrammar))
        End Sub

        Private Sub mnuPreviewXML_Click(sender As Object, e As RoutedEventArgs)
            PreviewXML()
        End Sub

        Private Sub mnuAnalyze_Click(sender As Object, e As RoutedEventArgs)
            Analyze()
        End Sub

        Private Sub GrammarEditor_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
            If Grammar IsNot Nothing AndAlso Grammar.IsDirty Then
                Dim result As DialogResult = MessageBox.Show("There are unsaved changes to the grammar file.  Would you like to save these changes now?", "Save Changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                Select Case result
                    Case Forms.DialogResult.Yes
                        e.Cancel = Not SaveFile()
                    Case Forms.DialogResult.No
                    Case Forms.DialogResult.Cancel
                        e.Cancel = True
                End Select
            End If
        End Sub

        Private Sub GrammarEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            GrammarAvailable = False
            MRUStorage = New MRUStorage("RecentGrammarFiles")
        End Sub

        Private Sub Save_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = Me.IsValid()
        End Sub

        Private Sub Open_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = True
        End Sub

        Private Sub Save_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            SaveFile()
        End Sub

        Private Sub SaveAs_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            SaveFileAs()
        End Sub

        Private Sub Open_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            OpenFile()
        End Sub

        Private Sub Close_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            Me.Close()
        End Sub

        Private Sub mnuRecent_FileSelected(sender As Object, e As MRU.MRUFileList.FileSelectedEventArgs) Handles mnuRecent.FileSelected
            Try
                OpenFile(e.FileName)
            Catch
                If MessageBox.Show("Unable to open the file " & e.FileName & ".  Would you like to remove it from the recent file list?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Forms.DialogResult.Yes Then
                    mnuRecent.RemoveItem(e.FileName)
                End If
            End Try
        End Sub

        Private Sub lnkTags_Click(sender As Object, e As RoutedEventArgs)
            Dim tags As New TagEditor
            tags.Owner = Me
            tags.TagList = Grammar.Tags
            If tags.ShowDialog Then
                Grammar.Tags = tags.TagList
            End If
        End Sub
#End Region
    End Class
End Namespace