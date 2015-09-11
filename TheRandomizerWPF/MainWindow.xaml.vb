Imports System.ComponentModel
Imports System.Text
Imports AutoUpdaterDotNET
Imports Grammars
Imports Utility
Imports System.Threading
Imports System.IO
Imports MahApps.Metro.Controls
Imports MahApps.Metro
Imports System.Windows.Controls.Primitives
Imports MahApps.Metro.Controls.Dialogs
Imports mshtml
Imports TheRandomizerWPF.Controls
Imports Dragablz
Imports TheRandomizerWPF.Settings
Imports System.Collections.Specialized

Class MainWindow
    Inherits MetroWindow

#Region "Classes"
    Private Class AccentComboItem
        Public Sub New(ByVal name As String,
                       ByVal color As Brush)
            _name = name
            _color = color
        End Sub

        Private _name As String
        Private _color As Brush

        Public ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        Public ReadOnly Property Color As Brush
            Get
                Return _color
            End Get
        End Property
    End Class

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
#End Region

#Region "Dependency Properties"
    Public Shared ReadOnly GrammarListProperty As DependencyProperty =
                      DependencyProperty.Register("GrammarList",
                                                  GetType(GrammarList),
                                                  GetType(MainWindow),
                                                  New PropertyMetadata(Nothing))
    Public Shared ReadOnly InterTabClientProperty As DependencyProperty =
                      DependencyProperty.Register("InterTabClientInstance",
                                                  GetType(TRInterTabClient),
                                                  GetType(MainWindow),
                                                  New PropertyMetadata(Nothing))
#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        LoadWindowSettings()
        _tabItems = CollectionViewSource.GetDefaultView(tabResults.Items)
        InterTabClientInstance = New TRInterTabClient
    End Sub
#End Region

#Region "Events"

#End Region

#Region "Constants"
    Private Const DATA_FILES_DIRECTORY As String = "DataFiles"
    Private Const THEME_FILES_DIRECTORY As String = "Themes"
    Private Const GENERATOR_DISPLAY_MEMBER As String = "Name"
    Private Const RESULTS_ODD_BACKGROUND_KEY As String = "ResultsOddBackground"
    Private Const RESULTS_EVEN_BACKGROUND_KEY As String = "ResultsEvenBackground"
    Private Const RESULTS_DIVIDER_COLOR_KEY As String = "ResultsDividerColor"
    Private Const SELECT_ALL_COMMAND As String = "SelectAll"
    Private Const UNSELECT_COMMAND As String = "Unselect"
    Private Const COPY_COMMAND As String = "Copy"
    Private Const SAVE_AS_COMMAND As String = "SaveAs"
    Private Const FILE_NAME_FORMAT As String = "{0}.html"
    Private Const AUTO_UPDATE_MANIFEST As String = "http://35887a069d1c5e40fcaa-9f14dcdabbf0f021d6ceb0c8533f4ebc.r20.cf1.rackcdn.com/appcast.xml"
    Private Const EXPLORER_PROCESS As String = "Explorer.exe"
    Private Const EXPLORER_PROCESS_ARGS As String = "/select,""{0}"""
    Private ReadOnly DEFAULT_RESULT_FONT As New System.Drawing.Font("Consolas", 12)
#End Region

#Region "Members"
    Private WithEvents _tabItems As ICollectionView
    Private WithEvents _worker As BackgroundWorker
    Private _splashScreen As SplashScreen
    Private _tags As List(Of String)
    Private _theme As String
    Private Const ACCENT_COLOR_BRUSH As String = "AccentColorBrush"
    Private _controller As ProgressDialogController
#End Region

