Imports System.Xml.Serialization
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Namespace Dice
    <System.Xml.Serialization.XmlRoot("DiceRoller", IsNullable:=False)>
    Public Class DiceRoll
        Inherits BaseGrammar

        Private Const ASSIGN_OPERATOR As String = ":="
        Private Const ROLL_FUNCTION_PARAMETER As String = "RollFunction"
        Private Const COMMENT_CHARACTER As String = "#"

        Private WithEvents _calculator As New Calculator(Me)
        Private _results As New Dictionary(Of Int32, String)

        Public Class RollFunction
            Implements INotifyPropertyChanged
            Implements IDataErrorInfo

            Public Sub New()
            End Sub

            Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

            Private _name As String
            Private _value As String

            <XmlAttribute("name")>
            Public Property Name As String
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
            <XmlText>
            Public Property Value As String
                Get
                    Return _value
                End Get
                Set(value As String)
                    If _value <> value Then
                        _value = value
                        OnPropertyChanged()
                    End If
                End Set
            End Property

            Public ReadOnly Property [Error] As String Implements IDataErrorInfo.Error
                Get
                    Return String.Empty
                End Get
            End Property

            Default Public ReadOnly Property Item(columnName As String) As String Implements IDataErrorInfo.Item
                Get
                    Select Case columnName
                        Case "Name" : If String.IsNullOrWhiteSpace(Name) Then Return "Name is required."
                        Case "Value" : If String.IsNullOrWhiteSpace(Value) Then Return "Function is required."
                    End Select
                    Return String.Empty
                End Get
            End Property

            Protected Overridable Sub OnPropertyChanged(<CallerMemberNameAttribute> Optional ByVal propertyName As String = Nothing)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
            End Sub
        End Class

        <XmlElement("function")>
        Property RollFunctions As New ObservableCollection(Of RollFunction)

        <XmlElement("supportsMaxLength")>
        Public Overrides Property SupportsMaxLength As Boolean
            Get
                Return False
            End Get
            Set(value As Boolean)
            End Set
        End Property

        Public Overrides Function Analyze() As String
            Return String.Empty
        End Function

        Protected Overrides Function GenerateName() As String
            Dim lines As New List(Of String)(Split(GetRollFunction, Chr(&HA)))
            Dim lineNumber As Int32 = 1

            _results.Clear()

            For Each line As String In lines
                line = line.Trim()
                If Not String.IsNullOrWhiteSpace(line) AndAlso Not line.StartsWith(COMMENT_CHARACTER) Then
                    Dim parts() As String = Split(line, ASSIGN_OPERATOR)
                    If parts.Count = 1 Then
                        _results.Add(lineNumber, _calculator.Evaluate(line).ToString)
                    Else
                        If GetParameter(parts(0)) IsNot Nothing Then
                            GetParameter(parts(0)).Value = parts(1)
                        Else
                            If Not Variables.ContainsKey(parts(0)) Then Variables.Add(parts(0), 0)
                            Variables(parts(0)) = _calculator.Evaluate(parts(1))
                        End If
                    End If
                Else
                    _results.Add(lineNumber, String.Empty)
                End If
                lineNumber += 1
            Next

            Return Join(_results.Values.ToArray, Environment.NewLine)
        End Function

        Private Function GetRollFunction() As String
            If RollFunctions.Count = 1 Then Return RollFunctions(0).Value
            Dim rollFunctionParameter As Parameter = GetParameter(ROLL_FUNCTION_PARAMETER)
            If rollFunctionParameter IsNot Nothing Then
                Dim name As Object = rollFunctionParameter.Value
                If name Is Nothing Then
                    Return RollFunctions(0).Value
                Else
                    Return RollFunctions.First(Function(rf As RollFunction) rf.Name = CStr(name)).Value
                End If
            End If
            Return String.Empty
        End Function

        Private Function GetRollResults(ByVal params() As Object) As String
            Dim delimiter As String = " "
            If params.Count > 0 Then delimiter = params(0).ToString
            Return Join(Calculator.LastRollResult.IndividualRolls.ToArray, delimiter)
        End Function

        Private Sub _calculator_EvaluateParameter(name As String, args As NCalc.ParameterArgs) Handles _calculator.EvaluateParameter
            If Variables.ContainsKey(name) Then
                args.Result = Variables(name)
            End If
        End Sub
    End Class
End Namespace