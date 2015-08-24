Imports System.Xml.Serialization
Imports Utility
Imports System.Reflection
Imports System.Text
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Collections.ObjectModel
Imports System.Windows.Media

<XmlRoot("Grammar", Namespace:="")>
Public MustInherit Class BaseGrammar
    Implements INotifyPropertyChanged
    Implements IDataErrorInfo

#Region "Event Arguments"
    Public Class ProgressUpdateEventArgs
        Inherits EventArgs

        Dim _value As Int32

        Public Sub New(ByVal value As Int32)
            _value = value
        End Sub

        Public ReadOnly Property Value As Int32
            Get
                Return _value
            End Get
        End Property
    End Class
#End Region

#Region "Events"
    Public Event ProgressUpdate As EventHandler(Of ProgressUpdateEventArgs)
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
#End Region

#Region "Shared Members"
    Private Shared _knownTypes As List(Of Type)
#End Region

#Region "Shared Properties"
    Public Shared ReadOnly Property KnownTypes As Type()
        Get
            If _knownTypes Is Nothing Then
                _knownTypes = New List(Of Type)(Assembly.GetExecutingAssembly.GetTypes.Where(Function(t As Type) GetType(BaseGrammar).IsAssignableFrom(t)).ToArray)
            End If
            Return _knownTypes.ToArray
        End Get
    End Property
#End Region

#Region "Shared Methods"
    Public Shared Function Open(ByVal fileName As String) As BaseGrammar
        Dim deserializer As New XmlSerializer(GetType(BaseGrammar), KnownTypes)
        Using reader As IO.StreamReader = IO.File.OpenText(fileName)
            Dim grammar As BaseGrammar = DirectCast(deserializer.Deserialize(reader), BaseGrammar)
            grammar.FilePath = fileName
            grammar.IsDirty = False
            Return grammar
        End Using
    End Function
#End Region

#Region "Constructors"
    Public Sub New()
    End Sub
#End Region

#Region "Constants"
    Private ReadOnly DEFAULT_ODD_COLOR As Color = Colors.White
    Private ReadOnly DEFAULT_EVEN_COLOR As Color = Color.FromArgb(255, 239, 239, 239)
    Private ReadOnly DEFAULT_DIVIDER_COLOR As Color = Colors.Black
    Private ReadOnly DEFAULT_RESULT_FONT As New System.Drawing.Font("Consolas", 12)
    Private Const HTML_COLOR_FORMAT As String = "#{0:X2}{1:X2}{2:X2}"
#End Region

#Region "Members"
    Private Shared _random As Random
    Private _name As String
    Private _description As String
    Private _author As String
    Private _category As String
    Private _genre As String
    Private _system As String
    Private _tags As New BindingList(Of String)
    Private _supportsMaxLength As Boolean = False
    Private _url As String
    Private _skipValidation As Boolean = False
    Private _cancel As Boolean = False
    Private WithEvents _parameters As New ObservableCollection(Of Parameter)
#End Region

