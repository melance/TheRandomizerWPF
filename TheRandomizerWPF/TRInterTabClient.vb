Public Class TRInterTabClient
    Implements Dragablz.IInterTabClient

    Public Function GetNewHost(interTabClient As Dragablz.IInterTabClient, partition As Object, source As Dragablz.TabablzControl) As Dragablz.INewTabHost(Of Window) Implements Dragablz.IInterTabClient.GetNewHost
        Dim host As New TabHost
        host.tabContainer.InterTabController = New Dragablz.InterTabController With {.InterTabClient = Me}
        Return New Dragablz.NewTabHost(Of TabHost)(host, host.tabContainer)
    End Function

    Public Function TabEmptiedHandler(tabControl As Dragablz.TabablzControl, window As Window) As Dragablz.TabEmptiedResponse Implements Dragablz.IInterTabClient.TabEmptiedHandler
        If TypeOf window Is MainWindow Then
            Return Dragablz.TabEmptiedResponse.DoNothing
        Else
            Return Dragablz.TabEmptiedResponse.CloseWindowOrLayoutBranch
        End If
    End Function
End Class
