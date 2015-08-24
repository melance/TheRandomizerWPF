Namespace Assignment
    Public Class MarkovChainGenerator

        Private START_LABEL As String = "Start"

        Private _grammar As AssignmentGrammar
        Private _samples As IEnumerable(Of String)
        Private _syllableLength As Int32
        Private _weightLimit As Int32 = 0

        Public Sub New(ByVal samples As IEnumerable(Of String),
                       ByVal syllableLength As Int32,
                       ByVal weightLimit As Int32)
            _samples = samples
            _syllableLength = syllableLength
            _weightLimit = weightLimit
        End Sub

        Public Function BuildGrammar() As AssignmentGrammar
            Dim list As New List(Of KeyValuePair(Of String, String))
            _grammar = New AssignmentGrammar

            For Each sample As String In _samples
                Dim i As Int32 = 0
                Dim diff As Int32 = sample.Length Mod _syllableLength
                Dim previous As String = START_LABEL
                Dim current As String

                If diff <> 0 Then
                    sample &= Space(diff)
                End If
                Do While i < sample.Length
                    current = sample.Substring(i, _syllableLength).Trim
                    list.Add(New KeyValuePair(Of String, String)(previous, current))
                    If i = sample.Length - _syllableLength Then
                        list.Add(New KeyValuePair(Of String, String)(current, String.Empty))
                    End If
                    previous = current.Trim
                    i += _syllableLength
                Loop
            Next

            For Each pair As KeyValuePair(Of String, String) In list
                Dim rule As LineItem = _grammar.Rules.FirstOrDefault(Function(r As LineItem)
                                                                         If r.Name = If(String.IsNullOrWhiteSpace(pair.Key), "Start", pair.Key) AndAlso
                                                                            If(String.IsNullOrEmpty(r.Next), r.Expression = String.Format("[{0}]", pair.Value), r.Next = pair.Value) Then Return True
                                                                         Return False
                                                                     End Function)
                If rule IsNot Nothing Then
                    If _weightLimit = 0 OrElse CInt(rule.Weight) < _weightLimit Then rule.Weight += 1
                Else
                    rule = New LineItem(pair.Key,
                                        If(pair.Key = START_LABEL, String.Empty, pair.Value),
                                        If(pair.Key = START_LABEL, String.Format("[{0}]", pair.Value), pair.Key))
                    rule.Weight = 1
                    _grammar.Rules.Add(rule)
                End If
            Next

            Return _grammar
        End Function

    End Class
End Namespace