Imports Utility

Friend Class Calculator

#Region "Helper Classes"
    Public Class DieRollResult
        Public Sub New()
        End Sub

        Public Property IndividualRolls As New List(Of String)
        Public Property Value As Int32

        Public Shared Operator +(ByVal l As DieRollResult, ByVal r As DieRollResult) As DieRollResult
            Dim result As DieRollResult = l
            result.IndividualRolls.AddRange(r.IndividualRolls)
            result.Value += r.Value
            Return result
        End Operator
    End Class
#End Region

#Region "Events"
    Public Event EvaluateFunction As NCalc.EvaluateFunctionHandler
    Public Event EvaluateParameter As NCalc.EvaluateParameterHandler
#End Region

#Region "Constants"
    ' Function Names
    Private Const GENERATE_FUNCTION As String = "Generate"
    Private Const ROLL_FUNCTION As String = "Roll"
    Private Const TARGET_ROLL_FUNCTION As String = "TargetRoll"
    Private Const RANDOM_FUNCTION As String = "Rnd"
    Private Const TO_TEXT_FUNCTION As String = "ToText"
    Private Const TO_ORDINAL_FUNCTION As String = "ToOrdinal"
    Private Const UCASE_FUNCTION As String = "UCase"
    Private Const LCASE_FUNCTION As String = "LCase"
    Private Const TCASE_FUNCTION As String = "TCase"
    Private Const FORMAT_FUNCTION As String = "Format"
    Private Const ROLL_RESULTS_FUNCTION As String = "RollResults"
    Private Const PICK_FUNCTION As String = "Pick"
    Private Const ROLLED_ONES_FUNCTION As String = "RolledOnes"

    ' Special Dice
    Private Const DIE_FATE As String = "F"
    Private Const DIE_PERCENTILE As String = "%"

    ' Exception Messages
    Private Const PARAMETER_COUNT_EXCEPTION_MESSAGE As String = "Invalid number of parameters for the {0} function received {1} and was expecting {2}."

    ' Dice Roll Constants
    Private Const DROP_LOWEST As String = "DL"
    Private Const DROP_HIGHEST As String = "DH"
    Private Const EXPLODE_DIE As String = "EX"
    Private Const COMPOUND_EXPLODE As String = "CX"
    Private Const REROLL_BELOW As String = "RB"
    Private Const TARGET_GREATER As String = "GT"
    Private Const TARGET_LESS As String = "LT"
    Private Const TARGET_RULE_OF_ONE As String = "R1"
    Private Const ROLL_SUCCESS As String = "<span style='color:Green'>{0}</span>"
    Private Const ROLL_ONE As String = "<span style='color:Red;'>{0}</span>"
#End Region

#Region "Constructor"
    Public Sub New()
    End Sub

    Public Sub New(ByVal grammar As BaseGrammar)
        _grammar = grammar
    End Sub
#End Region

#Region "Members"
    Private Shared _lastRollResult As DieRollResult
    Private WithEvents _calc As NCalc.Expression
    Private _grammar As BaseGrammar
#End Region

#Region "Public Shared Properties"
    Public Shared ReadOnly Property LastRollResult As DieRollResult
        Get
            Return _lastRollResult
        End Get
    End Property
#End Region

