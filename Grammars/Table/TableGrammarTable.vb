Imports System.Xml.Serialization
Imports System.Reflection
Imports NCalc
Imports Utility

Namespace Table

    Public Enum Actions
        Random
        [Loop]
        [Select]
    End Enum

    Public Class TableGrammarTable

#Region "Event Arguments"
        Public Class EvaluateTableParameterArgs
            Inherits System.EventArgs

            Public Sub New(ByVal name As String,
                           ByVal args As ParameterArgs)
                Me._name = name
                Me._args = args
            End Sub

            Private _name As String
            Private _args As ParameterArgs

            Public ReadOnly Property Name As String
                Get
                    Return _name
                End Get
            End Property
            Public ReadOnly Property Args As ParameterArgs
                Get
                    Return _args
                End Get
            End Property
        End Class

        Public Class EvaluateTableFunctionArgs
            Inherits System.EventArgs

            Public Sub New(ByVal name As String,
                           ByVal args As FunctionArgs)
                Me._name = name
                Me._args = args
            End Sub

            Private _name As String
            Private _args As FunctionArgs

            Public ReadOnly Property Name As String
                Get
                    Return _name
                End Get
            End Property
            Public ReadOnly Property Args As FunctionArgs
                Get
                    Return _args
                End Get
            End Property
        End Class
#End Region

#Region "Events"
        Public Event EvaluateParameter As EventHandler(Of EvaluateTableParameterArgs)
        Public Event EvaluateFunction As EventHandler(Of EvaluateTableFunctionArgs)
