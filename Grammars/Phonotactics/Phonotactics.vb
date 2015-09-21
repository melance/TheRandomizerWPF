Imports System.Xml.Serialization
Imports Utility
Imports System.Text
Imports System.Globalization
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports YamlDotNet.Serialization

Namespace Phonotactics

    Public Enum CaseType
        None
        Proper
        Upper
        Lower
    End Enum

    <System.Xml.Serialization.XmlRoot("Phonotactics", IsNullable:=False)>
    Partial Public Class PhonotacticsGrammar
        Inherits BaseGrammar

        Public Shared Function GenerateGrammar(ByVal name As String,
                                               ByVal description As String,
                                               ByVal author As String,
                                               ByVal tags As BindingList(Of String),
                                               ByVal supportsMaxLength As Boolean,
                                               ByVal sampleFile As String,
                                               ByVal definitionFile As String,
                                               ByVal maxWeight As Int32) As PhonotacticsGrammar
            Dim grammar As New PhonotacticsGrammar
            Dim samples As New List(Of String)(IO.File.ReadAllLines(sampleFile))
            Dim definitions As New List(Of String)(IO.File.ReadAllLines(definitionFile))

            grammar.Name = name
            grammar.Author = author
            grammar.Tags = tags
            grammar.Description = description
            grammar.SupportsMaxLength = supportsMaxLength

            For Each definition As String In definitions
                Dim key As Char = definition(0)
                Dim newDef As New Phonotactics.Definition
                newDef.Delimiter = ","
                newDef.Key = key
                newDef.Value = definition.Substring(2)
                DirectCast(grammar, Phonotactics.PhonotacticsGrammar).Definitions.Add(newDef)
            Next

            For Each sample As String In samples
                If Not String.IsNullOrWhiteSpace(sample) Then
                    Dim item As String = sample
                    Dim result(item.Length) As String
                    Dim pattern As String
                    For Each definition As Phonotactics.Definition In DirectCast(grammar, Phonotactics.PhonotacticsGrammar).Definitions
                        For Each value As String In definition.ValueList
                            Dim index As Int32 = CultureInfo.CurrentCulture.CompareInfo.IndexOf(item, value, CompareOptions.IgnoreCase)
                            Do While index >= 0
                                result(index) = definition.Key
                                index = CultureInfo.CurrentCulture.CompareInfo.IndexOf(item, value, index + 1, CompareOptions.IgnoreCase)
                            Loop
                        Next
                    Next
                    pattern = Join(result, String.Empty)
                    pattern = pattern.Trim
                    If DirectCast(grammar, Phonotactics.PhonotacticsGrammar).Patterns.FirstOrDefault(Function(p As Phonotactics.Pattern) p.Value = pattern) Is Nothing Then
                        DirectCast(grammar, Phonotactics.PhonotacticsGrammar).Patterns.Add(New Phonotactics.Pattern(pattern))
                    Else
                        DirectCast(grammar, Phonotactics.PhonotacticsGrammar).Patterns.FirstOrDefault(Function(p As Phonotactics.Pattern) p.Value = pattern).Weight += 1
                    End If
                End If
            Next

            Return grammar
        End Function

        Private Class PatternPart
            Public Sub New()
            End Sub

            Public Sub New(ByVal key As String,
                           ByVal [optional] As Boolean)
                Me.Key = key
                Me.Optional = [optional]
            End Sub

            Public Property Key As String
            Public Property [Optional] As Boolean

        End Class

        <XmlElement("case")>
        Public Property [Case]() As CaseType
        <XmlArray("definitions")>
        <XmlArrayItem("item", IsNullable:=False)>
        Public Property Definitions() As New ObservableCollection(Of Definition)
        <XmlArray("patterns")>
        <XmlArrayItem("item", IsNullable:=False)>
        Public Property Patterns() As New ObservableCollection(Of Pattern)

        Public Overrides Function Analyze() As String
            Const tableRow As String = "<tr>{0}</tr>"
            Const tableCell As String = "<td>{0}</td>"

            Dim html As String = My.Resources.PhonotacticsGrammarAnalysis
            Dim parameters As New StringBuilder

            html = html.Replace("[Name]", Me.Name)
            html = html.Replace("[Description]", Me.Description)
            html = html.Replace("[Author]", Me.Author)
            html = html.Replace("[Category]", Me.Category)
            html = html.Replace("[ParameterCount]", Me.Parameters.Count.ToString)
            html = html.Replace("[DefinitionCount]", Me.Definitions.Count.ToString("#,##0"))
            html = html.Replace("[PatternCount]", Me.Patterns.Count.ToString("#,##0"))
            html = html.Replace("[XMLLength]", Split(Me.ToString, Environment.NewLine).Count.ToString("#,##00"))

            For Each parameter As Parameter In Me.Parameters
                Dim row As New StringBuilder
                row.AppendFormatLine(tableCell, parameter.Name)
                parameters.AppendFormatLine(tableRow, row.ToString)
            Next

            html = html.Replace("[Parameters]", parameters.ToString)

            Return html
        End Function

        Protected Overrides Function GenerateName() As String
            Dim pattern As String = SelectPattern()
            Dim patternParts As List(Of PatternPart) = ParsePattern(pattern)
            Dim name As String = String.Empty

            For Each part As PatternPart In patternParts
                If Not part.Optional OrElse (part.Optional AndAlso Random.Next(100) > 50) Then
                    name &= GetValue(part.Key)
                End If
            Next

            Select Case [Case]
                Case CaseType.Lower : name = name.ToLower
                Case CaseType.Upper : name = name.ToUpper
                Case CaseType.Proper : name = StrConv(name, VbStrConv.ProperCase)
            End Select

            Return name
        End Function

        Private Function SelectPattern() As String
            Dim totalWeight As Int32 = Patterns.Sum(Function(p As Pattern) Math.Max(p.Weight, 1))
            Dim selected As Int32 = (BaseGrammar.Random.Next() Mod totalWeight) + 1
            Dim currentWeight As Int32 = 0
            For Each pattern As Pattern In Patterns
                currentWeight += pattern.Weight
                If currentWeight >= selected Then Return pattern.Value
            Next
            Return String.Empty
        End Function

        Private Function ParsePattern(ByVal pattern As String) As List(Of PatternPart)
            Const OPTIONAL_START As Char = "("c
            Const OPTIONAL_END As Char = ")"c

            Dim [optional] As Boolean = False
            Dim current As Char = Nothing
            Dim value As New List(Of PatternPart)

            For Each c As Char In pattern
                Select Case c
                    Case OPTIONAL_START
                        [optional] = True
                    Case OPTIONAL_END
                    Case Else
                        value.Add(New PatternPart(c, [optional]))
                        [optional] = False
                End Select
            Next

            Return value
        End Function

        Private Function GetValue(ByVal key As String) As String
            Dim definition As Definition = Definitions.FirstOrDefault(Function(d As Definition) d.Key = key)
            If definition IsNot Nothing Then
                Dim values As List(Of String) = definition.ValueList
                Dim index As Int32 = BaseGrammar.Random.Next Mod values.Count
                Return values(index)
            Else
                Return key
            End If
        End Function

    End Class

    Partial Public Class Definition

        Public Sub New()
        End Sub

        Public Sub New(ByVal key As String,
                       ByVal delimiter As Char,
                       ByVal value As String)
            Me.Key = key
            Me.Delimiter = delimiter
            Me.Value = value
        End Sub

        Private _valueList As List(Of String)

        <XmlAttribute("key")>
        Public Property Key() As String
        <XmlAttribute("delimiter")>
        Public Property Delimiter() As String
        <XmlText()>
        Public Property Value() As String

        <XmlIgnore>
        <YamlIgnore>
        Friend ReadOnly Property ValueList() As List(Of String)
            Get
                If _valueList Is Nothing Then
                    _valueList = New List(Of String)
                    _valueList.AddRange(Split(Value, Delimiter))
                End If
                Return _valueList
            End Get
        End Property

    End Class

    Partial Public Class Pattern
        Public Sub New()
        End Sub

        Public Sub New(ByVal value As String)
            Me.Value = value
        End Sub

        <XmlText()>
        Public Property Value() As String
        <XmlAttribute("weight")>
        Public Property Weight() As Int32 = 1
    End Class
End Namespace