#Region "Public Shared Methods"
    Public Shared Function DiceRoll(ByVal params() As Object) As Int32
        Dim count As Int32 = CInt(params(0))
        Dim min As Int32 = 1
        Dim die As String = CStr(params(1))
        Dim dropHigh As Int32 = 0
        Dim dropLow As Int32 = 0
        Dim explode As Boolean = False
        Dim compoundExplode As Boolean = False
        Dim rerollBelow As Int32 = 0
        Dim results As New List(Of Int32)

        If die.Equals(DIE_FATE, StringComparison.CurrentCultureIgnoreCase) Then
            die = "1"
            min = -1
        ElseIf die.Equals(DIE_PERCENTILE) Then
            die = "100"
        End If

        _lastRollResult = New DieRollResult

        For i As Int32 = 2 To params.Count - 1
            Select Case params(i).ToString
                Case DROP_HIGHEST
                    If i < params.GetUpperBound(0) AndAlso TypeOf params(i + 1) Is Int32 Then
                        dropHigh = CInt(params(i + 1))
                    Else
                        dropHigh = 1
                    End If
                Case DROP_LOWEST
                    If i < params.GetUpperBound(0) AndAlso TypeOf params(i + 1) Is Int32 Then
                        dropLow = CInt(params(i + 1))
                    Else
                        dropLow = 1
                    End If
                Case REROLL_BELOW
                    If i < params.GetUpperBound(0) AndAlso TypeOf params(i + 1) Is Int32 Then
                        rerollBelow = CInt(params(i + 1))
                    Else
                        rerollBelow = 1
                    End If
                Case EXPLODE_DIE : explode = True
                Case COMPOUND_EXPLODE : compoundExplode = True
            End Select
        Next

        If dropHigh + dropLow > count Then
            Throw New NCalc.EvaluationException("Dice count to drop " & dropHigh + dropLow & " is greater than the number of dice rolled " & count)
        End If

        For i As Int32 = 1 To count
            Dim roll As Int32 = RollDie(min, die.Val, explode, compoundExplode)
            Do While roll < rerollBelow
                roll = RollDie(min, die.Val, explode, compoundExplode)
            Loop
            _lastRollResult.IndividualRolls.Add(roll.ToString)
            results.Add(roll)
        Next

        For i As Int32 = 1 To dropHigh
            Dim index As Int32 = results.IndexOf(results.Max)
            results.RemoveAt(index)
        Next

        For i As Int32 = 1 To dropLow
            Dim index As Int32 = results.IndexOf(results.Min)
            results.RemoveAt(index)
        Next

        _lastRollResult.Value = results.Sum

        Return _lastRollResult.Value
    End Function

    Public Shared Function TargetRoll(ByVal params() As Object) As Int32
        Dim count As Int32 = CInt(params(0))
        Dim die As Int32 = CInt(params(1))
        Dim target As Int32 = CInt(params(2))
        Dim ruleOfOne As Boolean = False
        Dim explode As Boolean = False
        Dim compoundExplode As Boolean = False
        Dim greaterThan As Boolean = True

        _lastRollResult = New DieRollResult

        For i As Int32 = 3 To params.Count - 1
            Select Case params(i).ToString
                Case TARGET_RULE_OF_ONE : ruleOfOne = True
                Case EXPLODE_DIE : explode = True
                Case COMPOUND_EXPLODE : compoundExplode = True
                Case TARGET_GREATER : greaterThan = True
                Case TARGET_LESS : greaterThan = False
            End Select
        Next

        For i As Int32 = 1 To count
            Dim roll As DieRollResult
            If greaterThan Then
                roll = RollTargetOverDie(die,
                                         target,
                                         ruleOfOne,
                                         explode,
                                         compoundExplode)
            Else
                roll = RollTargetUnderDie(die,
                                          target,
                                          explode,
                                          compoundExplode)
            End If
            _lastRollResult += roll
        Next
        Return _lastRollResult.Value
    End Function
#End Region

#Region "Public Methods"
    Public Function Evaluate(ByVal expression As String) As Object
        _calc = New NCalc.Expression(expression, NCalc.EvaluateOptions.NoCache)
        Try
            Return _calc.Evaluate
        Catch ex As NCalc.EvaluationException
            ex.Data.Add("Expression", expression)
        End Try

        Return Nothing
    End Function

    Public Function Generate(ByVal grammarName As String,
                             ByVal maxLength As Int32) As String
        Return Generate(grammarName, maxLength, New Dictionary(Of String, String))
    End Function

    Public Function Generate(ByVal grammarName As String,
                             ByVal maxLength As Int32,
                             ByVal parameters As NLua.LuaTable) As String
        Return Generate(grammarName, maxLength, parameters.ToDictionary(Of String, String))
    End Function

    Public Function Generate(ByVal grammarName As String,
                             ByVal maxLength As Int32,
                             ByVal parameters As Dictionary(Of String, String)) As String
        Dim name As String = If(IO.Path.HasExtension(grammarName), grammarName, grammarName & ".rnd.xml")
        Dim path As String = String.Empty
        Dim value As String
        If Not IO.Path.IsPathRooted(name) Then
            Dim i As Int32 = 0

            If Not String.IsNullOrWhiteSpace(_grammar.FilePath) Then
                path = IO.Path.Combine(IO.Path.GetDirectoryName(_grammar.FilePath), name)
            End If

            Do While Not IO.File.Exists(path) AndAlso i < Utility.GrammarFilePaths.Count
                path = IO.Path.Combine(Utility.GrammarFilePaths(i), name)
            Loop
        End If

        Dim grammar As BaseGrammar = BaseGrammar.Open(path)

        For Each item As KeyValuePair(Of String, String) In parameters
            grammar.Parameters.Add(New Parameter(item.Key, item.Value))
        Next

        value = grammar.GenerateNames(1, maxLength, Nothing)

        For Each variable As KeyValuePair(Of String, Object) In grammar.Variables
            _grammar.Variable(variable.Key) = variable.Value
        Next

        Return ExtractBody(value)
    End Function
