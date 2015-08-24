
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Input

Namespace Help
    ''' <summary>
    ''' Provider class for online help.  
    ''' </summary>
    Public Class HelpProvider
#Region "Fields"
        ''' <summary>
        ''' Help topic dependency property. 
        ''' </summary>
        ''' <remarks>This property can be attached to an object such as a form or a textbox, and 
        ''' can be retrieved when the user presses F1 and used to display context sensitive help.</remarks>
        Public Shared ReadOnly HelpTopicProperty As DependencyProperty = DependencyProperty.RegisterAttached("HelpString", GetType(String), GetType(HelpProvider))
#End Region

#Region "Constructors"
        ''' <summary>
        ''' Static constructor that adds a command binding to Application.Help, binding it to 
        ''' the CanExecute and Executed methods of this class. 
        ''' </summary>
        ''' <remarks>With this in place, when the user presses F1 our help will be invoked.</remarks>
        Shared Sub New()
            CommandManager.RegisterClassCommandBinding(GetType(FrameworkElement), New CommandBinding(ApplicationCommands.Help, New ExecutedRoutedEventHandler(AddressOf ShowHelpExecuted), New CanExecuteRoutedEventHandler(AddressOf ShowHelpCanExecute)))
        End Sub
#End Region

#Region "Methods"
        ''' <summary>
        ''' Getter for <see cref="HelpTopicProperty"/>. Get a help topic that's attached to an object. 
        ''' </summary>
        ''' <param name="obj">The object that the help topic is attached to.</param>
        ''' <returns>The help topic.</returns>
        Public Shared Function GetHelpTopic(obj As DependencyObject) As String
            Return DirectCast(obj.GetValue(HelpTopicProperty), String)
        End Function

        ''' <summary>
        ''' Setter for <see cref="HelpTopicProperty"/>. Attach a help topic value to an object. 
        ''' </summary>
        ''' <param name="obj">The object to which to attach the help topic.</param>
        ''' <param name="value">The value of the help topic.</param>
        Public Shared Sub SetHelpTopic(obj As DependencyObject, value As String)
            obj.SetValue(HelpTopicProperty, value)
        End Sub

        ''' <summary>
        ''' Show help table of contents. 
        ''' </summary>
        Public Shared Sub ShowHelpTableOfContents()
            System.Windows.Forms.Help.ShowHelp(Nothing, My.Resources.HelpFile, HelpNavigator.TableOfContents)
        End Sub

        ''' <summary>
        ''' Show a help topic in the online CHM style help. 
        ''' </summary>
        ''' <param name="helpTopic">The help topic to show. This must match exactly with the name 
        ''' of one of the help topic's .htm files, without the .htm extention and with spaces instead of underscores
        ''' in the name. For instance, to display the help topic "This_is_my_topic.htm", pass the string "This is my topic".</param>
        ''' <remarks>You can also pass in the help topic with the underscore replacement already done. You can also 
        ''' add the .htm extension. 
        ''' Certain characters other than spaces are replaced by underscores in RoboHelp help topic names. 
        ''' This method does not yet account for all those replacements, so if you really need to find a help topic
        ''' with one or more of those characters, do the underscore replacement before passing the topic.</remarks>
        Public Shared Sub ShowHelpTopic(helpTopic As String)
            System.Windows.Forms.Help.ShowHelp(Nothing, My.Resources.HelpFile, HelpNavigator.TopicId, helpTopic)
        End Sub

        ''' <summary>
        ''' Whether the F1 help command can execute. 
        ''' </summary>
        Private Shared Sub ShowHelpCanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
            Dim senderElement As FrameworkElement = TryCast(sender, FrameworkElement)

            If HelpProvider.GetHelpTopic(senderElement) IsNot Nothing Then
                e.CanExecute = True
            End If
        End Sub

        ''' <summary>
        ''' Execute the F1 help command. 
        ''' </summary>
        ''' <remarks>Calls ShowHelpTopic to show the help topic attached to the framework element that's the 
        ''' source of the call.</remarks>
        Private Shared Sub ShowHelpExecuted(sender As Object, e As ExecutedRoutedEventArgs)
            ShowHelpTopic(HelpProvider.GetHelpTopic(TryCast(sender, FrameworkElement)))
        End Sub
#End Region
    End Class
End Namespace