#Region "Public Properties"
    Public Property InterTabClientInstance As TRInterTabClient
        Get
            If GetValue(InterTabClientProperty) Is Nothing Then InterTabClientInstance = New TRInterTabClient
            Return DirectCast(GetValue(InterTabClientProperty), TRInterTabClient)
        End Get
        Set(value As TRInterTabClient)
            SetValue(InterTabClientProperty, value)
        End Set
    End Property

    Public Property GrammarList As GrammarList
        Get
            Return Dispatcher.Invoke(Of GrammarList)(Function() CType(GetValue(GrammarListProperty), GrammarList))
        End Get
        Set(value As GrammarList)
            Dispatcher.Invoke(Sub() SetValue(GrammarListProperty, value))
        End Set
    End Property

    Public Property ThemeAccent As String
        Get
            Return My.Settings.ThemeAccent
        End Get
        Set(value As String)
            My.Settings.ThemeAccent = value
            My.Settings.Save()
            ApplyTheme()
        End Set
    End Property

    Public Property WindowTheme As String
        Get
            Return My.Settings.Theme
        End Get
        Set(value As String)
            My.Settings.Theme = value
            My.Settings.Save()
            ApplyTheme()
        End Set
    End Property
#End Region

#Region "Private Methods"
    Private Function AddTab(ByVal grammar As BaseGrammar) As TabItem
        Dim grammarControl As New GrammarTabItem(grammar)
        Dim tab As New TabItem
        AddHandler grammarControl.TagClicked, AddressOf lnkTag_Click
        tab.Header = grammar.Name
        tab.ToolTip = grammar.Name
        tab.Tag = grammar
        tab.Content = grammarControl
        tabResults.Items.Add(tab)
        Return tab
    End Function

    Private Sub ApplyTheme()
        Application.SetTheme(ThemeAccent, WindowTheme)
    End Sub

    Private Sub CheckForNewVersion()
        If My.Settings.CheckForUpdates Then
            AutoUpdater.LetUserSelectRemindLater = True
            AutoUpdater.Start(AUTO_UPDATE_MANIFEST)
        End If
    End Sub

    Private Function GetTagList() As List(Of String)
        Dim result As New List(Of String)
        For Each chkBox As ToggleButton In pnlTags.Children
            If Not chkBox.IsChecked Then result.Add(CStr(chkBox.Content))
        Next
        Return result
    End Function

    Private Sub FilterGrammarFiles()
        GrammarList.SelectedTags = GetTagList()
        lstGenerators.ItemsSource = Nothing
        lstGenerators.DisplayMemberPath = GENERATOR_DISPLAY_MEMBER
        lstGenerators.ItemsSource = GrammarList.FilteredList
        lstGenerators.Items.SortDescriptions.Clear()
        lstGenerators.Items.SortDescriptions.Add(New SortDescription("Name", ListSortDirection.Ascending))
        If lstGenerators.Items.Count > 0 Then lstGenerators.SelectedIndex = 0
    End Sub

    Private Async Sub LoadGrammarFilesWorker()
        If _splashScreen Is Nothing Then
            _controller = Await Me.ShowProgressAsync("Please wait, loading grammars...", "Please Wait", False)
            tabResults.Visibility = Windows.Visibility.Hidden
        End If
        _worker = New BackgroundWorker
        _worker.WorkerReportsProgress = True
        AddHandler _worker.DoWork, AddressOf LoadGrammarFiles
        AddHandler _worker.ProgressChanged, AddressOf LoadGrammars_ReportProgress
        AddHandler _worker.RunWorkerCompleted, AddressOf LoadGrammars_RunWorkerCompleted
        _worker.RunWorkerAsync()
    End Sub

    Private Async Sub LoadGrammars_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)
        If _controller IsNot Nothing Then Await _controller.CloseAsync()
        If _splashScreen IsNot Nothing Then
            _splashScreen.Close()
            _splashScreen = Nothing
        End If
        If e.Result IsNot Nothing Then
            Dispatcher.Invoke(Sub()
                                  Dim le As New LoadErrors
                                  le.ErrorList = DirectCast(e.Result, Dictionary(Of String, Exception))
                                  le.ShowDialog()
                              End Sub)
        End If
        If Not flySettings.IsVisible AndAlso tabResults.SelectedItem IsNot Nothing Then tabResults.Visibility = Windows.Visibility.Visible
        Visibility = Windows.Visibility.Visible
        LoadUserSettings()
    End Sub

    Private Sub LoadGrammars_ReportProgress(sender As Object, e As ProgressChangedEventArgs)
        _controller.SetProgress(e.ProgressPercentage / 100)
        _controller.SetMessage(e.UserState.ToString)
    End Sub

    Private Sub GrammarList_ReportProgress(sender As Object, e As ProgressChangedEventArgs)
        If _splashScreen IsNot Nothing Then
            Dispatcher.Invoke(Sub() _splashScreen.SetMessage(e.UserState.ToString))
            Dispatcher.Invoke(Sub() _splashScreen.SetProgress(e.ProgressPercentage))
        Else
            _controller.SetProgress(e.ProgressPercentage / 100)
            _controller.SetMessage(e.UserState.ToString)
        End If
    End Sub

    Private Sub LoadGrammarFiles(sender As Object, e As DoWorkEventArgs)
        Dim tags As New List(Of String)
        Dim paths As New List(Of String)
        Dim errorList As Dictionary(Of String, Exception)

        paths.Add(IO.Path.Combine(My.Application.Info.DirectoryPath, DATA_FILES_DIRECTORY))
        If Not String.IsNullOrWhiteSpace(My.Settings.GrammarFilePath) Then paths.Add(My.Settings.GrammarFilePath)

        Grammars.Utility.GrammarFilePaths.AddRange(paths)
        GrammarList = New GrammarList(paths)
        AddHandler GrammarList.ReportProgress, AddressOf GrammarList_ReportProgress
        errorList = GrammarList.Load()

        If My.Settings.ShowLoadErrors AndAlso errorList IsNot Nothing AndAlso errorList.Count > 0 Then
            e.Result = errorList
            'Dispatcher.Invoke(Sub()
            '    Dim le As New LoadErrors
            '    le.ErrorList = errorList
            '    le.ShowDialog()
            'End Sub)
        End If

        Dispatcher.Invoke(Sub() pnlTags.Children.Clear())

        For Each item As GrammarListItem In GrammarList
            For Each tag As String In item.Tags
                If tags.Find(Function(t As String) t.Equals(tag, StringComparison.InvariantCultureIgnoreCase)) Is Nothing Then
                    tags.Add(tag)
                End If
            Next
        Next

        tags.Sort()

        For Each tag As String In tags
            Dispatcher.Invoke(Sub() AddTag(tag))
        Next

        Dispatcher.Invoke(AddressOf FilterGrammarFiles)
    End Sub

    Private Sub AddTag(ByVal tagName As String)
        Dim chkTag As New ToggleButton()
        chkTag.Content = tagName
        chkTag.Padding = New Thickness(6, 0, 6, 0)
        chkTag.Margin = New Thickness(1)
        chkTag.IsChecked = False
        AddHandler chkTag.Checked, AddressOf chkTag_Click
        AddHandler chkTag.Unchecked, AddressOf chkTag_Click
        pnlTags.Children.Add(chkTag)
    End Sub

    Private Sub LoadThemes()
        If Not String.IsNullOrWhiteSpace(My.Settings.ThemeAccent) Then
            ThemeAccent = My.Settings.ThemeAccent
        Else
            ThemeAccent = ThemeManager.Accents(0).Name
        End If

        If Not String.IsNullOrWhiteSpace(My.Settings.Theme) Then
            WindowTheme = My.Settings.Theme
        Else
            WindowTheme = ThemeManager.AppThemes(0).Name
        End If

        For Each item As ComboBoxItem In cboTheme.Items
            If item.Name = WindowTheme Then cboTheme.SelectedValue = item
        Next

        For Each accent As Accent In ThemeManager.Accents
            Dim item As New AccentComboItem(accent.Name, CType(accent.Resources(ACCENT_COLOR_BRUSH), Brush))
            Dim index As Int32 = cboAccent.Items.Add(item)
            If item.Name = ThemeAccent Then cboAccent.SelectedValue = item
        Next

    End Sub

    Private Sub LoadWindowSettings()
        Dim settings As New WindowPosition
        Me.Height = settings.WindowHeight
        Me.Width = settings.WindowWidth
        Me.Top = settings.WindowTop
        Me.Left = settings.WindowLeft
        Me.WindowState = settings.WindowState
        Me.colGenerators.Width = New GridLength(settings.GeneratorsWidth)
    End Sub

    Private Sub LoadUserSettings()
        If My.Settings.SelectedTags IsNot Nothing Then
            SelectTheseTags(My.Settings.SelectedTags)
        End If
    End Sub

    Private Sub OpenMCGenerator()
        Dim mcg As New Tools.MCGenerator
        mcg.Owner = Me
        mcg.ShowDialog()
    End Sub

    Private Sub OpenILConverter()
        Dim ilc As New Tools.ItemListConverter
        ilc.Owner = Me
        ilc.ShowDialog()
    End Sub

    Private Sub OpenPhonotacticsGenerator()
        Dim pg As New Tools.PhonotacticsGenerator
        pg.Owner = Me
        pg.ShowDialog()
    End Sub

    Private Sub SaveWindowSettings()
        Dim settings As New WindowPosition
        settings.WindowHeight = Me.Height
        settings.WindowWidth = Me.Width
        settings.WindowTop = Me.Top
        settings.WindowLeft = Me.Left
        settings.WindowState = Me.WindowState
        settings.GeneratorsWidth = Me.colGenerators.Width.Value
        settings.Save()
    End Sub

    Private Sub SaveUserSettings()
        If My.Settings.SelectedTags Is Nothing Then My.Settings.SelectedTags = New StringCollection
        My.Settings.SelectedTags.Clear()
        My.Settings.SelectedTags.AddRange(GetTagList().ToArray)
        My.Settings.Save()
    End Sub

    Private Sub ShowAbout()
        Dim frmAbout As New About
        frmAbout.Owner = Me
        frmAbout.ShowDialog()
    End Sub

    Private Sub ShowEditor()
        Dim frmEditor As New Tools.GrammarEditor
        frmEditor.Owner = Me
        frmEditor.ShowDialog()
    End Sub

    Private Sub ShowHelp()
        Windows.Forms.Help.ShowHelp(Nothing,
                                    IO.Path.Combine(My.Application.Info.DirectoryPath, My.Resources.HelpFile),
                                    Forms.HelpNavigator.TableOfContents)
    End Sub

    Private Sub ShowSettings()
        flySettings.IsOpen = Not flySettings.IsOpen
    End Sub

    Private Sub ToggleAllTags(ByVal toggle As Boolean?)
        For Each chkTag As ToggleButton In pnlTags.Children
            If toggle Is Nothing Then
                chkTag.IsChecked = Not chkTag.IsChecked
            Else
                chkTag.IsChecked = toggle
            End If
        Next
        FilterGrammarFiles()
    End Sub

    Private Sub SelectTheseTags(ByVal tagList As StringCollection)
        For Each chkTag As ToggleButton In pnlTags.Children
            chkTag.IsChecked = Not tagList.Contains(chkTag.Content.ToString)
        Next
        FilterGrammarFiles()
    End Sub
