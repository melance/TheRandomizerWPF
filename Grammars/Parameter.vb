Imports System.Xml.Serialization
Imports System.ComponentModel
Imports System.Windows.Controls
Imports Utility
Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports System.Windows.Data

Public Enum DataTypes
    List
    Text
    CheckBox
End Enum

Public Class Parameter
    Implements INotifyPropertyChanged

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Public Class OptionClass
        <XmlAttribute("display")>
        Public Property Display As String
        <XmlText>
        Public Property Value As String
    End Class

    Public Sub New()
    End Sub

    Public Sub New(ByVal name As String,
                   ByVal value As String)
        Me.Name = name
        Me.Value = value
    End Sub

    Public Enum ValueTypes
        Value
        Label
    End Enum

    <XmlAttribute("name")>
    Public Property Name As String
    <XmlAttribute("display")>
    Public Property Display As String
    <XmlText()>
    Public Property Description As String
    <XmlAttribute("type")>
    Public Property Type As DataTypes = DataTypes.List
    <XmlAttribute("valueType")>
    Public Property ValueType As ValueTypes = ValueTypes.Label
    <XmlElement("option")>
    Public Property Options As New ObservableCollection(Of [Option])
    <XmlAttribute("default")>
    Public Property [Default] As String

    <XmlIgnore>
    Public Property Value As String
    <XmlIgnore>
    Public ReadOnly Property Control As DockPanel
        Get
            Dim value As Control = Nothing
            Dim label As New Label
            Dim panel As New DockPanel
            Select Case Type
                Case DataTypes.List
                    Dim cboValue As New ComboBox
                    For Each [option] As [Option] In Options
                        If String.IsNullOrWhiteSpace([option].Display) Then [option].Display = [option].Value
                        Dim index As Int32 = cboValue.Items.Add([option])
                        If [option].Value = [Default] Then cboValue.SelectedIndex = index
                        If cboValue.SelectedIndex < 0 Then cboValue.SelectedIndex = 0
                    Next
                    cboValue.DisplayMemberPath = "Display"
                    value = cboValue
                Case DataTypes.Text
                    Dim txtvalue As New TextBox
                    txtvalue.Text = [Default]
                    value = txtvalue
                Case DataTypes.CheckBox
                    Dim chkValue As New CheckBox
                    chkValue.IsChecked = If([Default] Is Nothing, False, CBool([Default]))
                    label.Visibility = Windows.Visibility.Collapsed
                    chkValue.Content = Me.Display
                    value = chkValue
            End Select
            value.Tag = Name
            value.ToolTip = Description
            value.Margin = New Windows.Thickness(3, 3, 3, 3)
            label.Margin = New Windows.Thickness(3, -3, 3, 0)
            label.Content = Me.Display
            label.Height = value.Height
            label.VerticalContentAlignment = Windows.VerticalAlignment.Center
            label.ToolTip = Description
            'panel.Height =
            DockPanel.SetDock(label, Dock.Left)
            DockPanel.SetDock(value, Dock.Right)
            panel.Children.Add(label)
            panel.Children.Add(value)
            panel.LastChildFill = True
            panel.Margin = New Windows.Thickness(0, 3, 0, 3)
            DockPanel.SetDock(panel, Dock.Top)
            Return panel
        End Get
    End Property

    Protected Overridable Sub OnPropertyChanged(<CallerMemberNameAttribute> Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

End Class

Public Class [Option]
    <XmlText>
    Public Property Value As String
    <XmlAttribute("display")>
    Public Property Display As String
End Class