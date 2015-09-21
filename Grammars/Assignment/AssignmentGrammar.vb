Option Infer On
Option Strict Off

Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.FileIO
Imports Utility
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports System.ComponentModel
Imports YamlDotNet.Serialization

Namespace Assignment
    <XmlRoot("grammar")>
    Public Class AssignmentGrammar
        Inherits BaseGrammar

#Region "Enumerators"
        Private Enum TokenType
            None = 0
            Identifier = 1
            [String] = 2
            Calc = 3
        End Enum
#End Region

#Region "Classes"
        Public Class ItemFile
            Implements INotifyPropertyChanged
            Implements IDataErrorInfo

            Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

            Public Sub New()
            End Sub

            Private _fileName As String
            Private _label As String

            Public Property FileName As String
                Get
                    Return _fileName
                End Get
                Set(value As String)
                    If _fileName <> value Then
                        _fileName = value
                        If String.IsNullOrEmpty(Label) Then Label = IO.Path.GetFileNameWithoutExtension(value)
                        OnPropertyChanged()
                    End If
                End Set
            End Property

            Public Property Label As String
                Get
                    Return _label
                End Get
                Set(value As String)
                    If _label <> value Then
                        _label = value
                        OnPropertyChanged()
                    End If
                End Set
            End Property

            Protected Overridable Sub OnPropertyChanged(<CallerMemberNameAttribute> Optional ByVal propertyName As String = Nothing)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
            End Sub

            Public ReadOnly Property [Error] As String Implements IDataErrorInfo.Error
                Get
                    Return String.Empty
                End Get
            End Property

            Default Public ReadOnly Property Item(columnName As String) As String Implements IDataErrorInfo.Item
                Get
                    Select Case columnName
                        Case HelperMethods.GetPropertyName(Function(i As ItemFile) i.FileName)
                            If Not IO.File.Exists(FileName) Then Return "File does not exist."
                        Case HelperMethods.GetPropertyName(Function(i As ItemFile) i.Label)
                            If String.IsNullOrWhiteSpace(Label) Then Return "Label cannot be blank."
                    End Select
                    Return String.Empty
                End Get
            End Property
        End Class

        Private Class SampleItem
            Public Sub New(ByVal previous As String,
                           ByVal [next] As String)
                Me.Previous = previous
                Me.Next = [next]
            End Sub

            Public Property Previous As String
            Public Property [Next] As String
            Public Property Weight As Int32 = 1
        End Class
#End Region

