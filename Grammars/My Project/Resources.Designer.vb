﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.18444
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'This class was auto-generated by the StronglyTypedResourceBuilder
    'class via a tool like ResGen or Visual Studio.
    'To add or remove a member, edit your .ResX file then rerun ResGen
    'with the /str option, or rebuild your VS project.
    '''<summary>
    '''  A strongly-typed resource class, for looking up localized strings, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("Grammars.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Overrides the current thread's CurrentUICulture property for all
        '''  resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        '''
        '''&lt;html lang=&quot;en&quot; xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
        '''&lt;head&gt;
        '''    &lt;meta charset=&quot;utf-8&quot; /&gt;
        '''    &lt;title&gt;[Name]&lt;/title&gt;
        '''    &lt;style type=&quot;text/css&quot;&gt;
        '''        body {
        '''            font-family: Verdana;
        '''        }
        '''        h1 {
        '''            background: gray;
        '''            width: 100%;
        '''            padding: 3px;
        '''            margin: 0px;
        '''        }
        '''        caption {
        '''            font-weight: bolder;
        '''            font-size: 14px;
        '''        }
        '''        #description {
        '''            border-bottom: 1p [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property AssignmentGrammarAnalysis() As String
            Get
                Return ResourceManager.GetString("AssignmentGrammarAnalysis", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        '''
        '''&lt;html lang=&quot;en&quot; xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
        '''&lt;head&gt;
        '''    &lt;meta charset=&quot;utf-8&quot; /&gt;
        '''    &lt;title&gt;[Name]&lt;/title&gt;
        '''    &lt;style type=&quot;text/css&quot;&gt;
        '''        body {
        '''            font-family: Verdana;
        '''        }
        '''        h1 {
        '''            background: gray;
        '''            width: 100%;
        '''            padding: 3px;
        '''            margin: 0px;
        '''        }
        '''        caption {
        '''            font-weight: bolder;
        '''            font-size: 14px;
        '''        }
        '''        #description {
        '''            border-bottom: 1p [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property PhonotacticsGrammarAnalysis() As String
            Get
                Return ResourceManager.GetString("PhonotacticsGrammarAnalysis", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &lt;html&gt;
        '''&lt;head&gt;
        '''    &lt;meta charset=&quot;ISO-8859-1&quot; /&gt;
        '''    &lt;title&gt;&lt;/title&gt;
        '''    &lt;style type=&quot;text/css&quot;&gt;
        '''        .result {
        '''            margin:0;
        '''            padding:0;
        '''        }
        '''        .odd {
        '''            background: {OddColor};
        '''        }
        '''        .even {
        '''            background: {EvenColor};
        '''            border-top: 1px solid {DividerColor};
        '''            border-bottom: 1px solid {DividerColor};
        '''        }
        '''        body {
        '''            font-family: {FontFamily};
        '''            font-size: {FontSize}px;
        '''      [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property ResultsTemplate() As String
            Get
                Return ResourceManager.GetString("ResultsTemplate", resourceCulture)
            End Get
        End Property
    End Module
End Namespace