#Region "Properties"
    <XmlIgnore>
    Public Property FilePath As String

    <XmlIgnore>
    Public ReadOnly Property FileDirectory As String
        Get
            Return IO.Path.GetDirectoryName(FilePath)
        End Get
    End Property

    <XmlIgnore>
    Public ReadOnly Property FileName As String
        Get
            Return IO.Path.GetFileNameWithoutExtension(IO.Path.GetFileNameWithoutExtension(FilePath))
        End Get
    End Property

    <XmlIgnore>
    Public Shared ReadOnly Property Random As Random
        Get
            If _random Is Nothing Then
                Randomize()
                _random = New Random
            End If
            Return _random
        End Get
    End Property

    <XmlElement("name")>
    Public Overridable Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            If _name <> value Then
                _name = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    <XmlElement("author")>
    Public Overridable Property Author As String
        Get
            Return _author
        End Get
        Set(value As String)
            If _author <> value Then
                _author = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    <XmlElement("description")>
    Public Overridable Property Description As String
        Get
            Return _description
        End Get
        Set(value As String)
            If _description <> value Then
                _description = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    <XmlElement("category")>
    Public Overridable Property Category As String
        Get
            Return _category
        End Get
        Set(value As String)
            If _category <> value Then
                _category = value
                Tags.Add(_category)
                OnPropertyChanged()
                OnPropertyChanged("Genre")
                OnPropertyChanged("System")
                OnPropertyChanged("Tags")
            End If
        End Set
    End Property

    <XmlElement("genre", IsNullable:=True)>
    Public Overridable Property Genre As String
        Get
            Return _genre
        End Get
        Set(value As String)
            If _genre <> value Then
                _genre = value
                Tags.Add(_genre)
                OnPropertyChanged()
                OnPropertyChanged("Category")
                OnPropertyChanged("System")
                OnPropertyChanged("Tags")
            End If
        End Set
    End Property

    <XmlElement("system", IsNullable:=True)>
    Public Overridable Property System As String
        Get
            Return _system
        End Get
        Set(value As String)
            If _system <> value Then
                _system = value
                Tags.Add(_system)
                OnPropertyChanged()
                OnPropertyChanged("Genre")
                OnPropertyChanged("Category")
                OnPropertyChanged("Tags")
            End If
        End Set
    End Property

    <XmlArray("tags")>
    <XmlArrayItem("tag")>
    Public Overridable Property Tags As BindingList(Of String)
        Get
            Return _tags
        End Get
        Set(value As BindingList(Of String))
            _tags = value
            OnPropertyChanged()
        End Set
    End Property

    <XmlIgnore>
    Public ReadOnly Property TagList As String
        Get
            Return Join(Tags.ToArray, ", ")
        End Get
    End Property

    <XmlArray("parameters")>
    <XmlArrayItem("parameter")>
    Public Overridable Property Parameters As ObservableCollection(Of Parameter)
        Get
            Return _parameters
        End Get
        Set(value As ObservableCollection(Of Parameter))
            If Not _parameters.Equals(value) Then
                _parameters = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    <XmlElement("supportsMaxLength")>
    Public Overridable Property SupportsMaxLength As Boolean
        Get
            Return _supportsMaxLength
        End Get
        Set(value As Boolean)
            If _supportsMaxLength <> value Then
                _supportsMaxLength = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    <XmlElement("url")>
    Public Overridable Property URL As String
        Get
            Return _url
        End Get
        Set(value As String)
            If _url <> value Then
                _url = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    <XmlElement("visible")>
    Public Overridable Property Visible As Boolean = True

    <XmlElement("oddResultColor")>
    Public Overridable Property OddResultColor As Color = DEFAULT_ODD_COLOR

    <XmlElement("evenResultColor")>
    Public Overridable Property EvenResultColor As Color = DEFAULT_EVEN_COLOR

    <XmlElement("dividerColor")>
    Public Overridable Property DividerColor As Color = DEFAULT_DIVIDER_COLOR

    <XmlIgnore()>
    Public Overridable Property ResultFont As System.Drawing.Font = DEFAULT_RESULT_FONT

    <XmlElement("css")>
    Public Overridable Property CSS As String

    <XmlIgnore>
    Public Overridable ReadOnly Property OddResultColorSpecified As Boolean
        Get
            Return OddResultColor <> DEFAULT_ODD_COLOR
        End Get
    End Property

    <XmlIgnore>
    Public Overridable ReadOnly Property EvenResultColorSpecified As Boolean
        Get
            Return EvenResultColor <> DEFAULT_EVEN_COLOR
        End Get
    End Property

    <XmlIgnore>
    Public Overridable ReadOnly Property DividerColorSpecified As Boolean
        Get
            Return DividerColor <> DEFAULT_DIVIDER_COLOR
        End Get
    End Property

    <XmlIgnore>
    Public Overridable ReadOnly Property ResultFontSpecified As Boolean
        Get
            Return Not ResultFont.Equals(DEFAULT_RESULT_FONT)
        End Get
    End Property

    <XmlIgnore>
    Public Overridable ReadOnly Property CSSSpecified As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(CSS)
        End Get
    End Property

    <XmlIgnore>
    Public Property IsDirty As Boolean = False

    <XmlIgnore>
    Public Overridable Property Variables As New Dictionary(Of String, Object)

    <XmlIgnore>
    Public Property Variable(ByVal name As String) As Object
        Get
            If Not Variables.ContainsKey(name) Then Variables.Add(name, String.Empty)
            Return Variables(name)
        End Get
        Set(value As Object)
            If Not Variables.ContainsKey(name) Then
                Variables.Add(name, value)
            Else
                Variables(name) = value
            End If
        End Set
    End Property

    <XmlIgnore>
    Public ReadOnly Property CategorySpecified As Boolean
        Get
            Return Category IsNot Nothing
        End Get
    End Property

    <XmlIgnore>
    Public ReadOnly Property GenreSpecified As Boolean
        Get
            Return Genre IsNot Nothing
        End Get
    End Property

    <XmlIgnore>
    Public ReadOnly Property SystemSpecified As Boolean
        Get
            Return System IsNot Nothing
        End Get
    End Property

    Protected Property MaxLength As Int32 = Int32.MaxValue

    <XmlIgnore>
    Public ReadOnly Property Parameter(ByVal name As String) As Object
        Get
            Dim value As Object = Parameters.FirstOrDefault(Function(p As Parameter) p.Name.Equals(name)).Value
            Return value
        End Get
    End Property

    <XmlIgnore>
    Public Overridable ReadOnly Property [Error] As String Implements IDataErrorInfo.Error
        Get
            Return String.Empty
        End Get
    End Property

    <XmlIgnore>
    Default Public Overridable ReadOnly Property Item(columnName As String) As String Implements IDataErrorInfo.Item
        Get
            If SkipValidation Then Return String.Empty
            Select Case columnName
                Case HelperMethods.GetPropertyName(Function(bg As BaseGrammar) bg.Name) : If String.IsNullOrWhiteSpace(Name) Then Return "Name is required."
                Case HelperMethods.GetPropertyName(Function(bg As BaseGrammar) bg.Description) : If String.IsNullOrWhiteSpace(Description) Then Return "Description is required."
                Case HelperMethods.GetPropertyName(Function(bg As BaseGrammar) bg.Genre),
                     HelperMethods.GetPropertyName(Function(bg As BaseGrammar) bg.System),
                     HelperMethods.GetPropertyName(Function(bg As BaseGrammar) bg.Category) : If FilterComponentsInError Then Return "At least one of Genre, System or Category must exist."
            End Select
            Return String.Empty
        End Get
    End Property

    <XmlIgnore>
    Private ReadOnly Property FilterComponentsInError As Boolean
        Get
            Return String.IsNullOrWhiteSpace(Genre) AndAlso
                   String.IsNullOrWhiteSpace(System) AndAlso
                   String.IsNullOrWhiteSpace(Category)
        End Get
    End Property

    <XmlIgnore>
    Public Property SkipValidation As Boolean
        Get
            Return _skipValidation
        End Get
        Set(value As Boolean)
            If _skipValidation <> value Then
                _skipValidation = value
                OnPropertyChanged()
                OnPropertyChanged("Name")
                OnPropertyChanged("Description")
                OnPropertyChanged("Genre")
                OnPropertyChanged("System")
                OnPropertyChanged("Category")
            End If
        End Set
    End Property