#End Region

#Region "Routed Commands"
    Private Sub About_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        ShowAbout()
    End Sub


    Private Sub Help_Executed(sender As Object, e As ExecutedRoutedEventArgs)
        ShowHelp()
    End Sub

#End Region

#Region "Event Handlers"

    Private Sub chkTag_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        FilterGrammarFiles()
    End Sub

    Private Sub mnuGrammarEditor_Click(sender As Object, e As RoutedEventArgs)
        ShowEditor()
    End Sub

    Private Sub MainWindow_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        SaveWindowSettings()
    End Sub

    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        SaveUserSettings()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        _splashScreen = New SplashScreen
        _splashScreen.Topmost = True
        _splashScreen.Owner = Me
        _splashScreen.WindowStartupLocation = Windows.WindowStartupLocation.CenterScreen
        Application.SetTheme(_splashScreen, My.Settings.ThemeAccent, My.Settings.Theme)
        _splashScreen.Show()
        Visibility = Windows.Visibility.Hidden
        LoadGrammarFilesWorker()
        LoadThemes()
    End Sub

    Private Sub mnuDonatePaypal_Click(sender As Object, e As RoutedEventArgs)
        Process.Start(My.Resources.Paypal)
    End Sub

    Private Sub mnuDonateDwolla_Click(sender As Object, e As RoutedEventArgs)
        Process.Start(My.Resources.Dwolla)
    End Sub

    Private Sub mnuPreferences_Click(sender As Object, e As RoutedEventArgs)
        ShowSettings()
    End Sub

    Private Sub mnuMarkovChainGenerator_Click(sender As Object, e As RoutedEventArgs)
        OpenMCGenerator()
    End Sub

    Private Sub mnuItemListConverter_Click(sender As Object, e As RoutedEventArgs)
        OpenILConverter()
    End Sub

    Private Sub mnuPhonotacticsGenerator_Click(sender As Object, e As RoutedEventArgs)
        OpenPhonotacticsGenerator()
    End Sub

    Private Sub btnClearTags_Click(sender As Object, e As RoutedEventArgs)
        ToggleAllTags(True)
    End Sub

    Private Sub btnSelectTags_Click(sender As Object, e As RoutedEventArgs)
        ToggleAllTags(False)
    End Sub

    Private Sub mnuRefreshGrammars_Click(sender As Object, e As RoutedEventArgs)
        Mouse.OverrideCursor = Cursors.Wait
        LoadGrammarFilesWorker()
        Mouse.OverrideCursor = Cursors.Arrow
    End Sub

    Private Sub mnuRefreshThemes_Click(sender As Object, e As RoutedEventArgs)
        LoadThemes()
    End Sub

    Private Sub lnkGrammars_RequestNavigate(sender As Object, e As RequestNavigateEventArgs)
        System.Diagnostics.Process.Start(My.Resources.Grammars)
    End Sub

    Private Sub btnSettings_Click(sender As Object, e As RoutedEventArgs)
        ShowSettings()
    End Sub

    Private Sub btnDefaultResultsFont_Click(sender As Object, e As RoutedEventArgs)
        Dim dlg As New System.Windows.Forms.FontDialog
        Dim font As System.Drawing.Font = My.Settings.DefaultResultFont

        dlg.Font = font
        If dlg.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.DefaultResultFont = dlg.Font
        End If
    End Sub

    Private Sub btnGrammarPath_Click(sender As Object, e As RoutedEventArgs)
        Dim dlg As New Forms.FolderBrowserDialog()
        dlg.SelectedPath = My.Settings.GrammarFilePath
        If dlg.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.GrammarFilePath = dlg.SelectedPath
        End If
        LoadGrammarFilesWorker()
    End Sub

    Private Sub cboTheme_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        WindowTheme = DirectCast(e.AddedItems(0), ComboBoxItem).Tag.ToString
    End Sub

    Private Sub cboAccent_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        ThemeAccent = DirectCast(e.AddedItems(0), AccentComboItem).Name
    End Sub

    Private Sub flySettings_IsVisibleChanged(sender As Object, e As DependencyPropertyChangedEventArgs) Handles flySettings.IsVisibleChanged
        If flySettings.IsVisible Then
            tabResults.Visibility = Windows.Visibility.Hidden
        Else
            tabResults.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Private Sub lstGenerators_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        Dim grammar As GrammarListItem = DirectCast(DirectCast(sender, ListBoxItem).DataContext, GrammarListItem)
        tabResults.SelectedItem = AddTab(grammar.Grammar)
    End Sub

    Private Sub lnkTag_Click(sender As Object, e As RoutedEventArgs)
        Dim lnkTag As Hyperlink = TryCast(sender, Hyperlink)
        If lnkTag IsNot Nothing Then
            For Each chkTag As ToggleButton In pnlTags.Children
                If chkTag.Content.ToString = DirectCast(lnkTag.Inlines(0), System.Windows.Documents.Run).Text Then
                    chkTag.IsChecked = False
                    FilterGrammarFiles()
                Else
                    chkTag.IsChecked = True
                End If
            Next
        End If
    End Sub
#End Region

End Class