#Region "Shared Methods"
        Public Shared Function MCGenerator(ByVal name As String,
                                           ByVal description As String,
                                           ByVal author As String,
                                           ByVal tags As BindingList(Of String),
                                           ByVal supportsMaxLength As Boolean,
                                           ByVal sourceFile As String,
                                           ByVal syllableLength As Int32,
                                           ByVal maxWeight As Int32,
                                           ByVal prefix As String,
                                           ByVal createLibrary As Boolean) As Object
            Dim samples As New List(Of SampleItem)
            Dim source As New List(Of String)(IO.File.ReadAllLines(sourceFile))
            Dim rules As New List(Of LineItem)
            Dim value As Object

            For Each sample As String In source
                Dim i As Int32 = 0
                Dim diff As Int32 = sample.Length Mod syllableLength
                Dim previous As String = String.Empty
                Dim current As String

                If diff <> 0 Then
                    sample &= Space(diff)
                End If
                Do While i < sample.Length
                    current = sample.Substring(i, syllableLength)
                    Dim existing As SampleItem = samples.Find(Function(si As SampleItem) (si.Previous = previous And si.Next = current))
                    If existing IsNot Nothing Then
                        existing.Weight += 1
                    Else
                        samples.Add(New SampleItem(previous, current))
                    End If
                    previous = current
                    i += syllableLength
                Loop
                current = String.Empty
                Dim final As SampleItem = samples.Find(Function(si As SampleItem) (si.Previous = previous And si.Next = current))
                If final IsNot Nothing Then
                    final.Weight += 1
                Else
                    samples.Add(New SampleItem(previous, current))
                End If
            Next

            For Each item As SampleItem In samples
                If String.IsNullOrWhiteSpace(item.Previous) Then
                    If createLibrary Then
                        rules.Add(New LineItem(prefix,
                                               String.Empty,
                                               If(String.IsNullOrEmpty(prefix), String.Format(ITEM_FORMAT, item.Next), String.Format(ITEM_FORMAT_WITH_PREFIX, prefix, item.Next)),
                                               item.Weight))
                    Else
                        rules.Add(New LineItem(START_LABEL,
                                               String.Empty,
                                               If(String.IsNullOrEmpty(prefix), String.Format(ITEM_FORMAT, item.Next), String.Format(ITEM_FORMAT_WITH_PREFIX, prefix, item.Next)),
                                               item.Weight))
                    End If
                Else
                    rules.Add(New LineItem(String.Format(NAME_FORMAT_WITH_PREFIX, prefix, item.Previous),
                                           If(String.IsNullOrEmpty(prefix), item.Next, String.Format(NAME_FORMAT_WITH_PREFIX, prefix, item.Next)),
                                           item.Previous,
                                           item.Weight))
                End If
            Next

            rules.Sort()

            If createLibrary Then
                Dim library As New Library
                library.Rules.AddRange(rules)
                value = library
            Else
                Dim grammar As New AssignmentGrammar
                grammar.Name = name
                grammar.Description = description
                grammar.Author = author
                grammar.Tags = tags
                grammar.SupportsMaxLength = supportsMaxLength
                grammar.Rules.AddRange(rules)
                value = grammar
            End If

            Return value
        End Function

        Public Shared Function ItemListConverter(ByVal name As String,
                                                 ByVal description As String,
                                                 ByVal author As String,
                                                 ByVal tags As BindingList(Of String),
                                                 ByVal supportsMaxLength As Boolean,
                                                 ByVal listFiles As IEnumerable(Of ItemFile),
                                                 ByVal weightAdjustment As Int32,
                                                 ByVal removeDuplicates As Boolean,
                                                 ByVal caseSensitive As Boolean) As AssignmentGrammar
            Dim grammar As New AssignmentGrammar With {.Name = name,
                                                       .Description = description,
                                                       .Author = author,
                                                       .Tags = tags,
                                                       .SupportsMaxLength = supportsMaxLength}
            Dim rules As List(Of LineItem) = CreateRules(listFiles,
                                                         weightAdjustment,
                                                         removeDuplicates,
                                                         caseSensitive)
            For Each rule As LineItem In rules
                grammar.Rules.Add(rule)
            Next

            Return grammar
        End Function

        Friend Shared Function CreateRules(ByVal listFiles As IEnumerable(Of ItemFile),
                                           ByVal weightAdjustment As Int32,
                                           ByVal removeDuplicates As Boolean,
                                           ByVal caseSensitive As Boolean) As List(Of LineItem)
            Dim rank As Int32 = 1
            Dim [step] As Int32 = 0
            Dim lines As Dictionary(Of ItemFile, List(Of String)) = GetFileContents(listFiles,
                                                                                    removeDuplicates,
                                                                                    caseSensitive)
            Dim rules As New List(Of LineItem)
            For Each entry As KeyValuePair(Of ItemFile, List(Of String)) In lines
                Dim label As String = entry.Key.Label
                If weightAdjustment < 0 Then
                    rank = entry.Value.Count
                Else
                    rank = 1
                End If

                For Each line As String In entry.Value
                    Dim rule As New LineItem(label,
                                             String.Empty,
                                             line)
                    rule.Weight = rank
                    rank += weightAdjustment
                    rules.Add(rule)
                Next
            Next
            Return rules
        End Function

        Private Shared Function GetFileContents(ByVal files As IEnumerable(Of ItemFile),
                                                ByVal removeDuplicates As Boolean,
                                                ByVal caseSensitive As Boolean) As Dictionary(Of ItemFile, List(Of String))
            Dim values As New Dictionary(Of ItemFile, List(Of String))

            For Each file As ItemFile In files
                Dim fileValues As New List(Of String)(IO.File.ReadAllLines(file.FileName))

                fileValues.RemoveAll(Function(s As String) String.IsNullOrWhiteSpace(s))

                If removeDuplicates Then
                    Dim comparer As StringComparer
                    Dim newValues As New List(Of String)

                    If caseSensitive Then
                        comparer = StringComparer.CurrentCulture
                    Else
                        comparer = StringComparer.CurrentCultureIgnoreCase
                    End If

                    For Each line As String In fileValues
                        If Not newValues.Contains(line, comparer) Then
                            newValues.Add(line)
                        End If
                    Next

                    fileValues = newValues
                End If

                values.Add(file, fileValues)
            Next
            Return values
        End Function

