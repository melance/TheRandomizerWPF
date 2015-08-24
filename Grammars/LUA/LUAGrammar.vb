Imports NLua
Imports System.Xml.Serialization
Imports System.Text
Imports System.Windows
Imports System.IO

Namespace LUA
    Public Class LUAGrammar
        Inherits BaseGrammar

        Private _script As String
        Private _scriptPath As String
        Private _output As StringBuilder

        <XmlElement("script")>
        Public Property Script As String
            Get
                Return _script
            End Get
            Set(value As String)
                If value <> _script Then
                    _script = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

        <XmlElement("scriptPath")>
        Public Property ScriptPath As String
            Get
                If Path.IsPathRooted(_scriptPath) Then Return _scriptPath
                Return Path.Combine(Path.GetDirectoryName(Reflection.Assembly.GetCallingAssembly.Location), "DataFiles", _scriptPath)
            End Get
            Set(value As String)
                _scriptPath = value
            End Set
        End Property

        Default Public Overrides ReadOnly Property Item(columnName As String) As String
            Get
                If columnName = "Script" AndAlso String.IsNullOrWhiteSpace(Script) Then Return "Script is required."
                Return MyBase.Item(columnName)
            End Get
        End Property

        Public Overrides Function Analyze() As String
            Return "Not Supported"
        End Function

        Protected Overrides Function GenerateName() As String
            Dim l As New NLua.Lua
            Dim name As New StringBuilder
            Dim calc As New Calculator(Me, l)
            Dim modulePath As String = String.Empty

            Try
                _output = Nothing
                For Each path As String In Utility.GrammarFilePaths
                    modulePath &= ";" & IO.Path.Combine(path, "lua")
                    modulePath &= "\?.lua"
                Next

                modulePath = modulePath.Replace("\", "\\")

                For Each parameter As Parameter In Parameters
                    l(parameter.Name) = parameter.Value
                Next

                l("Grammar") = Me
                l("Me") = l
                l.RegisterFunction("print", Me, Me.GetType.GetMethod("Print"))
                l.RegisterFunction("printif", Me, Me.GetType.GetMethod("PrintIf"))
                l.DoString("luanet.load_assembly('Grammars')")
                l.DoString("CalcClass=luanet.import_type('Grammars.LUA.Calculator')")
                l.DoString("calc=CalcClass(Grammar,Me)")
                l.DoString("package.path = package.path .. '" & modulePath & "'")
                l.DoString("import = function() end")

                If String.IsNullOrWhiteSpace(Script) Then
                    l.DoFile(ScriptPath)
                Else
                    l.DoString(Script)
                End If

                If _output Is Nothing Then Return String.Empty
                Return _output.ToString()
            Catch ex As NLua.Exceptions.LuaScriptException
                If _output IsNot Nothing Then
                    Return _output.ToString & "<br /><br /><span style='color:red;'>" & ex.ToString & "</span>"
                Else
                    Return "<span style='color:red;'>" & ex.ToString & "</span>"
                End If

            End Try
        End Function

        Public Sub Print(ByVal value As Object)
            If _output Is Nothing Then _output = New StringBuilder
            _output.AppendLine(value.ToString)
        End Sub

        Public Sub PrintIf(ByVal condition As Boolean,
                           ByVal value As Object)
            If condition Then
                Print(value)
            End If
        End Sub
    End Class
End Namespace