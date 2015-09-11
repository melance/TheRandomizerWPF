Imports Grammars
Imports MahApps.Metro.Controls
Imports System.ComponentModel

Namespace Controls
    Public Class GrammarTabItem

        Private Class GenerateParameters

            Public Sub New(ByVal grammar As BaseGrammar,
                           ByVal repeat As Int32,
                           ByVal maxLength As Int32,
                           ByVal parameters As Dictionary(Of String, Object))
                Me.Grammar = grammar
                Me.Repeat = repeat
                Me.MaxLength = maxLength
                Me.Parameters = parameters
            End Sub

            Public Function Result() As String
                Return Grammar.GenerateNames(Repeat, MaxLength, Parameters)
            End Function

            Public Property Grammar As BaseGrammar
            Public Property Repeat As Int32
            Public Property MaxLength As Int32
            Public Property Parameters As Dictionary(Of String, Object)
        End Class

#Region "Constructor"
        Public Sub New(ByVal grammar As BaseGrammar)
            ' This call is required by the designer.
            InitializeComponent()

            Me.Grammar = grammar
        End Sub
#End Region

#Region "Events"
        Public Event TagClicked As RoutedEventHandler
#End Region

#Region "Dependecy Properties"
        Public Shared ReadOnly BaseGrammarProperty As DependencyProperty = DependencyProperty.Register("Grammar",
                                                                                                       GetType(BaseGrammar),
                                                                                                       GetType(GrammarTabItem),
                                                                                                       New PropertyMetadata(Nothing))
        Public Shared ReadOnly CountProperty As DependencyProperty = DependencyProperty.Register("Count",
                                                                                                 GetType(Int32),
                                                                                                 GetType(GrammarTabItem),
                                                                                                 New PropertyMetadata(1))
        Public Shared ReadOnly MaxLengthProperty As DependencyProperty = DependencyProperty.Register("MaxLength",
                                                                                                     GetType(Int32),
                                                                                                     GetType(GrammarTabItem),
                                                                                                     New PropertyMetadata(20))
#End Region

#Region "Constants"
        Private Const SELECT_ALL_COMMAND As String = "SelectAll"
        Private Const UNSELECT_COMMAND As String = "Unselect"
        Private Const COPY_COMMAND As String = "Copy"
        Private Const SAVE_AS_COMMAND As String = "SaveAs"
        Private Const FILE_NAME_FORMAT As String = "{0}.html"
        Private Const RESULTS_ODD_BACKGROUND_KEY As String = "ResultsOddBackground"
        Private Const RESULTS_EVEN_BACKGROUND_KEY As String = "ResultsEvenBackground"
        Private Const RESULTS_DIVIDER_COLOR_KEY As String = "ResultsDividerColor"
        Private Const EXPLORER_PROCESS As String = "Explorer.exe"
        Private Const EXPLORER_PROCESS_ARGS As String = "/select,""{0}"""
#End Region

#Region "Methods"
        Private WithEvents _worker As BackgroundWorker
#End Region

#Region "Proerties"
        Public Property Grammar As BaseGrammar
            Get
                Return CType(GetValue(BaseGrammarProperty), BaseGrammar)
            End Get
            Set(value As BaseGrammar)
                SetValue(BaseGrammarProperty, value)
                InitializeParameters()
            End Set
        End Property

        Public Property Count As Int32
            Get
                Return CInt(GetValue(CountProperty))
            End Get
            Set(value As Int32)
                SetValue(CountProperty, value)
            End Set
        End Property

        Public Property MaxLength As Int32
            Get
                Return CInt(GetValue(MaxLengthProperty))
            End Get
            Set(value As Int32)
                SetValue(MaxLengthProperty, value)
            End Set
        End Property

        Public ReadOnly Property Document As mshtml.IHTMLDocument2
            Get
                Return DirectCast(webBrowser.Document, mshtml.IHTMLDocument2)
            End Get
        End Property

        Public ReadOnly Property AreResultsEmpty As Boolean
            Get
                Return webBrowser Is Nothing OrElse webBrowser.Document Is Nothing OrElse Document.body Is Nothing
            End Get
        End Property
#End Region