#End Region

#Region "Private Shared Methods"
    Private Shared Function ExtractBody(ByVal html As String) As String
        Dim document As New Xml.XmlDocument
        Dim body As Xml.XmlElement
        document.LoadXml(html)

        body = CType(document.SelectSingleNode("/html/body/div"), Xml.XmlElement)

        Return body.InnerXml
    End Function

    Private Shared Function RollDie(ByVal low As Int32,
                                    ByVal high As Int32,
                                    ByVal explode As Boolean,
                                    ByVal compoundExplode As Boolean) As Int32
        Dim result As Int32 = BaseGrammar.Random.Next(low, high + 1)
        If result = high Then
            If explode Then
                result += RollDie(low, high, False, False)
            ElseIf compoundExplode Then
                result += RollDie(low, high, False, True)
            End If
        End If
        Return result
    End Function

    Private Shared Function RollTargetOverDie(ByVal die As Int32,
                                              ByVal target As Int32,
                                              ByVal ruleOfOne As Boolean,
                                              ByVal explode As Boolean,
                                              ByVal compoundExplode As Boolean) As DieRollResult
        Dim result As New DieRollResult
        Dim roll As Int32 = RollDie(1, die, False, False)
        If roll >= target Then
            result.IndividualRolls.Add(String.Format(ROLL_SUCCESS, roll))
            result.Value += 1
            If roll = die AndAlso explode Then
                result += RollTargetOverDie(die, target, False, False, False)
            ElseIf roll = die AndAlso compoundExplode Then
                result += RollTargetOverDie(die, target, False, False, True)
            End If
        ElseIf roll = 1 AndAlso ruleOfOne Then
            result.IndividualRolls.Add(String.Format(ROLL_ONE, roll))
            result.Value -= 1
        Else
            result.IndividualRolls.Add(roll.ToString)
        End If
        Return result
    End Function

    Private Shared Function RollTargetUnderDie(ByVal die As Int32,
                                               ByVal target As Int32,
                                               ByVal explode As Boolean,
                                               ByVal compoundExplode As Boolean) As DieRollResult
        Dim result As New DieRollResult
        Dim roll As Int32 = RollDie(1, die, False, False)
        If roll <= target Then
            result.IndividualRolls.Add(String.Format(ROLL_SUCCESS, roll))
            result.Value += 1
            If roll = die AndAlso explode Then
                result += RollTargetUnderDie(die, target, False, False)
            ElseIf roll = die AndAlso compoundExplode Then
                result += RollTargetUnderDie(die, target, False, True)
            End If
        Else
            result.IndividualRolls.Add(roll.ToString)
        End If
        Return result
    End Function

    Private Shared Function GetRollResults(ByVal params() As Object) As String
        Dim delimiter As String = " "
        If params.Count > 0 Then delimiter = params(0).ToString
        Return Join(Calculator.LastRollResult.IndividualRolls.ToArray, delimiter)
    End Function

    Private Shared Function CountOnes() As Int32
        If Calculator.LastRollResult IsNot Nothing Then
            Return Calculator.LastRollResult.IndividualRolls.Sum(Function(r As String) As Int32
                                                                     If r.Val = 1 Then Return 1
                                                                     Return 0
                                                                 End Function)
        Else
            Throw New NCalc.EvaluationException("RolledOnes can only be called after a Roll or TargetRoll call")
        End If
    End Function
#End Region

