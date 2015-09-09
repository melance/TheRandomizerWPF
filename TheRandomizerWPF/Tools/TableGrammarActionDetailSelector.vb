Imports Grammars.Table

Namespace Tools
    Public Class TableGrammarActionDetailSelector
        Inherits DataTemplateSelector

        Public Property RandomDataTemplate() As DataTemplate
        Public Property LoopDataTemplate() As DataTemplate
        Public Property SelectDataTemplate() As DataTemplate

        Public Overrides Function SelectTemplate(item As Object, container As DependencyObject) As DataTemplate
            Dim grammar As TableGrammarTable = DirectCast(item, TableGrammarTable)

            Select Case grammar.Action
                Case Actions.Loop : Return LoopDataTemplate
                Case Actions.Random : Return RandomDataTemplate
                Case Actions.Select : Return SelectDataTemplate
            End Select

            Return Nothing
        End Function

    End Class
End Namespace