#Region "Commands"
        Public Sub CanGenerate(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs)
            e.CanExecute = Grammar IsNot Nothing
        End Sub

        Public Sub Generate_Executed()
            Try
                Me.UpdateLayout()
                btnGenerate.IsEnabled = False
                Generate()
            Finally
                btnGenerate.IsEnabled = True
            End Try
        End Sub

        Private Sub Cancel_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            If Grammar IsNot Nothing Then Grammar.Cancel()
        End Sub

        Private Sub Clear_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            Document.clear()
        End Sub

        Private Sub Clear_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = Not AreResultsEmpty
        End Sub

        Private Sub Copy_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            Document.execCommand(COPY_COMMAND)
        End Sub

        Private Sub Copy_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = Not AreResultsEmpty
        End Sub

        Private Sub Generate_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            Try
                Me.UpdateLayout()
                btnGenerate.IsEnabled = False
                Generate()
            Finally
                btnGenerate.IsEnabled = True
            End Try
        End Sub

        Private Sub Generate_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = Grammar IsNot Nothing
        End Sub

        Private Sub Print_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            Document.execCommand("Print", True)
        End Sub

        Private Sub Print_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = Not AreResultsEmpty
        End Sub

        Private Sub Save_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            Document.execCommand("Save", True)
        End Sub

        Private Sub Save_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = Not AreResultsEmpty
        End Sub

        Private Sub SelectAll_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            Document.execCommand(SELECT_ALL_COMMAND)
        End Sub

        Private Sub SelectAll_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = Not AreResultsEmpty
        End Sub

        Private Sub SelectNone_Executed(sender As Object, e As ExecutedRoutedEventArgs)
            Document.execCommand(UNSELECT_COMMAND)
        End Sub

        Private Sub SelectNone_CanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            e.CanExecute = Not AreResultsEmpty
        End Sub
#End Region

#Region "Private Methods"

        Private Sub ProgressUpdate(ByVal sender As Object, ByVal e As BaseGrammar.ProgressUpdateEventArgs)
            Dispatcher.Invoke(Sub(value As Int32)
                                  prgResults.Value = value
                                  prgResults.InvalidateVisual()
                              End Sub,
                              e.Value)
        End Sub

        Private Sub Generate()
            If Grammar IsNot Nothing Then
                If CInt(nudRepeat.Value) > 1 Then
                    btnGenerate.Visibility = Windows.Visibility.Collapsed
                    btnCancel.Visibility = Windows.Visibility.Visible
                    prgResults.Maximum = CInt(nudRepeat.Value)
                    prgResults.Value = 0
                    grdProgress.Visibility = Windows.Visibility.Visible
                End If

                AddHandler Grammar.ProgressUpdate, AddressOf ProgressUpdate

                If Not Grammar.OddResultColorSpecified AndAlso TryFindResource(RESULTS_ODD_BACKGROUND_KEY) IsNot Nothing Then Grammar.OddResultColor = DirectCast(FindResource(RESULTS_ODD_BACKGROUND_KEY), Color)
                If Not Grammar.EvenResultColorSpecified AndAlso TryFindResource(RESULTS_EVEN_BACKGROUND_KEY) IsNot Nothing Then Grammar.EvenResultColor = DirectCast(FindResource(RESULTS_EVEN_BACKGROUND_KEY), Color)
                If Not Grammar.DividerColorSpecified AndAlso TryFindResource(RESULTS_DIVIDER_COLOR_KEY) IsNot Nothing Then Grammar.DividerColor = DirectCast(FindResource(RESULTS_DIVIDER_COLOR_KEY), Color)
                If Not Grammar.ResultFontSpecified Then Grammar.ResultFont = My.Settings.DefaultResultFont

                If Document IsNot Nothing Then Document.clear()

                Dim p As New GenerateParameters(Grammar,
                                                CInt(nudRepeat.Value),
                                                If(pnlMaxLength.Visibility = Windows.Visibility.Visible, CInt(nudMaxLength.Value), Int32.MaxValue),
                                                GetParameters())
                _worker = New BackgroundWorker
                _worker.WorkerSupportsCancellation = True
                AddHandler _worker.DoWork, AddressOf DoWork
                AddHandler _worker.RunWorkerCompleted, AddressOf RunWorkerCompleted
                _worker.RunWorkerAsync(p)
            End If
        End Sub

        Private Function GetParameters() As Dictionary(Of String, Object)
            Dim value As New Dictionary(Of String, Object)

            For Each control As FrameworkElement In pnlParameterList.Children
                Dim panel As DockPanel = TryCast(control, DockPanel)
                For Each child As FrameworkElement In panel.Children
                    If Not TypeOf child Is Label Then
                        Dim param As KeyValuePair(Of String, Object) = GetParameterValue(child)
                        value.Add(param.Key, param.Value)
                    End If
                Next
            Next
            Return value
        End Function

        Private Function GetParameterValue(ByVal control As FrameworkElement) As KeyValuePair(Of String, Object)
            Select Case control.GetType
                Case GetType(TextBox) : Return New KeyValuePair(Of String, Object)(CStr(control.Tag), DirectCast(control, TextBox).Text)
                Case GetType(ComboBox) : Return New KeyValuePair(Of String, Object)(CStr(control.Tag), DirectCast(DirectCast(control, ComboBox).SelectedItem, [Option]).Value)
                Case GetType(CheckBox) : Return New KeyValuePair(Of String, Object)(CStr(control.Tag), DirectCast(control, CheckBox).IsChecked)
            End Select
            Return Nothing
        End Function

        Private Sub InitializeParameters()
            pnlParameterList.Children.Clear()
            For Each parameter As Parameter In Grammar.Parameters
                Dim temp As DockPanel = parameter.Control
                pnlParameterList.Children.Add(temp)
            Next
        End Sub

        Private Sub DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
            e.Result = DirectCast(e.Argument, GenerateParameters).Result
        End Sub

        Private Sub RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
            Dim result As String
            If e.Error IsNot Nothing Then
                result = "<span style='color:red'>" & e.Error.Message
                For Each pair As KeyValuePair(Of String, String) In e.Error.Data
                    result &= Environment.NewLine & pair.Key & ":" & pair.Value
                Next
                result &= "</span>"
            Else
                result = e.Result.ToString
            End If
            RemoveHandler _worker.DoWork, AddressOf DoWork
            RemoveHandler _worker.RunWorkerCompleted, AddressOf RunWorkerCompleted
            _worker = Nothing
            grdProgress.Visibility = Windows.Visibility.Collapsed
            btnCancel.Visibility = Windows.Visibility.Collapsed
            btnGenerate.Visibility = Windows.Visibility.Visible
            webBrowser.NavigateToString(result.ToString)
        End Sub
