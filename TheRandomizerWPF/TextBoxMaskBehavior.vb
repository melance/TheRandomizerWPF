Imports System.Globalization

Public Enum MaskType
    Any
    [Integer]
    [Decimal]
End Enum

Public Class TextBoxMaskBehavior

    Public Shared Function GetMinimumValue(ByVal obj As DependencyObject) As Double
        Return CDbl(obj.GetValue(MinimumValueProperty))
    End Function

    Public Shared Sub SetMinimumValue(ByVal obj As DependencyObject, ByVal value As Double)
        obj.SetValue(MinimumValueProperty, value)
    End Sub

    Public Shared Function GetMaximumValue(ByVal obj As DependencyObject) As Double
        Return CDbl(obj.GetValue(MaximumValueProperty))
    End Function

    Public Shared Sub SetMaximumValue(ByVal obj As DependencyObject, ByVal value As Double)
        obj.SetValue(MaximumValueProperty, value)
    End Sub

    Public Shared Function GetMask(obj As DependencyObject) As MaskType
        Return DirectCast(obj.GetValue(MaskProperty), MaskType)
    End Function

    Public Shared Sub SetMask(obj As DependencyObject, value As MaskType)
        obj.SetValue(MaskProperty, value)
    End Sub

    Public Shared Function GetSupportUpDown(obj As DependencyObject) As Boolean
        Return CBool(obj.GetValue(SupportUpDownProperty))
    End Function

    Public Shared Sub SetSupportUpDown(obj As DependencyObject, value As Boolean)
        obj.SetValue(SupportUpDownProperty, value)
    End Sub

    Public Shared Function GetUpDownLoop(obj As DependencyObject) As Boolean
        Return CBool(obj.GetValue(UpDownLoopProperty))
    End Function

    Public Shared Sub SetUpDownLoop(obj As DependencyObject, value As Boolean)
        obj.SetValue(UpDownLoopProperty, value)
    End Sub

    Public Shared ReadOnly MinimumValueProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("MinimumValue",
                                            GetType(Double),
                                            GetType(TextBoxMaskBehavior),
                                            New FrameworkPropertyMetadata(Double.NaN, AddressOf MinimumValueChangedCallback))

    Public Shared ReadOnly MaximumValueProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("MaximumValue",
                                            GetType(Double),
                                            GetType(TextBoxMaskBehavior),
                                            New FrameworkPropertyMetadata(Double.NaN, AddressOf MaximumValueChangedCallBack))

    Public Shared ReadOnly MaskProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("Mask",
                                            GetType(MaskType),
                                            GetType(TextBoxMaskBehavior),
                                            New FrameworkPropertyMetadata(AddressOf MaskChangedCallback))

    Public Shared ReadOnly SupportUpDownProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("SupportUpDown",
                                            GetType(Boolean),
                                            GetType(TextBoxMaskBehavior),
                                            New FrameworkPropertyMetadata(AddressOf SupportUpDownChangedCallback))

    Public Shared ReadOnly UpDownLoopProperty As DependencyProperty =
        DependencyProperty.RegisterAttached("UpDownLoop",
                                            GetType(Boolean),
                                            GetType(TextBoxMaskBehavior))

    Private Shared Sub MaskChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If TypeOf e.OldValue Is TextBox Then
            Dim txt As TextBox = DirectCast(e.OldValue, TextBox)
            RemoveHandler txt.PreviewTextInput, AddressOf TextBox_PreviewTextInput
            DataObject.RemovePastingHandler(txt, AddressOf TextBoxPastingEventHandler)
        End If

        Dim txtBox As TextBox = TryCast(d, TextBox)
        If txtBox IsNot Nothing Then
            If DirectCast(e.NewValue, MaskType) <> MaskType.Any Then
                AddHandler txtBox.PreviewTextInput, AddressOf TextBox_PreviewTextInput
                DataObject.AddPastingHandler(txtBox, AddressOf TextBoxPastingEventHandler)
            End If

            ValidateTextBox(txtBox)
        End If
    End Sub

    Private Shared Sub SupportUpDownChangedCallback(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        If TypeOf e.OldValue Is TextBox Then
            Dim txt As TextBox = DirectCast(e.OldValue, TextBox)
            RemoveHandler txt.PreviewKeyDown, AddressOf TextBox_PreviewKeyDown
        End If

        Dim txtBox As TextBox = TryCast(d, TextBox)
        If txtBox IsNot Nothing Then
            AddHandler txtBox.PreviewKeyDown, AddressOf TextBox_PreviewKeyDown
        End If
    End Sub

    Private Shared Sub MinimumValueChangedCallback(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        ValidateTextBox(DirectCast(d, TextBox))
    End Sub

    Private Shared Sub MaximumValueChangedCallBack(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        ValidateTextBox(DirectCast(d, TextBox))
    End Sub

    Private Shared Sub TextBoxPastingEventHandler(ByVal sender As Object, ByVal e As DataObjectPastingEventArgs)
        Dim txtBox As TextBox = DirectCast(sender, TextBox)
        Dim clipboard As String = CStr(e.DataObject.GetData(GetType(String)))
        clipboard = ValidateValue(GetMask(txtBox), clipboard, GetMinimumValue(txtBox), GetMaximumValue(txtBox))
        If Not String.IsNullOrEmpty(clipboard) Then txtBox.Text = clipboard
        e.CancelCommand()
        e.Handled = True
    End Sub

    Private Shared Sub TextBox_PreviewKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
        Dim txtBox As TextBox = DirectCast(sender, TextBox)
        If GetMask(txtBox) <> MaskType.Any AndAlso GetSupportUpDown(txtBox) Then
            If e.Key = Key.Up OrElse e.Key = Key.Down Then
                Dim value As String = ValidateValue(GetMask(txtBox), txtBox.Text, GetMinimumValue(txtBox), GetMaximumValue(txtBox))
                Dim dblValue As Double
                If String.IsNullOrEmpty(value) Then
                    dblValue = GetMinimumValue(txtBox)
                Else
                    dblValue = CInt(value)
                End If
                If e.Key = Key.Up Then
                    dblValue += 1
                    If dblValue > GetMaximumValue(txtBox) Then
                        If GetUpDownLoop(txtBox) Then
                            dblValue = GetMinimumValue(txtBox)
                        Else
                            dblValue = GetMaximumValue(txtBox)
                        End If
                    End If
                ElseIf e.Key = Key.Down Then
                    dblValue -= 1
                    If dblValue < GetMinimumValue(txtBox) Then
                        If GetUpDownLoop(txtBox) Then
                            dblValue = GetMaximumValue(txtBox)
                        Else
                            dblValue = GetMinimumValue(txtBox)
                        End If
                    End If
                End If
                txtBox.Text = CStr(dblValue)
            End If
        End If
    End Sub

    Private Shared Sub TextBox_PreviewTextInput(ByVal sender As Object, ByVal e As TextCompositionEventArgs)
        Dim _this As TextBox = TryCast(sender, TextBox)
        Dim isValid As Boolean = IsSymbolValid(GetMask(_this), e.Text)
        e.Handled = Not isValid
        If isValid Then
            Dim caret As Integer = _this.CaretIndex
            Dim text As String = _this.Text
            Dim textInserted As Boolean = False
            Dim selectionLength As Integer = 0

            If _this.SelectionLength > 0 Then
                text = text.Substring(0, _this.SelectionStart) + text.Substring(_this.SelectionStart + _this.SelectionLength)
                caret = _this.SelectionStart
            End If

            If e.Text = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator Then
                While True
                    Dim ind As Integer = text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    If ind = -1 Then
                        Exit While
                    End If

                    text = text.Substring(0, ind) + text.Substring(ind + 1)
                    If caret > ind Then
                        caret -= 1
                    End If
                End While

                If caret = 0 Then
                    text = Convert.ToString("0") & text
                    caret += 1
                Else
                    If caret = 1 AndAlso String.Empty + text(0) = NumberFormatInfo.CurrentInfo.NegativeSign Then
                        text = NumberFormatInfo.CurrentInfo.NegativeSign + "0" + text.Substring(1)
                        caret += 1
                    End If
                End If

                If caret = text.Length Then
                    selectionLength = 1
                    textInserted = True
                    text = text + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator & Convert.ToString("0")
                    caret += 1
                End If
            ElseIf e.Text = NumberFormatInfo.CurrentInfo.NegativeSign Then
                textInserted = True
                If _this.Text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign) Then
                    text = text.Replace(NumberFormatInfo.CurrentInfo.NegativeSign, String.Empty)
                    If caret <> 0 Then
                        caret -= 1
                    End If
                Else
                    text = NumberFormatInfo.CurrentInfo.NegativeSign + _this.Text
                    caret += 1
                End If
            End If

            If Not textInserted Then
                text = text.Substring(0, caret) + e.Text + (If((caret < _this.Text.Length), text.Substring(caret), String.Empty))

                caret += 1
            End If

            Try
                Dim val As Double = Convert.ToDouble(text)
                Dim newVal As Double = ValidateLimits(GetMinimumValue(_this), GetMaximumValue(_this), val)
                If val <> newVal Then
                    text = newVal.ToString()
                ElseIf val = 0 Then
                    If Not text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) Then
                        text = "0"
                    End If
                End If
            Catch
                text = "0"
            End Try

            While text.Length > 1 AndAlso text(0) = "0"c AndAlso String.Empty + text(1) <> NumberFormatInfo.CurrentInfo.NumberDecimalSeparator
                text = text.Substring(1)
                If caret > 0 Then
                    caret -= 1
                End If
            End While

            While text.Length > 2 AndAlso String.Empty + text(0) = NumberFormatInfo.CurrentInfo.NegativeSign AndAlso text(1) = "0"c AndAlso String.Empty + text(2) <> NumberFormatInfo.CurrentInfo.NumberDecimalSeparator
                text = NumberFormatInfo.CurrentInfo.NegativeSign + text.Substring(2)
                If caret > 1 Then
                    caret -= 1
                End If
            End While

            If caret > text.Length Then
                caret = text.Length
            End If

            _this.Text = text
            _this.CaretIndex = caret
            _this.SelectionStart = caret
            _this.SelectionLength = selectionLength
            e.Handled = True
        End If
    End Sub

    Private Shared Function IsSymbolValid(ByVal mask As MaskType, ByVal text As String) As Boolean
        Select Case mask
            Case MaskType.Any
                Return True
            Case MaskType.Integer
                If text = NumberFormatInfo.CurrentInfo.NegativeSign OrElse
                   text = NumberFormatInfo.CurrentInfo.PositiveSign Then
                    Return True
                End If
            Case MaskType.Decimal
                If text = NumberFormatInfo.CurrentInfo.NegativeSign OrElse
                   text = NumberFormatInfo.CurrentInfo.PositiveSign OrElse
                   text = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator Then
                    Return True
                End If
        End Select

        For Each c As Char In text
            If Not Char.IsDigit(c) Then Return False
        Next

        Return True
    End Function

    Private Shared Sub ValidateTextBox(ByVal txtBox As TextBox)
        If GetMask(txtBox) <> MaskType.Any Then
            txtBox.Text = ValidateValue(GetMask(txtBox), txtBox.Text, GetMinimumValue(txtBox), GetMaximumValue(txtBox))
        End If
    End Sub

    Private Shared Function ValidateValue(ByVal mask As MaskType,
                                          ByVal text As String,
                                          ByVal minimumValue As Double,
                                          ByVal maximumValue As Double) As String
        If String.IsNullOrEmpty(text) Then Return String.Empty

        text = text.Trim
        Select Case mask
            Case MaskType.Integer
                Try
                    Dim int As Int64 = Convert.ToInt64(text)
                    Return CStr(ValidateLimits(minimumValue, maximumValue, int))
                Catch ex As Exception
                    Return String.Empty
                End Try
            Case MaskType.Decimal
                Try
                    Dim dbl As Double = Convert.ToDouble(text)
                    Return CStr(ValidateLimits(minimumValue, maximumValue, dbl))
                Catch ex As Exception
                    Return String.Empty
                End Try
        End Select
        Return text
    End Function

    Private Shared Function ValidateLimits(ByVal min As Double, ByVal max As Double, ByVal value As Double) As Double
        If Not min.Equals(Double.NaN) AndAlso value < min Then Return min
        If Not max.Equals(Double.NaN) AndAlso value > max Then Return max
        Return value
    End Function
End Class