#End Region

#Region "Constants"
        Private Const IDENTIFIER_START As Char = "["c
        Private Const IDENTIFIER_END As Char = "]"c
        Private Const CALC_NOTIFICATION As Char = "="c
        Private Const START_LABEL As String = "Start"
        Private Const END_ITEM As String = "Stop"
        Private Const LENGTH_PARAMETER As String = "Length"
        Private Const OR_OPERATOR As Char = "|"c
        Private Const AND_OPERATOR As Char = "+"c
        Private Const LIBRARY_EXTENSION As String = ".lib.xml"

        Private Const ITEMS_PARAMETER As String = "Items"
        Private Const ITEMS_FUNCTION As String = "Items"
        Private Const ITEM_FORMAT As String = "[{0}]"
        Private Const ITEM_FORMAT_WITH_PREFIX As String = "[{0}{1}]"
        Private Const NAME_FORMAT_WITH_PREFIX As String = "{0}{1}"

        Private Const REPEAT_FUNCTION As String = "Repeat"
#End Region

#Region "Classes"
        Private Class CachedLineItem

            Public Sub New()
                Me.TotalWeight = 0
                Me.LastSelect = 0
                Me.Rules = New List(Of LineItem)
            End Sub

            Public Sub New(ByVal totalWeight As Int32,
                           ByVal lastSelect As Int32)
                Me.TotalWeight = totalWeight
                Me.LastSelect = lastSelect
                Me.Rules = New List(Of LineItem)
            End Sub

            Public Sub New(ByVal totalWeight As Int32,
                           ByVal lastSelect As Int32,
                           ByVal rule As LineItem)
                Me.TotalWeight = totalWeight
                Me.LastSelect = lastSelect
                Me.Rules = New List(Of LineItem) From {rule}
            End Sub

            Public Sub New(ByVal totalWeight As Int32,
                           ByVal lastSelect As Int32,
                           ByVal rules As List(Of LineItem))
                Me.TotalWeight = totalWeight
                Me.LastSelect = lastSelect
                Me.Rules = rules
            End Sub

            Public Property TotalWeight As Int32
            Public Property LastSelect As Int32
            Public Property Rules As List(Of LineItem)
        End Class
#End Region

#Region "Members"
        Private WithEvents _rules As New ObservableCollection(Of LineItem)
        Private WithEvents _import As New ObservableCollection(Of String)
        Private WithEvents _calculator As New Calculator(Me)
        Private _rulesByName As New Dictionary(Of String, CachedLineItem)
        Private _name As String
        Private _startItem As String = START_LABEL
        Private _itemsEvaluated As Int32 = 0
        Private _itemsEvaluatedByName As New Dictionary(Of String, Int32)
#End Region

#Region "Constructors"
#End Region

#Region "Properties"
        <XmlArray("items")>
        <XmlArrayItem("item")>
        <YamlMember(Alias:="items")>
        Public Property Rules As ObservableCollection(Of LineItem)
            Get
                Return _rules
            End Get
            Set(value As ObservableCollection(Of LineItem))
                _rules = value
                OnPropertyChanged()
            End Set
        End Property

        <XmlArray("imports")>
        <XmlArrayItem("import")>
        <YamlMember(Alias:="imports")>
        Public Property Import As ObservableCollection(Of String)
            Get
                Return _import
            End Get
            Set(value As ObservableCollection(Of String))
                _import = value
                OnPropertyChanged()
            End Set
        End Property
#End Region

