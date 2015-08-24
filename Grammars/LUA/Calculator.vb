Imports NLua
Imports Utility
Imports Microsoft.VisualBasic.FileIO
Imports System.Xml

Namespace LUA
    Friend Class Calculator

        Private _calculator As Grammars.Calculator
        Private _grammar As BaseGrammar
        Private _lua As NLua.Lua

        Public Sub New(ByVal grammar As LUAGrammar,
                       ByVal lua As NLua.Lua)
            _grammar = grammar
            _lua = lua
            _calculator = New Grammars.Calculator(_grammar)
        End Sub

        Public Function ExpandPath(ByVal path As String) As String
            Dim value As String = Environment.ExpandEnvironmentVariables(path)
            If Not IO.Path.IsPathRooted(value) Then
                value = IO.Path.Combine(IO.Path.GetDirectoryName(_grammar.FilePath), value)
            End If
            Return value
        End Function

        Public Function Roll(ByVal ParamArray params() As Object) As Int32
            Return Grammars.Calculator.DiceRoll(params)
        End Function

        Public Function Rnd(ByVal min As Int32, ByVal max As Int32) As Int32
            Return BaseGrammar.Random.Next(min, max + 1)
        End Function

        Public Function TargetRoll(ByVal ParamArray params() As Object) As Int32
            Return Grammars.Calculator.TargetRoll(params)
        End Function

        Public Function Generate(ByVal grammarName As String,
                                 ByVal maxLength As Int32) As String
            Return _calculator.Generate(grammarName, maxLength)
        End Function

        Public Function Generate(ByVal grammarName As String,
                                 ByVal maxLength As Int32,
                                 ByVal parameters As LuaTable) As String
            Return _calculator.Generate(grammarName, maxLength, parameters)
        End Function

        Public Function Evaluate(ByVal expression As String) As Object
            Return _calculator.Evaluate(expression)
        End Function

        Public Function SelectFromTable(ByVal items As LuaTable) As Object
            Dim maxRoll As Int32 = 0
            Dim index As Int32 = 0

            For Each item As Int32 In items.Keys
                If maxRoll < item Then maxRoll = item
            Next

            index = BaseGrammar.Random.Next(1, maxRoll + 1)

            For Each item As Int32 In items.Keys
                If index <= item Then Return items.Item(item)
            Next

            Return Nothing
        End Function

        Public ReadOnly Property LastRollResult As Grammars.Calculator.DieRollResult
            Get
                Return Grammars.Calculator.LastRollResult
            End Get
        End Property

        Public Function FileToString(ByVal filePath As String) As String
            Dim path As String = filePath
            If Not IO.Path.IsPathRooted(path) Then path = IO.Path.Combine(IO.Path.GetDirectoryName(_grammar.FilePath), filePath)
            Return IO.File.ReadAllText(filePath)
        End Function

        Public Function FileToTable(ByVal filePath As String) As String()
            Return IO.File.ReadAllLines(filePath)
        End Function

        Public Function XMLFileToTable(ByVal filePath As String) As LuaTable
            Dim value As LuaTable
            Dim document As New XmlDocument
            document.Load(filePath)

            value = ProcessXMLNode(document.DocumentElement)

            Return value
        End Function

        Private Function ProcessXMLNode(ByVal parent As XmlNode) As LuaTable
            Dim value As LuaTable = CreateTable()
            Dim indexed As Boolean = False
            Dim index As Int32 = 1

            If parent.Attributes("indexed") IsNot Nothing Then
                indexed = CBool(parent.Attributes("indexed").Value)
            End If

            For Each child As XmlNode In parent.ChildNodes
                If child.HasChildNodes AndAlso child.FirstChild.NodeType = XmlNodeType.Element Then
                    If indexed Then
                        value(index) = ProcessXMLNode(child)
                        index += 1
                    Else
                        value(child.Name) = ProcessXMLNode(child)
                    End If
                ElseIf child.NodeType <> XmlNodeType.Comment Then
                    If indexed Then
                        value(index) = child.InnerText
                        index += 1
                    Else
                        value(child.Name) = child.InnerText
                    End If
                End If
            Next
            Return value
        End Function

        Private Function CreateTable() As LuaTable
            Return CType(_lua.DoString("return {}")(0), LuaTable)
        End Function

        Public Function CSVToTable(ByVal csvPath As String) As LuaTable
            Dim parser As New TextFieldParser(csvPath)
            Dim header() As String
            Dim value As LuaTable = CreateTable()
            Dim index As Int32 = 1
            parser.TextFieldType = FieldType.Delimited
            parser.HasFieldsEnclosedInQuotes = True
            parser.SetDelimiters(",")

            header = parser.ReadFields

            Do While Not parser.EndOfData
                Dim line() As String = parser.ReadFields
                Dim record As LuaTable = CreateTable()
                For i As Int32 = 0 To line.Length - 1
                    record(header(i)) = line(i)
                Next
                value(index) = record
                index += 1
            Loop

            Return value
        End Function
    End Class
End Namespace