#End Region

#Region "Public Methods"
    Public Function GenerateNames(ByVal count As Int32,
                                  ByVal maxLength As Int32,
                                  ByVal parameters As Dictionary(Of String, Object)) As String
        Dim value As New List(Of String)
        Me.MaxLength = maxLength
        If parameters IsNot Nothing Then
            For Each parameter As KeyValuePair(Of String, Object) In parameters
                Dim param As Parameter = Me.Parameters.Where(Function(p As Parameter) p.Name.Equals(parameter.Key))(0)
                param.Value = CStr(parameter.Value)
            Next
        End If
        For i As Int32 = 0 To count - 1
            If _cancel Then
                _cancel = False
                If value IsNot Nothing AndAlso value.Count > 0 Then Return FormatResults(value)
                Return String.Empty
            End If
            value.Add(GenerateName)
            RaiseEvent ProgressUpdate(Me, New ProgressUpdateEventArgs(i + 1))
        Next
        Return FormatResults(value)
    End Function

    Public Sub Cancel()
        _cancel = True
    End Sub

    Public Overridable Sub Serialize(ByVal fileName As String)
        Dim knownTypes() As Type = Assembly.GetExecutingAssembly.GetTypes.Where(Function(t As Type) GetType(BaseGrammar).IsAssignableFrom(t)).ToArray
        Dim serializer As New XmlSerializer(Me.GetType, knownTypes)
        Using file As New IO.FileStream(fileName, IO.FileMode.Create)
            serializer.Serialize(file, Me)
        End Using
    End Sub

    Public Overrides Function ToString() As String
        Dim knownTypes() As Type = Assembly.GetExecutingAssembly.GetTypes.Where(Function(t As Type) GetType(BaseGrammar).IsAssignableFrom(t)).ToArray
        Dim serializer As New XmlSerializer(GetType(BaseGrammar), knownTypes)
        Using stream As New IO.MemoryStream
            serializer.Serialize(stream, Me)
            Return Encoding.UTF8.GetString(stream.ToArray)
        End Using
    End Function

    Public Overridable Function GetParameter(ByVal name As String) As Parameter
        Return Parameters.FirstOrDefault(Function(p As Parameter) p.Name.Equals(name))
    End Function

    Public Overridable Function ParameterExists(ByVal name As String) As Boolean
        Return Parameters.FirstOrDefault(Function(p As Parameter) p.Name.Equals(name)) IsNot Nothing
    End Function

    Public MustOverride Function Analyze() As String

    Protected Overridable Sub OnPropertyChanged(<CallerMemberNameAttribute> Optional ByVal propertyName As String = Nothing)
        IsDirty = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
