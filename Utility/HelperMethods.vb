Imports System.Reflection
Imports System.Linq.Expressions
Imports System.IO
Imports System.Security.AccessControl
Imports Microsoft.Win32
Imports System.Security
Imports System.Drawing
Imports System.Windows

Public NotInheritable Class HelperMethods

    Public Shared Function ConvertToFont(ByVal text As String) As Font
        Dim fc As New FontConverter
        If fc.IsValid(text) Then Return DirectCast(fc.ConvertFromString(text), Font)
        Return Nothing
    End Function

    Public Shared Function ConvertFromFont(ByVal font As Font) As String
        Dim fc As New FontConverter
        Return fc.ConvertToString(font)
    End Function

    Public Shared Function GetPropertyName(Of TModel, TValue)(propertyId As Expression(Of Func(Of TModel, TValue))) As String
        Dim member As MemberInfo = DirectCast(propertyId.Body, MemberExpression).Member
        Return member.Name
    End Function

    Public Shared Function HasWritePermissionOnDir(path As String) As Boolean
        Dim writeAllow As Boolean = False
        Dim writeDeny As Boolean = False
        Dim accessControlList As DirectorySecurity = Directory.GetAccessControl(path)
        If accessControlList Is Nothing Then
            Return False
        End If
        Dim accessRules As AuthorizationRuleCollection = accessControlList.GetAccessRules(True, True, GetType(System.Security.Principal.SecurityIdentifier))
        If accessRules Is Nothing Then
            Return False
        End If

        For Each rule As FileSystemAccessRule In accessRules
            If (FileSystemRights.Write And rule.FileSystemRights) <> FileSystemRights.Write Then
                Continue For
            End If

            If rule.AccessControlType = AccessControlType.Allow Then
                writeAllow = True
            ElseIf rule.AccessControlType = AccessControlType.Deny Then
                writeDeny = True
            End If
        Next

        Return writeAllow AndAlso Not writeDeny
    End Function
End Class
