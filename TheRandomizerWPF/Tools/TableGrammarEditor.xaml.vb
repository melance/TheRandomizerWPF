Imports System.Data

Namespace Tools
    Public Class TableGrammarEditor
       
        Private isManualEditCommit As Boolean = False

        Private Sub HandleMainDataGridCellEditEnding(sender As Object, e As DataGridCellEditEndingEventArgs)
            If Not isManualEditCommit Then
                isManualEditCommit = True
                Dim grid As DataGrid = DirectCast(sender, DataGrid)
                grid.CommitEdit(DataGridEditingUnit.Row, True)
                isManualEditCommit = False
            End If
        End Sub

    End Class
End Namespace