#End Region

        Private Const COMMENT_TOKEN As String = "#"

        Private _table As DataTable
        Private WithEvents _ncalc As New Calculator()

        <XmlAttribute("action")>
        Public Property Action() As Actions = Actions.Random
        <XmlAttribute("column")>
        Public Property Column() As String
        <XmlAttribute("randomModifier")>
        Public Property RandomModifier() As String
        <XmlAttribute("loopId")>
        Public Property LoopId() As String
        <XmlAttribute("repeat")>
        Public Property Repeat As String
        <XmlAttribute("repeatJoin")>
        Public Property RepeatJoin As String
        <XmlAttribute("selectValue")>
        Public Property SelectValue As String
        <XmlAttribute("name")>
        Public Property Name() As String
        <XmlAttribute("delimiter")>
        Public Property Delimiter() As String
        <XmlAttribute("skipTable")>
        Public Property SkipTable() As String
        <XmlText()>
        Public Property Value() As String
        <XmlIgnore()>
        Public ReadOnly Property Table As DataTable
            Get
                Return _table
            End Get
        End Property

        Public Function ProcessTable() As Dictionary(Of String, Object)
            If Not String.IsNullOrWhiteSpace(SkipTable) AndAlso CBool(_ncalc.Evaluate(SkipTable)) Then Return New Dictionary(Of String, Object)
            Select Case Action
                Case Actions.Random : Return ProcessRandom()
                Case Actions.Loop : Return ProcessLoop()
                Case Actions.Select : Return ProcessSelect()
            End Select
            Return New Dictionary(Of String, Object)
        End Function

        Private Function ProcessSelect() As Dictionary(Of String, Object)
            Dim value As String = SelectValue
            Dim row As DataRow
            Dim result As New Dictionary(Of String, Object)
            If value.StartsWith("=") Then
                value = _ncalc.Evaluate(value.Substring(1)).ToString
            End If

            ParseTable()

            row = _table.AsEnumerable.FirstOrDefault(Function(r As DataRow) r.Item(Column).Equals(value))

            ProcessRow(result, row)
            Return result
        End Function

        Private Function ProcessLoop() As Dictionary(Of String, Object)
            Dim result As New Dictionary(Of String, Object)
            Dim count As Int32 = GetRepeat()

            ParseTable()

            For i As Int32 = 1 To count
                For Each row As DataRow In _table.Rows
                    Dim id As String = row(LoopId).ToString
                    For Each column As DataColumn In Table.Columns
                        Dim key As String = id & "." & column.ColumnName
                        Dim expression As String = row(column.ColumnName).ToString
                        Dim value As Object
                        If expression.StartsWith("=") Then
                            value = _ncalc.Evaluate(expression.Substring(1))
                        Else
                            value = expression
                        End If
                        If result.ContainsKey(key) Then
                            result(key) = result(key).ToString & RepeatJoin & value.ToString
                        Else
                            result.Add(key, value)
                        End If
                    Next
                Next
            Next

            Return result
        End Function

        Private Function ProcessRandom() As Dictionary(Of String, Object)
            If _table Is Nothing Then ParseTable()
            Dim random As New Random
            Dim max As Int32 = CInt(_table.AsEnumerable.Max(Function(r As DataRow) CInt(r(Column))))
            Dim value As Int32
            Dim result As New Dictionary(Of String, Object)
            Dim modifier As Int32 = 0
            Dim count As Int32 = GetRepeat()

            For i As Int32 = 1 To count
                Dim selectedRow As DataRow = Nothing
                Dim index As Int32 = 0

                If Not String.IsNullOrWhiteSpace(RandomModifier) Then
                    If RandomModifier.StartsWith("=") Then
                        modifier = CInt(_ncalc.Evaluate(RandomModifier.Substring(1)))
                    Else
                        modifier = CInt(_ncalc.Evaluate(RandomModifier))
                    End If
                End If

                value = BaseGrammar.Random.Next(Math.Abs(max + modifier)) * If(max + modifier < 0, -1, 1)
                Do While index < Table.Rows.Count AndAlso selectedRow Is Nothing
                    Dim row As DataRow = Table.Rows(index)
                    If value < CInt(row(Column)) Then selectedRow = row
                    index += 1
                Loop

                If selectedRow Is Nothing Then selectedRow = Table.Rows(Table.Rows.Count - 1)

                ProcessRow(result, selectedRow)
            Next
            Return result
        End Function

        Private Sub ProcessRow(ByVal result As Dictionary(Of String, Object),
                               ByVal row As DataRow)
            If row IsNot Nothing Then
                For Each column As DataColumn In row.Table.Columns
                    Dim thisValue As Object
                    If row(column).ToString.StartsWith("=") Then
                        thisValue = _ncalc.Evaluate(row(column).ToString.Substring(1))
                    Else
                        thisValue = row(column)
                    End If
                    If result.ContainsKey(column.ColumnName) Then
                        result(column.ColumnName) = result(column.ColumnName).ToString & RepeatJoin & thisValue.ToString
                    Else
                        result.Add(column.ColumnName, thisValue)
                    End If
                Next
            End If
        End Sub

        Private Function GetRepeat() As Int32
            If String.IsNullOrWhiteSpace(Repeat) Then Return 1
            If Repeat.StartsWith("=") Then
                Return CInt(_ncalc.Evaluate(Repeat.Substring(1)))
            Else
                Return CInt(Repeat)
            End If
        End Function

        Private Sub ParseTable()
            If _table Is Nothing Then
                Dim reader As New IO.StringReader(Value)
                Dim parser As New Microsoft.VisualBasic.FileIO.TextFieldParser(reader)
                parser.Delimiters = {Delimiter}
                parser.TextFieldType = FileIO.FieldType.Delimited
                parser.CommentTokens = {COMMENT_TOKEN}
                parser.HasFieldsEnclosedInQuotes = False
                parser.TrimWhiteSpace = True
                _table = New DataTable
                _table.TableName = Name

                ' Read headers
                Dim headers() As String = parser.ReadFields
                For Each header As String In headers
                    If Not String.IsNullOrWhiteSpace(header) Then _table.Columns.Add(header)
                Next

                ' Read data
                Do While Not parser.EndOfData
                    Dim fields() As String = parser.ReadFields
                    _table.Rows.Add(fields)
                Loop
            End If
        End Sub

        Private Sub _ncalc_EvaluateFunction(name As String, args As FunctionArgs) Handles _ncalc.EvaluateFunction
            RaiseEvent EvaluateFunction(Me, New EvaluateTableFunctionArgs(name, args))
        End Sub

        Private Sub _ncalc_EvaluateParameter(name As String, args As ParameterArgs) Handles _ncalc.EvaluateParameter
            RaiseEvent EvaluateParameter(Me, New EvaluateTableParameterArgs(name, args))
        End Sub
    End Class
End Namespace