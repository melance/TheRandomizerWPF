Imports System.Xml.Serialization
Imports YamlDotNet.Serialization

Namespace Assignment
    Public Class LineItem
        Implements IComparable(Of LineItem)

        Private Const DELIMITER As Char = ":"c

        Public Sub New()
        End Sub

        Public Sub New(ByVal line As String)
            Dim parts() As String = line.Split(DELIMITER)
            Name = parts(0)
            Expression = parts(1)
            Weight = CInt(parts(2))
            If parts.GetUpperBound(0) = 3 Then [Next] = parts(3)
        End Sub
        Private Const START_ITEM As String = "Start"

        Public Sub New(ByVal fields() As String)
            Name = fields(0)
            [Next] = fields(1)
            Weight = CInt(fields(2))
            Expression = fields(3)
        End Sub

        Public Sub New(ByVal name As String,
                       ByVal [next] As String,
                       ByVal expression As String)
            Me.Name = name
            Me.Expression = expression
            Me.Next = [next]
        End Sub

        Public Sub New(ByVal name As String,
                       ByVal [next] As String,
                       ByVal expression As String,
                       ByVal weight As Int32)
            Me.Name = name
            Me.Expression = expression
            Me.Next = [next]
            Me.Weight = weight
        End Sub

        Private _from As Int32 = 0
        Private _to As Int32 = 0

        <XmlAttribute("name")>
        <YamlMember(Alias:="name")>
        Public Property Name As String
        <XmlText>
        <YamlMember(Alias:="value")>
        Public Property Expression As String
        <XmlAttribute("weight")>
        <YamlMember(Alias:="weight")>
        Public Property Weight As Int32 = 1
        <XmlAttribute("next")>
        <YamlMember(Alias:="next")>
        Public Property [Next] As String
        <XmlAttribute("variable")>
        <YamlMember(Alias:="variable")>
        Public Property Variable As String
        <XmlAttribute("from")>
        <YamlMember(Alias:="from")>
        Public Property FromValue As Int32
            Get
                Return _from
            End Get
            Set(value As Int32)
                If _from <> value Then
                    _from = value
                    If _to >= _from Then Weight = _to - _from + 1
                End If
            End Set
        End Property

        <XmlAttribute("to")>
        <YamlMember(Alias:="to")>
        Public Property ToValue As Int32
            Get
                Return _to
            End Get
            Set(value As Int32)
                If _to <> value Then
                    _to = value
                    If _to >= _from Then Weight = _to - _from + 1
                End If
            End Set
        End Property

        Public ReadOnly Property FromValueSpecified As Boolean
            Get
                Return FromValue <> 0
            End Get
        End Property

        Public ReadOnly Property ToValueSpecified As Boolean
            Get
                Return ToValue <> 0
            End Get
        End Property

        Public ReadOnly Property WeightSpecified As Boolean
            Get
                Return Weight <> 1 AndAlso FromValue = 0 AndAlso ToValue = 0
            End Get
        End Property

        Public ReadOnly Property NextSpecified As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace([Next])
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Name & DELIMITER & Expression & DELIMITER & Weight & DELIMITER & [Next]
        End Function

        Public Shared Operator =(ByVal l As LineItem, ByVal r As LineItem) As Boolean
            Return l.Name = r.Name AndAlso
                   l.Next = r.Next AndAlso
                   l.ToValue = r.ToValue AndAlso
                   l.FromValue = r.FromValue AndAlso
                   l.Weight = r.Weight
        End Operator

        Public Shared Operator <>(ByVal l As LineItem, ByVal r As LineItem) As Boolean
            Return Not l = r
        End Operator

        Public Function CompareTo(other As LineItem) As Integer Implements IComparable(Of LineItem).CompareTo
            If Me.Name = START_ITEM AndAlso other.Name = START_ITEM Then Return 0
            If Me.Name = START_ITEM Then Return -1
            If other.Name = START_ITEM Then Return 1
            Return Me.Name.CompareTo(other.Name)
        End Function
    End Class
End Namespace