#Region "Public Methods"
        Public Overrides Function Analyze() As String
            Const tableRow As String = "<tr>{0}</tr>"
            Const tableCell As String = "<td>{0}</td>"
            Const tableNumberCell As String = "<td class='number'>{0}</td>"

            Dim html As String = My.Resources.AssignmentGrammarAnalysis
            Dim parameters As New StringBuilder
            Dim rules As New StringBuilder
            Dim importList As New StringBuilder
            Dim warnings As New StringBuilder
            Dim foundRules As New List(Of String)

            GatherImports()

            html = html.Replace("[Name]", Me.Name)
            html = html.Replace("[Description]", Me.Description)
            html = html.Replace("[Author]", Me.Author)
            html = html.Replace("[Category]", Me.Category)
            html = html.Replace("[ParameterCount]", Me.Parameters.Count.ToString)
            html = html.Replace("[RuleCount]", Me.Rules.Count.ToString("#,##0"))
            html = html.Replace("[XMLLength]", Split(Me.ToString, Environment.NewLine).Count.ToString("#,##00"))

            For Each parameter As Parameter In Me.Parameters
                Dim row As New StringBuilder
                row.AppendFormatLine(tableCell, parameter.Name)
                parameters.AppendFormatLine(tableRow, row.ToString)
            Next

            html = html.Replace("[Parameters]", parameters.ToString)
            Dim query As IEnumerable = Me.Rules.GroupBy(Function(li As LineItem) li.Name).Select(Function(li) New With {
                                                                                                     Key .key = li.Key,
                                                                                                     Key .weight = li.Sum(Function(w As LineItem) w.Weight),
                                                                                                     Key .count = li.Count})

            For Each current In query
                Dim row As New StringBuilder
                row.AppendFormatLine(tableCell, current.key)
                row.AppendFormatLine(tableNumberCell, current.count)
                row.AppendFormatLine(tableNumberCell, current.weight)
                rules.AppendFormatLine(tableRow, row.ToString)
            Next

            html = html.Replace("[Rules]", rules.ToString)

            For Each rule As LineItem In Me.Rules
                Dim parsed As List(Of KeyValuePair(Of TokenType, String)) = ParseExpression(rule.Expression)
                Dim [next] As String = rule.Next

                If Not String.IsNullOrWhiteSpace([next]) AndAlso Not foundRules.Contains([next]) Then
                    If Me.Rules.Where(Function(li As LineItem) li.Name = [next]).Count > 0 Then
                        foundRules.Add([next])
                    Else
                        Dim warning As New StringBuilder
                        warning.AppendFormatLine(tableCell, [next])
                        warning.AppendFormatLine(tableCell, "next")
                        warning.AppendFormatLine(tableCell, "Could not locate rule.")
                        warnings.AppendFormatLine(tableRow, warning)
                    End If
                End If

                For Each item As KeyValuePair(Of TokenType, String) In parsed
                    If item.Key = TokenType.Identifier Then
                        Dim parts() As String = item.Value.Split(AND_OPERATOR, OR_OPERATOR)
                        For Each part As String In parts
                            Dim name As String = part.Remove(IDENTIFIER_START, IDENTIFIER_END)
                            If Not foundRules.Contains(name) AndAlso Me.Rules.Where(Function(li As LineItem) li.Name = name).Count > 0 Then
                                foundRules.Add(name)
                            Else
                                Dim warning As New StringBuilder
                                warning.AppendFormatLine(tableCell, name)
                                warning.AppendFormatLine(tableCell, "expression")
                                warning.AppendFormatLine(tableCell, "Could not locate rule.")
                                warnings.AppendFormatLine(tableRow, warning)
                            End If
                        Next
                    End If
                Next
            Next

            html = html.Replace("[Warnings]", warnings.ToString)

            Return html
        End Function
#End Region

#Region "Protected Methods"
        Protected Overrides Function GenerateName() As String
            Dim calc As NCalc.Expression
            Dim startItem As LineItem
            GatherImports()
            _name = String.Empty
            calc = New NCalc.Expression(_startItem)
            AddHandler calc.EvaluateParameter, AddressOf EvaluateParameter
            startItem = GetItemByName(_startItem)
            Evaluate(startItem)
            Return If(_name.Length > MaxLength, _name.Substring(0, MaxLength), _name)
        End Function
#End Region