#Region "Event Handlers"
    Private Sub _calc_EvaluateFunction(ByVal name As String,
                                       ByVal args As NCalc.FunctionArgs) Handles _calc.EvaluateFunction
        Select Case name
            Case ROLL_FUNCTION
                If args.Parameters.Count >= 2 Then
                    args.Result = DiceRoll(args.EvaluateParameters)
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, ROLL_FUNCTION, args.Parameters.Count, "2 or more"))
                End If
            Case TARGET_ROLL_FUNCTION
                If args.Parameters.Count >= 3 Then
                    args.Result = TargetRoll(args.EvaluateParameters)
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, TARGET_ROLL_FUNCTION, args.Parameters.Count, "3 or more"))
                End If
            Case ROLL_RESULTS_FUNCTION
                If args.Parameters.Count <= 1 Then
                    args.Result = GetRollResults(args.EvaluateParameters)
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, ROLL_RESULTS_FUNCTION, args.Parameters.Count, "1 or less"))
                End If
            Case ROLLED_ONES_FUNCTION
                If args.Parameters.Count = 0 Then
                    args.Result = CountOnes()
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, ROLL_RESULTS_FUNCTION, args.Parameters.Count, "0"))
                End If
            Case RANDOM_FUNCTION
                If args.Parameters.Count = 1 Then
                    args.Result = BaseGrammar.Random.Next(CInt(args.Parameters(0).Evaluate)) + 1
                ElseIf args.Parameters.Count = 2 Then
                    args.Result = BaseGrammar.Random.Next(CInt(args.Parameters(0).Evaluate), CInt(args.Parameters(1).Evaluate) + 1)
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, RANDOM_FUNCTION, args.Parameters.Count, "1 or 2"))
                End If
            Case GENERATE_FUNCTION
                If _grammar IsNot Nothing Then
                    If args.Parameters.Count = 1 Then
                        args.Result = Generate(CStr(args.Parameters(0).Evaluate), Int32.MaxValue)
                    ElseIf args.Parameters.Count = 2 Then
                        args.Result = Generate(CStr(args.Parameters(0).Evaluate), CInt(args.Parameters(1).Evaluate))
                    ElseIf args.Parameters.Count >= 3 Then
                        Dim parameters As New Dictionary(Of String, String)
                        For i As Int32 = 2 To args.Parameters.Count - 1 Step 2
                            parameters.Add(CStr(args.Parameters(i).Evaluate), CStr(args.Parameters(i + 1).Evaluate))
                        Next
                        args.Result = Generate(CStr(args.Parameters(0).Evaluate), CInt(args.Parameters(1).Evaluate), parameters)
                    Else
                        Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, GENERATE_FUNCTION, args.Parameters.Count, 2))
                    End If
                End If
            Case TO_TEXT_FUNCTION
                If args.Parameters.Count = 1 Then
                    args.Result = CInt(args.Parameters(0).Evaluate).ToText
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, TO_TEXT_FUNCTION, args.Parameters.Count, 1))
                End If
            Case TO_ORDINAL_FUNCTION
                If args.Parameters.Count = 1 Then
                    args.Result = CInt(args.Parameters(0).Evaluate).ToOrdinal
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, TO_ORDINAL_FUNCTION, args.Parameters.Count, 1))
                End If
            Case UCASE_FUNCTION
                If args.Parameters.Count = 1 Then
                    args.Result = Globalization.CultureInfo.CurrentCulture.TextInfo.ToUpper(CStr(args.Parameters(0).Evaluate))
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, UCASE_FUNCTION, args.Parameters.Count, 1))
                End If
            Case LCASE_FUNCTION
                If args.Parameters.Count = 1 Then
                    args.Result = Globalization.CultureInfo.CurrentCulture.TextInfo.ToLower(CStr(args.Parameters(0).Evaluate))
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, LCASE_FUNCTION, args.Parameters.Count, 1))
                End If
            Case TCASE_FUNCTION
                If args.Parameters.Count = 1 Then
                    args.Result = Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CStr(args.Parameters(0).Evaluate))
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, TCASE_FUNCTION, args.Parameters.Count, 1))
                End If
            Case FORMAT_FUNCTION
                If args.Parameters.Count = 2 Then
                    args.Result = Format(args.Parameters(0).Evaluate, CStr(args.Parameters(1).Evaluate))
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, FORMAT_FUNCTION, args.Parameters.Count, 2))
                End If
            Case PICK_FUNCTION
                If args.Parameters.Count >= 1 Then
                    args.Result = args.Parameters(BaseGrammar.Random.Next(0, args.Parameters.Count)).Evaluate
                Else
                    Throw New NCalc.EvaluationException(String.Format(PARAMETER_COUNT_EXCEPTION_MESSAGE, FORMAT_FUNCTION, args.Parameters.Count, 1))
                End If
            Case Else
                RaiseEvent EvaluateFunction(name, args)
        End Select
    End Sub

    Private Sub _calc_EvaluateParameter(ByVal name As String,
                                        ByVal args As NCalc.ParameterArgs) Handles _calc.EvaluateParameter
        If _grammar IsNot Nothing AndAlso _grammar.ParameterExists(name) Then
            args.Result = _grammar.Parameter(name)
        ElseIf _grammar IsNot Nothing AndAlso _grammar.Variables.ContainsKey(name) Then
            args.Result = _grammar.Variable(name)
        Else
            Select Case name
                Case Else : RaiseEvent EvaluateParameter(name, args)
            End Select
        End If
    End Sub
#End Region

End Class