#End Region

#Region "Protected Methods"
    Protected MustOverride Function GenerateName() As String

    Protected Overridable Sub Initialize()
    End Sub

    Protected Overridable Function FormatResults(ByVal values As List(Of String)) As String
        Const DIV_OPEN As String = "<div class='result {0}'>"
        Const NEW_LINE As String = "<br />"
        Const DIV_CLOSE As String = "</div>"
        Const BODY_PARAMETER As String = "{Body}"
        Const FONT_FAMILY_PARAMETER As String = "{FontFamily}"
        Const FONT_SIZE_PARAMETER As String = "{FontSize}"
        Const ODD_COLOR_PARAMETER As String = "{OddColor}"
        Const EVEN_COLOR_PARAMETER As String = "{EvenColor}"
        Const DIVIDER_COLOR_PARAMETER As String = "{DividerColor}"
        Const CSS_PARAMETER As String = "{CSS}"
        Const HTML_DIV_ODD As String = "odd"
        Const HTML_DIV_EVEN As String = "even"
        Dim results As New StringBuilder
        Dim html As String
        Dim odd As Boolean = True

        For Each value As String In values
            results.AppendFormat(DIV_OPEN, If(odd, HTML_DIV_ODD, HTML_DIV_EVEN))
            results.Append(value.Replace(Environment.NewLine, NEW_LINE))
            If value.Contains(Environment.NewLine) Then results.AppendLine(NEW_LINE)
            results.Append(DIV_CLOSE)
            odd = Not odd
        Next

        html = My.Resources.ResultsTemplate.Replace(BODY_PARAMETER, results.ToString)
        html = html.Replace(CSS_PARAMETER, CSS)
        html = html.Replace(ODD_COLOR_PARAMETER, ColorToHTML(OddResultColor))
        html = html.Replace(EVEN_COLOR_PARAMETER, ColorToHTML(EvenResultColor))
        html = html.Replace(DIVIDER_COLOR_PARAMETER, ColorToHTML(DividerColor))
        html = html.Replace(FONT_FAMILY_PARAMETER, ResultFont.FontFamily.Name)
        html = html.Replace(FONT_SIZE_PARAMETER, ResultFont.Size.ToString)

        Return html
    End Function

#End Region

#Region "Private Methods"
    Private Shared Function ColorToHTML(ByVal color As Color) As String
        Return String.Format(HTML_COLOR_FORMAT, color.R, color.G, color.B)
    End Function
#End Region

#Region "Event Handlers"
    Private Sub _parameters_CollectionChanged(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs) Handles _parameters.CollectionChanged
        IsDirty = True
    End Sub
#End Region
End Class