#Region "Private Methods"
        Private Sub GatherImports()
            Dim paths As New List(Of String)
            If Not String.IsNullOrEmpty(FileName) Then paths.Add(FileName)
            paths.AddRange(Utility.GrammarFilePaths)
            For Each item As String In Import
                Dim library As Library
                Dim libraryPath As String = item
                If Not IO.Path.HasExtension(libraryPath) Then libraryPath &= LIBRARY_EXTENSION
                If Not IO.Path.IsPathRooted(libraryPath) Then
                    Dim path As String = String.Empty
                    Dim i As Int32 = 0
                    Do While Not IO.File.Exists(path) AndAlso i < paths.Count
                        path = IO.Path.Combine(paths(i), libraryPath)
                        i += 1
                    Loop
                    If Not IO.File.Exists(path) Then Throw New IO.FileNotFoundException("Could not locate library the file " & item)
                    libraryPath = path
                End If
                library = library.FromFile(libraryPath)
                For Each rule As LineItem In library.Rules
                    Rules.Add(rule)
                Next
            Next
            Import.Clear()
        End Sub

        Private Function GetCalculator(ByVal expression As String) As NCalc.Expression
            Dim calc As New NCalc.Expression(expression)
            calc.Parameters.Add(LENGTH_PARAMETER, _name.Length)
            AddHandler calc.EvaluateParameter, AddressOf EvaluateParameter
            Return calc
        End Function

        Private Function GetItemByName(ByVal name As String) As LineItem
            Dim nameListOr As String() = Split(name, OR_OPERATOR)

            If nameListOr.Count > 1 Then
                name = nameListOr(Random.Next(0, nameListOr.Count))
            End If

            If _rulesByName Is Nothing OrElse Not _rulesByName.ContainsKey(name) Then
                Dim nameListAnd As String() = name.Split(AND_OPERATOR)
                _rulesByName.Add(name, New CachedLineItem)
                For Each nameItem As String In nameListAnd
                    For Each item As LineItem In CType(Rules.Where(Function(li As LineItem) li.Name = nameItem.Remove("[", "]")).ToList, List(Of LineItem))
                        _rulesByName(name).TotalWeight += item.Weight
                        _rulesByName(name).Rules.Add(item)
                    Next
                Next
            End If

            If _rulesByName(name).TotalWeight > 0 Then
                Dim sum As Int32 = 0
                Dim index As Int32 = (BaseGrammar.Random.Next Mod _rulesByName(name).TotalWeight) + 1
                Do While _rulesByName(name).Rules.Count > 1 AndAlso index = _rulesByName(name).LastSelect
                    index = (BaseGrammar.Random.Next Mod _rulesByName(name).TotalWeight) + 1
                Loop
                _rulesByName(name).LastSelect = index
                For Each item As LineItem In _rulesByName(name).Rules
                    sum += CInt(item.Weight)
                    If sum >= index Then
                        If Not _itemsEvaluatedByName.ContainsKey(name) Then _itemsEvaluatedByName.Add(name, 0)
                        _itemsEvaluatedByName(name) += 1
                        _itemsEvaluated += 1
                        Return item
                    End If
                Next
            End If
            Return Nothing
        End Function

        Private Sub Evaluate(ByVal item As LineItem)
            If item IsNot Nothing AndAlso _name.Length < MaxLength Then
                If Not String.IsNullOrWhiteSpace(item.Expression) Then
                    Dim parts As List(Of KeyValuePair(Of TokenType, String)) = ParseExpression(item.Expression)

                    For Each part As KeyValuePair(Of TokenType, String) In parts
                        Select Case part.Key
                            Case TokenType.String
                                If String.IsNullOrWhiteSpace(item.Variable) Then
                                    _name &= part.Value
                                Else
                                    Variable(item.Variable) &= part.Value
                                End If
                            Case TokenType.Identifier
                                Dim lineItem As LineItem = GetItemByName(part.Value)
                                If lineItem Is Nothing Then
                                    Dim parameterValue As String = GetValue(part.Value)
                                    If Not String.IsNullOrWhiteSpace(parameterValue) Then
                                        lineItem = GetItemByName(parameterValue)
                                    End If
                                End If
                                Evaluate(lineItem)
                            Case TokenType.Calc
                                Dim expression As String = part.Value.Substring(1, part.Value.Length - 2)
                                If String.IsNullOrWhiteSpace(item.Variable) Then
                                    Evaluate(New LineItem(item.Name, String.Empty, CStr(_calculator.Evaluate(expression))))
                                Else
                                    Variable(item.Variable) &= _calculator.Evaluate(expression).ToString
                                End If
                        End Select
                    Next
                End If
                If Not String.IsNullOrWhiteSpace(item.Next) AndAlso
                   Not item.Next.Equals(END_ITEM, StringComparison.CurrentCultureIgnoreCase) Then
                    If item.Next.StartsWith(CALC_NOTIFICATION) Then
                        Evaluate(GetItemByName(_calculator.Evaluate(item.Next.Substring(1)).ToString))
                    Else
                        Evaluate(GetItemByName(item.Next))
                    End If
                End If
            End If
        End Sub

        Private Sub SetVariable(ByVal variable As String,
                                ByVal value As String)
            If Variables.ContainsKey(variable) Then
                Variables(variable) = value
            Else
                Variables.Add(variable, value)
            End If
        End Sub

        Private Function ParseExpression(ByVal expression As String) As List(Of KeyValuePair(Of TokenType, String))
            Dim value As New List(Of KeyValuePair(Of TokenType, String))
            Dim type As TokenType
            Dim current As String = String.Empty
            Dim openBrackets As Int32 = 0

            For i As Int32 = 0 To expression.Length - 1
                Dim c As Char = expression(i)
                Select Case c
                    Case CALC_NOTIFICATION
                        If type = TokenType.Identifier Then
                            type = TokenType.Calc
                        Else
                            If type = TokenType.None Then type = TokenType.String
                            current &= c
                        End If
                    Case IDENTIFIER_START
                        openBrackets += 1
                        If openBrackets = 1 Then
                            If Not String.IsNullOrEmpty(current) Then value.Add(New KeyValuePair(Of TokenType, String)(type, current))
                            type = TokenType.Identifier
                            current = c
                        Else
                            current &= c
                        End If
                    Case IDENTIFIER_END
                        current &= c
                        openBrackets -= 1
                        If type.In(TokenType.Identifier, TokenType.Calc) AndAlso openBrackets = 0 Then
                            If Not String.IsNullOrEmpty(current) Then value.Add(New KeyValuePair(Of TokenType, String)(type, current))
                            type = TokenType.None
                            current = String.Empty
                        End If
                    Case Else
                        If type = TokenType.None Then type = TokenType.String
                        current &= c
                End Select
            Next
            If Not String.IsNullOrEmpty(current) Then value.Add(New KeyValuePair(Of TokenType, String)(type, current))

            Return value
        End Function
