Imports System.Reflection
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports NCalc
Imports Utility
Imports Grammars.Table.TableGrammarTable
Imports System.Collections.ObjectModel

Namespace Table
    <XmlRoot([Namespace]:="", IsNullable:=False)>
    Public Class TableGrammar
        Inherits BaseGrammar

#Region "Constants"
        Const CALCULATION_REGEX As String = "\[=.*?\]"
        Const VARIABLE_OPEN As Char = "["c
        Const VARIABLE_CLOSE As Char = "]"c
        Const CALCULATION_OPEN As String = "[="
#End Region

#Region "Members"
        Private _valueList As New Dictionary(Of String, Object)
        Private WithEvents _calculator As New Calculator(Me)
#End Region

#Region "Public Properties"
        <XmlArray("tables")>
        <XmlArrayItem("table", IsNullable:=False)>
        Public Property Tables() As New ObservableCollection(Of TableGrammarTable)

        <XmlElement("output")>
        Public Property Output() As String
#End Region

#Region "Public Methods"
        Public Overrides Function Analyze() As String
            Return String.Empty
        End Function
#End Region

#Region "Protected Methods"
        Protected Overrides Function GenerateName() As String
            _valueList = New Dictionary(Of String, Object)
            For Each table As TableGrammarTable In Tables
                AddHandler table.EvaluateParameter, AddressOf Tables_EvaluateParameter
                AddHandler table.EvaluateFunction, AddressOf Tables_EvaluteFunction
                Dim newResults As Dictionary(Of String, Object) = table.ProcessTable()
                For Each result As KeyValuePair(Of String, Object) In newResults
                    _valueList.Add(table.Name & "." & result.Key, result.Value)
                Next
                RemoveHandler table.EvaluateParameter, AddressOf Tables_EvaluateParameter
                RemoveHandler table.EvaluateFunction, AddressOf Tables_EvaluteFunction
            Next
            Return FillOutput()
        End Function
#End Region

#Region "Private Methods"
        Private Function FillOutput() As String
            Dim result As String = Output
            Dim regex As New Regex(CALCULATION_REGEX, RegexOptions.Multiline)
            Dim index As Int32

            For Each pair As KeyValuePair(Of String, Object) In _valueList
                result = result.Replace(VARIABLE_OPEN & pair.Key & VARIABLE_CLOSE, pair.Value.ToString)
            Next

            index = result.IndexOf(CALCULATION_OPEN)

            Do While index >= 0
                Dim length As Int32 = 2
                Dim expression As String = String.Empty
                If index >= 0 Then
                    Dim parenthesis As Int32 = 1
                    Do While parenthesis > 0
                        Select Case result.Substring(index + length, 1)
                            Case VARIABLE_OPEN
                                parenthesis += 1
                            Case VARIABLE_CLOSE
                                parenthesis -= 1
                        End Select
                        If Not parenthesis = 0 Then length += 1
                    Loop
                    expression = result.Substring(index + 2, length - 2)
                End If
                result = result.Stuff(index, length + 1, CStr(_calculator.Evaluate(expression)))
                index = result.IndexOf(CALCULATION_OPEN)
            Loop

            Return result
        End Function
#End Region

#Region "Event Handlers"
        Private Sub Tables_EvaluateParameter(ByVal sender As Object, ByVal args As EvaluateTableParameterArgs)
            Dim key As String = args.Name

            If Not key.Contains(".") Then
                Dim tableName As String = DirectCast(sender, TableGrammarTable).Name
                key = tableName & "." & args.Name
            End If

            If _valueList.ContainsKey(key) Then
                args.Args.Result = _valueList(key)
            ElseIf ParameterExists(args.Name) Then
                args.Args.Result = Parameter(args.Name)
            End If
        End Sub

        Private Sub Tables_EvaluteFunction(sender As Object, e As EvaluateTableFunctionArgs)

        End Sub

        Private Sub _calculator_EvaluateParameter(name As String, args As ParameterArgs) Handles _calculator.EvaluateParameter
            If _valueList.ContainsKey(name) Then
                args.Result = _valueList(name)
            End If
        End Sub
#End Region

    End Class
End Namespace