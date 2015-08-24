Imports System.Runtime.CompilerServices
Imports System.ComponentModel
Imports System.Collections.ObjectModel

Friend Module Extensions

    <Extension>
    Public Sub AddRange(Of T)(ByRef extended As BindingList(Of T), ByVal items As IEnumerable(Of T))
        For Each item As T In items
            extended.Add(item)
        Next
    End Sub

    <Extension>
    Public Sub AddRange(Of T)(ByRef extended As ObservableCollection(Of T), ByVal items As IEnumerable(Of T))
        For Each item As T In items
            extended.Add(item)
        Next
    End Sub

    <Extension>
    Public Function ToDictionary(Of K, V)(ByVal extended As NLua.LuaTable) As Dictionary(Of K, V)
        Dim result As New Dictionary(Of K, V)

        For i As Int32 = 0 To extended.Keys.Count - 1
            result.Add(CType(extended.Keys(i), K), CType(extended.Values(i), V))
        Next

        Return result
    End Function

    <Extension>
    Public Function ToList(Of T)(ByVal extended As NLua.LuaTable) As List(Of T)
        Dim result As New List(Of T)

        For i As Int32 = 0 To extended.Values.Count - 1
            result.Add(CType(extended.Values(i), T))
        Next

        Return result
    End Function

End Module