#End Region

#Region "Event Handlers"
        Private Function GetValue(ByVal name As String) As String
            Dim value As String = If(name.StartsWith(IDENTIFIER_START) AndAlso name.EndsWith(IDENTIFIER_END), name, IDENTIFIER_START & name & IDENTIFIER_END)
            Dim parameter As Parameter = Parameters.FirstOrDefault(Function(p As Parameter) IDENTIFIER_START & p.Name & IDENTIFIER_END = value)
            If parameter IsNot Nothing Then Return parameter.Value
            Dim variable As String = If(Variables.ContainsKey(name), Variables(name), String.Empty)
            Return String.Empty
        End Function

        Private Sub EvaluateParameter(ByVal name As String,
                                      ByVal e As NCalc.ParameterArgs)
            Dim parameter As Parameter = Parameters.FirstOrDefault(Function(p As Parameter) p.Name = name)
            If parameter IsNot Nothing Then e.Result = parameter.Value
        End Sub

        Private Sub _calculator_EvaluateFunction(ByVal name As String,
                                                 ByVal args As NCalc.FunctionArgs) Handles _calculator.EvaluateFunction
            Select Case name
                Case ITEMS_FUNCTION
                    If args.Parameters.Count = 1 Then
                        Dim itemName As String = args.Parameters(0).Evaluate
                        If _itemsEvaluatedByName.ContainsKey(itemName) Then
                            args.Result = _itemsEvaluatedByName(itemName)
                        Else
                            args.Result = 0
                        End If
                    ElseIf args.Parameters.Count = 0 Then
                        args.Result = _itemsEvaluated
                    Else
                        Throw New NCalc.EvaluationException("Invalid number of arguments for 'Generate' function, 0 or 1 expected.")
                    End If
            End Select
        End Sub

        Private Sub _calculator_EvaluateParameter(ByVal name As String,
                                                  ByVal args As NCalc.ParameterArgs) Handles _calculator.EvaluateParameter
            If Variables.ContainsKey(name) Then
                args.Result = Variable(name)
            Else
                Select Case name
                    Case ITEMS_PARAMETER : args.Result = _itemsEvaluated
                End Select
            End If
        End Sub

        Private Sub _import_CollectionChanged(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs) Handles _import.CollectionChanged
            IsDirty = True
        End Sub

        Private Sub _rules_CollectionChanged(sender As Object, e As Specialized.NotifyCollectionChangedEventArgs) Handles _rules.CollectionChanged
            IsDirty = True
        End Sub
#End Region
    End Class
End Namespace