#End Region

#Region "Event Handlers"
        Private Sub lnkTag_Click(sender As Object, e As RoutedEventArgs)
            RaiseEvent TagClicked(sender, e)
        End Sub

        Private Sub lnkTags_Click(sender As Object, e As RoutedEventArgs)
            Dim path As String = IO.Path.GetDirectoryName(Grammar.FilePath)
            If Not Utility.HelperMethods.HasWritePermissionOnDir(path) Then
                MessageBox.Show("Access to the file was denied.  Either the file is readonly or the application must be run with administrator access.", "Access denied", MessageBoxButton.OK)
            Else
                Dim tagEditor As New TagEditor
                tagEditor.TagList = Grammar.Tags
                tagEditor.Owner = Window.GetWindow(Me)
                If tagEditor.ShowDialog() Then
                    Grammar.Category = String.Empty
                    Grammar.Genre = String.Empty
                    Grammar.System = String.Empty
                    Grammar.Tags = tagEditor.TagList
                    IO.File.WriteAllText(Grammar.FilePath, Grammar.ToString)
                    Grammar = BaseGrammar.Open(Grammar.FilePath)
                End If
            End If
        End Sub

        Private Sub lnkFilePath_Click(sender As Object, e As RoutedEventArgs)
            Process.Start(EXPLORER_PROCESS, String.Format(EXPLORER_PROCESS_ARGS, Grammar.FilePath))
        End Sub

        Private Sub lnkDetailLink_Click(sender As Object, e As RoutedEventArgs)
            If e.Source IsNot Nothing Then
                Process.Start(DirectCast(e.Source, Hyperlink).NavigateUri.ToString)
            End If
        End Sub

        Private Sub GrammarTabItem_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If My.Settings.DefaultDetailWidth > 0 Then
                grdMain.ColumnDefinitions(0).Width = New GridLength(My.Settings.DefaultDetailWidth)
            End If
        End Sub
#End Region

    End Class
End Namespace