Imports System.ComponentModel
Imports Utility

Namespace Tools
    Public Class GrammarInfo

        Public ReadOnly Property Tags As BindingList(Of String)
            Get
                Dim value As New BindingList(Of String)
                For Each item As String In lstTags.Items
                    value.Add(item)
                Next
                Return value
            End Get
        End Property

        Public Property GrammarName As String
            Get
                Return txtName.Text
            End Get
            Set(value As String)
                txtName.Text = value
            End Set
        End Property

        Public Property Description As String
            Get
                Return txtDescription.Text
            End Get
            Set(value As String)
                txtDescription.Text = value
            End Set
        End Property

        Public Property Author As String
            Get
                Return txtAuthor.Text
            End Get
            Set(value As String)
                txtAuthor.Text = value
            End Set
        End Property

        Public Property SupportsMaxLength As Boolean
            Get
                Return CBool(chkSupportsMaxLength.IsChecked)
            End Get
            Set(value As Boolean)
                chkSupportsMaxLength.IsChecked = value
            End Set
        End Property

        Private Sub lnkTags_Click(sender As Object, e As RoutedEventArgs)
            Dim tagEditor As New TagEditor
            Dim list As New BindingList(Of String)

            For Each item As String In lstTags.Items
                list.Add(item)
            Next
            tagEditor.TagList = list
            tagEditor.Owner = Me.GetParentWindow()

            If tagEditor.ShowDialog Then
                lstTags.Items.Clear()
                For Each item As String In tagEditor.TagList
                    lstTags.Items.Add(item)
                Next
            End If
        End Sub

    End Class
End Namespace